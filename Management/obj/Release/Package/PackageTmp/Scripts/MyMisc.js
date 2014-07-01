(function ($) {
    $.extend($,
        {
            //显示消息：如果有easyui，则调用easyui的message组件显示消息
            alertMsg: function (msg, title, funcSuc) {
                //error,question,info,warning
                if ($.messager) {
                    $.messager.alert(title, msg, "info", function () {
                        if (funcSuc) funcSuc();
                    });
                } else {
                    alert(title + "\r\n     " + msg);
                    if (funcSuc) funcSuc();
                }
            },
            //统一处理 返回的json数据格式
            procAjaxData: function (data, funcSuc, funcErr) {
                if (!data || !data.Status) {
                    return;
                }

                switch (data.Status) {
                    case 101:
                        if (data.Msg && data.Msg.trim() != "") { $.alertMsg(data.Msg, "系统提示", function () { funcSuc(data); }); }
                        else funcSuc(data);
                        break;
                    case 102:
                        if (data.Msg && data.Msg.trim() != "") {
                            $.alertMsg(data.Msg, "系统提示", function () {
                                if (funcErr) {
                                    funcErr(data);
                                }
                                else {
                                    funcSuc(data);
                                }
                            });
                        }
                        else funcErr(data);
                        break;
                    case 103:
                        $.alertMsg(data.Msg, "系统提示", function () { window.location = data.BackUrl; });
                        break;
                }
            }
        });
}(jQuery));

function ajaxJump(jsonData) {
    $.procAjaxData(jsonData, function (data) {
        if (data.BackUrl && data.BackUrl.length > 0) {
            window.location = data.BackUrl;
        }
    });
}

function cancelAction(targetUrl) {
    location.href = targetUrl;
}

function operateSelectedDatagrid(idStr,callback) {
    var selectedObj = $(idStr).datagrid('getSelected');
    if (selectedObj) {
        callback(selectedObj);
    } else {
        $.alertMsg("请选择一条记录！", "提示");
    }
}

function operateSelectedTreegrid(idStr, callback) {
    var selectedObj = $(idStr).treegrid('getSelected');
    if (selectedObj) {
        callback(selectedObj);
    } else {
        $.alertMsg("请选择一条记录！", "提示");
    }
}

function caTypeFormatter(value, row, index) {
    if (value == 'Controller') {
        return '控制器';
    }
        
    if (value == 'Action') {
        return '操作';
    }

    return "";
}

function mpTypeFormatter(value, row, index) {
    if (value == 'Market') {
        return '市场';
    }

    if (value == 'Product') {
        return '产品';
    }

    return "";
}

function gridBeforeLoad(row, param) {
    if (!row) {    // load top level rows
        param.id = 0;    // set id=0, indicate to load new page rows
    }
}