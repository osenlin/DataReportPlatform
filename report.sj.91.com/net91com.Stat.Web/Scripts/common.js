function GetSoftList() {
    var str = "";
    var rex = new RegExp(".*" + $("#txtSearch").val().toLowerCase() + ".*");

    for (var i = 0; i < softs.length; i++) {
        var obj = softs[i];
        if (obj.name.toLowerCase().search(rex) != -1 || obj.py.toLowerCase().search(rex) != -1)
            str += '<div class="paddedRowTwo"><input type="radio" name="softradio" class="softcheck" style="vertical-align: -3px"  value="' + obj.name + '" />' + obj.name + '</div>';
    }
    $("#productItme").html(str);
}
//<!--这个是为了兼容存储的不是名称而是id 的形式-->
function GetSoftList2() {
    var str = "";
    var rex = new RegExp(".*" + $("#txtSearch").val().toLowerCase() + ".*");

    for (var i = 0; i < softs.length; i++) {
        var obj = softs[i];
        if (obj.name.toLowerCase().search(rex) != -1 || obj.py.toLowerCase().search(rex) != -1)
            str += '<div class="paddedRowTwo"><input type="radio" name="softradio" class="softcheck" style="vertical-align: -3px"  value="' + obj.id + '" />' + obj.name + '</div>';
    }
    $("#productItme").html(str);
}


function GetSoftListCheckBox() {
    var str = "";
    var rex = new RegExp(".*" + $("#txtSearch").val().toLowerCase() + ".*");
    var count = 0;
    //debugger;
    for (var i = 0; i < softs.length; i++) {
        var obj = softs[i];
        if (obj.name.toLowerCase().search(rex) != -1 || obj.py.toLowerCase().search(rex) != -1) {
            if (count == 0)
                str = '<table class="selectdate" style="border-collapse:collapse;width:100%">';
            count += 1;
            if (count % 2 == 0) {
                str += '<td style="width:50%;padding:10px 4px"><input type="checkbox" class="versioncheck" />' + obj.name + '</td></tr>';
             }
            else {
                str += '<tr class="paddedRow"><td style="width:50%;padding:10px 4px"><input type="checkbox" class="versioncheck" />' + obj.name + '</td>  ';
            }
        }

    }
    if (count % 2 != 0)
        str += "<td></td></tr></table>";
    else
        str += "</table>";
    $("#productItme").html(str);
}

//<!--这个是为了兼容存储的不是名称而是id 的形式-->
function GetSoftListCheckBox2() {
    var str = "";
    var rex = new RegExp(".*" + $("#txtSearch").val().toLowerCase() + ".*");
    var count = 0;
    //debugger;
    for (var i = 0; i < softs.length; i++) {
        var obj = softs[i];
        if (obj.name.toLowerCase().search(rex) != -1 || obj.py.toLowerCase().search(rex) != -1) {
            if (count == 0)
                str = '<table class="selectdate" style="border-collapse:collapse;width:100%">';
            count += 1;
            if (count % 2 == 0) {
                str += '<td style="width:50%;padding:10px 4px"><input type="checkbox" class="softcheck" value="'+obj.id +'" />' + obj.name + '</td></tr>';
            }
            else {
                str += '<tr class="paddedRow"><td style="width:50%;padding:10px 4px"><input type="checkbox" class="softcheck" value="'+obj.id+'"/>' + obj.name + '</td>  ';
            }
        }

    }
    if (count % 2 != 0)
        str += "<td></td></tr></table>";
    else
        str += "</table>";
    $("#productItme").html(str);
}




function GetSoftListByIDValue() {
    var str = "";
    var rex = new RegExp(".*" + $("#txtSearch").val().toLowerCase() + ".*");

    for (var i = 0; i < softs.length; i++) {
        var obj = softs[i];
        if (obj.name.toLowerCase().search(rex) != -1 || obj.py.toLowerCase().search(rex) != -1)
            str += '<div class="paddedRowTwo"><input type="radio" name="softradio" id="soft'+obj.id+'" class="versioncheck" style="vertical-align: -3px" value="'+obj.id+'" /><label for="soft'+obj.id+'" id="softlbl'+obj.id+'">'+obj.name+'</label></div>';
    }
    $("#productItme").html(str);
}


function SelectAll() {
    $("input[checktype='1']").attr('checked', $("#checkbox11").attr('checked'));
}

///全局打印的方法
function printout() {
    var BrowserAgent = navigator.userAgent;
    ///打印完回退
    // var css="<style>body{font-size:14px;color:#ccc;margin:0 0 0 0;}";
    var css = '<link rel="stylesheet" type="text/css" href="/css/printScreen.css" />';
    var headstr = "<html><head>" + css + "</head><body>";
    //var headstr = "<html><head></head><body>";
    var footstr = "\<script\>window.print()\</script\></body></html>"
    var newstr = document.getElementById("mytable").innerHTML;
    var str = headstr + newstr + footstr;
    var printWindow = window.open("", "_blank");
    printWindow.document.write(str);

    printWindow.document.close();
    if (BrowserAgent.indexOf("Chrome") == -1) {
        ///取消就关闭
        printWindow.close();
    }
    return false;
}

    
       
