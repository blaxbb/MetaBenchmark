window.ShowModal = function (id) {
    $(id).modal();
}

let focusHandlers = {};

window.ShowModalFocus = function (modal, focus) {
    if (focusHandlers[modal] === undefined) {
        focusHandlers[modal] = true;
        $(modal).on('shown.bs.modal', function () {
            console.log(focus);
            $(focus).trigger('focus');
        });
    }
    $(modal).modal();
}

window.HideModal = function (id) {
    $(id).modal('hide');
}

window.ShowSelectPicker = function () {
    $('.selectpicker').selectpicker('refresh');
}

window.RemoveSelectPicker = function (index) {
    $(".bootstrap-select:eq(" + index + ")").remove();
}

window.SelectPickerAll = function (selector) {
    $(selector).selectpicker('selectAll');
}

window.SelectPickerNone = function (selector) {
    $(selector).selectpicker('deselectAll');
}

window.GetVal = function (selector) {
    return $(selector).val();
}

window.MoveFilterToNav = function () {
}

window.SetUrl = function (url) {
    window.history.replaceState({}, document.title, url)
}

window.SetStorage = function (name, value) {
    localStorage.setItem(name, value);
}

window.GetStorage = function (name) {
    return localStorage.getItem(name);
}
window.ClearStorage = function () {
    localStorage.clear();
}

window.DownloadFile = function (zipName, filenames, texts) {
    if (filenames.length != texts.length) {
        console.logerror("Error creating zip, filenames and texts must match!");
        return;
    }

    var zip = new JSZip();

    for (var i = 0; i < filenames.length; i++) {
        var filename = filenames[i];
        var text = texts[i];
        zip.file(filename, text);
    }

    zip.generateAsync({ type: "blob" })
        .then(function (content) {
            // see FileSaver.js
            saveAs(content, zipName);
        });

}