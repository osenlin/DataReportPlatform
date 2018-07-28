var platform = { "1": "iPhone", "2": "WM", "3": "S60", "4": "Android" }
var enum_dbtype = { "0": "DEFAULT", "1": "APPSTORE", "2": "WINPHONE" }
var pop = {
    softDownUrsl: null,
    target: null,
    init: function () {
        this.target = $(document).find("#popDiv")
        this.target.hide();
    },
    page: 1,
    size: 4,
    index: 0,
    pagecount: 1,
    pt: "proj=100&",//fw=0
    url: "http://ressearch.sj.91.com/service.ashx?",
    urlItune: "http://ressearch2.sj.91.com/service.ashx?act=9&",
    urlIndia: "http://ressearch.macrobile.com/service.ashx?",
    urlEn: "http://ressearch.moborobo.com/service.ashx?",
    platform: 0,
    restype: 0,
    postAjax: function () {
        this.index = 0;
        if (this.key.length > 9) {
            pop.target.hide();
            return;
        }
        var destVisitUrl = this.url;
        switch (this.restype) {
            case 1:
                destVisitUrl = this.urlItune;
                break;
            case 100:
                destVisitUrl = this.urlIndia;
                break;
            case 101:
                destVisitUrl = this.urlEn;
                break;
            default:
                destVisitUrl = this.url;
                break;
        }
        var pf = (this.platform == 7 ? "1&pad=1" : this.platform);
        $.ajax({
            url: destVisitUrl + this.pt + "&keyword=" + encodeURIComponent(this.key) + "&page=" + this.page + "&size=" + this.size + "&platform=" + pf + "&callback=?",
            dataType: "jsonp",
            success: this.searchSuccess
        });

    },
    first: function () {
        if (this.page == 1)
            return;
        this.page = 1;
        this.postAjax();
    },
    pre: function () {
        if (this.page > 1) {
            this.page--;
        } else
            return;
        this.postAjax();
    },
    next: function () {
        if (this.page < this.pagecount) {
            this.page++;
        } else
            return;
        this.postAjax();
    },
    end: function () {
        if (this.page == this.pagecount)
            return;
        this.page = this.pagecount;
        this.postAjax();
    },
    fromSelect: function (tep) {
        this.target.find('table[index=' + this.index + ']').removeClass("hover");
        if (tep == -1 && this.index > 0) {
            this.index--;
        } else if (tep == 1 && this.index < this.target.find('table[index]').length - 1) {
            this.index++;
        }
        this.target.find('table[index=' + this.index + ']').addClass("hover");
    },
    toSelect: function () {
        var o = this.target.find('table[index=' + this.index + ']');
        if (o.length > 0) {
            alert(o.attr("fid") + "_" + o.attr("fname"));
            pop.target.hide();
        }
    },
    flip: function (obj) {
        switch (obj.innerHTML) {
            case "首页":
                {
                    this.first();
                } break;
            case "上页":
                {
                    this.pre();
                } break;
            case "下页":
                {
                    this.next();

                } break;
            case "尾页":
                {
                    this.end();
                } break;
        }
    },
    searchSuccess: function (backdata, textStatus) {
        pop.target.empty();
        if (backdata && typeof (backdata) == "string") {
            try {
                backdata = eval("[" + backdata + "]")[0];
            }
            catch (e) {
                pop.target.hide();
                return;
            }
        }
        if (backdata == undefined || backdata == "") {
            pop.target.hide();
            return;
        }
        var thisPos = $('#input_f_showname').position();
        pop.target.css({ 'top': thisPos.top + 30, 'left': thisPos.left });
        var i = 0;
        while (backdata.data[i]) {
            var _table = $("<table index='" + i + "' class='" + (i == 0 ? "hover" : "") + "' style='width:100%;border-bottom:1px solid #C9CCD3; ' fid='" + backdata.data[i].f_id + "' fname='" + backdata.data[i].f_name + "' fimgsrc='" + backdata.data[i].f_imgsrc + "' fsummary='" + backdata.data[i].f_summary + "'><tr><td rowspan='4' style='width:64px;'><img style='width:64px;height:60px;margin:5px 8px 0px 8px;' src='" + backdata.data[i].f_imgsrc + "' /></td><td style='width:200px;'><b>" + backdata.data[i].f_name + "</b></td><td style='width:*;' rowspan='4'>ID:" + backdata.data[i].f_id + "</td></tr><tr><td style='color:#FF8C18'>作者:" + backdata.data[i].f_author + "</td></tr><tr><td style='color:#FF8C18'>分类:" + backdata.data[i].f_category_name + "</td></tr><tr><td style='color:#FF8C18;'>平台:" + platform[backdata.data[i].f_platform] + "</td></tr></table>")[0];
            pop.target.append(_table);
            i++;
        }
        if (i == 0) {
            pop.target.hide();
            return;
        }
        pop.pagecount = Math.ceil(backdata.cnt / pop.size);
        var info = "<font style='float:left;'>显示 第" + ((pop.page - 1) * 4 + 1) + "-" + pop.page * 4 + "条</font>"
        var _table = $("<div id='pagebottom' style='width:95%;text-align:right;height:30px;line-height:30px;padding-left:15px;' unclose>" + info + "<span unclose>首页</span><span unclose>上页</span><span unclose>下页</span><span unclose>尾页</span></div>")[0];
        pop.target.append(_table);

        pop.target.find('table').bind('mouseenter', function () {
            var _index = parseInt($(this).attr("index"));
            if (_index != pop.index) {
                pop.target.find('table[index=' + pop.index + ']').removeClass("hover");
                pop.index = _index;
                $(this).addClass("hover");
            }
        });
        pop.target.find('div span').bind('click', function () {
            pop.flip(this);
        });
        pop.target.find('table').bind('click', function () {
            pop.toSelect();
        });
        pop.target.show();
    }
}