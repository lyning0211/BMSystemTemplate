//是否是合法令牌
var isLegalToken = true;
function checkUserToken(p) {
    var clientToken = sessionStorage.getItem("token");
    $.ajaxSettings.async = false;
    var root = sessionStorage.getItem("root");//GetCookie("root");
    var myUrl = "../../UserToken.ashx";
    if (p == "1")
        myUrl = "../UserToken.ashx";
    else if (p == "0")
        myUrl = "./UserToken.ashx";
    $.post(myUrl, { "token": clientToken }, function (data) {
        if (data == "false") {
            isLegalToken = false;
            var sessionObj = sessionStorage.getItem("session");
            if (sessionObj.Language == "zh-CN")
                window.location.href = root + "logout.html";
            else
                window.location.href = root + "logout_en.html";
        }
        else if (data.indexOf("alert") >= 0) {
            isLegalToken = false;
            window.location.href = root + data;
        }
        else {
            localStorage.setItem("count_down", new Date().toDateformat("yyyy-MM-dd HH:mm:ss"));
        }
    });
    $.ajaxSettings.async = true;
}


//得到错误信息
function getError(req_guid) {
    var clientToken = sessionStorage.getItem("token");
    $.ajaxSettings.async = false;
    var root = sessionStorage.getItem("root");//GetCookie("root");
    $.post("../../GetErrorInfo.ashx", { "req_guid": req_guid }, function (data) {
        myAlert(data);
    });
    $.ajaxSettings.async = true;
}

//是否全选
function checkAlls(ele) {
    $('[name="chkItem"]:visible').not("[disabled]").attr("checked", $(ele).is(":checked"));
}

//得到选中的值
function GetCheckItemVal(myID) {
    var s = "";
    $("#" + myID + " [name='chkItem']:checked").each(function (i) {
        i == 0 ? s += $(this).val() : s += "," + $(this).val();
    });
    return s;
}

//得到值
function GetItemVal(myID) {
    var s = "";
    $("#" + myID + " [name='chkItem']").each(function (i) {
        i == 0 ? s += $(this).val() : s += "," + $(this).val();
    });
    return s;
}

jQuery.extend({
    /**
    * 清除当前选择内容
    */
    unselectContents: function () {
        if (window.getSelection)
            window.getSelection().removeAllRanges();
        else if (document.selection)
            document.selection.empty();
    }
});
jQuery.fn.extend({
    /**
    * 选中内容
    */
    selectContents: function() {
        $(this).each(function(i) {
            var node = this;
            var selection, range, doc, win;
            if ((doc = node.ownerDocument) && (win = doc.defaultView) && typeof win.getSelection != 'undefined' && typeof doc.createRange != 'undefined' && (selection = window.getSelection()) && typeof selection.removeAllRanges != 'undefined') {
                range = doc.createRange();
                range.selectNode(node);
                if (i == 0) {
                    selection.removeAllRanges();
                }
                selection.addRange(range);
            }
            else if (document.body && typeof document.body.createTextRange != 'undefined' && (range = document.body.createTextRange())) {
                range.moveToElementText(node);
                range.select();
            }
        });
    },
    /**
    * 初始化对象以支持光标处插入内容
    */
    setCaret: function() {
        if (!$.browser.msie) return;
        var initSetCaret = function() {
            var textObj = $(this).get(0);
            textObj.caretPos = document.selection.createRange().duplicate();
        };
        $(this).click(initSetCaret).select(initSetCaret).keyup(initSetCaret);
    },
    /**
    * 在当前对象光标处插入指定的内容
    */
    insertAtCaret: function(textFeildValue) {
        var textObj = $(this).get(0);
        if (document.all && textObj.createTextRange && textObj.caretPos) {
            var caretPos = textObj.caretPos;
            caretPos.text = caretPos.text.charAt(caretPos.text.length - 1) == '' ? textFeildValue + '' : textFeildValue;
        }
        else if (textObj.setSelectionRange) {
            var rangeStart = textObj.selectionStart;
            var rangeEnd = textObj.selectionEnd;
            var tempStr1 = textObj.value.substring(0, rangeStart);
            var tempStr2 = textObj.value.substring(rangeEnd);
            textObj.value = tempStr1 + textFeildValue + tempStr2;
            textObj.focus();
            var len = textFeildValue.length;
            textObj.setSelectionRange(rangeStart + len, rangeStart + len);
            textObj.blur();
        }
        else {
            textObj.value += textFeildValue;
        }
    }
});

//弹出导出文件选择菜单层
$.extend({
    facebox: function(title) {
        if ($("#facebox").length == 0) {
            var str = '\<div id="facebox" class="facebox" style="display:none;"> \
                            <a class="close" onclick="$.faceboxClose(true);"> </a> \
                            <h1 class="TitleTck">' + title + '</h1>\
                            <table border="0" cellspacing="0" style="line-height:25px;width:120px; margin:0px 15px;">\
                                <tr><td><input name="filetype" id="Radio1" type="radio" value=".xls" checked="checked" /> Excel File (*.xls) </td></tr> \
                                <tr><td><input name="filetype" id="Radio2" type="radio" value=".doc" /> Word File (*.doc) <td></tr> \
                                <tr><td><input name="filetype" id="Radio3" type="radio" value=".chm" disabled="disabled" /> CHM File (*.chm) <td></tr> \
                                <tr><td><input name="filetype" id="Radio4" type="radio" value=".pdf" disabled="disabled" /> PDF File (*.pdf) <td></tr> \
                                <tr><td><input name="filetype" id="Radio5" type="radio" value=".sas" disabled="disabled" /> SAS File (*.sas) <td></tr> \
                                <tr><td><input name="filetype" id="Radio6" type="radio" value=".sps" disabled="disabled" /> SPSS File (*.sps) <td></tr> \
                                <tr><td align="center"><span id="btnOK" class="imgbotton" onclick="$.faceboxExport();">OK</span><td></tr> \
                            </table>\
                        </div>';
            $(document.body).append(str);
        }
    },
    faceboxClose: function(animate) {
        if (animate)
            $("#facebox").hide("fast");
        else
            $("#facebox").hide();
    },
    faceboxExport: function() {
        try { onExportOK(); } catch (e) { }
        $.faceboxClose(false);
    }
});

//遮罩层:window.setTimeout("$.cover.hide();", 500);
$.extend({
    cover:
    {
        showLoad1: function ()    //进度条loading,用于frameset,中间带图
        {
            this.showLoad();
        },
        showLoad: function()    //进度条loading,用于frameset,中间带图
        {
            $.cover.hide();
            var str = "<div id='cover' style='position:absolute;background-color: #666; text-align:center;"
            str += "width:" + $.getDocument.width() + "px;";
            str += "height:" + $.getDocument.height() + "px;"
            str += "opacity:0.5;filter:alpha(opacity=50);"
            str += "top:0;left:0;z-index:1001;'>";
            str += "<div id='covertext' style=\"position: absolute; left: 50%; top: 50%; margin:-50px 0px 0px -50px;width:100px;height:100px;background-image:url(data:image/gif;base64,R0lGODlhZABkAMQAAP////f39+/v7+bm5t7e3tbW1szMzMXFxb29vbW1ta2traWlpZmZmZmZmYyMjISEhHt7e3Nzc2ZmZmZmZgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACH/C05FVFNDQVBFMi4wAwEAAAAh+QQFBwASACwFAAQAWQBaAAAF/6AkjmRpnuj5KMuTvnAsz3NkADhgRHTv/7RCLlcAGo/Gx3DoQjqfqcQyl4BarxLpFFDFekeOQwHhkDG2AMb3ywi43QfZYDpYex3vvBoGIQwHPDMQDoF2KAd5bwYzDAksPREKLIWGJQWJbkWGDguSTZUlCJgBmnYQkgsQoCZ4mHuGg5SrI4h5cbO4JwwFBAWvucDBwsPExcbHyMnKyzQRDJPMaw8DAtUDv9FPEdTV1qrZLw8tMQzd5grgKQ4J7J8oCubdi+knC+yPL+XxAuj0Jo3tYGzb980fiQicysSAEA+bQSAROiko+LCixYsYM2rcyLGjx48gQ4oceTHiAQMJZP+RlGBjgMsBBFR+dKCQgUsBL7ugwHEihwmfJIaU4CmCKNERQlNESIDAkQQDL18SeHEUKQAJVXlmvbrVKterVmU4aIoAwYMDOG/CpAo26Fe3QIu+DSs3qVysbU+MJQthQVSXOnvmvdu1cN6jWg2/WMq0X4GoMdlWxTs4MdzKbY1iHozCwSeTKGWuHE26tOnTqFOrXs26teuQ7k4vIEC7X8YIsVPMpl0749gDZGLwog0zssUIB4ADj0GcN4Hc9CAkVy56xHDn1bNFQKAcATneMG1b/J2AYopdvRZwzP66vfv38OPLN322wAGFoBwwgL7qQQFe/+FnRwIGFKheMAn0AuBfLXY4YMBJBZq3CgIAAlgJAwVmKOAsCfynYGBfQFAghBKCAsF/APKHwgMqorBAhg7NckpTLZYgXQEPlpgCBBCwF81JONp32okGBGmAjiHZYGQBPnpEYJDikbZUkSkFEwIAIfkECQcAEgAsAAAAAGQAZAAABf+gJI5kaZ5oqq5s675wLM90bd94ru987zoOn3A4YggCAQGDyNQtkdBlc0o7QpECqhbmuF6D27Cq60WCxb0IQ8GItCJlpHsVSQgAgwR69RgI/gMPLQllenQEAImJA3soEX5/gBCDV4YrCIqZlo1FkZ4LLmoOky0BmYpZnCQKnpEKQ6eZqiQLrX+vQrGJqbMSj7ZzPge6m7MMkH9SQnCnBMG9EhAKBgrPy8MAAQfW0N3e3+Dh4uPk5eZhEaPnTREIA+8G3Os67u/w8yQQD/Iwj/b2gvBBWKCgmg0H/+yBwvegoISANCAkfHdmnTQFC0jVMJCwAL4R+jTWiMDxXQGRH3f/DISYsqXLlzBjypxJs6bNmzhz6tzJc9YDgsp6qlhAoCiBYjQj8CsBgYAEowSCzlygpyIKokWfEkBgU5qEBAmWisAKlWtNCGC/io2W1ahUmaASWL0KFelMpS9+LngrtK/fv4ADCx5MuLDhw4gTK+b0IEGBA3MDP/BYoHLkchEUIFhoQ09lAo9Tav7KmQaCAqApp0SAQI/ZGgoqS6h8IOUCrglKz4BQGXUBluvUIOArA21r4IuTK1/OvLnz54YjUDU4JQLKbiQNFDAQr4kDBAcQXObEQLt53Wm4hn/dDZT5AnZVHgg//7qq8hLME98Bfj77blxph8BaODwAHgL2zRLBLgMM7HcVd20ctsAB3BngYF8UGkBhbYYhUKEE/wm2RIYX9uWAAnJBp+KKLLa4YggAIfkEBQcAEgAsBQAFAFgAWQAABf+gJI5kaZ6o+RyCcDxpLM90PT9D2w6w7f9AmkGnMwSPyCORmGw6ZUvdcyqLPCC2QrRQcxwMBwe1FkEMzocIzbEMiGeHgFzOGM/MgvPAuM4JCG8yDHOEgXYmEHp6AlhkjUKEcweHKDiLZwuUEgWRclyaJYmKZz2HnJ0IoCYsip+Ug52GqhIRBnoEj5RxhJOzKgqymgwFBAV1vsjJysvMzc7P0NHS09R2EQkCAAIJMwwGBASZ1TQRBADn5wMxDAQD4AQK43Do9NwoxO/gavIpAfToBFLka0egFD8T/+ilwOdO30EUCc8JSLFgYLyHJw5EtIdiAb6LGE1E8EdP3Y2QMSL/aAQQAME+lDBjypxJs6bNmzhz6tzJs6fPGREcOMj1M9Q3YsGKbipAjNjLE+egRi2BjmpVElMlTM0qQmGMBwoWQHjQtOkxiAAQpuUale3atCOyVnVbQ4HdBWTLEjgrVa1WuF29Bv6LFXDCwoRjhA0r4WjTp36tJh5Mly7lyoBRQAirBoLjpIW5/s1M2K1ouXBPiyYnFLLS17Bjy55Nu7bt27hz697tMwKDBAtcK71WwECBNDcZLABNwlvx4iBn/laQgLmIYcaNc5y5IIF3viccZC8ujqYD7wkMniiTnY9NBwzUp1DuQDjv+/jz69/Pv/+0BwxY99MCBhS4nSYRKIBAVnDMePZFgQI6oSACCzLjRYFfgHcIAglwmMoynhVYIFGHLOBhecp48wWKCC6ggIbJWEGiDGApIN9rKxyAwAu0LaDjAQew+JqJQFY4Wxk6ulQbBPDNqEoIACH5BAkHABIALAAAAABkAGQAAAX/oCSOZGmeaKqubOu+cCzPdG3feK7vfO//QNriYGBEgkhdxDBoDgzHpFTodC6m2NhBUpVss2AWszr4hs+nhJM7QLgiDIURvWIUCIWri0yIrh4DAoIDD3QoDASJiXosEQmJCX4qEIGCg5KGIncEEokFQQuWogyZJYqnhT8KopYKpSR3nZycQKusAoyvC6cErkARtwOYpQqKvkEMogOkryURD8M/EAoGChDN2Nna29zd3t/g4eLeDggFBw44EAkICanjIgcB8/PMNBAF+RIF7+IM9ADT0VBwR58ZcQUA0jsYw1xBPPA6KZz3icYjfeciJpzohsYDjAUEjvs3UeSMBwnO/5kcJw8gw4g67JyzB7OmzZs4c+rcybOnz59Agwodyo1BAncvIlwDCoEAgKcACkQj4ciAl6U9B0CFalVFSgMFDLzEuSDAVqj9TlgFayAtTgVnoR5TK2FtAaw6E8R9mssEArtjby7YC8Btib9Xf2o923WF0qARFj+VStRFHKSVM2vezLmz58+gQ4seTfozBAYrRS9Ym+AmBAdTYVyzuiV1uGkSrN2AsLYuzXEOFAg3DOOIWKvEv0UQriD2C1JW+8KDAE3Ha7yls2vfzr279+/gd1TPXu4AAts+HCxAX8oNAvPOcxg9yp7OtfeJkZBKoED6q/cASuEAf5hpE4F79Ym3QCtyr2BnWYKcOcJOJKWVIwE7EGaWDjsSZJiZAm74F9oDDoZn4okopqiiCiEAACH5BAUHABIALAYABQBaAFkAAAX/oCSOZGmeqAktC5S+cCzPtLQQ+LDUfO/LjxyO8PgZj77bkKBDOp+vxUBIUECvWAljiWPQII5ItgZRIBLFWYKJS8wihoEcIR7DIIU8oZCWMRIIXjNxcnN2MAl6eW6HD4WPdYcnCAV7lQWSUo9yDpIoa4oHko6bAy6eJg+KBZ2SBZsGqCgPBwUHrZJ4hQaRsr4pDgt9v8TFxsfIycrLzM3Oz9A0EQsKDL0vEcGC0TEMAwLgA8MplAUGvNwvEN/g4dcmDubyO+koCu342ygM8gYFjPVMGMDXzsoLBv4SGgxY4h5BAfpQIEiIjmEJCA8HvDMR4c+CjRYZ4BNnEUoEBQpa/5RcybKly5cwY8qcSbOmzZs4c/LIhkvnpwBAIY5ggOAAvZwJgCoF4MXBuXMRTQAAcGIq1RJWsWYlMXVEVwlfvW5VobSsAAkKzh04ByOsWLBXRXR1OzcuXLFU6do94aBsWQcJ1j5tu/eu3rGG7Ya1irjuPr9KHTh9GlWr1LyKMXPV/FYu5sx3T0SAHOCslqLWCLuFu9cx3tZxv65GbCKp38o+bZfG7VNChAc9ewsfTry48ePIkytfztxnhANTA9D5ApLhaMYACFQf4aAoguAloWMHAJBc0QPTW46fajoFhAPo4Z9iuX71pPPpWQZYPyAGhMDfvSQeduVhE1MEA2DXn2JxJwkAgAAFNifhhBRWaOGFGJIwDQIKbHcFBA/Md8wCZ3AoSxkoeSgJICXK8gBKC4D3ywIsHpULjCpK8kdqJz6QYzoQiHjcHwnw5lwCCSiQwI81RYAkkkzWBFgCMhIX5RghAAAh+QQJBwASACwAAAAAZABkAAAF/6AkjmRpnmiqrmzrvnAsz3Rt33iu73zv/8Da4xEs9iCFZAFibN6UkoIh4qzGHMlogUC0elnYKGHr+JpTVGiBeu5FHAsHu4UdE8quxbbAaKMTEgYFCDARDAxzLAsEjIx9fiZlgoKPQFsSjIGQJguTgYBAD42jmyUMk1ILRpmNpSUGk1NFCqMEqq4kCwkKiUCqjAq4m3jCxcbHyMnKy8zNzs9BEA+9NRAOTNAiEQcD3QjUMaewt9AG3eeENREGB7AGXc4P3RLnA9gzEO7tlc0K9fQD4MlgYkATsWYO6p27N2NBOwmgoBH4dwDHAwYHn0GY2E1WNh9EGH4cSbKkyZMoU/+qXMmypcuXMGPKTIZFgIB3LSIsWCASpQObQAOugIDggFGBKAlIAGqzogoGB4oiCKYSAlOmK8pEPUA1ZYSrQIcaPcqSAFinKiAw4NmyDFOhM1XUvNkzrt27ePPq3cu3r9+/gAML9sEgiQF+fA8EWLwYbY0FU8EpY8C4csYYqhJMzVagMmPHMhAAQpDuGQHPiwvY6CO6q7POqEvTYICA3DPKqC/jVVwZ8V4HCAbpHky8uPHjyFluAwAgwAHJdr8yn04AehsHh4wcmM49oisHgBL45sGdu4BiunbZ9lGeezEiCRIM1xGgPYDz7zEaQWDfe94IBJQ3wEoQKKAAUipEkIAeAAAQ4N9JBi7gml8SKrBeXxDsVFdyHHbo4YcgghgCACH5BAUHABIALAYABgBZAFoAAAX/oCSOZGmeaBklhZFEaSzPdF0nRlskdu//vYguZ4ABj8gjhKiDJJ9Q2oF5iFppEYjTBjm0DlvaI4FQhK80hmG98EEevUeBMDec0anldH3HJ+gFgTx4Mg5rBlMMhCSBc3MIizFLhwYOkSJedHSDlycLiC6dEg6BgHCiJw8MlqgOXgenqLKztLW2t7i5uru8vb6/wLlGwTULjqwyEAkHCMjEKQoEAwTSijIIzMzDzyYR1N90ydgH5H3cIg/S1NMEM9jj2+cj3uDUMw/vzvIk0eoEbTQgxNvHj1oBgASjxErIsKHDhxAjSpxIsaLFixgzaty4YsqCYQ/0XfQ2oOSAIhIU/yBA8EIGAAAnXsIsIZNmTRIvR+SUsFPnzRMJTA4QMIABPjLNXM7ECbMnz6ZLn0r16RMq0xnShJ6EsJKMSJsmcjqVOdYqVRFky049kZVoySoKyLSM4RStWbtrparF+1Nv1BIItAqw5uBr2L5P/4qNShYs3ryJY5A0iXKjChwHPlrezLmz58+gQ4seTbq0aYkQFCjQ/JmBgNdEF6ZwsMCaQwiwYQ8YWMJBgri2GSrIDTu4p7gJEDI0QPy1AhljEiQwfG54c+OpFshOCGFo7t2eGXgnao7jAgUMeJ9ez769+/e2GDjCbvlAgPv3q9yK4KD8FQb4BUhdJKmp5l8UBQSIn2N+tDygmgIDQpGgggEUYEsE5y1woAnnJVeDARQGAMmFD2xIQndkEWCiAyFG6MsAaQEwgBQK0kcMAzG+ZGNvCBSQVEIJ5AgAJxsFmSORGj0g5HYZFRCjhZ1FYABZlXnmoHa4hAAAIfkECQcAEgAsAAAAAGQAZAAABf+gJI5kaZ5oqq5s675wLM90bd94ru987//AoHAocyQSDqLSxpAYnM2lFIZwOqvTLOvw5B604FTzaYiGz6QFlxGBMRiONvoEQRwQSWEEQUbI5yRVdlhBDGRQgCQQd18IEEIJTgUGBWaJEnaMfz8LlJ6WiUYSBw9DEQaeT5clEJtBEQoJC66rtba3uLm6u7y9vr/AwcJDDwkICY84DLK0vA8FBAXSyTULVQkKwgfSEtLZNsbGhL/Q0tHjMwuRCN/A293QkTfWCs27DubSpcM7DtsJ+/gJHEiwoMGDCBMqXMiwocOHECNKnMJgwYKALCLYI5iAgAQCBBa0MCJLIQOQKAn/UEMRIRK2lQURoPwYcgUslzAJykxZc0USBSITnvQIckDOExoZdkQZdCIKBkAxOp1KtarVq1izat3KtavXqw8uboVQYIBZAxtfQHBwlFdZs2a/2IAgUkHbXA7MSoAr4W6LUgoUSOUlkq/ZwTBgBfZ76wHcx4wzPoh868ljdFQjHIDrZ2ycr6BDix5N2ofmAAAAHEibA5YBu7UiEEhNOwBrJgJyCxgACk0C2sDl+oCgW/eA20sEAAcOREFx3U3nLGf+w/lzAe3mKJ8ewAUEOJQlLLguoHeY39Mxn0gQoH0AeSsgDHh+fNWA5fVXsHffHr4K4sYhhoYC92F3WwT88YdcLARQsWGQAwm6lwdVEEYYwIRUCRChAFgxEKF5TjGgYQDlceUAhqWlqOKKM4QAACH5BAUHABIALAcABgBYAFkAAAX/oCSOZGmeaAk5DJS+cCzPc4QcR+LSfO/TixuO8SsajwvcbXFsOmWPg/DxrFpLD4WCyoNAIlfeQrEAh00Lg5p4hgUTCGZ7BFEbDobd/ISA9/ciDHd2DoAnQQgICoYOdmp6hiMRQQqQcwlqB3KRnCksXJ2hoqOkpaanqKmqq6ytrq+wPxEMC2wxDwyFsS8RagUGCGYpDwnFuruHBr+/xygLxWTIKAnLyrYoxMXX0iMKyt/bJw613Ce93wflTZMJDMLq8PHy8/T19vf4+fr7/P3+/8giOHDwDqAEBwQKEMgjAcIYSycAAIgo0YTEiSQuYhxRUUTHjhwvpoiQsKQBCWO0/8QAGVICy4ovJ8YMKXOjS5soGBTYyfNBykUwWHqsmVFj0ZtHhxptKZSEzpIKvyxYACpFU5g2sR6dOfRmVqIoIijcubDHUpoltNLECfIj26YlECpkaFCSgwcF6+rdy7ev37+AAwseTLiNjQASD+RVVzUsAY0ABCxGpoBAwk0nEkBOHK8ygQGWMZcQsBluLJKWLQ8oYLU0TmQPPqe2nGJA6QDwUMtOmELz5nTwPKcOR8K2RgKTdy2YS7xEAtICEiQvTL269evYVz0t0DzMuMacDgQYPx74HAgFBggYYGC6d/LwuztJP6D+APORxMMfz7rNA/v1rQfRHAXsx98cCgBoH2B4cyBgYAAnzfAFDRDYt159A7bhwIPNeEKAAOt1mAJ99iEgCgP74edJACC2KKI5B9in2CgOIFAAAi+eUECLLfYnAwR4wcMjj3UN2WJdBhgZIUD/8ThAhvtAcACIB0C5RwgAIfkECQcAEgAsAAAAAGQAZAAABf+gJI5kaZ5oqq5s675wLM90bd94ru987//AoHA4e0CISJwigVAknzKHBEGVQq8sB5NpxXpPESYiEfmaT47umfdYPJAOxAFxXJseiaUaKO/bTQsSCQmBQhBzB3N1fyJaeQxDEYl+jCQOC3tAD5NvlV8Ri56io6SlpqeoqaqrrK2urzkQDqE2EUawgQYSkDcRCwoKZa1vBxK6wjUPwJiuDLrPtDIQvwrRqFIGB7rWMRGzsAnGEoWwOw4M3OXq6+zt7u/w8fLz9PX29/gkeAm8+SgRBQAIBDAgXTkIyFQYGDhwALxDBgqQUfGAIUNy7Io9C5dCgcWBTto9eCZOxYKPAjn/sjtCUleKiigxrjtQYOOKhRYdvjtkDAELgAwJGISF8MW+fv6SKl3KtKnTp1CjSp1K9QUDmgWQNj0QoGvXYq0c0EzQiYYDr2i1nnJQoAABt2VlcEXbtQArmhLsSqxRgG5XAqzctm3rkwYCvwHsrsL6VkLIGWf9qjX1QG/boSsY0AXLCs8YzFkQFECQqarp06hTq0YdQYGBakhsJWTEYICA2wIm81BAoPfjPxFs474NmgZvwL1lnlkwHPfvHsh7N2akoPnt5zseSO8tATBt6wKU73BLYEBvxcCF4x4wOwWC3hNbLNhOQPcXCOoHxE0RofyA/wS0h8ICgtlnRgQMKMCAMYAnJPDfgwOoxNQBD0rwn0tNGQDhf5wxtYBDFYqXVAEQGsCgPwhmI+JqLLbo4osqhAAAIfkEBQcAEgAsBAAGAFoAWQAABf+gJI5kaZ5oKkVq675w7DpJ4sh4rsdRXbO7oHDX8wGHyGSLkVAwlNCoCQKRWlGRR/Vaiiyc3JdXsdiGJYsEArE4qx6K+MMtWasTdBQkrjCHFWpseXpaeT1sR4OKi4yNjo+QkZKTlJWWl5g5CQIAAgmJmUkDAKSkBKChJqgoCaWuB6koDggHCX4oBK6lAbGqtL8uuq69JRAHxwcItyacwgDEJBHJyQgurcKw0CMPxwg3LRG5rgGrvRHLKREIAaQH5drw8fLz9PX29/j5+vv8/Y0ODt75YyAggEE8/kwwMMgwAMJ9ERgkeyKhYEOD6ESQOlHKREcSw0A+0zhyo0iTKBj/GFhpAOBFht84jhQpAWXNZzY35pzZceeLlQcM1HLw0iBFmR5xzrz5keRNmiRDOrVpguVKBRWLvqP61KfXpSh1flWxIOjKGwsvPpRJtelUsG6fOpU7Iu6JsgiOEmS4NmEJgAL9Ch5MuLDhw4gTK17M+B6ELwoC73swQIBlAUfheUnAQHKEypctZwwVwUAB09WWhL6MFZ4C07Azn1Cw2rKBeAlO65atsLaA1toW6DYdEwvoywNGh4Jd4JMLysh5Q2OwICCMiGQkN97Ovbv37/weVEdcekDlAsqTODhQ4MCcRgfMyy9wxkGB+wQKvFcEwbwA+QLsZwV7+RVAQF90OCDfZ4JtcGEgfgWkpkgE//lnnoBS5FbggY0gsOAAt8HAAAKcxfDAfQYakN4V6shngHYSJEDAjBzC8IAafUASgQMrjsAAjQPMKF0/CtBII3CCLTBjkDM2ONgDRgaJoV8LMEmAk4U9VkYjIQAAIfkECQcAEgAsAAAAAGQAZAAABf+gJI5kaZ5oqq5s675wLM90bd94ru987//AoHBILBpPkcUCcmzGFgrFwkltRaNVYIIAECQir4f0kfUNAGg0AVwuJ9JwxNCxILdNAng6IHQoEgl2dyN6cEILgAlTgyN5hQBCDBKADowjb4UHQwwMlZYjZ3ABbJ9HEQcBaAekpa2ur7CxsrO0tbaMDggFB563PQwBwcGavjsOwsiSt0kIyjWoyMEFvn8SCIs0BdHBBL4JCAgJNwjbAdO3CpMS2DPH2861EArXONDCxMU7DAYFBfD5AAMKHEiwoMGDCBMqXMiwocOHOx4YECCgQC+IJx4MoEhRwsWC8hgwWXGAI8duBx//HEBwYNUKkyZHFlSgaeU/PDApsho4byUvkjlRGiTTEoFMFBA2mvxIEIKSnUgnVmSKsarVq1izat3KtavXr2DD8mAiqGuBAWgJHLXlQCQOA2jjnruVwIAEA+xkJI0blyqsSgYO2F0LQyNaCXHT2dp3V8KBsjH28h0AWZZEwXehwpATt7GvBXYN3IwRAUFcl8UgOCBMI8JqsbBjy55Nu3aJCA80Z1VAoLdiIRH+5GW0oLcE38A18TOgO8u03tAPNebnNwt04wQq76h7twDeUgWwE5j7Q1Joi6UYXCcwPEUETs1NlA6NIH4VBuELtEdRaRr6F5w4YJ8tYPg3zYAN9WdgIXUQOWCgBP9pFQE/EPbjFQT99MNaVg9oZ9uHIIYo4gkhAAA7); color:Yellow; font-size:22px; font-weight:bold; z-index: 1402;\" ></div>";
            str += "</div>";
            $(document.body).append(str);
        },
        showLoadText: function()    //进度条loading,显示文本
        {
            var str = "<div id='cover' style='position:absolute;background-color: #666; text-align:center;"
            str += "width:" + $.getDocument.width() + "px;";
            str += "height:" + $.getDocument.height() + "px;"
            str += "opacity:0.5;filter:alpha(opacity=50);"
            str += "top:0;left:0;z-index:1001;'></div>";
            str += "<div id='covertext' style=\"z-index: 1002; position: absolute; padding:5px; background-color:White; font-size:10pt;\" ></div>";
            $(document.body).append(str);
            $("#covertext").css(
            {
                left: ($(window).width() - $("#covertext").width()) / 2 + "px", top: ($(window).height() - $("#covertext").height()) / 2 + $(window).scrollTop() + "px"
            });
        },
        showLoadMessage: function(msg)    //进度条loading,显示文本
        {
            var str = "<div id='cover' style='position:absolute;background-color: #666; text-align:center;"
            str += "width:" + $.getDocument.width() + "px;";
            str += "height:" + $.getDocument.height() + "px;"
            str += "opacity:0.5;filter:alpha(opacity=50);"
            str += "top:0;left:0;z-index:1001;'></div>";
            str += "<div id='covertext' style=\"z-index: 1002; position: absolute; padding:5px; background-color:White; font-size:10pt;\" >" + msg + "</div>";
            $(document.body).append(str);
            $("#covertext").css(
            {
                left: ($(window).width() - $("#covertext").width()) / 2 + "px", top: ($(window).height() - $("#covertext").height()) / 2 + $(window).scrollTop() + "px"
            });
        },
        showLoad100: function()     //100% 进度条loading,用于frameset,中间带图
        {
            $.cover.hide();
            var str = "<div id='cover' style='position:absolute;background-color: #666;"
            str += "width:100%;";
            str += "height:100%;"
            str += "opacity:0.5;filter:alpha(opacity=50);"
            str += "top:0;left:0;z-index:1401;'>";
            str += "<div align=\"center\" id=\"light\" style=\"position: absolute;top: 45%;left: 45%;z-index: 1402;background-color: #666;\" ><img src=\"../../images/loading_100x100.gif\" /><br/><br/><div style=\"width:200px;\" id='covertext' style=\"color:white;\"></div></div>";
            str += "</div>";
            $(document.body).append(str);
        },
        show: function()    //遮罩层
        {
            $.cover.hide();
            var str = "<div id='cover' style='position:absolute;background-color: #666;"
            str += "width:" + $.getDocument.width() + "px;";
            str += "height:" + $.getDocument.height() + "px;"
            str += "opacity:0.5;filter:alpha(opacity=50);"
            str += "top:0;left:0;z-index:1001;'>";
            str += "</div>";
            $(document.body).append(str);
        },
        show100: function()     //遮罩层
        {
            $.cover.hide();
            var str = "<div id='cover' style='position:absolute;background-color: #666;"
            str += "width:100%;";
            str += "height:100%;"
            str += "opacity:0.5;filter:alpha(opacity=50);"
            str += "top:0;left:0;z-index:1001;'>";
            str += "</div>";
            $(document.body).append(str);
        },
        showDiv: function(divID)     //遮罩层
        {
            $.cover.hide();
            var str = "<div id='cover' style='position:absolute;background-color: #666;"
            str += "width:" + $("#" + divID).width() + "px;";
            str += "height:" + $("#" + divID).height() + "px;";
            str += "opacity:0.5;filter:alpha(opacity=50);"
            str += "top:0;left:0;z-index:999999;'>";
            str += "</div>";
            $(document.body).append(str);
        },
        hide: function() {
            $("#covertext").remove();
            $("#cover").remove();
        }
    },

    getDocument:
    {
        height: function() {
            return $(window).height() + $(document).scrollTop();
        },
        width: function() {
            return $(window).width() + $(document).scrollLeft();
        }
    }
});

var toast =  {
    msg: function (msg, options) {
        var str = "";
        var opts = {};
        if (options && options.constructor === Object) {
            opts = Object.assign({
                delay: 1200,
                hasCover: false
            }, options)
        } else {
            opts = {
                delay: 1200,
                hasCover: false
            }
        }
        var curWin = window, delay = opts.delay, hasCover = opts.hasCover;
        curWin.$("#covertext").length && curWin.$("#covertext").remove();
        curWin.$("#cover").length && curWin.$("#cover").remove();
        var msgStr = `<div id='covertext' style="z-index: 2002; position: fixed;font-size: 12px;font-weight: bold;padding: 10px;
                        left: 50%;
                        top: 50%;
	                    transform: translate(-50%,-50%);
                        background-color: #000000cc;
                        border-radius: 4px;
                        min-width: 100px;
                        min-height: 40px;
                        color: #fff;
                        display: flex;
                        align-items: center;
                        justify-content: center;">${msg}</div>`;
        if (hasCover) {
            str = `<div id="cover" style="position:fixed;width: ${$.toast.getDocument.width(curWin)}px;height:${$.toast.getDocument.height(curWin)}px;background-color: #66666688;top:0;left:0;z-index:2001;">${msgStr}</div>`;
        } else {
            str = msgStr;
        }
        $("body", curWin.document).append(str);
        curWin.toast.closeMsg(curWin, delay);
    },
    closeMsg: function (curWin = window, delay = 1200) {
        curWin.setTimeout(function () {
            curWin.$("#covertext").length && curWin.$("#covertext").remove();
            curWin.$("#cover").length && curWin.$("#cover").remove();
        }, delay)
    },
    getDocument:
    {
        height: function (curWin) {
            return $(curWin).height() + $(curWin.document).scrollTop();
        },
        width: function (curWin) {
            return $(curWin).width() + $(curWin.document).scrollLeft();
        }
    },
}
top.toast = toast;

//需要配合Jquery插件
//array to json
$.extend({
    arrayToJson: function (itemList) {
        if (itemList.length == 0) return "";
        var jsonStr = "[";
        for (var i = 0; i < itemList.length; i++) {
            var str = "{";
            for (var key in itemList[i]) {
                if (key != undefined) {
                    var value = itemList[i][key];
                    if (typeof value != "function") {
                        if (typeof value == "object" && value != null) {
                            str += ('"' + key + '"' + ':' + this.objectToJosn(value) + ',');      //数组下包含数组
                        }
                        else {
                            str += ('"' + key + '"' + ':"' + $.string2Json(value) + '",');
                        }
                    }
                }
            }
            jsonStr += (str.substring(0, str.length - 1) + "},");
        }
        if (jsonStr.length == 1) {
            var str = "{";
            for (var key in itemList) {
                if (key != undefined) {
                    var value = itemList[key];
                    if (typeof value != "function") {
                        if (typeof value == "object" && value != null) {
                            str += (key + ':' + this.objectToJosn(value) + ',');      //数组下包含数组
                        }
                        else {
                            str += (key + ':"' + $.string2Json(value) + '",');
                        }
                    }
                }
            }
            if (str == "{")
                jsonStr += ("{},");
            else
                jsonStr += (str.substring(0, str.length - 1) + "},");
        }
        if (jsonStr == "[")
            return ("[]");
        else
            return (jsonStr.substring(0, jsonStr.length - 1) + "]");
    },
    objectToJson: function (item) {
        var str = "{";
        for (var key in item) {
            if (key != undefined) {
                var value = item[key];
                if (typeof value != "function") {
                    if (typeof value == "object" && value != null) {
                        str += ('"' + key + '"' + ':' + this.objectToJson(value) + ',');            //对象下包含对象
                    }
                    else {
                        str += ('"' + key + '"' + ':"' + $.string2Json(value) + '",');
                    }
                }
            }
        }
        if (str == "{")
            return ("{}");
        else
            return (str.substring(0, str.length - 1) + "}");
    },
    string2Json: function(value)            //特殊字符进行处理
    {
        value += "";
        value = value.replace(/\\/g, "\\\\");   // \换\\
        value = value.replace(/\"/g, "\\\"");   // "换\"
        value = value.replace(/\//g, "\\\/");   // /换\/
        //        value = value.replace(/\b/g, "\\b");   // b换\b
        //        value = value.replace(/\f/g, "\\f");   // f换\f
        value = value.replace(/\n/g, "\\n");   // n换\n
        value = value.replace(/\r/g, "\\r");   // r换\r
        value = value.replace(/\t/g, "\\t");   // t换\t
        return value;
    }
});

//控制并只允许输入数字(包含小数)
function InputNum(ele) {
    var txtval = ele.value;
    var key = event.keyCode;
    if ((key < 48 || key > 57) && key != 46 && key != 45) {
        event.keyCode = 0;
    }
    else {
        if (key == 46) {
            if (txtval.indexOf(".") != -1 || txtval.length == 0)
                event.keyCode = 0;
        }
        if (key == 45) {
            if (txtval.indexOf("-") != -1 || txtval.length != 0)
                event.keyCode = 0;
        }
    }
}



// 功能：检查页面输入的文本框中的数字是否合法
// 函数名：CheckNumber
// @ ctlName:页面的控件名称或ID
// @ err_msg:错误提示信息
// @ flag:0为数字,1表示为整数,2表示正数,3表示0或正数,4表示正整数,5表示0或正整数
// 说明：如果为数字则不做任何事，不为数字则提示信息并返回，不执行。
function CheckNumber(ctlvalue, flag) {
    if (ctlvalue == "") return true;     //为空时不校验
    if (isNaN(ctlvalue)) {
        return false;
    }
    else {
        if (flag == 1 && ctlvalue.indexOf(".") != -1) {
            return false;
        }
        if (flag == 2 && ctlvalue / 1 <= 0) {
            return false;
        }
        if (flag == 3 && ctlvalue / 1 < 0) {
            return false;
        }
        if (flag == 4 && (ctlvalue.indexOf(".") != -1 || ctlvalue / 1 <= 0)) {
            return false;
        }
        if (flag == 5 && (ctlvalue.indexOf(".") != -1 || ctlvalue / 1 < 0)) {
            return false;
        }
        return true;
    }
}


//检查日期格式是否正确
function checkDateTime(str) {
    str = $.trim(str);
    if (str == "") return true;
    var reg = /^(\d+)-(\d{1,2})-(\d{1,2}) (\d{1,2}):(\d{1,2}):(\d{1,2})$/;
    var regShort = /^(\d+)-(\d{1,2})-(\d{1,2})$/;
    if (reg.test(str)) {
        var r = str.match(reg);
        if (r == null) return false;
        r[2] = r[2] - 1;
        var d = new Date(r[1], r[2], r[3], r[4], r[5], r[6]);
        if (d.getFullYear() != r[1]) return false;
        if (d.getMonth() != r[2]) return false;
        if (d.getDate() != r[3]) return false;
        return true;
    }
    else {
        var r = str.match(regShort);
        if (r == null) return false;
        r[2] = r[2] - 1;
        var d = new Date(r[1], r[2], r[3]);
        if (d.getFullYear() != r[1]) return false;
        if (d.getMonth() != r[2]) return false;
        if (d.getDate() != r[3]) return false;
        return true;
    }
}

// 功能：检查是否为Email地址
// 函数名：ChkEmail
// @ a:要检查的值
// 说明：合法返回1，不合法返回0
function checkEmail(a) {
    var i = a.length;
    var temp = a.indexOf('@');
    var tempd = a.indexOf('.');
    if (temp > 1) {
        if ((i - temp) > 3) {
            if (tempd != -1) {
                return 1;
            }
        }
    }
    return 0;
}

//+---------------------------------------------------
//| 日期合法性验证
//| 格式为：YYYY-MM-DD或YYYY/MM/DD
//+---------------------------------------------------
//function isValidDate(sTest) {
//    var isDate = /(?:19|20\d{2})\/(?:0[1-9]|1[0-2])\/(?:0[1-9]|[12][0-9]|3[01])/;
//    return isDate.test(sTest);
//}
function isValidDate(oStartDate) {
    //对日期格式进行验证 要求为2000-2099年  格式为 yyyy-mm-dd 并且可以正常转换成正确的日期
    if (oStartDate == "")
        return true;
    var pat_hd = /^(19|20)\d{2}-((0[1-9]{1})|(1[0-2]{1}))-((0[1-9]{1})|([1-2]{1}[0-9]{1})|(3[0-1]{1}))$/;

    try {
        if (!pat_hd.test(oStartDate)) { throw "Invalid Date"; }
        var arr_hd = oStartDate.split("-");
        var dateTmp;
        dateTmp = new Date(arr_hd[0], parseFloat(arr_hd[1]) - 1, parseFloat(arr_hd[2]));
        if (dateTmp.getFullYear() != parseFloat(arr_hd[0]) || dateTmp.getMonth() != parseFloat(arr_hd[1]) - 1 || dateTmp.getDate() != parseFloat(arr_hd[2])) {
            throw "Invalid Date";
        }
    }
    catch (ex) {
        if (ex.description)
        { return false; }
        else
        { return false; }
    }
    return true;
}

//检验是否为有效字符，不允许有/\*?"<>|
function CheckCharRegelars(str) {
    var regexp = new RegExp('[\\\\/:*?\"<>|]');
    return regexp.test(str);
}

//全局替换
String.prototype.replaceAll = function (AFindText, ARepText) {
    var raRegExp = new RegExp(AFindText.replace(/([\(\)\[\]\{\}\^\$\+\-\*\?\.\"\'\|\/\\])/g, "\\$1"), "ig");
    return this.replace(raRegExp, ARepText);
}
//移除数组的元素
Array.prototype.remove = function (dx) {
    if (isNaN(dx) || dx > this.length) { return false; }
    for (var i = 0, n = 0; i < this.length; i++) {
        if (this[i] != this[dx]) {
            this[n++] = this[i]
        }
    }
    this.length -= 1
}
//克隆对象
function cloneByObject(myObj) {
    if (typeof (myObj) != 'object') return myObj;
    if (myObj == null) return myObj;

    var myNewObj = new Object();

    for (var i in myObj)
        myNewObj[i] = cloneByObject(myObj[i]);

    return myNewObj;
}

//得到URL参数值，若无，返回null
function QueryString(fieldName) {
    var urlString = document.location.href;
    if (urlString != null) {
        var typeQu = fieldName + "=";
        var urlEnd = urlString.indexOf(typeQu);
        if (urlEnd != -1) {
            var paramsUrl = urlString.substring(urlEnd + typeQu.length);
            var isEnd = paramsUrl.indexOf('&');
            if (isEnd != -1) {
                return paramsUrl.substring(0, isEnd).replaceAll("#", "");
            }
            else {
                return paramsUrl.replaceAll("#", "");
            }
        }
        else {
            return null;
        }
    }
    else {
        return null;
    }
}

var pop;
function OpenWinMax(URL, TTL) {
    var showwin = window.open(URL, TTL, "width=" + window.screen.availWidth + "px,height=" + (window.screen.availHeight - 58) + "px,left=0,top=0,toolbar=no,menubar=no,scrollbars=yes,resizable=yes,location=no,status=yes,directories=no");
    if (showwin) {
        showwin.focus();
    }
    else {
        alert(pop);
        return false;
    }
}

function CopyListToList(cboName1ID, cboName2ID) {
    var cboName1 = document.getElementById(cboName1ID);
    var cboName2 = document.getElementById(cboName2ID);
    if (cboName1.selectedIndex == -1) {
        return;
    }
    var length1 = cboName1.options.length - 1;
    var n = 0;
    //*向列表控件2中添加控件1中被选中的值
    for (var j = 0; j <= length1; j++) {
        if (cboName1.options[j].selected) {
            var oOption = document.createElement('OPTION');
            oOption.text = cboName1.options[j].text;
            oOption.value = cboName1.options[j].value;
            oOption.title = cboName1.options[j].title;
            oOption.setAttribute("form_type", cboName1.options[j].getAttribute("form_type"));
            cboName2.options.add(oOption);
        }
    }
}

/**
* FUNC : 将选中的值从一个列表框控件1中移到另一个列表控件2中
* PARM : cboName1  ---列表控件名1
*        cboName2  ---列表控件名2
* RETU : void
* AUTH : Mitnick.Chen
* DESC :
*/
function MoveListToList(cboName1ID, cboName2ID) {
    var cboName1 = document.getElementById(cboName1ID);
    var cboName2 = document.getElementById(cboName2ID);
    if (cboName1.selectedIndex == -1)
        return;

    //*移动控件1中被选中的项到列表控件2中
    var j = 0;
    while (j < cboName1.options.length) {
        if (cboName1.options[j].selected) {
            var isExisted = false;
            for (var k = 0; k < cboName2.options.length; k++) {
                if (cboName1.options[j].value == cboName2.options[k].value) {
                    isExisted = true;
                    break;
                }
            }
            if (!isExisted) {
                $(cboName2).append($(cboName1.options[j]));  //移动option(保留option所有属性), length - 1
                continue;
            }
        }
        j++;
    }
    //cboName2.selectedIndex = -1;

    //*删除列表控件1中被选择的但未被移动的项
    for (var j = cboName1.options.length - 1; j >= 0; j--) {
        if (cboName1.options[j].selected)
            cboName1.options.remove(j);
    }
}

/**
* FUNC : 列表控件中的值向上移动
* PARM : cboName  ---列表控件ID
* RETU : void
* AUTH : Mitnick.Chen
* DESC :
*/
function OnMoveUp(cboNameID) {
    var cboName = document.getElementById(cboNameID);
    if (cboName == null)
        return;

    var vIndex = cboName.selectedIndex;
    if (cboName.selectedIndex <= 0)
        return;

    var vText = cboName.options[vIndex - 1].text;
    var vValue = cboName.options[vIndex - 1].value;
    var vTitle = cboName.options[vIndex - 1].title;
    var vDisabled = cboName.options[vIndex - 1].disabled;
    var vFormType = cboName.options[vIndex - 1].getAttribute("form_type");

    cboName.options[vIndex - 1].text = cboName.options[vIndex].text;
    cboName.options[vIndex - 1].value = cboName.options[vIndex].value;
    cboName.options[vIndex - 1].title = cboName.options[vIndex].title;
    cboName.options[vIndex - 1].disabled = cboName.options[vIndex].disabled;
    cboName.options[vIndex - 1].setAttribute("form_type", cboName.options[vIndex].getAttribute("form_type"));

    cboName.options[vIndex].text = vText;
    cboName.options[vIndex].value = vValue;
    cboName.options[vIndex].title = vTitle;
    cboName.options[vIndex].disabled = vDisabled;
    cboName.options[vIndex].setAttribute("form_type", vFormType);

    cboName.options[vIndex].selected = false;
    cboName.options[vIndex - 1].selected = true;
}

/**
* 20230710 hzq
* FUNC : 列表控件中的值向上移动到顶部
* PARM : cboName  ---列表控件ID
* RETU : void
* AUTH : Mitnick.Chen
* DESC :
*/
function OnMoveUpTop(cboNameID) {
    var cboName = document.getElementById(cboNameID);
    if (cboName == null)
        return;

    var vIndex = cboName.selectedIndex;
    if (cboName.selectedIndex <= 0)
        return;

    //当前控件selectedIndex-1到顶部0的所有控件向下移动一步
    var bText = cboName.options[vIndex].text;
    var bValue = cboName.options[vIndex].value;
    var bTitle = cboName.options[vIndex].title;
    var bDisabled = cboName.options[vIndex].disabled;
    var bFormType = cboName.options[vIndex].getAttribute("form_type");

    for (var i = vIndex-1; i >= 0; i--)
    {
        var vText = cboName.options[i].text;
        var vValue = cboName.options[i].value;
        var vTitle = cboName.options[i].title;
        var vDisabled = cboName.options[i].disabled;
        var vFormType = cboName.options[i].getAttribute("form_type");

        cboName.options[i + 1].text = vText;
        cboName.options[i + 1].value = vValue;
        cboName.options[i + 1].title = vTitle;
        cboName.options[i + 1].disabled = vDisabled;
        cboName.options[i + 1].setAttribute("form_type", vFormType);
        cboName.options[i + 1].selected = false;
    }

    cboName.options[0].text = bText;
    cboName.options[0].value = bValue;
    cboName.options[0].title = bTitle;
    cboName.options[0].disabled = bDisabled;
    cboName.options[0].setAttribute("form_type", bFormType);
    cboName.options[0].selected = true;
}


/**
* FUNC : 列表控件中的值向下移动
* PARM : cboName  ---列表控件ID
* RETU : void
* AUTH : Mitnick.Chen
* DESC :
*/
function OnMoveDown(cboNameID) {
    var cboName = document.getElementById(cboNameID);
    if (cboName == null)
        return;

    var vIndex = cboName.selectedIndex;
    if (vIndex == -1 || vIndex == cboName.options.length - 1)
        return;

    var vText = cboName.options[vIndex + 1].text;
    var vValue = cboName.options[vIndex + 1].value;
    var vTitle = cboName.options[vIndex + 1].title;
    var vDisabled = cboName.options[vIndex + 1].disabled;
    var vFormType = cboName.options[vIndex + 1].getAttribute("form_type");

    cboName.options[vIndex + 1].text = cboName.options[vIndex].text;
    cboName.options[vIndex + 1].value = cboName.options[vIndex].value;
    cboName.options[vIndex + 1].title = cboName.options[vIndex].title;
    cboName.options[vIndex + 1].disabled = cboName.options[vIndex].disabled;
    cboName.options[vIndex + 1].setAttribute("form_type", cboName.options[vIndex].getAttribute("form_type"));

    cboName.options[vIndex].text = vText;
    cboName.options[vIndex].value = vValue;
    cboName.options[vIndex].title = vTitle;
    cboName.options[vIndex].disabled = vDisabled;
    cboName.options[vIndex].setAttribute("form_type", vFormType);

    cboName.options[vIndex].selected = false;
    cboName.options[vIndex + 1].selected = true;
}

/** 
* 20230710 hzq
* FUNC : 列表控件中的值向下移动
* PARM : cboName  ---列表控件ID
* RETU : void
* AUTH : Mitnick.Chen
* DESC :
*/
function OnMoveDownBottom(cboNameID) {
    var cboName = document.getElementById(cboNameID);
    if (cboName == null)
        return;

    var totalCnt = cboName.options.length;
    var vIndex = cboName.selectedIndex;
    if (vIndex == -1 || vIndex == totalCnt - 1)
        return;

    //当前控件selectedIndex+1到底部的所有控件向上移动一步
    var bText = cboName.options[vIndex].text;
    var bValue = cboName.options[vIndex].value;
    var bTitle = cboName.options[vIndex].title;
    var bDisabled = cboName.options[vIndex].disabled;
    var bFormType = cboName.options[vIndex].getAttribute("form_type");

    for (var i = vIndex + 1; i <= totalCnt - 1; i++) {
        var vText = cboName.options[i].text;
        var vValue = cboName.options[i].value;
        var vTitle = cboName.options[i].title;
        var vDisabled = cboName.options[i].disabled;
        var vFormType = cboName.options[i].getAttribute("form_type");

        cboName.options[i - 1].text = vText;
        cboName.options[i - 1].value = vValue;
        cboName.options[i - 1].title = vTitle;
        cboName.options[i - 1].disabled = vDisabled;
        cboName.options[i - 1].setAttribute("form_type", vFormType);
        cboName.options[i - 1].selected = false;
    }

    cboName.options[totalCnt - 1].text = bText;
    cboName.options[totalCnt - 1].value = bValue;
    cboName.options[totalCnt - 1].title = bTitle;
    cboName.options[totalCnt - 1].disabled = bDisabled;
    cboName.options[totalCnt - 1].setAttribute("form_type", bFormType);
    cboName.options[totalCnt - 1].selected = true;
}


//ImageButton
//调用方法：$("XXX").ImageButton();
(function($) {
    $.fn.ImageButton = function() {
        this.each(function() {
            if (!$(this).hasClass("imgbotton"))
                $(this).addClass("imgbotton");

            if ($(this).attr("icon") != undefined && $(this).find("a").length == 0) {   //若已经构建<a>, 则无需重复执行
                var anchor = $("<a>");
                anchor.addClass($(this).attr("icon")).text($(this).text());
                $(this).removeAttr("icon").html(anchor);
            }
        });
    };
	    //设置table中的同列相邻单元格的变化标识
    $.fn.setChangeFlag = function () {
        var tb = this;
        var noFloagCells = $(tb).find("thead").find("td[noFloag='true']"); //获取无需表示的列
        var cellIndexs = new Array(); //定义无需表示列号数组
        for (var n = 0; n < noFloagCells.length; n++) {
            var currentIndex = $(tb).find("thead").find("td").index(noFloagCells[n]);
            cellIndexs[n] = currentIndex; //将无需表示列号加入数组
        }
        var trs = $(tb).find("tbody").find("tr"); //得到标题的tr
        var trsCount = trs.length;
        var cells = $(trs[0]).find("td");
        var cellsCount = cells.length;
        for (var i = 0; i < trsCount; i++) {
            var currentTr = trs[i];
            if (i > 0) {
                var prevTr = trs[i - 1]; //获得上一行数据
                for (var j = 0; j < cellsCount; j++) { //循环单元格数量
                    if (cellIndexs.indexOf(j) == -1) {//排除不需要标记的
                        var prevCell = $(prevTr).find("td")[j]; //同列上一单元格
                        var currentCell = $(currentTr).find("td")[j]; //当前单元格
                        if ($(prevCell).text().toString().trim() != $(currentCell).text().toString().trim()) { //相比较
                            $(currentCell).css({"background-color":"yellow"});
                        }
                    }
                }
            }
        }
    };
})(jQuery);

$.extend({
    ImageButton: {
        disabled: function(id) {
            $("#" + id).addClass("disable");
        },
        enabled: function(id, event) {
            var button = $("#" + id);
            button.removeClass("disable");
            if (event != undefined) {
                var type = typeof (event);
                if (type == "string")
                    button.bind("click", function() { eval(event); });
                else if (type == "function")
                    button.bind("click", event);
            }
        },
        remove: function(id) {
            $("#" + id).remove();
        }
    }
});

//hover 延迟处理
(function ($) {
    $.fn.hoverDelay = function (options) {
        var defaults = {
            hoverDuring: 200,
            hoverEvent: function ($this) { $.noop(); },
            outEvent: function ($this) { $.noop(); }
        };
        var sets = $.extend(defaults, options || {});
        var hoverTimer, outTimer;
        return $(this).each(function () {
            var $this = $(this);
            $this.hover(function () {
                clearTimeout(outTimer);
                hoverTimer = setTimeout(function () {
                    sets.hoverEvent($this);
                }, sets.hoverDuring);
            }, function () {
                clearTimeout(hoverTimer);
                sets.outEvent($this);
            });
        });
    };
})(jQuery);

//区分浏览器，并考虑IE5.5 6 7 8 9 10 
function myBrowser() {
    var userAgent = navigator.userAgent; //取得浏览器的userAgent字符串
    var isOpera = userAgent.indexOf("Opera") > -1; //判断是否Opera浏览器
    var isIE = userAgent.indexOf("compatible") > -1 && userAgent.indexOf("MSIE") > -1 && !isOpera; //判断是否IE浏览器
    var isFF = userAgent.indexOf("Firefox") > -1; //判断是否Firefox浏览器
    var isSafari = userAgent.indexOf("Safari") > -1; //判断是否Safari浏览器
    var isChrome = userAgent.indexOf("Chrome") > -1; //判断Chrome浏览器
    if (isIE) {
        var IE5 = IE55 = IE6 = IE7 = IE8 = IE9 = IE10 = false;
        var reIE = new RegExp("MSIE (\\d+\\.\\d+);");
        reIE.test(userAgent);
        var fIEVersion = parseFloat(RegExp["$1"]);
        IE55 = fIEVersion == 5.5;
        IE6 = fIEVersion == 6.0;
        IE7 = fIEVersion == 7.0;
        IE8 = fIEVersion == 8.0;
        IE7 = fIEVersion == 9.0;
        IE8 = fIEVersion == 10.0;
        if (IE55) {
            return "IE55";
        }
        if (IE6) {
            return "IE6";
        }
        if (IE7) {
            return "IE7";
        }
        if (IE8) {
            return "IE8";
        }
        if (IE9) {
            return "IE9";
        }
        if (IE10) {
            return "IE10";
        }
    }

    if (isFF) {
        return "FF";
    }
    if (Chrome) {
        return "Chrome";
    }
}

function isIE6() {
    return navigator.userAgent.split(";")[1].toLowerCase().indexOf("msie 6.0") > "-1";
}
function isIE7() {
    return navigator.userAgent.split(";")[1].toLowerCase().indexOf("msie 7.0") > "-1";
}
function isIE8() {
    return navigator.userAgent.split(";")[1].toLowerCase().indexOf("msie 8.0") > "-1";
}
function isIE9() {
    return navigator.userAgent.split(";")[1].toLowerCase().indexOf("msie 9.0") > "-1";
}
function isIE10() {
    return navigator.userAgent.split(";")[1].toLowerCase().indexOf("msie 10.0") > "-1";
}
function isFirefox() {
    return navigator.userAgent.split(";")[1].toLowerCase().indexOf("Firefox") > "-1";
}
function isChrome() {
    return navigator.userAgent.split(";")[1].toLowerCase().indexOf("Chrome") > "-1";
}
//ID内的表单是否发生变化
function isChanged(myId) {
    var ischanged = false;
    $("#" + myId + " input[type='text'],textarea").each(function () {
        if (this.value != this.defaultValue) {
            ischanged = true;
            return true;
        }
    });
    if (ischanged) return true;

    $("#" + myId + " input[type='radio'],input[type='checkbox']").each(function () {
        if (this.checked != this.defaultChecked) {
            ischanged = true;
            return true;
        }
    });
    if (ischanged) return true;

    $("#" + myId + " select").each(function () {
        for (var j = 0; j < this.options.length; j++) {
            if (this.options[j].selected != this.options[j].defaultSelected) {
                ischanged = true;
                return true;
            }
        }
    });
    if (ischanged) return true;
    return false;
}

//得到ID区域内的变量数据（Id--Value)
function getAllDataById(pId) {
    var ele = new Object();
    $("#" + pId + " input[id]").each(function () {
        switch ($(this).attr("type")) {
            case "checkbox":
                if ($(this).is(":checked")) {
                    ele[$(this).attr("id")] = $.trim($(this).val());
                }
                else {
                    ele[$(this).attr("id")] = "";
                }
                break;
            case "radio":
                var id = $(this).attr("id");
                var name = $(this).attr("name");
                if (ele.hasOwnProperty(name) == false) {
                    if (typeof ($("input[name='" + name + "']:checked").attr("value")) == "undefined") {
                        ele[id] = "";
                    }
                    else {
                        ele[id] = $.trim($("input[name='" + name + "']:checked").attr("value"));
                    }
                }
                break;
            default:
                ele[$(this).attr("id")] = $.trim($(this).val());
                break;
        }
    });
    $("#" + pId + " select[id]").each(function () {
        ele[$(this).attr("id")] = $.trim($(this).val());
    });
    $("#" + pId + " textarea[id]").each(function () {
        ele[$(this).attr("id")] = $.trim($(this).val());
    });
    return ele;
}


//得到ID区域内的变量数据（Name--Value)
function getAllDataByName(pId) {
    var ele = new Object();
    if (pId.substring(0, 1) == ".") {
        getFixData(pId, ele);//获取
        getDanamicData($(pId + " table[isAutoEdit='true']"), ele);
    }
    else {
        getFixData("#" + pId, ele);//获取
        getDanamicData($("#" + pId + " table[isAutoEdit='true']"), ele);
    }
    return ele;
}

function getFixData(pId, ele) {
    $(pId + " input[name]:not(table[isAutoEdit='true'] input,span[field_source='Mirror'] input)").each(function () {
        var name = $(this).attr("name");
        switch ($(this).attr("type")) {
            case "checkbox":
                if ($(this).is(":checked")) {
                    if (ele.hasOwnProperty(name)) {
                        if (ele[name] == "")
                            ele[name] = $.trim($(this).val());
                        else
                            ele[name] += "," + $.trim($(this).val());
                    }
                    else {
                        ele[name] = $.trim($(this).val());
                    }
                }
                else {
                    if (ele.hasOwnProperty(name) == false) {
                        ele[name] = "";
                    }
                }
                break;
            case "radio":
                if (ele.hasOwnProperty(name) == false) {
                    if (typeof ($(pId + " input[name='" + name + "']:checked").attr("value")) == "undefined") {
                        ele[name] = "";
                    }
                    else {
                        ele[name] = $.trim($(pId + " input[name='" + name + "']:checked").attr("value"));
                    }
                }
                break;
            case "range":
                var output_ele = $(this).siblings(".output");
                if (output_ele.length > 0) {
                    ele[name] = $.trim(output_ele.text());
                } else {
                    ele[name] = $.trim($(this).val());
                }
                break;
            default:
                if ($(this).parent().attr("field_type") && $(this).parent().attr("field_type").indexOf("Partial") >= 0)
                    ele[name] = $(this).val();
                else
                    ele[name] = $.trim($(this).val());
                break;
        }
    });
    $(pId + " select[name]:not(table[isAutoEdit='true'] select,span[field_source='Mirror'] select)").each(function () {
        ele[$(this).attr("name")] = $.trim($(this).val());
    });
    $(pId + " textarea[name]:not(table[isAutoEdit='true'] textarea,span[field_source='Mirror'] textarea)").each(function () {
        ele[$(this).attr("name")] = $.trim($(this).val());
    });
    $(pId + " pre[name]:not(table[isAutoEdit='true'] pre,span[field_source='Mirror'] pre)").each(function () {
        var spanObj = $(this).parent("span");
        if (spanObj.length > 0) {
            var fieldValue = $.trim(spanObj.attr("field_value"));
            if (spanObj.attr("field_value") == undefined || fieldValue.charAt(0) == "{" || fieldValue.charAt(0) == "[" || fieldValue == "-" || fieldValue == "\\" || fieldValue == "/") {
                ele[$(this).attr("name")] = $.trim($(this).text());
            } else {
                ele[$(this).attr("name")] = fieldValue;
            }
        } else {
            ele[$(this).attr("name")] = $.trim($(this).text());
        }       
    });
    return ele;
}

function getDanamicData(DanamicObj, ele) {
    var trs = DanamicObj.find("tr");
    var datas = [];
    for (var i = 0; i < trs.length; i++) {
        if (i != 0 && i != trs.length - 1) {
            var objData = {};
            $(trs[i]).find("input").each(function () {
                var name = $(this).attr("name");
                switch ($(this).attr("type")) {
                    case "checkbox":
                        if ($(this).is(":checked")) {
                            if (objData.hasOwnProperty(name)) {
                                if (objData[name] == "")
                                    objData[name] = $.trim($(this).val());
                                else
                                    objData[name] += "," + $.trim($(this).val());
                            }
                            else {
                                objData[name] = $.trim($(this).val());
                            }
                        }
                        else {
                            if (objData.hasOwnProperty(name) == false) {
                                objData[name] = "";
                            }
                        }
                        break;
                    case "radio":
                        var fieldName = $(this).attr("fieldName");
                        if (!fieldName) {
                            fieldName = $(this).attr("name");
                        }
                        if ($(this).is(":checked")) {
                            if (objData.hasOwnProperty(fieldName) == false) {
                                if (typeof ($(this).attr("value")) == "undefined") {
                                    objData[fieldName] = "";
                                }
                                else {
                                    objData[fieldName] = $.trim($(this).attr("value"));
                                }
                            }
                        }
                        break;
                    case "range":
                        var output_ele = $(this).siblings(".output");
                        if (output_ele.length > 0) {
                            objData[name] = $.trim(output_ele.text());
                        } else {
                            objData[name] = $.trim($(this).val());
                        }
                        break;
                    default:
                        if ($(this).parent().attr("field_type") && $(this).parent().attr("field_type").indexOf("Partial") >= 0)
                            objData[name]= $(this).val();
                        else
                            objData[name]= $.trim($(this).val());
                        break;
                }
            });
            $(trs[i]).find("select[name]").each(function () {
                objData[$(this).attr("name")] = $.trim($(this).val());
            });
            $(trs[i]).find("textarea[name]").each(function () {
                objData[$(this).attr("name")] = $.trim($(this).val());
            });

            datas.push(objData);
            ele["table"] = datas;
        }
    }
}

//设置ID区域内的变量数据（Name--Value)
function setAllDataByName(pId, ele, myIndex) {
    if (arguments.length < 3)
        myIndex = "";
    var myId = "";
    if (pId.substring(0, 1) == ".") {
        myId = pId;
    }
    else {
        myId = "#" + pId;
    }
    $(myId + " input[name]").each(function () {
        var name = $(this).attr("name") + myIndex;
        if (ele[name] == undefined) {
            if (ele[name.toLowerCase()] != undefined) {
                name = name.toLowerCase();
            }
        }
        switch ($(this).attr("type")) {
            case "checkbox":
                if (ele.hasOwnProperty(name)) {
                    $(this).attr("checked", ("," + ele[name] + ",").indexOf("," + $(this).val() + ",") >= 0);
                }
                break;
            case "radio":
                if (ele.hasOwnProperty(name)) {
                    if (ele[name] == $(this).val()) {
                        //$(this).attr("checked", true);
                        $(this).prop("checked", true);
                    }
                    else {
                        $(this).prop("checked", false);
                        //$(this).removeAttr("checked");
                        //this.removeAttribute("checked");    //attr("checked", false)和removeAttr("checked")有bug，改用原生js方法 by xiaoyuanji 20161209
                    }
                }
                break;
            case "hidden":
                $(this).val(ele[name]);
                if (ele.hasOwnProperty(name) && $(this).attr("slider") == "true")
                    $("#" + name.toUpperCase() + "1").slider({ value: ele[name] });
                break;
            case "range":
                if (ele.hasOwnProperty(name)) {
                    if (ele[name] == "") {
                        $(this).siblings("ins.output").text(ele[name]);
                        $(this).val($(this).attr("min"));
                    }  
                    else
                        $(this).attr("value", ele[name]).siblings("ins.output").text(ele[name]);
                }
                break;
            default:
                if (ele.hasOwnProperty(name))
                    $(this).attr("value", ele[name]);
                break;
        }
    });
    $(myId + " select[name]").each(function () {
        var name = $(this).attr("name") + myIndex;
        if (ele.hasOwnProperty(name)) {
            //为了保持outerHTML与selected值一致，同时避免value中含有单/双引号的问题，采用each遍历方法 by xiaoyuanji 20161209
            $(this).children().each(function () {
                if ($(this).val() == ele[name])
                    $(this).attr("selected", true);
                else
                    this.removeAttribute("selected");    //attr("selected", false)和removeAttr("selected")有bug，改用原生js方法 by xiaoyuanji 20161209
            });
        }
    });
    $(myId + " textarea[name]").each(function () {
        var name = $(this).attr("name") + myIndex;
        if (ele.hasOwnProperty(name)) {
            $(this).attr("value", ele[name]).text(ele[name]);    //ie下设置value可以显示，但chrome/firefox下无法显示，需设置text才能显示，by xiaoyuanji 20170828
        }
    });
    $(myId + " pre[name]").each(function () {
        var name = $(this).attr("name") + myIndex;
        var spanObj = $(this).parent("span");
        if (spanObj.length > 0) {
            var fieldValue = $.trim(spanObj.attr("field_value"));
            if (ele.hasOwnProperty(name) && (spanObj.attr("field_value") == undefined || $(this).attr("is_translate") != "true" || fieldValue.charAt(0) == "{" || fieldValue.charAt(0) == "[" || fieldValue == "-" || fieldValue == "\\" || fieldValue == "/")) {
                $(this).html(ele[name].replace(/</g, '&lt;').replace(/>/g, '&gt;'));
            }
        } else if (ele.hasOwnProperty(name)) {
            $(this).html(ele[name].replace(/</g, '&lt;').replace(/>/g, '&gt;'));
        }
    });
}

//设置ID区域内的可视变量数据（Name--Value)
function setAllVisualDataByName(pId, ele, myIndex) {
    if (arguments.length < 3)
        myIndex = "";
    var myId = "";
    if (pId.substring(0, 1) == ".") {
        myId = pId;
    }
    else {
        myId = "#" + pId;
    }
    $(myId + " input[name]:visible").each(function () {
        var name = $(this).attr("name") + myIndex;
        if (ele[name] == undefined) {
            if (ele[name.toLowerCase()] != undefined) {
                name = name.toLowerCase();
            }
        }
        switch ($(this).attr("type")) {
            case "checkbox":
                if (ele.hasOwnProperty(name)) {
                    $(this).attr("checked", ("," + ele[name] + ",").indexOf("," + $(this).val() + ",") >= 0);
                }
                break;
            case "radio":
                if (ele.hasOwnProperty(name)) {
                    if (ele[name] == $(this).val()) {
                        //$(this).attr("checked", true);
                        $(this).prop("checked", true);
                    }
                    else {
                        $(this).prop("checked", false);
                        //$(this).removeAttr("checked");
                        //this.removeAttribute("checked");    //attr("checked", false)和removeAttr("checked")有bug，改用原生js方法 by xiaoyuanji 20161209
                    }
                }
                break;
            case "hidden":
                $(this).val(ele[name]);
                if (ele.hasOwnProperty(name) && $(this).attr("slider") == "true")
                    $("#" + name.toUpperCase() + "1").slider({ value: ele[name] });
                break;
            case "range":
                if (ele.hasOwnProperty(name)) {
                    if (ele[name] == "") {
                        $(this).siblings("ins.output").text(ele[name]);
                        $(this).val($(this).attr("min"));
                    }
                    else
                        $(this).attr("value", ele[name]).siblings("ins.output").text(ele[name]);
                }
                break;
            default:
                if (ele.hasOwnProperty(name))
                    $(this).attr("value", ele[name]);
                break;
        }
    });
    $(myId + " select[name]:visible").each(function () {
        var name = $(this).attr("name") + myIndex;
        if (ele.hasOwnProperty(name)) {
            //为了保持outerHTML与selected值一致，同时避免value中含有单/双引号的问题，采用each遍历方法 by xiaoyuanji 20161209
            $(this).children().each(function () {
                if ($(this).val() == ele[name])
                    $(this).attr("selected", true);
                else
                    this.removeAttribute("selected");    //attr("selected", false)和removeAttr("selected")有bug，改用原生js方法 by xiaoyuanji 20161209
            });
        }
    });
    $(myId + " textarea[name]:visible").each(function () {
        var name = $(this).attr("name") + myIndex;
        if (ele.hasOwnProperty(name)) {
            $(this).attr("value", ele[name]).text(ele[name]);    //ie下设置value可以显示，但chrome/firefox下无法显示，需设置text才能显示，by xiaoyuanji 20170828
        }
    });
    $(myId + " pre[name]").each(function () {
        var name = $(this).attr("name") + myIndex;
        var spanObj = $(this).parent("span");
        if (spanObj.length > 0) {
            var fieldValue = $.trim(spanObj.attr("field_value"));
            if (ele.hasOwnProperty(name) && (spanObj.attr("field_value") == undefined || $(this).attr("is_translate") != "true" || fieldValue.charAt(0) == "{" || fieldValue.charAt(0) == "[" || fieldValue == "-" || fieldValue == "\\" || fieldValue == "/")) {
                $(this).html(ele[name].replace(/</g, '&lt;').replace(/>/g, '&gt;'));
            }
        } else if (ele.hasOwnProperty(name)) {
            $(this).html(ele[name].replace(/</g, '&lt;').replace(/>/g, '&gt;'));
        }
    })
}

function myAlert(message, event) {
    if (arguments.length == 2 && typeof (event) === 'number') {
        asyncbox.html({
            content: '<div class="asyncbox_alert"><span></span>' + message + '</div><p style="text-align: right;padding:15px 15px 0px 0px;"><i>' + event + '</i> S</p>',
            width: 317,
            height: 85,
            id: "selfCloseBox",
            title: "CIMS",
            buttons: "",
            onload: function (cntWin) {
                var selfCloseBoxID = setInterval(function () {
                    $("#selfCloseBox_content p i").text(--event);
                    if (event == 0) {
                        clearInterval(selfCloseBoxID);
                        asyncbox.close('selfCloseBox');
                    }
                }, 1000);
            }
        });
    } else {
        asyncbox.alert(message, "CIMS", function (action) {
            if (typeof (event) == "function") {
                event();
            }
        });
    }
}

function myConfirm(message, acceptEvent, cancleEvent) {
    asyncbox.confirm(message, "CIMS", function (buttonResult) {
        if (buttonResult == "ok") {
            if (typeof (acceptEvent) == "function") {
                acceptEvent();
            }
            bm_return = true;
            //return true;
        } else {
            if (typeof (cancleEvent) == "function") {
                cancleEvent();
            }
            bm_return = false;
            //return false;
        }
    });
}

//验证是否为空
function checkEmpty(isVisible, jqElement, cWin) {
    var alertWin = cWin || window;
    var requiredlist = null;
    if (typeof (isVisible) == "undefined" || isVisible) {
        if (typeof (jqElement) == "undefined")
            requiredlist = $("input[validate='required']:visible,select[validate='required']:visible,textarea[validate='required']:visible").not("[disabled]");
        else
            requiredlist = jqElement.find("input[validate='required']:visible,select[validate='required']:visible,textarea[validate='required']:visible").not("[disabled]");
    }
    else {
        if (typeof (jqElement) == "undefined")
            requiredlist = $("input[validate='required'],select[validate='required'],textarea[validate='required']").not("[disabled]");
        else
            requiredlist = jqElement.find("input[validate='required'],select[validate='required'],textarea[validate='required']").not("[disabled]");
    }
    for (var i = 0; i < requiredlist.length; i++) {
        var field_value = "";
        var myType = $(requiredlist[i]).attr("type");
        if (myType == "radio") {
            var field_name = $(requiredlist[i]).attr("name");
            field_value = $("input[name='" + field_name + "']:checked").val();
            if (field_value == undefined) field_value = "";
        }
        else {
            field_value =$.trim($(requiredlist[i]).val());
        }
        if (field_value == "") {
            alertWin.myAlert($(requiredlist[i]).attr("msgtitle"), function () {
                $(requiredlist[i]).focus();
                if (myType == "radio") {
                    $("input[name='" + field_name + "']").parent().addClass("cOrange");
                }
                else {
                    $(requiredlist[i]).addClass("cOrange");
                }
            });
            return false;
        }
        else {
            if (myType == "radio") {
                $("input[name='" + field_name + "']").parent().removeClass("cOrange");
            }
            else {
                $(requiredlist[i]).removeClass("cOrange");
            }
        }
    }
    return true;
}

// 功能：验证数据录入中，数据格式（#CIMSV5-8需求，对此函数进行扩展，保留原函数名）
// 函数名：checkMaxLength
// @ pId：参数名称，待验证文本框父元素的class或id
function checkMaxLength(pId) {
    var myId = pId;
    if (pId.substring(0, 1) != ".") {
        myId = "#" + pId;
    }
    var checkOk = true;
    $(myId).find(":text:visible").each(function () {
        //验证录入长度
        var maxLength = $(this).attr("maxlength");
        var thisVal = $(this).val().trim();
        var valLength = thisVal.length;
        if (maxLength && valLength > 0 && maxLength < valLength) {
            $(this).addClass("cOrange");
            checkOk = false;
        }
        else {
            $(this).removeClass("cOrange");
        }
        //验证数据格式
        var spanElm = $(this).parent("span");
        if (spanElm && checkOk) {
            var field_formats = spanElm.attr("field_formats");
            var field_type = spanElm.attr("field_type");
            var field_style = spanElm.attr("field_style");
            if (field_formats && (field_type == "Char" || field_type == "Num") && field_style != "SearchBox" && field_style != "DropDownTable" && field_style != "FileBox" && !$(this).prop("readonly") && !$(this).prop("disabled")) {
                fieldFormat(field_formats, this);
                if ($(this).hasClass("cOrange")) {
                    checkOk = false;
                }
            }
            if (field_formats && field_type == "Date" && !$(this).prop("readonly") && thisVal != "" && !$(this).prop("disabled")) {
                switch (field_formats) {
                    case "yyyy-MM":
                        if (!/^\d{4}-\d{2}$/.test(thisVal)) {
                            $(this).addClass("cOrange");
                            checkOk = false;
                        } else {
                            $(this).removeClass("cOrange");
                        }
                        break;
                    case "MMM/yyyy":
                        if (!/^[A-Za-z]{3}\/\d{4}$/.test(thisVal)) {
                            $(this).addClass("cOrange");
                            checkOk = false;
                        } else {
                            $(this).removeClass("cOrange");
                        }
                        break;
                    case "yyyy-MM-dd":
                        if (!/^\d{4}-\d{2}-\d{2}$/.test(thisVal)) {
                            $(this).addClass("cOrange");
                            checkOk = false;
                        } else {
                            $(this).removeClass("cOrange");
                        }
                        break;
                    case "dd/MMM/yyyy":
                        if (!/^\d{2}\/[A-Za-z]{3}\/\d{4}$/.test(thisVal)) {
                            $(this).addClass("cOrange");
                            checkOk = false;
                        } else {
                            $(this).removeClass("cOrange");
                        }
                        break;
                    case "MM/dd/yyyy":
                        if (!/^\d{2}\/\d{2}\/\d{4}$/.test(thisVal)) {
                            $(this).addClass("cOrange");
                            checkOk = false;
                        } else {
                            $(this).removeClass("cOrange");
                        }
                        break;
                }
            }
            if (field_formats && field_type == "Time" && !$(this).prop("readonly") && thisVal != "" && !$(this).prop("disabled")) {
                switch (field_formats) {
                    case "HH:mm:ss":
                        if (!/^\d{2}:\d{2}:\d{2}$/.test(thisVal)) {
                            $(this).addClass("cOrange");
                            checkOk = false;
                        } else {
                            $(this).removeClass("cOrange");
                        }
                        break;
                    case "HH:mm":
                        if (!/^\d{2}:\d{2}$/.test(thisVal)) {
                            $(this).addClass("cOrange");
                            checkOk = false;
                        } else {
                            $(this).removeClass("cOrange");
                        }
                        break;
                    case "HH:mm:00":
                        if (!/^\d{2}:\d{2}:00$/.test(thisVal)) {
                            $(this).addClass("cOrange");
                            checkOk = false;
                        } else {
                            $(this).removeClass("cOrange");
                        }
                        break;
                    case "HH:00:00":
                        if (!/^\d{2}:00:00$/.test(thisVal)) {
                            $(this).addClass("cOrange");
                            checkOk = false;
                        } else {
                            $(this).removeClass("cOrange");
                        }
                        break;
                    case "00:mm:ss":
                        if (!/^00:\d{2}:\d{2}$/.test(thisVal)) {
                            $(this).addClass("cOrange");
                            checkOk = false;
                        } else {
                            $(this).removeClass("cOrange");
                        }
                        break;
                    case "HH":
                        if (!/^\d{2}$/.test(thisVal)) {
                            $(this).addClass("cOrange");
                            checkOk = false;
                        } else {
                            $(this).removeClass("cOrange");
                        }
                        break;
                }
            }
            if (field_formats && field_type == "DateTime" && !$(this).prop("readonly") && thisVal != "" && !$(this).prop("disabled")) {
                switch (field_formats) {
                    case "yyyy-MM-dd HH:mm:ss":
                        if (!/^\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}$/.test(thisVal)) {
                            $(this).addClass("cOrange");
                            checkOk = false;
                        } else {
                            $(this).removeClass("cOrange");
                        }
                        break;
                    case "yyyy-MM-dd HH:mm:00":
                        if (!/^\d{4}-\d{2}-\d{2} \d{2}:\d{2}:00$/.test(thisVal)) {
                            $(this).addClass("cOrange");
                            checkOk = false;
                        } else {
                            $(this).removeClass("cOrange");
                        }
                        break;
                    case "yyyy-MM-dd HH:00:00":
                        if (!/^\d{4}-\d{2}-\d{2} \d{2}:00:00$/.test(thisVal)) {
                            $(this).addClass("cOrange");
                            checkOk = false;
                        } else {
                            $(this).removeClass("cOrange");
                        }
                        break;
                    case "yyyy-MM-dd HH:mm":
                        if (!/^\d{4}-\d{2}-\d{2} \d{2}:\d{2}$/.test(thisVal)) {
                            $(this).addClass("cOrange");
                            checkOk = false;
                        } else {
                            $(this).removeClass("cOrange");
                        }
                        break;
                    case "dd/MMM/yyyy HH:mm:ss":
                        if (!/^\d{2}\/[A-Za-z]{3}\/\d{4} \d{2}:\d{2}:\d{2}$/.test(thisVal)) {
                            $(this).addClass("cOrange");
                            checkOk = false;
                        } else {
                            $(this).removeClass("cOrange");
                        }
                        break;
                    case "dd/MMM/yyyy HH:00:00":
                        if (!/^\d{2}\/[A-Za-z]{3}\/\d{4} \d{2}:00:00$/.test(thisVal)) {
                            $(this).addClass("cOrange");
                            checkOk = false;
                        } else {
                            $(this).removeClass("cOrange");
                        }
                        break;
                    case "dd/MMM/yyyy HH:mm:00":
                        if (!/^\d{2}\/[A-Za-z]{3}\/\d{4} \d{2}:\d{2}:00$/.test(thisVal)) {
                            $(this).addClass("cOrange");
                            checkOk = false;
                        } else {
                            $(this).removeClass("cOrange");
                        }
                        break;
                    case "dd/MMM/yyyy HH:mm":
                        if (!/^\d{2}\/[A-Za-z]{3}\/\d{4} \d{2}:\d{2}$/.test(thisVal)) {
                            $(this).addClass("cOrange");
                            checkOk = false;
                        } else {
                            $(this).removeClass("cOrange");
                        }
                        break;
                    case "yyyy-MM-dd HH":
                        if (!/^\d{4}-\d{2}-\d{2} \d{2}$/.test(thisVal)) {
                            $(this).addClass("cOrange");
                            checkOk = false;
                        } else {
                            $(this).removeClass("cOrange");
                        }
                        break;
                }
            }
        }
    });
    return checkOk;
}

// 功能：获取页面url参数值
// 函数名：getQueryString
// @ name:参数名称
// 说明：如果没有次参数返回null
function getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return decodeURI(r[2]); return null;
}

//扩展textBox
$.fn.textExpand = function () {
    $(this).each(function () {
        var thisWidth = $(this).width();
        var thisHeight = $(this).height();
        var id = $(this).attr("id");
        var event = $(this).attr("onclick");
        $(this).removeAttr("onclick");
        $(this).css("border", "none");
        var value = $(this).val();
        var textBox = document.getElementById(id).outerHTML;
        var imgBtn = "<a href='javascript:" + event + "'><img src='../images/searchbox_button.png' style='float:left;height:22px;z-index:100;border:none'/></a>";
        var div = "<div style='border:solid 1px #cbcbcb; width: " + (parseInt(thisWidth) + 24) + "px; height:" + (parseInt(thisHeight) + 2) + "px;'>";
        div += textBox + imgBtn + "</div>";
        $(this).replaceWith(div);
    });
};
    
//选项卡事件
function bindTabsEvent() {
    var div = $(".con1_form_1").children("div").addClass("tabs");
    $(".titlebg li").click(function () {
        $(this).siblings().children("a").removeClass("hover");
        $(this).addClass("li_hover").children("a").addClass("hover");
        div.removeClass("on").eq($(this).index()).addClass("on").attr("tab", "on").siblings().removeAttr("tab");//针对锁表头display：none 取不到高度，宽度
    });
}

//获取url域(Origin,scheme+host+port)
function getUrlOrigin(url) {
    url = url ? url.toLowerCase() : "";
    if (url.startsWith("https"))
        return /^https?:\/\/[\w-.]+(:\d+)?/i.exec(url)[0];
    else if (url.startsWith("http"))
        return /^http?:\/\/[\w-.]+(:\d+)?/i.exec(url)[0];
    else
        return "";
}
    
//隐藏iframe下载文件
function HiddenDownloadFile(url) {
    if (!url)
        return;
    //因ie浏览器默认设置为禁用跨域，所以ie浏览器采用弹出新窗口或标签(tab)，但是也需用户允许弹窗(浏览器有提示)；非ie浏览器采用隐藏iframe跨域下载。
    if (url.toLowerCase().startsWith("http") && getUrlOrigin(url) !== getUrlOrigin(document.location.href) && ((navigator.userAgent.indexOf("compatible") > -1 && navigator.userAgent.indexOf("MSIE") > -1) || (navigator.userAgent.indexOf('Trident') > -1 && navigator.userAgent.indexOf("rv:11.0") > -1)))
        window.top.open(url, "_blank");
    else
        HiddenDownloadFile.iframe().src = url;
}
HiddenDownloadFile.iframe = function () {
    window.addEventListener("message", function (e) {
        if (e.data && e.data.src && e.data.src === "DOWNLOADFILE")
            myAlert(e.data.msg);
    }); //监听message，接收跨域推送消息

    //iframe载入跨域页面后，父页面无法控制跨域子页面，须移除后重建iframe
    if (HiddenDownloadFile._iframe)
        $(HiddenDownloadFile._iframe).remove();
    HiddenDownloadFile._iframe = $("<iframe style='display:none;'></iframe>").get(0);
    $(HiddenDownloadFile._iframe).appendTo(document.body);
    return HiddenDownloadFile._iframe;
};

//查看项目信息的跳转
function viewProjData(def, sub, proj, seq, type) {
    window.open("../Project_Info_View.aspx?projID=" + proj + "&seq=" + (seq - 1) + "&appType=" + type, "_blank");
}

/*********************** 替换密码框为文本框 ***************************/
function viewEnptPwd(title) {
    $("input[type='password']").each(function() { replace2Txt(this, title); });
}

function replace2Txt(obj, title) {
    if ($(obj).val() == "") {
        $($("<div></div>").append($(obj).clone()).html().replace('type="password"', 'type="text"').replace("type=password", "type=text"))
        .val(title).css("color", "Gray").bind("focus", function() { replace2Pwd(this, title); }).insertAfter(obj);
        $(obj).remove();
    }
}

function replace2Pwd(obj, title) {
    var rep = $("<div></div>").append($(obj).clone()).html();
    rep = rep.indexOf('type="text"') > -1 ? rep.replace('type="text"', 'type="password"') : rep.replace("INPUT", "INPUT type=password");   //for ie8
    $(rep).val("").css("color", "Black").bind("blur", function() { replace2Txt(this, title); }).insertAfter(obj).focus();
    $(obj).remove();
}
/*********************** 替换密码框为文本框 ***************************/

/******************** file-box控件函数 ***************************/
function replaceFile(title) {
    $("input[type='file']").each(function() {
        $("<div class='file-box" + ($(this).attr("disabled") ? " file-disable" : "") + "' onmousemove='moveInputFile(this,event);'><span class='txt'></span><span class='btn'>" + (title ? title : "") + "...</span></div>")
        .insertAfter(this).append($(this).removeAttr("style").attr("class", "file").attr("onchange", "showInputFileName(this)"));   //将bind改为attr以解决在asyncbox中bind事件丢失的问题
    });
}

function enableInputFile(obj) {
    $(obj).attr("disabled", false).parent().removeClass("file-disable");
}

function disableInputFile(obj) {
    obj = $(obj);
    obj.attr("disabled", true).siblings(".txt").text("");
    obj.parent().addClass("file-disable");
    obj[0].outerHTML = obj[0].outerHTML;
}

function showInputFileName(obj) {
    var temp = $(obj).val().split("\\");
    $(obj).siblings(".txt").text(temp[temp.length - 1]);
}

function moveInputFile(obj, event) {
    var offPos = obj.offsetLeft, scrPos = 0;
    var vTempObj = obj
    while (vTempObj = vTempObj.offsetParent) {
        offPos += vTempObj.offsetLeft;  //向上获取控件的offset父控件，计算控件到document左边的offset距离
    }
    vTempObj = obj.parentNode;
    while (vTempObj != document) {
        scrPos += vTempObj.scrollLeft;  //向上获取控件的父控件，计算控件的scroll距离
        vTempObj = vTempObj.parentNode;
    }
    var e = event ? event : (window.event ? window.event : null);
    vTempObj = $(obj).children("input");
    //e.clientX - offPos + scrPos: 触发事件时鼠标到document左边的距离 - 控件到document左边的offset距离 + 控件的scroll距离 = 触发事件时鼠标相对于控件左边的距离 
    vTempObj.css("left", e.clientX - offPos + scrPos - (vTempObj.width() / 2)); //设置file控件位置，使之随鼠标移动，始终处于鼠标下面
}
/******************** file-box控件函数 ***************************/

function equal(objA, objB) {
    if (typeof arguments[0] != typeof arguments[1])
        return false;

    //数组
    if (arguments[0] instanceof Array) {
        if (arguments[0].length != arguments[1].length)
            return false;

        var allElementsEqual = true;
        for (var i = 0; i < arguments[0].length; ++i) {
            if (typeof arguments[0][i] != typeof arguments[1][i])
                return false;

            if (typeof arguments[0][i] == 'number' && typeof arguments[1][i] == 'number')
                allElementsEqual = (arguments[0][i] == arguments[1][i]);
            else
                allElementsEqual = arguments.callee(arguments[0][i], arguments[1][i]);            //递归判断对象是否相等                
        }
        return allElementsEqual;
    }

    //对象
    if (arguments[0] instanceof Object && arguments[1] instanceof Object) {
        var result = true;
        var attributeLengthA = 0, attributeLengthB = 0;
        for (var o in arguments[0]) {
            //判断两个对象的同名属性是否相同（数字或字符串）
            if (typeof arguments[0][o] == 'number' || typeof arguments[0][o] == 'string')
                result = eval("arguments[0]['" + o + "'] == arguments[1]['" + o + "']");
            else {
                //如果对象的属性也是对象，则递归判断两个对象的同名属性
                //if (!arguments.callee(arguments[0][o], arguments[1][o]))
                if (!arguments.callee(eval("arguments[0]['" + o + "']"), eval("arguments[1]['" + o + "']"))) {
                    result = false;
                    return result;
                }
            }
            ++attributeLengthA;
        }

        for (var o in arguments[1]) {
            ++attributeLengthB;
        }

        //如果两个对象的属性数目不等，则两个对象也不等
        if (attributeLengthA != attributeLengthB)
            result = false;
        return result;
    }
    return arguments[0] == arguments[1];
}
//去除数组重复
Array.prototype.unique5 = function () {
    var res = [], hash = {};
    for (var i = 0, elem; (elem = this[i]) != null; i++) {
        if (!hash[elem]) {
            res.push(elem);
            hash[elem] = true;
        }
    }
    return res;
}

//绑定事件
function PlaceEventBind(oid) {
    $("#" + oid + " input[placeholder]").bind("keyup", window.Event, PlaceEvent).bind("blur", window.Event, PlaceEvent);
    $("#" + oid + " textarea[placeholder]").bind("keyup", window.Event, PlaceEvent).bind("blur", window.Event, PlaceEvent);
}

function PlaceEvent(ele) {
    if (ele.target && ele.target == this)       //判断ele参数是否为event参数,如果是则将ele转换为元素element对象
        ele = ele.target;
    if ($(ele).val() == "" || $(ele).val() == $(ele).attr('placeholder')) {
        $(ele).val('');
        $(ele).addClass('cPrompt');
    }
    else {
        $(ele).removeClass('cPrompt');
    }
    return false;
}

//调整窗体大小
function setResize() {
    var conterHeight = 0;
    var vObj = $('.rightContent');
    if (vObj.length > 0) {
        var windowHeight = $(window).height();
        windowHeight = windowHeight < 300 ? 300 : windowHeight;

        var vObj2 = $(".rightNavigation");
        conterHeight += (vObj2.length > 0 ? vObj2.height() : 0);

        vObj2 = $(".rightButton");
        conterHeight += (vObj2.length > 0 ? vObj2.height() : 0);

        conterHeight = windowHeight - conterHeight;
        vObj.height(conterHeight);
    }

    //resize table container
    vObj = $(".rightContent .tableList");
    if (vObj.length > 0) {
        var sibHeight = 0; //计算兄弟节点所占高度
        vObj.siblings().each(function () { sibHeight += $(this).outerHeight(true); });
        vObj.height(conterHeight - sibHeight - 10);   //留10px边距,否则部分浏览器会出现滚动条
    }
}
//resize page size

$(function () {
    setResize();
    $(window).resize(function () { setResize(); });
});

//一个非常实用的javascript读写Cookie函数 
function GetCookieVal(offset)
    //获得Cookie解码后的值 
{
    var endstr = document.cookie.indexOf(";", offset);
    if (endstr == -1)
        endstr = document.cookie.length;
    return unescape(document.cookie.substring(offset, endstr));
}

    //设定Cookie值 
function SetCookie(name, value)
{
    var expdate = new Date();
    var argv = SetCookie.arguments;
    var argc = SetCookie.arguments.length;
    var expires = (argc > 2) ? argv[2] : null;
    var path = (argc > 3) ? argv[3] : null;
    var domain = (argc > 4) ? argv[4] : null;
    var secure = (argc > 5) ? argv[5] : false;
    if (expires != null) expdate.setTime(expdate.getTime() + (expires * 1000));
    document.cookie = name + "=" + escape(value) + ((expires == null) ? "" : ("; expires=" + expdate.toGMTString()))
    + ((path == null) ? "" : ("; path=" + path)) + ((domain == null) ? "" : ("; domain=" + domain))
    + ((secure == true) ? "; secure" : "");
}

//删除Cookie 
function DelCookie(name)
{
    var exp = new Date();
    exp.setTime(exp.getTime() - 1);
    var cval = GetCookie(name);
    document.cookie = name + "=" + cval + "; expires=" + exp.toGMTString();
}

//获得Cookie的原始值 
function GetCookie(name)
{
    var arg = name + "=";
    var alen = arg.length;
    var clen = document.cookie.length;
    var i = 0;
    while (i < clen) {
        var j = i + alen;
        if (document.cookie.substring(i, j) == arg)
            return GetCookieVal(j);
        i = document.cookie.indexOf(" ", i) + 1;
        if (i == 0) break;
    }
    return null;
}

//下拉输入选择
function onSelectInput(ele) {
    ele = $(ele);
    var ShowId = $("#" + ele.attr("name")), sli_val = ele.val();
    ele.bind("blur", function () { ShowId.fadeOut(); });
    //设置为 true 时，计算外边距 margin在内
    var sli = ShowId.css({
        "width": ele.outerWidth(true),
        "top": ele.offset().top + 1,
        "left": ele.offset().left,
        "display": "block",
        "z-index": "1000000"
    }).children().unbind("click").bind("click", function () { ele.val($(this).text()); }).filter(function () {
        return $(this).text() == sli_val;
    });
    if (sli && sli.length > 0)
        ShowId.scrollTop(sli.height() * sli.index());   //滚动到当前li
}

function onDownInput(ele) {
    var top = $(ele).position().top + 22;
    var left = $(ele).position().left;
    var width = $(ele).outerWidth(true);//设置为 true 时，计算外边距 margin在内
    var ShowId = $(ele).attr("name");
    $("#" + ShowId).css({ "width": width, "top": top, "left": left, "display": "block", "z-index": "1000000" });
    $("#" + ShowId + " li").unbind("click");
    $("#" + ShowId + " li").bind("click", function () {
        $(ele).val($(this).text());
    });
    $(ele).bind("blur", function () {
        $("#" + ShowId).fadeOut();
    });
}

//解码特殊字符
String.prototype.toDecSpecialChar = function () {
    return this.replaceAll("Ｔ", "T");
}

//转换为数值, 参数n: 保留n为小数（不舍入，不足n位则补零）
String.prototype.toNum = function (n) {
    if (isNaN(this)) {
        return "";
    }
    else
    {
        if (arguments.length == 0)
            return Number(this);
        else {
            n = Number(n);
            return (Math.floor(Number(this) * Math.pow(10, n)) / Math.pow(10, n)).toFixed(n);
        }
            
    }
}

Number.prototype.toNum = function (n) {
    if (isNaN(this)) {
        return "";
    }
    else {
        if (arguments.length == 0)
            return Number(this);
        else {
            n = Number(n);
            return (Math.floor(Number(this) * Math.pow(10, n)) / Math.pow(10, n)).toFixed(n);
        }

    }
}

//绝对值
String.prototype.toAbs = function () {
    if (isNaN(this)) {
        return "";
    }
    else
    {
        return Math.abs(this);
    }
}

Number.prototype.toAbs = function () {
    if (isNaN(this)) {
        return "";
    }
    else {
        return Math.abs(this);
    }
}

//向上取整
String.prototype.toCeil = function () {
    if (isNaN(this)) {
        return "";
    }
    else
    {
        return Math.ceil(this);
    }
}

Number.prototype.toCeil = function () {
    if (isNaN(this)) {
        return "";
    }
    else {
        return Math.ceil(this);
    }
}

//向下取整
String.prototype.toFloor = function () {
    if (isNaN(this)) {
        return "";
    }
    else
    {
        return Math.floor(this);
    }
}

Number.prototype.toFloor = function () {
    if (isNaN(this)) {
        return "";
    }
    else {
        return Math.floor(this);
    }
}

//四舍五入
String.prototype.toRound = function (num, flag) { //num 数字,保留小数的位数; flag 0或1
    if (isNaN(this)) {
        return "";
    }
    else {
        var iflag = flag;        
        if (arguments.length == 1) {
            iflag = 0;
        }
        var res;
        if (this.charAt(0) == "-") {
            res = -(Math.round(-(this).toString() + "e+" + num) + "e-" + num)
        } else {
            res = +(Math.round(+(this).toString() + "e+" + num) + "e-" + num)
        }       
        if (iflag) {
            res = res + "";
            var num_arry = res.split(".");
            var addNum = num / 1;
            if (num_arry.length == 2) {
                addNum = addNum - num_arry[1].length;
            }
            for (var i = 0; i < addNum; i++) {
                if (num_arry.length == 1) {
                    if(i == 0)
                        res = res + ".0";
                    else
                        res = res + "0";
                }
                if (num_arry.length == 2)
                    res = res + "0";
            }
        }
        return res;
    }
}

Number.prototype.toRound = function (num, flag) { //num 数字,保留小数的位数; flag 0或1
    that = (this).toString();
    if (isNaN(that)) {
        return "";
    }
    else {
        var iflag = flag;
        if (arguments.length == 1) {
            iflag = 0;
        }
        var res;
        if (that.charAt(0) == "-") {
            res = -(Math.round(-(that).toString() + "e+" + num) + "e-" + num)
        } else {
            res = +(Math.round(+(that).toString() + "e+" + num) + "e-" + num)
        }        
        if (iflag) {
            res = res + "";
            var num_arry = res.split(".");
            var addNum = num / 1;
            if (num_arry.length == 2) {
                addNum = addNum - num_arry[1].length;
            }
            for (var i = 0; i < addNum; i++) {
                if (num_arry.length == 1) {
                    if (i == 0)
                        res = res + ".0";
                    else
                        res = res + "0";
                }
                if (num_arry.length == 2)
                    res = res + "0";
            }
        }
        return res;
    }
}

//英文月份定义
var month = new Array(12);
month[0] = "Jan";
month[1] = "Feb";
month[2] = "Mar";
month[3] = "Apr";
month[4] = "May";
month[5] = "Jun";
month[6] = "Jul";
month[7] = "Aug";
month[8] = "Sep";
month[9] = "Oct";
month[10] = "Nov";
month[11] = "Dec";

//格式化日期 参数 fmt: yyyy-MM-dd HH:mm:ss （MMM 返回英文月份缩写）
Date.prototype.toDateformat = function (fmt) {
    var o = {
        "M+": this.getMonth() + 1, //月份 
        "d+": this.getDate(), //日 
        "H+": this.getHours(), //小时 
        "m+": this.getMinutes(), //分 
        "s+": this.getSeconds(), //秒
        "ms": this.getMilliseconds() //毫秒 
    };
    if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o) {
        if (k == "M+" && fmt.indexOf("MMM") >= 0) {
            if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, month[o[k]-1]);
        }
        else {
            if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
        }
    }
    return fmt;
}

String.prototype.toDateformat = function (fmt) {
    var dateStr = TransformEnDate(this);
    var dateobj = new Date(dateStr.replace(/-/g, '/'));
    var o = {
        "M+": dateobj.getMonth() + 1, //月份 
        "d+": dateobj.getDate(), //日 
        "H+": dateobj.getHours(), //小时 
        "m+": dateobj.getMinutes(), //分 
        "s+": dateobj.getSeconds(), //秒
        "ms": dateobj.getMilliseconds() //毫秒 
    };
    if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (dateobj.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o) {
        if (k == "M+" && fmt.indexOf("MMM") >= 0) {
            if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, month[o[k]-1]);
        }
        else {
            if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
        }
    }
    return fmt;
}

//日期加  dateadd("yy", 3, dateStr ) 表示在dateStr上加3年
function Dateadd(par, num, dateStr) {
    par = par.toLowerCase();
    dateStr = TransformEnDate(dateStr);
    if (dateStr.indexOf(" ") >= 0 || dateStr.indexOf(":") < 0) {
        var d = new Date(dateStr.replace(/-/g, '/'));
    } else {
        var newdate = new Date().toDateformat("yyyy/MM/dd");
        var newStr = newdate + " " + dateStr
        var d = new Date(newStr.replace(/-/g, '/'));
    }
    if (num == 0)
        return new Date(d);
    var k = { 'yy': 'FullYear', 'yyyy': 'FullYear', 'm': 'Month', 'mm': 'Month', 'wk': 'Date', 'ww': 'Date', 'd': 'Date', 'dd': 'Date', 'h': 'Hours', 'hh': 'Hours', 'n': 'Minutes', 'mi': 'Minutes', 's': 'Seconds', 'ss': 'Seconds', 'ms': 'MilliSeconds' };
    var n = { 'wk': 7, 'ww': 7 };
    eval('d.set' + k[par] + '(d.get' + k[par] + '()+' + ((n[par] || 1) * num) + ')');
    return d;
}

//日期减 datediff( "h", "15:20:30", "16:30:30") 返回两个日期相差几小时
function Datediff(par, dateStr, dateStr2) {
    par = par.toLowerCase();
    dateStr = TransformEnDate(dateStr);
    dateStr2 = TransformEnDate(dateStr2);
    if (dateStr.indexOf(" ") >= 0 || dateStr.indexOf(":") < 0) {
        var dateObj1 = new Date(dateStr.replace(/-/g, '/'));
    } else {
        var date1 = new Date().toDateformat("yyyy/MM/dd");
        var newStr = date1 + " " + dateStr
        var dateObj1 = new Date(newStr.replace(/-/g, '/'));
    }
    if (dateStr2.indexOf(" ") >= 0 || dateStr.indexOf(":") < 0) {
        var dateObj2 = new Date(dateStr2.replace(/-/g, '/'));
    } else {
        var date2 = new Date().toDateformat("yyyy/MM/dd");
        var newStr2 = date2 + " " + dateStr2
        var dateObj2 = new Date(newStr2.replace(/-/g, '/'));
    }
    var i = {}, t = dateObj1.getTime(), t2 = dateObj2.getTime();
    i['yy'] = dateObj2.getFullYear() - dateObj1.getFullYear();
    i['yyyy'] = dateObj2.getFullYear() - dateObj1.getFullYear();
    i['m'] = i['yy'] * 12 + dateObj2.getMonth() - dateObj1.getMonth();
    i['mm'] = i['yy'] * 12 + dateObj2.getMonth() - dateObj1.getMonth();
    i['ms'] = dateObj2.getTime() - dateObj1.getTime();
    i['ww'] = Math.floor((t2 + 345600000) / (604800000)) - Math.floor((t + 345600000) / (604800000));
    i['wk'] = Math.floor((t2 + 345600000) / (604800000)) - Math.floor((t + 345600000) / (604800000));
    i['d'] = Math.floor(t2 / 86400000) - Math.floor(t / 86400000);
    i['dd'] = Math.floor(t2 / 86400000) - Math.floor(t / 86400000);
    i['h'] = Math.floor(t2 / 3600000) - Math.floor(t / 3600000);
    i['hh'] = Math.floor(t2 / 3600000) - Math.floor(t / 3600000);
    i['n'] = Math.floor(t2 / 60000) - Math.floor(t / 60000);
    i['mi'] = Math.floor(t2 / 60000) - Math.floor(t / 60000);
    i['s'] = Math.floor(t2 / 1000) - Math.floor(t / 1000);
    i['ss'] = Math.floor(t2 / 1000) - Math.floor(t / 1000);
    return i[par];
}

//获取部分日期
/****参数****
d: 日期时间 如 2017-1-3 9:9:9
p: 返回格式 取值如下
   yy    年  17
   yyyy  年  2017
   M     月   1
   MM    月   01
   MMM   月份英文缩写
   d     日   3
   dd    日   03
   H     时   9
   HH    时   09
   m     分   9
   mm    分   09
   s     秒   9
   ss    秒   09
   ms    毫秒
************/
function Datepart(p, d) {
    d = d + ""; //转为字符串
    if (d == "")
        return "";
    d = d.replace(/-/g, '/');
    d = TransformEnDate(d);
    //修复谷歌浏览器下 new Date("UN UN 01") 能返回确认日期的问题 /Mon Jan 01 2001 00:00:00 GMT+0800
    var onlyLettersRegex = /^[a-zA-Z]+$/;
    var dp = d.split(" ");
    var isPartialDate = false;
    var EnMonth = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
    for (var i = 0; i < dp.length; i++) {
        var element = dp[i];
        // 检查元素是否仅由字母组成  
        if (onlyLettersRegex.test(element.trim())) { 
            if (EnMonth.indexOf(element) === -1) {
                isPartialDate = true;
                break;
            }
        }
    }
    if (isPartialDate)
        return "";
    if (d.indexOf(" ") >= 0 || d.indexOf(":") < 0) { // Date & DateTime (yyyy/MM/dd HH、MMM/yyyy 不能用new Date()转为有效日期，需要特殊处理)
        var dateObj = new Date(d);
        if (/^\d{4}\/\d{1,3}\/\d{1,2} \d{1,2}$/.test(d)) { //yyyy/MM/dd HH 格式
            dateObj = new Date(d + ":00:00");
        }
        if (/^[A-Za-z]{3} \d{4}$/.test(d)) { //MMM yyyy 格式
            dateObj = new Date("01 " + d);
        }
    }
    else // time
    {
        var newdate = new Date().toDateformat("yyyy/MM/dd");
        var newStr = newdate + " " + d
        var dateObj = new Date(newStr.replace(/-/g, '/'));
    }
    var o = {
        "M+": dateObj.getMonth() + 1, //月份 
        "d+": dateObj.getDate(), //日 
        "H+": dateObj.getHours(), //小时 
        "m+": dateObj.getMinutes(), //分 
        "s+": dateObj.getSeconds(), //秒
        "ms": dateObj.getMilliseconds() //毫秒 
    };
    if (/(y+)/.test(p)) p = p.replace(RegExp.$1, (dateObj.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o) {
        if (k == "M+" && p.indexOf("MMM") >= 0) {
            if (new RegExp("(" + k + ")").test(p)) p = p.replace(RegExp.$1, month[o[k] - 1]);
        }
        else {
            if (new RegExp("(" + k + ")").test(p)) p = p.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
        }
    }
    return p;
}

//转换英文日期格式 20/Sep/2017 为 20 Sep 2017，以兼容IE浏览器
function TransformEnDate(dateStr) {
    var reg = /^[^a-zA-Z]+$/;
    if (!reg.test(dateStr)) {
        dateStr = dateStr.replace(/\//g, ' ');
    }
    return dateStr;
}

/*将服务器日期转化为本地日期
*
*s_date:<string> 服务器日期
* 
*/
function ServerToLocalDate(s_date) {
    if (!Date.parse(s_date)) {
        myAlert("Conversion Failed, Invalid Date!");
        return "";
    }
    //服务端时间与UTC时间的偏差
    var serUtcOffset = sessionStorage.getItem("ser_utc_offset") / 1;//单位：秒
    //本地时间与UTC时间的偏差
    var localUtcOffset = -(new Date().getTimezoneOffset() * 60);//单位：秒
    //服务器时间与本地时间的偏差
    var localSerOffset = localUtcOffset.toFixed(0) / 1 - serUtcOffset.toFixed(0) / 1;
    return Dateadd("s", localSerOffset, s_date)
}

/*
 * Created with Sublime Text 3.
 * license: http://www.lovewebgames.com/jsmodule/index.html
 * github: https://github.com/tianxiangbing/format-number
 * User: 田想兵
 * Date: 2015-08-05
 * Time: 11:27:55
 * Contact: 55342775@qq.com
 */
;
(function (root, factory) {
    //amd
    if (typeof define === 'function' && define.amd) {
        define(['$'], factory);
    } else if (typeof exports === 'object') { //umd
        module.exports = factory();
    } else {
        root.Loading = factory(window.Zepto || window.jQuery || $);
    }
})(this, function ($) {
    var Loading = function () { };
    Loading.prototype = {
        loadingTpl: '<div class="ui-loading"><div class="ui-loading-mask"></div><i></i></div>',
        stop: function () {
            var content = $(this.target);
            this.loading.remove();
        },
        start: function () {
            var _this = this;
            var target = _this.target;
            var content = $(target);
            var loading = this.loading;
            if (!loading) {
                loading = $(_this.loadingTpl);
                $('body').append(loading);
            }
            this.loading = loading;
            var ch = $(content).outerHeight();
            var cw = $(content).outerWidth();
            if ($(target)[0].tagName == "HTML") {
                ch = Math.max($(target).height(), $(window).height());
                cw = Math.max($(target).width(), $(window).width());
            }
            loading.height(ch).width(cw);
            loading.find('div').height(ch).width(cw);
            if (ch < 100) {
                loading.find('i').height(ch).width(ch);
            }
            var offset = $(content).offset();
            loading.css({
                top: offset.top,
                left: offset.left
            });
            var icon = loading.find('i');
            var h = ch,
				w = cw,
				top = 0,
				left = 0;
            if ($(target)[0].tagName == "HTML") {
                h = $(window).height();
                w = $(window).width();
                top = (h - icon.height()) / 2 + $(window).scrollTop();
                left = (w - icon.width()) / 2 + $(window).scrollLeft();
            } else {
                top = (h - icon.height()) / 2;
                left = (w - icon.width()) / 2;
            }
            icon.css({
                top: top,
                left: left
            })
        },
        init: function (settings) {
            settings = settings || {};
            this.loadingTpl = settings.loadingTpl || this.loadingTpl;
            this.target = settings.target || 'html';
            this.bindEvent();
        },
        bindEvent: function () {
            var _this = this;
            $(this.target).bind('stop', function () {
                _this.stop();
            });
        }
    }
    return Loading;
});

/* 
 * drag 1.0
 * 滑动验证
 * date 2017-06-7
 * 拖动滑块
 */
(function ($) {
    $.fn.drag = function (options, options1) {
        var lang = $("#select_Language option:selected").val();
        var Nvalue = "";
        var Yvalue = "";
        var x, drag = this, isMove = false, defaults = {
        };
        
        //添加背景，文字，滑块
        var html = '<div class="drag_bg"></div>' +
                    '<div class="drag_text" onselectstart="return false;" unselectable="on" style=\"text-indent: 30px;\">' + options + '</div>' +
                    '<div class="handler handler_bg"></div>';
        this.html("").append(html).css("color", "#000");
        this.attr("Verification", "Verification");
        var handler = drag.find('.handler');
        var drag_bg = drag.find('.drag_bg');
        var text = drag.find('.drag_text');
        var maxWidth = drag.width() - handler.width();  //能滑动的最大间距

        //鼠标按下时候的x轴的位置
        handler.mousedown(function (e) {
            isMove = true;
            x = e.pageX - parseInt(handler.css('left'), 10);
        });

        //鼠标指针在上下文移动时，移动距离大于0小于最大间距，滑块x轴位置等于鼠标移动距离
        $(document).mousemove(function (e) {
            var _x = e.pageX - x;
            if (isMove) {
                if (_x > 0 && _x <= maxWidth) {
                    handler.css({ 'left': _x });
                    drag_bg.css({ 'width': _x });
                } else if (_x > maxWidth) {  //鼠标指针移动距离达到最大时清空事件
                    handler.css({ 'left': maxWidth });
                    drag_bg.css({ 'width': maxWidth });
                    dragOk();
                }
            }
        }).mouseup(function (e) {
            isMove = false;
            var _x = e.pageX - x;
            if (_x < maxWidth) { //鼠标松开时，如果没有达到最大距离位置，滑块就返回初始位置
                handler.css({ 'left': 0 });
                drag_bg.css({ 'width': 0 });
            }
        });

        //清空事件
        function dragOk() {
            handler.removeClass('handler_bg').addClass('handler_ok_bg');
            text.text(options1);
            text.css("text-indent", "-30px");
            drag.css({ 'color': '#fff' });
            handler.unbind('mousedown');
            $(document).unbind('mousemove');
            $(document).unbind('mouseup');
            if ($("#drag").attr("Verification") == "Verification") {
                $("#drag").removeAttr("Verification");
            }
        }
    };
})(jQuery);

//禁止键盘方向键
function forbidArrowKey(event) {
    var key_code = event.which || event.keyCode;
    if (key_code == 37 || key_code == 38 || key_code == 39 || key_code == 40) { //左、上、右、下键
        return false;
    }
}

//获取网址的根目录
function getRootPath_dc() {
    var pathName = window.location.pathname.replace("//","/").substring(1);
    var webName = pathName == '' ? '' : pathName.substring(0, pathName.indexOf('/'));
    if (webName == "") {
        return window.location.protocol + '//' + window.location.host+"/";
    }
    else {
        return window.location.protocol + '//' + window.location.host + '/' + webName+"/";
    }
}


//导出文件
function Export(ele) {
    var top = $(ele).offset().top + 2;
    var left = $(ele).offset().left;
    var width = $(ele).outerWidth(true);//设置为 true 时，计算外边距 margin在内
    var func = "";
    if ($(ele).children("dt").length == 0)
        $(ele).append("<dt id=\"export\" class=\"Select_depict\"><dl name=\"excel\"><span><img style=\"vertical-align:text-top\" src=\"../../images/icons/page_excel.png\" /></span><span style=\"margin-left:5px;\">Excel</span></dl><dl name=\"csv\"><span><img style=\"vertical-align:text-top\" src=\"../../images/icons/doc_excel_csv.png\" /></span><span style=\"margin-left:5px;\">CSV</span></dl></dt>");

    $("#export").css({ "width": width, "top": top, "left": left, "display": "block", "z-index": "1000000" });
    $("#export dl").unbind("click");
    $("#export dl").bind("click", function () {
        if ($(this).attr("name") == "excel") {
            ExportExcel();
        }
        else {
            ExportCSV();
        }
        $("#export").fadeOut("fast");
    });
    $(document).bind("click", function (e) {
        var target = $(e.target);
        if (target.closest(ele).length == 0) {
            $("#export").fadeOut("fast");
        }
    });
}

/*
 * 根据Value格式化为带有换行、空格格式的HTML代码
 * @param strValue {String} 需要转换的值
 * @return  {String}转换后的HTML代码
 * @example  
 * getFormatCode("测\r\n\s试")  =>  “测<br/> 试”
 */
var getFormatCode = function (strValue) {
    return strValue.replace(/\r\n/g, '<br/>').replace(/\n/g, '<br/>').replace(/\s/g, ' ');
}

function getViewFileUrl() {
    return sessionStorage.getItem("root");
    //return GetCookie("root");
}

//根据语言设置字体
(function setFontFamily() {
    if (sessionStorage.getItem("session")) {
        var lang = JSON.parse(sessionStorage.getItem("session")).Language;
        if (lang == "en") {
            $("head").append("<style type='text/css'>.divForm span[is_field='false'],.divForm span[is_field='true']{font-family:'Times New Roman';}</style>");
        } else {
            $("head").append("<style type='text/css'>.divForm span[is_field='false'],.divForm span[is_field='true']{font-family:SimSun;}</style>");
        }
    }   
}());

//数据报告参数
function reportChartOption() {
    var options = {};
    options.option = {
        title: {
            left: 'center',
            textStyle: {
                fontSize: 22
            }
        },
        //color: ['#31bece', '#3174ce', '#314bce', '#5231ce', '#39bf5f', '#e6ae37', '#e68e37', '#e64637', '#af31ce', '#aaaaaa', '#3195ce'],
        legend: {
            bottom: 5,
            type: 'scroll',
        },
        tooltip: {
            show: true            
        },
        yAxis: {
            type: 'value'
        },
        xAxis: {
            type: 'category',
            axisLine: { onZero: false },
            axisLabel: {
                show: true,
                interval: "auto",
                fontSize: 14,
            },
            axisTick: {
                show: false,
            }
        },
        grid: {
            left: '80px',
            right: '60px',
            top: '50px',
            bottom: '70px'
        }
    };
    options.barMaxWidth = 60;
    options.symbolSize = 8;
    options.barStyle = {
        shadowBlur: 2,
        shadowOffsetX: 1,
        barBorderRadius: [1,1,0,0]
    };
    options.shadowBlur = 2;
    options.shadowOffsetX = 1;
    options.barBorderRadius = [1, 1, 0, 0];
    return options;
}

// 语言名称
function GetLanguageName(pLang)
{
    switch (pLang) {
        case "zh-CN":
            return "简体中文";
        case "en":
            return "English";
        case "zh-TW":
            return "繁体中文";
        case "ja":
            return "日文";
        case "ko":
            return "韩文";
        default:
            return "English";
    }
}

// js 加、减、乘、除运算
function calcDigit(list) {
    var result = 0;
    for (var _i = 0, list_1 = list; _i < list_1.length; _i++) {
        var num = Math.abs(list_1[_i]/1);
        var str = num.toExponential();
        var match = str.match(/^\d(?:\.(\d+))?e([\+\-])(\d+)$/);
        var ret = match && match[1] && match[1].length || 0;
        if (match && match[2] === "+") {
            ret -= Number(match[3]);
        }
        else if (match) {
            ret += Number(match[3]);
        }
        if (result < ret) {
            result = ret;
        }
    }
    return Math.pow(10, result);
};
function calcToUpperDigit(num, digit) {
    return Math.round(num/1 * digit/1);
};
//加
function calcAdd() {
    var args = [];
    for (var _i = 0; _i < arguments.length; _i++) {
        args[_i] = arguments[_i];
    }
    var digit = calcDigit(args);
    var result = calcToUpperDigit(args[0], digit) || 0;
    for (var i = 1; i < args.length; i++) {
        result += calcToUpperDigit(args[i], digit);
    }
    return result / digit;
};
//减
function calcSub() {
    var args = [];
    for (var _i = 0; _i < arguments.length; _i++) {
        args[_i] = arguments[_i];
    }
    var digit = calcDigit(args);
    var result = calcToUpperDigit(args[0], digit) || 0;
    for (var i = 1; i < args.length; i++) {
        result -= calcToUpperDigit(args[i], digit);
    }
    return result / digit;
};
//乘
function calcMulti() {
    var args = [];
    for (var _i = 0; _i < arguments.length; _i++) {
        args[_i] = arguments[_i];
    }
    var digit = calcDigit(args);
    var result = calcToUpperDigit(args[0], digit) || 0;
    for (var i = 1; i < args.length; i++) {
        result *= calcToUpperDigit(args[i], digit);
    }
    return result / Math.pow(digit, args.length);
};
//除
function calcDiv() {
    var args = [];
    for (var _i = 0; _i < arguments.length; _i++) {
        args[_i] = arguments[_i];
    }
    var digit = calcDigit(args);
    var result = calcToUpperDigit(args[0], digit) || 0;
    for (var i = 1; i < args.length; i++) {
        result /= calcToUpperDigit(args[i], digit);
    }
    return result;
};

/**
 * 添加水印
 * @param {string} content:水印文字
 * @param {object} container:水印容器，jquery对象
 * @param {string} width:生成图片宽度
 * @param {string} height:生成图片高度
 * @param {string} fillStyle:水印颜色 rgba
 * @param {string} rotate:水印旋转角度
 * @param {number} zIndex:css z-index
 * @returns {Null}
 */
(function () {
    function addWatermark({
        content = 'CIMS',
        container = $("body"),
        width = '100px',
        height = '100px',
        fillStyle = 'rgba(250, 52, 52, 0.7)',       
        rotate = '20',
        zIndex = 1000
    } = {}) {
        var canvas = document.createElement('canvas');
        canvas.setAttribute('width', width);
        canvas.setAttribute('height', height);
        var ctx = canvas.getContext("2d");

        ctx.textAlign = 'end';
        ctx.textBaseline = 'top';
        ctx.font = "20px microsoft yahei";
        ctx.fillStyle = fillStyle;
        ctx.rotate(Math.PI / 180 * rotate);
        ctx.fillText(content, parseFloat(width), 0);

        var base64Url = canvas.toDataURL();
        var watermarkDiv = document.createElement("div");

        watermarkDiv.setAttribute('style', `
              position:absolute;
              top:0;
              right:0;
              width:${width};
              height:${height};
              z-index:${zIndex};
              pointer-events:none;
              `);
        $(watermarkDiv).html(`<img src="${base64Url}" />`)

        container.css("position", "relative");
        container.prepend(watermarkDiv);
    };
    window.addWatermark = addWatermark;
})();

//将" < > 替换为字符实体（避免字符串包含此类字符时，该字符串作为HTML属性被截断问题）
function toEntity(str) {
    return str.replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/"/g, '&quot;');
}

//IP归属地查询
function getIpLocation(ip, elm) {
    $.ajaxSettings.async = false;
    $(elm).unbind("mouseleave");
    var ielm = $(elm).find("i");
    if (ielm.length > 0 && ielm.text() != "Query Failed") {
        ielm.show();
    } else {
        var IP = JSON.parse(sessionStorage.getItem("IP") || "{}");
        var queryIp = IP[ip];        
        if (queryIp) {
            ielm = $('<i>' + queryIp +'</i>');
        } else {
            ielm = $('<i>Loading...</i>');
            $.post("../../Resource/GetIPLocation.ashx", { "ip": ip }, function (data) {
                if (data.code == 200) {
                    var res = data.data.location;
                    var ipRes = res.country + "，" + res.province + "，" + res.city;
                    ielm.text(ipRes);
                    IP[ip] = ipRes;
                    sessionStorage.setItem("IP", JSON.stringify(IP));
                } else {
                    ielm.text("Query Failed");
                }
            });
        }
        ielm.css({
            "position": "absolute",
            "top": "-30px",
            "background-color": "#FAFBCF",
            "color": "#333",
            "z-index": "99",
            "border": "1px solid #ccc",
            "padding": " 0px 5px",
        })
        $(elm).append(ielm);
        var iObj = $(elm).find("i");
        iObj.css("left", -iObj.outerWidth() + $(elm).width())
    }
    $(elm).bind("mouseleave", function () {
        ielm.hide();
    });
    $.ajaxSettings.async = true;
}

//Enter键搜索
(function () {
    document.onkeydown = function (event) {
        var key_code = event.which || event.keyCode;
        if (key_code == 13) {
            var searchElm = $(".leftCondition a.find").parent();
            if (searchElm.length > 0) {
                searchElm.click();
                return false;
            }
        }
    }
})();