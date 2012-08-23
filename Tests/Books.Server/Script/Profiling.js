$(document).ready(function () {
    $("#cvs").width($(window).width());
    var canvas = document.getElementById('cvs');
    canvas.width = $(window).width();
    $('#btnRefId').bind('click', function () {
        $("#divError").hide();
        var refId = $("#txtRefId").val();
        if (refId.length === 0) {
            $("#divError").text("Reference Id is mandatory");
            $("#divError").show();
            return false;
        }

        $('#divResult').html("");

        $.ajax({
            beforeSend: function (x) {
                if (x && x.overrideMimeType) {
                    x.overrideMimeType("application/j-son;charset=UTF-8");
                }
            },
            dataType: "json",
            type: 'get',
            async: false,
            url: Profiling.getServiceUrl(refId),
            success: function (response) {
                if (response.Code) {
                    if (response.Code === "success") {
                        if (response.Head == null) {
                            $("#divError").text("No data exists for given reference ID.");
                            $("#divError").show();
                            $("#divProfilingData").slideUp();
                        }
                        else {
                            Profiling.generateChart(response);
                            Profiling.generateTabularData(response);    
                        }
                    }
                    else {
                        $("#divError").text("An error occured while fetching the data.");
                        $("#divError").show();
                        $("#divProfilingData").slideUp();
                    }
                }
                else {
                    $("#divError").text("An error occured while fetching the data.");
                    $("#divError").show();
                    $("#divProfilingData").slideUp();
                }
            },
            error: function (response) {
                $("#divError").text("An error occured while fetching the data.");
                $("#divError").show();
                $("#divProfilingData").slideUp();
                return false;
            }
        });

    });
});

String.prototype.format = String.prototype.f = function () {
    var s = this,
        i = arguments.length;

    while (i--) {
        s = s.replace(new RegExp('\\{' + i + '\\}', 'gm'), arguments[i]);
    }
    return s;
};

var Profiling = {
    data: new Object(),
    events: [],
    tooltips: [],
    getServiceUrl: function (refId) {
        return "profile.svc/{0}?format=json".format(refId);
    },
    gChart: new Object(),
    clearCanvas: function () {
        var canvas = document.getElementById("cvs");
        var context = canvas.getContext("2d");
        context.clearRect(0, 0, canvas.width, canvas.height);
    },
    getLogDescription: function (call) {

    },
    generateChart: function (response) {



        Profiling.setChartData(response.Head);

        var total = response.Head.Duration;
        var labels = [];
        var unit = response.Head.Duration / 10;
        for (var i = 1; i <= 10; i++) {
            labels.push((unit * i).toFixed(0) + " ms");
        }

        var canvas = document.getElementById('cvs');
        canvas.height = 50 * labels.length;

        this.clearCanvas();

        this.gChart = new RGraph.Gantt('cvs');



        // Configure the chart to appear as you want.
        this.gChart.Set("chart.background.grid.autofit", true);
        this.gChart.Set("chart.background.grid.autofit.numhlines", Profiling.events.length);
        this.gChart.Set("chart.background.grid.autofit.numvlines", 10);
        this.gChart.Set('chart.xmax', total);
        this.gChart.Set('chart.gutter.left', 300);
        this.gChart.Set('chart.gutter.right', 100);
        this.gChart.Set('chart.labels', labels);
        this.gChart.Set('chart.title', '');
        this.gChart.Set('chart.defaultcolor', 'rgba(231,231,231,1)');
        this.gChart.Set('chart.tooltips', Profiling.tooltips);
        this.gChart.Set("chart.tooltips.event", "onmousemove");
        this.gChart.Set("chart.borders", false);
        this.gChart.Set('chart.events', Profiling.events);

        this.gChart.Draw();

        $("#divProfilingData").slideDown();
    },
    setChartData: function (headCall) {
        Profiling.events = [];
        var headCallName = headCall.Name.substr(0, headCall.Name.indexOf("["));
        Profiling.events.push([Profiling.formatDuration(headCall.Start), Profiling.formatDuration(headCall.Duration), parseFloat((headCall.DurationWithOutChildren * 100 / headCall.Duration).toFixed(2)), headCallName, null, 'rgba(242, 103, 57, 1)']);
        Profiling.tooltips.push(Profiling.formatTooltip(headCall));

        var next = headCall.Children;

        function recurse(nextTimings) {
            for (var i = 0; nextTimings[i] != null; i++) {
                Profiling.events.push([Profiling.formatDuration(nextTimings[i].Start), Profiling.formatDuration(nextTimings[i].Duration), parseFloat((nextTimings[i].DurationWithOutChildren * 100 / nextTimings[i].Duration).toFixed(2)), nextTimings[i].Name, null, 'rgba(242, 103, 57, 1)']);
                Profiling.tooltips.push(Profiling.formatTooltip(nextTimings[i]));
                next = nextTimings[i].Children;
                recurse(next);
            }
        };

        recurse(next);
        return true;
    },
    formatTooltip: function (call) {
        var html = "<table>";
        html += "<tr><td>Name</td><td>" + call.Name + "</td></tr>";
        html += "<tr><td>Start</td><td>" + Profiling.formatDuration(call.Start) + " ms</td></tr>";
        html += "<tr><td>Total</td><td>" + Profiling.formatDuration(call.Duration) + " ms</td></tr>";
        html += "<tr><td>Self</td><td>" + Profiling.formatDuration(call.DurationWithOutChildren) + " ms</td></tr>";
        html += "<tr><td>Children</td><td>" + Profiling.formatDuration(call.Duration - call.DurationWithOutChildren) + " ms</td></tr>";
        html += "</table>";
        return html;
    },
    formatDuration: function (duration) {
        return parseFloat(duration.toFixed(3));
    },
    formatSummary: function (call) {
        var html = "<hr /><table>";
        html += "<tr><td align='right'>Service : </td><td>{0}</td><td align='right'>Request Url : </td><td>{1}</td><tr />".format(call.Head.Name, call.RequestUrl);
        html += "<tr><td align='right'>Server : </td><td>{0}</td><td align='right'>Request Time : </td><td>{1}</td><tr />".format(call.MachineName, call.Started);
        html += "<tr><td align='right'>Request Type : </td><td>{0}</td><td align='right'>Requested By : </td><td>{1}</td><tr />".format(call.HttpMethod, call.UserName);
        html += "</table> <hr />";
        return html;
    },
    generateTabularData: function (returnValue) {
        $('#divProfileName').html(returnValue.Name);
        $('#divProfileDescription').html(Profiling.formatSummary(returnValue));

        var indentCount = 0;
        var table = "<table border='0' style='margin:0 auto;'><tr><td>&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp</td><td>&nbsp&nbsp <strong>duration(ms)</strong> &nbsp&nbsp</td><td>&nbsp&nbsp <strong>without children(ms)</strong> &nbsp&nbsp</td><td>&nbsp&nbsp <strong>from start(ms)</strong> &nbsp&nbsp</td>";
        var head = returnValue.Head;
        if (head !== null) {
            table += "<tr  class='alt'><td>" + head.Name + "&nbsp&nbsp </td><td>&nbsp&nbsp " + head.Duration.toFixed(2) + "</td><td>&nbsp&nbsp " + head.DurationWithOutChildren.toFixed(2) + "</td><td>&nbsp&nbsp " + head.Start.toFixed(2) + "</td></tr>";
        }
        var next = head.Children;
        var isAlt = false;
        function recurse(nextTimings) {
            for (var i = 0; nextTimings[i] != null; i++) {
                indentCount++;
                if (isAlt) {
                    table += "<tr  class='alt'><td>";
                    isAlt = false;
                }
                else {
                    table += "<tr><td>";
                    isAlt = true;
                }
                for (var j = 0; j < indentCount; j++) {
                    table += "&nbsp&nbsp";
                }
                table += nextTimings[i].Name + "&nbsp&nbsp </td><td>&nbsp&nbsp " + nextTimings[i].Duration.toFixed(2) + "</td><td>&nbsp&nbsp " + nextTimings[i].DurationWithOutChildren.toFixed(2) + "</td><td>&nbsp&nbsp " + nextTimings[i].Start.toFixed(2) + "</td></tr>";
                next = nextTimings[i].Children;
                recurse(next);
                indentCount--;
            }
        };
        recurse(head.Children);
        table += "</table>";
        $(table).appendTo($('#divResult'));
    }
};