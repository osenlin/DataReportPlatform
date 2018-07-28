
/*
<script src="/Scripts/HeadScript/jquery-ui.min.js" type="text/javascript"></script>
<script src="/Scripts/HeadScript/jquery.multiselect.js" type="text/javascript" charset="GBK"></script>
<script src="/Scripts/HeadScript/jquery.multiselect.filter.js" type="text/javascript" charset="GBK"></script> 必须引用这两个*/
//id 就是绑定控件,noneSelectedText 未选择的情况下的问题,ismultiple 是否是多选 
//beforecolse 在关闭前执行的动作 beforeopen 在打开前执行的动作  needsearch 是否需要搜索
function createSelectControl(id, noneSelectedText, ismultiple, beforecolse, beforeopen, needsearch, needhead, height, minWitdh, afterselect) {
    if (needhead == undefined)
        needhead = true;
    if (height == undefined)
        height = 275;
    if (minWitdh == undefined)
        minWitdh = 170;

    var mycontrol = $("#" + id).multiselect({
        multiple: ismultiple,
        noneSelectedText: noneSelectedText,
        header: needhead,
        height: height,
        minWidth: minWitdh,
        selectedList: 100,
        uncheckAllText: '清空',

        showCheckAll: true,
        checkAllText: '',
        beforeclose: beforecolse,
        beforeopen: beforeopen

    });
    if (needsearch)
        mycontrol.multiselectfilter({ label: "搜索", width: 60, placeholder: "关键字" });
    var resultControl = {
        mycontrol: mycontrol,
        getSelectValue: function () {
            return $("#" + id).val();
        },
        refresh: function () {
            mycontrol.multiselect('refresh');
        }
    };
    
    return resultControl;
};

function createSimpleSelectControl(id, noneSelectedText, ismultiple, beforecolse, beforeopen, needsearch, afterselect) {
    var height = 275;
    var minWitdh = 170;

    var mycontrol = $("#" + id).multiselect({
        multiple: ismultiple,
        noneSelectedText: noneSelectedText,
        height: height,
        minWidth: minWitdh,
        selectedList: 100,
        uncheckAllText: '清空',
        showCheckAll: true,
        checkAllText: '',
        beforeclose: beforecolse,
        beforeopen: beforeopen,
        click:afterselect
    });
    mycontrol.boolMulti = true;
    if (needsearch)
        mycontrol.multiselectfilter({ label: "搜索", width: 60, placeholder: "关键字" });
    var resultControl = {
        mycontrol: mycontrol,
        getSelectValue: function() {
            return $("#" + id).val();
        },
        refresh: function() {
            mycontrol.multiselect('refresh');
        },
        getCheckedCount: function () {
           return mycontrol.multiselect('getChecked');
        }

};
    return resultControl;
};

/*
 *   add by :xuminghui_9101942
 *   date: 20141211
 *   1、multiple属性判断是否多选
 *   2、beforeClose、beforeOpen默认返回true，回调时传递当前元素作为参数
 *   3、_91SimpleSelect(id) 已存在该控件直接返回
 *   4、使用对象传递配置参数
 */
function _91SimpleSelect(id, config) {
    
    _91SimpleSelect.controls = _91SimpleSelect.controls || {};

    //控件存在直接返回
    if (_91SimpleSelect.controls[id])
        return _91SimpleSelect.controls[id];

    var item = $("#" + id);
    config = $.extend({
        defaultText: "请选择",
        ismultiple: item.attr("multiple") || false
    }, config);
    
    var control = createSelectControl(id, config.defaultText, config.ismultiple, function () {
        if (typeof (config.beforeClose) == "function")
            return config.beforeClose(item) !== false;
        return true;
    }, function () {
        if (typeof (config.beforeOpen) == "function")
            return config.beforeOpen(item) !== false;
        return true;
    }, true);

    _91SimpleSelect.controls[id] = control;

    return control;

};