
var ischecked = false;
var chart;
///这个最上面的菜单 
$(function () {
    ///选择软件时候的切换mychoiceversion
    $("#versionselect").click(function () {
        if ($("#mychoiceversion").is(":hidden"))
            $("#mychoiceversion").show();
        else
            $("#mychoiceversion").hide();

    });
    //选择周期
    $("#perioda").click(function () {
        if ($("#perioddiv").is(":hidden"))
            $("#perioddiv").show();
        else
            $("#perioddiv").hide();
    });

    ///选择平台的切换
    $("#platformselect").click(
        function () {
            ///选择平台前先选软件
            if ($.trim($("#versionspan").text()) == "选择软件") {
                alert("请先选择软件");
                return;
            }
            if ($("#mychoiceplatform").is(":hidden"))
                $("#mychoiceplatform").show();
            else
                $("#mychoiceplatform").hide();
        });

    ////选择软件一个单选后触发的事件
    $("#mychoiceversion").mouseleave(function () {
        softclickorout();
    });
    ////选择平台一个单选后触发的事件
    $("#mychoiceplatform").mouseleave(function () {
        platclickorout();
    });
    $("#perioddiv").mouseleave(function () {
        $("input[name=rdoperiod]").each(function () {
            if ($(this).attr("checked")) {
                $("#inputzhouqi").val($(this).val());
                $("#divperiodspan").text($("#lbl" + $(this).val()).text());
            }
        });
        $("#perioddiv").hide();
    });

});
///软件选择或者离开div 触发的事件
function softclickorout() {
    var softid = $("#inputversionselect").val();
    $("input[name=softradio]").each(function () {
        if ($(this).attr("checked")) {
            $("#inputversionselect").val($(this).val());
            $("#versionspan").text($("#softlbl" + $(this).val()).text());
        }
    });
    getPlatForm();
    $("#mychoiceversion").hide();
    if (softid == $("#inputversionselect").val()) {
        var plat = $("#inputplatformselect").val();
        $("#plat" + plat).attr("checked", true);
        $("#platformspan").text($("#platlbl" + plat).text());
    }

}
//平台选择后或者离开div 触发的事件
function platclickorout() {
   
    var count = 0;
    $("input[name=retainplat]").each(function () {
        if ($(this).attr("checked")) {
            $("#inputplatformselect").val($(this).val());
            $("#platformspan").text($("#platlbl" + $(this).val()).text());
            count = 1;
        }
    });
    if (count == 0) {
        $("#inputplatformselect").val();
        $("#platformspan").text('请选择平台');
    }
    $("#mychoiceplatform").hide();
}
///增加一个剔除重复的函数
Array.prototype.delRepeat = function () {
    var newArray = [];
    var provisionalTable = {};
    for (var i = 0, item; (item = this[i]) != null; i++) {
        if (!provisionalTable[item]) {
            newArray.push(item);
            provisionalTable[item] = true;
        }
    }
    return newArray;
}
///异步获取平台
function getPlatForm() {
  
    var myhtml = "";
    var softid = $("#inputversionselect").val();
    for (var i = 0; i < softs.length; i++) {
        if (softs[i].id == softid) {
            for (var j = 0; j < softs[i].platforms.length; j++) {
                if (j == 0) {
                    myhtml += '<div class="paddedRow"> <input type="radio" checked="checked" name="retainplat" id="plat' + softs[i].platforms[j].val + '" value=' + softs[i].platforms[j].val
                        + ' style="vertical-align:-3px"  class="platformcheck" onclick="platclickorout()" /><label for="plat' + softs[i].platforms[j].val + '" id="platlbl' + softs[i].platforms[j].val + '">' + softs[i].platforms[j].name
                        + '</label></div>';
                    $("#platformspan").text(softs[i].platforms[j].name);
                    $("#inputplatformselect").val(softs[i].platforms[j].val);
                }
                else {
                    myhtml += '<div class="paddedRow"> <input type="radio" name="retainplat" id="plat' + softs[i].platforms[j].val + '" value=' + softs[i].platforms[j].val
                        + ' style="vertical-align:-3px"  class="platformcheck" onclick="platclickorout()" /><label for="plat' + softs[i].platforms[j].val + '" id="platlbl' + softs[i].platforms[j].val + '">' + softs[i].platforms[j].name
                        + '</label></div>';
                }
            }
        }
    }
    $("#selectdataplat").html(myhtml);
}
////最上面菜单结束


///其他的设置
$(function () {
    ///报表制作
    chart = new Highcharts.Chart({
        chart: {
            renderTo: 'containner',
            defaultSeriesType: 'pie',
            plotBackgroundColor: null,
            plotBorderWidth: null,
            plotShadow: false

        },
        title: {
            text: reportTitle,
            style: { fontWeight: 'bold' }
        },
        tooltip: {
            formatter: function () {
                return "<b>" + this.point.name + ': </b>' + this.point.y + '人';
            }

        },
        plotOptions: {
            pie: {
                allowPointSelect: true,
                cursor: 'pointer',
                dataLabels: {
                    enabled: true,
                    color: '#000000',
                    connectorColor: '#000000',
                    formatter: function () {
                        return '<b>' + this.point.name + ': </b>' + Highcharts.numberFormat(this.percentage, 2) + ' %';
                    }
                },
                showInLegend: true
            }

        },
        series: chartjson,

        ///打印下载模块按钮设置
        navigation: {
            buttonOptions: {
                height: 30,
                width: 38,
                symbolSize: 18,
                symbolX: 20,
                symbolY: 16,
                symbolStrokeWidth: 2,
                backgroundColor: 'white'
            }
        },
        ///设置导出相关的参数，下面设置他的位置
        exporting: {
            buttons: {
                exportButton: {
                    enabled: false
                },
                printButton: {
                    x: -10
                }
            }
        },
        ///设置右下角的广告，我这里不启用他
        credits:
                {
                    enabled: false
                },
        //设置下面标题
        legend: {
            enabled: true
        }
    });
    ///报表制作结束

    ///图表表格的样式
    $("table").tablesorter({ widgets: ['zebra'],
        headers: { 0: { sorter: false }, 1: { sorter: false }, 2: { sorter: false} }
    });
    ///下载到excel
    $(".download2").click(function () {
        var mytp = $("#inputtype").val();
        var url = "ServerHandler.ashx?act=" + act + "&platform=" + $("#inputplatformselect").val() + "&soft=" + $("#inputversionselect").val() + "&zhouqi=" + $("#inputzhouqi").val() + "&mytp=" + mytp;
        window.open(url, "newexceldown", "height=200,width=400");
    });
    $(".printexcel").click(function () {
        ///打印
        printout();
    });
    ///查询提交表单
    $("#seachout").click(function () {
        checkcondition();
        if (ischecked == true) {
            $("#form1").submit();
            chart.showLoading();
        }
    });
    ///onready 结束
});
///提交前检查
function checkcondition() {

    if ($("#inputversionselect").val() == "") {
        alert("请选择软件");
        return false;
    }
    if ($("#inputplatformselect").val() == "" ) {
        alert("请选择平台");
        return false;
    }
    ischecked = true;

}
///打印实现
function printout() {
    var BrowserAgent = navigator.userAgent;
    ///打印完回退
    // var css="<style>body{font-size:14px;color:#ccc;margin:0 0 0 0;}";
    var css = '<link rel="stylesheet" type="text/css" href="../css/printScreen.css" />';
    var headstr = "<html><head>" + css + "</head><body>";
    //var headstr = "<html><head></head><body>";
    var footstr = "\<script\>window.print()\</script\></body></html>"
    var newstr = document.getElementById("mytable").innerHTML;
    var str = headstr + newstr + footstr;
    var printWindow = window.open("", "_blank");
    printWindow.document.write(str);

    printWindow.document.close();
    ///不是谷歌浏览器设置
    if (BrowserAgent.indexOf("Chrome") == -1) {
        ///取消就关闭
        printWindow.close();
    }
    return false;

}
         
        
    