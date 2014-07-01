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
                if (!data || !data.Statu) {
                    return;
                }

                switch (data.Statu) {
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