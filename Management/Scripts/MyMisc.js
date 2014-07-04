(function ($) {
    $.extend($,
        {
            //显示消息：如果有easyui，则调用easyui的message组件显示消息
            alertMsg: function (msg, icon, funcSuc) {
                //error,question,info,warning
                if (layer) {
                    $.layer({
                        title: false,
                        shadeClose: true,
                        closeBtn: [0, true],
                        shade: [0],
                        time: 2,
                        move: false,
                        btns: 0,
                        end: function () {
                            if (funcSuc) funcSuc();
                        },
                        dialog: {
                            type: icon,
                            msg: msg
                        }
                    });
                } else {
                    alert(msg);
                    if (funcSuc) funcSuc();
                }
            },
            //统一处理 返回的json数据格式
            procAjaxData: function (data, funcSuc, funcErr) {
                if (!data || !data.Status) {
                    return;
                }

                switch (data.Status) {
                    //success
                    case 101:
                        if (data.Msg && data.Msg.trim() != "") {
                            $.alertMsg(data.Msg, 10, function () {
                                if (funcSuc) {
                                    funcSuc(data);
                                }
                            });
                        } else {
                            if (funcSuc) {
                                funcSuc(data);
                            }
                        }
                        break;
                        
                    //error
                    case 102:
                        if (data.Msg && data.Msg.trim() != "") {
                            $.alertMsg(data.Msg, 9, function () {
                                if (funcErr) {
                                    funcErr(data);
                                }
                            });
                        } else {
                            if (funcErr) {
                                funcErr(data);
                            }
                        }
                        break;
                    case 103:
                        $.alertMsg(data.Msg, 1, function () { window.location = data.BackUrl; });
                        break;
                    
                    case 104:
                        $.alertMsg(data.Msg, 5, function() {
                            if (funcSuc) {
                                funcSuc(data);
                            }else if (funcErr) {
                                funcErr(data);
                            }
                        });
                }
            }
        });
}(jQuery));

function ajaxJump(jsonData) {
    $.procAjaxData(jsonData, function (data) {
        if (data.BackUrl && data.BackUrl.length > 0) {
            location.href = data.BackUrl;
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

chineseLang =
{
    "sProcessing": "处理中...",
    "sLengthMenu": "每页 _MENU_ 项",
    "sZeroRecords": "没有匹配结果",
    "sInfo": "第 _START_ 至 _END_ 项，共 _TOTAL_ 项",
    "sInfoEmpty": "显示第 0 至 0 项结果，共 0 项",
    "sInfoFiltered": "(由 _MAX_ 项结果过滤)",
    "sInfoPostFix": "",
    "sSearch": "搜索:",
    "sUrl": "",
    "sEmptyTable": "表中数据为空",
    "sLoadingRecords": "载入中...",
    "sInfoThousands": ",",
    "oPaginate": {
        "sFirst": "首页",
        "sPrevious": "",
        "sNext": "",
        "sLast": "末页"
    },
    "oAria": {
        "sSortAscending": ": 以升序排列此列",
        "sSortDescending": ": 以降序排列此列"
    }
};