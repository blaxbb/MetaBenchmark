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
    $('.selectpicker').selectpicker('render');
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

window.SetSession = function (name, value) {
    sessionStorage.setItem(name, value);
}

window.GetSession = function (name) {
    return sessionStorage.getItem(name);
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
                    },
                    caretSize: 0
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

    if (Chart.Tooltip.positioners.custom == null) {
        Chart.Tooltip.positioners.custom = function (items) {
            const pos = Chart.Tooltip.positioners.average(items);

            // Happens when nothing is found
            if (pos === false) {
                return false;
            }
            let right = pos.x > (chart.chartArea.width / 2);

            return {
                x: pos.x + (right ? -50 : 50),
                y: (chart.chartArea.bottom + chart.chartArea.top) / 2,
                xAlign: right ? 'right' : 'left',
                yAlign: 'center',
            };
        };
    }

    chart.options.plugins.tooltip.position = 'custom';
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

function ShowCollapse(selector) {
    $(selector).collapse("show");
}

function HideCollapse(selector) {
    $(selector).collapse("hide");
}

function FadeOut(selector) {
    $(selector).removeClass("fade-in");
    $(selector).addClass("fade-out");
}

function FadeIn(selector) {
    $(selector).removeClass("fade-out");
    $(selector).addClass("fade-in");
}

window.dbPromise = null;
function setupDb() {
    dbPromise = idb.openDB('mbtest', 1, {
        upgrade(db) {
            db.createObjectStore("Benchmarks", {
                keyPath: 'id'
            });
            db.createObjectStore("Sources", {
                keyPath: 'id'
            });
            db.createObjectStore("Products", {
                keyPath: 'id'
            });
            db.createObjectStore("Specifications", {
                keyPath: 'id'
            });
            db.createObjectStore("AllProducts", {
                keyPath: 'id'
            });
            db.createObjectStore("Settings");
        }
    });
}

async function DBGet(table, key) {
    return (await dbPromise).get(table, key);
};
async function DBGetAll(table) {
    return (await dbPromise).getAll(table);
};

async function DBSet(table, val) {
    return (await dbPromise).put(table, val);
};

async function DBSetKeyVal(table, key, val) {
    return (await dbPromise).put(table, val, key);
};

async function DBSetAll(table, values) {
    await DBClear(table);
    let transaction = (await dbPromise).transaction(table, "readwrite");
    await Promise.all(values.map(v => transaction.store.put(v)));
};
async function DBDel(table, key) {
    return (await dbPromise).delete(table, key);
};
async function DBClear(table) {
    return (await dbPromise).clear(table);
};
async function DBKeys(table) {
    return (await dbPromise).getAllKeys(table);
};