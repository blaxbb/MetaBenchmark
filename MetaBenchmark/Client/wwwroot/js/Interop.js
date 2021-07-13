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
    $('.selectpicker').selectpicker();
    $('.selectpicker').selectpicker('refresh');
    console.log('refresh');
}

window.RemoveSelectPicker = function (index) {
    $(".bootstrap-select:eq(" + index + ")").remove();
}

window.SelectPickerAll = function (selector) {
    $(selector).selectpicker('selectAll');
}

window.GetActiveBenchmarks = function () {
    return $("#benchmarkFilter").val();
}

window.GetActiveSources = function () {
    return $("#sourceFilter").val();
}

window.MoveFilterToNav = function () {
    document.getElementById("filterContainer").appendChild(document.getElementById("benchmarkFilter").parentElement)
    document.getElementById("filterContainer").appendChild(document.getElementById("sourceFilter").parentElement)
}