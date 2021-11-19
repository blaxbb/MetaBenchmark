﻿window.ShowModal = function (id) {
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

window.SelectPickerNone = function (selector) {
    $(selector).selectpicker('deselectAll');
}

window.SelectValue = function (selector, value) {
    $(selector).val(value);
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

window.SetupChart2 = function (id, labels, values) {
    console.log(id);
    var data = values.map((value, index) => { return { value: value, meta: labels[index] } })
    console.log(data);
    var data = {
        labels: labels,
        series: [
            values.map((value, index) => { return { meta: labels[index], value: value } })
        ]
    };

    var options = {
        height: 200
    };

    var responsiveOptions = {
        plugins: [
            Chartist.plugins.tooltip()
        ]
    }

    new Chartist.Line('.ct-chart', data, options, responsiveOptions);

}

var chart = null;
window.SetupChart = function (id, labels, values, maxVal) {
    SetupMultiChart(id, labels, [values], maxVal, null);
}

window.SetupMultiChart = function (id, labels, values, maxVal, datasetNames) {
    console.log(id);
    console.log(values);
    if(datasetNames == null){
        datasetNames = [""];
    }

    function getColor(val) {
        if (val > 100) {
            return "#28a745";
        }
        if (val > 60) {
            return "#4c9be8";
        }
        return "#ffc107";
    }

    const ctx = document.getElementById(id);
    if (ctx == null) {
        return;
    }

    if (chart != null) {
        chart.destroy();
    }

    Chart.defaults.color = "#fff";
    chart = new Chart(ctx, {
        type: 'line',
        options: {
            maintainAspectRatio: false,
            scales: {
                y: {
                    beginAtZero: true,
                    suggestedMax: 150,
                    suggestedMin: 50,
                    ticks: {
                        stepSize: 50
                    },
                },
                x: {
                    display: false
                }
            },
            elements: {
                point: {
                    radius: 6,
                    hoverRadius: 9,
                    hitRadius: 3
                },
                line: {
                    borderColor: "#fff"
                }
            },
            interaction: {
                mode: 'index',
                intersect: false,
            },
            plugins: {
                legend: {
                    display: false
                },
                tooltip: {
                    padding: 20,
                    displayColors: false,
                    titleFont : {
                      size: 16
                    },
                    bodyFont : {
                      size: 16
                    }
                },
            }
        }
    });

    chart.data = {
        labels: labels,
        datasets: values.map((v, index) => {
            return {
                data: v,
                label: datasetNames[index],
                backgroundColor: v.map((val) => { return getColor(val); }),
                borderColor: namedColor(index),
                borderWidth: 1
            };
        })
    };
    chart.update();
}

//begin https://github.com/chartjs/Chart.js/blob/master/docs/scripts/utils.js
var CHART_COLORS = {
    red: 'rgb(255, 99, 132)',
    orange: 'rgb(255, 159, 64)',
    yellow: 'rgb(255, 205, 86)',
    green: 'rgb(75, 192, 192)',
    blue: 'rgb(54, 162, 235)',
    purple: 'rgb(153, 102, 255)',
    grey: 'rgb(201, 203, 207)'
};
  
var NAMED_COLORS = [
    CHART_COLORS.red,
    CHART_COLORS.orange,
    CHART_COLORS.yellow,
    CHART_COLORS.green,
    CHART_COLORS.blue,
    CHART_COLORS.purple,
    CHART_COLORS.grey,
];
  
function namedColor(index) {
    return NAMED_COLORS[index % NAMED_COLORS.length];
}
//end https://github.com/chartjs/Chart.js/blob/master/docs/scripts/utils.js