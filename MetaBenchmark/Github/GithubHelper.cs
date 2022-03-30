using Octokit;
using MetaBenchmark.Github;

namespace MetaBenchmark.Github
{
    public class GithubHelper
    {
        readonly string productHeader = "blaxbb";
        readonly string clientId = "79e0f050ccaa47765771";
        public string AppUrl => $"https://github.com/settings/connections/applications/{clientId}";

        GitHubClient github;
        string AccessToken;

        public enum GithubState
        {
            Initializing,
            Unauthenticated,
            WaitingForAuthorization,
            Authenticated
        }

        private GithubState state;

        public GithubState State {
            get => state;
            set
            {
                if (value != state)
                {
                    state = value;
                    GithubStateChanged?.Invoke(state);
                }
                state = value;
            }
        }

        public delegate void GithubStateChangedCallback(GithubState state);
        public event GithubStateChangedCallback GithubStateChanged;

        public User? CurrentUser { get; set; }

        public GithubHelper()
        {
            Init();
        }

        public void Init()
        {
            State = GithubState.Initializing;
            CurrentUser = null;
            github = new GitHubClient(new ProductHeaderValue(productHeader));
        }

        public string LoginUrl()
        {
            return github.Oauth.GetGitHubLoginUrl(new OauthLoginRequest(clientId)
            {
                Scopes = {"public_repo"}
            }).AbsoluteUri;
        }

        public string GetLoginUrl(string csrf)
        {
            return github.Oauth.GetGitHubLoginUrl(new OauthLoginRequest(clientId) {
                Scopes = { "public_repo" },
                State = csrf,
            }).AbsoluteUri;
        }

        public async Task<bool> IsAuthenticated()
        {
            try
            {
                CurrentUser = await github.User.Current();
                State = GithubState.Authenticated;
                return true;
            }
            catch(AuthorizationException e)
            {
                State = GithubState.Unauthenticated;
                CurrentUser = default;
                return false;
            }
            catch(Exception e)
            {
                throw;
            }
        }

        public async Task<string?> GetAccessToken(string responseCode)
        {
            //
            // Throws ApiException if usercode times out
            //
            var result = await github.CreateAccessToken(new OauthTokenRequest(clientId, "dummysecret", responseCode));
            if (result != null && !string.IsNullOrEmpty(result.AccessToken))
            {
                SetToken(result.AccessToken);
            }
            return AccessToken;
        }

        public void SetToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return;
            }

            AccessToken = token;
            github.Credentials = new Credentials(AccessToken, AuthenticationType.Oauth);
        }

        public async Task<Repository> ForkOrGet(string owner, string repositoryName)
        {
            if (github is null)
            {
                throw new ArgumentNullException(nameof(github));
            }

            if (string.IsNullOrEmpty(owner))
            {
                throw new ArgumentException($"'{nameof(owner)}' cannot be null or empty.", nameof(owner));
            }

            if (string.IsNullOrEmpty(repositoryName))
            {
                throw new ArgumentException($"'{nameof(repositoryName)}' cannot be null or empty.", nameof(repositoryName));
            }

            var user = await github.User.Current();
            Repository? existing;
            try
            {
                existing = await github.Repository.Get(user.Login, repositoryName);
            }
            catch (NotFoundException)
            {
                existing = null;
            }

            Repository fork;
            if (existing != null)
            {
                if (!existing.Fork && owner != user.Login)
                {
                    throw new Exception("Attempted to fork repository, but user already has a repository of the same name which is not a fork.");
                }
                fork = existing;
            }
            else
            {
                fork = await github.Repository.Forks.Create(owner, repositoryName, new NewRepositoryFork());
            }

            return fork;
        }

        public async Task<Reference?> UpdateFiles(Repository repo, string message, IList<string> paths, IList<string> contents)
        {
            if (github is null)
            {
                throw new ArgumentNullException(nameof(github));
            }

            if (paths is null)
            {
                throw new ArgumentNullException(nameof(paths));
            }

            if (contents is null)
            {
                throw new ArgumentNullException(nameof(contents));
            }

            if (paths.Count() != contents.Count())
            {
                throw new ArgumentException("Paths and contents parameters must be of equal length!");
            }

            var authed = await IsAuthenticated();

            // https://laedit.net/2016/11/12/GitHub-commit-with-Octokit-net.html

            var mainRef = await github.Git.Reference.Get(repo.Id, "heads/main");
            var lastCommit = await github.Git.Commit.Get(repo.Id, mainRef.Object.Sha);

            var tree = new NewTree() { BaseTree = lastCommit.Tree.Sha };

            for (int i = 0; i < paths.Count; i++)
            {
                //var file = files.FirstOrDefault(f => f.Path == paths[i]);
                tree.Tree.Add(
                    new NewTreeItem()
                    {
                        Mode = "100644",
                        Type = TreeType.Blob,
                        Content = contents[i],
                        Path = paths[i]
                    }
                );
            }
            var oldTree = await github.Git.Tree.Get(repo.Id, lastCommit.Tree.Sha);

            try
            {

                var newTree = await github.Git.Tree.Create(repo.Id, tree);

                var commit = await github.Git.Commit.Create(
                    repo.Id,
                    new NewCommit(
                        message,
                        newTree.Sha,
                        mainRef.Object.Sha)
                );

                return await github.Git.Reference.Update(repo.Id, "heads/main", new ReferenceUpdate(commit.Sha));
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return default;
        }
    }
}
