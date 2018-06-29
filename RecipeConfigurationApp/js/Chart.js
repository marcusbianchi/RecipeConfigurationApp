tempxValues = JSON.parse(document.getElementById('temperatureXValues').textContent);
tempyValues = JSON.parse(document.getElementById('temperatureYValues').textContent);

presxValues = JSON.parse(document.getElementById('pressureXValues').textContent);
presyValues = JSON.parse(document.getElementById('pressureYValues').textContent);

vacxValues = JSON.parse(document.getElementById('vaacumXValues').textContent);
vacyValues = JSON.parse(document.getElementById('vaacumYValues').textContent);

var vac = [];
for (var i = 0; i < vacxValues.length; i++) {
    vac.push([vacxValues[i], vacyValues[i]]);
}

var pres = [];
for (var i = 0; i < presxValues.length; i++) {
    pres.push([presxValues[i], presyValues[i]]);
}

var temp = [];
for (var i = 0; i < tempxValues.length; i++) {
    temp.push([tempxValues[i], tempyValues[i]]);
}

var topx = [tempxValues[tempxValues.length - 1], presxValues[presxValues.length - 1], vacxValues[vacxValues.length - 1]]
topx.sort();
console.log(vac);
console.log(pres);
console.log(temp);
console.log(topx);

$.plot("#ChartDiv1", [
    { data: temp, label: "Temperature (°C)", yaxis: 1, color: "red" },
    { data: pres, label: "Pressão (bar)", yaxis: 2, color: "blue" },
    { data: vac, label: "Vacuo (bar)", yaxis: 3, color: "green" },
], {
        xaxis: {
            min: 1,
            max: topx[0]
        },
        yaxes: [{
            position: "left", tickLength: 0, font: {
                color: "red"
            }
        },
        {
            position: "left", tickLength: 0, font: {
                color: "blue"
            }
        },
        {
            position: "left", tickLength: 0, font: {
                color: "green"
            }
        }
        ],
        legend: { position: "ne" }
    }
);


//var Temperatura = {
//    x: tempxValues,
//    y: tempyValues,
//    mode: 'lines+markers',
//    name: 'Temperatura',
//    line: {
//        color: '#1f77b4',
//        width: 0.5
//    }
//};

//var Pressao = {
//    x: presxValues,
//    y: presyValues,
//    mode: 'lines+markers',
//    name: 'Pressão',
//    yaxis: 'y2',
//    line: {
//        color: '#ff7f0e',
//        width: 0.5
//    }
//};

//var Vacuo = {
//    x: vacxValues,
//    y: vacyValues,
//    mode: 'lines+markers',
//    name: 'Vacuo',
//    yaxis: 'y3',
//    line: {
//        color: '#d62728',
//        width: 0.5
//    }

//};

//var data1 = [Temperatura, Pressao, Vacuo];


//var layout = {
//    title: 'Receita',
//    xaxis: { domain: [0.3, 1], title: 'Tempo (min)' },
//    yaxis: {
//        title: 'Temperatura (°C)',
//        titlefont: { color: '#1f77b4' },
//        tickfont: { color: '#1f77b4' }
//    },
//    yaxis2: {
//        title: 'Pressão (bar)',
//        titlefont: { color: '#ff7f0e' },
//        tickfont: { color: '#ff7f0e' },
//        anchor: 'free',
//        overlaying: 'y',
//        side: 'left',
//        position: 0.10
//    },
//    yaxis3: {
//        title: 'Vacuo (bar)',
//        titlefont: { color: '#d62728' },
//        tickfont: { color: '#d62728' },
//        overlaying: 'y',
//        side: 'left',
//        position:  0.20
//    }

//};

//Plotly.newPlot('ChartDiv1', data1, layout);


