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

window.GetVal = function (selector) {
    return $(selector).val();
}

window.MoveFilterToNav = function () {
    document.getElementById("filterContainer").innerHTML = "";
    document.getElementById("filterContainer").appendChild(document.getElementById("benchmarkFilter").parentElement)
    document.getElementById("filterContainer").appendChild(document.getElementById("sourceFilter").parentElement)
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