// ==UserScript==
// @name         XXX视频一键下载
// @namespace    http://tampermonkey.net/
// @version      0.1
// @description  XXX播放页面生成下载按钮
// @author       Charles
// @match        https://www.xxx.com/video.html*
// @grant        none
// @run-at       document-end
// @compatible   chrome
// @require      http://cdn.staticfile.org/jquery/1.12.1/jquery.min.js
// ==/UserScript==

(function () {
    'use strict';

    //!window.jQuery && document.write('<script src=https://apps.bdimg.com/libs/jquery/1.11.1/jquery.min.js><\/script>');

    if (!window.interval_custom) {
        window.interval_custom = setInterval(function () {
            //--------------------------------------------------主要修改区域 Start--------------------------------------------------
            let videoElement = document.querySelector("video");
            if (videoElement && videoElement.readyState > 0 && $(videoElement).children("source").length > 0) {
                let reg = /(https?.+m3u8.*)/ig;
                if (reg.test($(videoElement).children("source").prop("src"))) {
                    clearInterval(window.interval_custom);
                    window.interval_custom = null;
                    let video_url = RegExp.$1;

                    //移除文件名中包含的非法字符（Windows）
                    let video_name = replaceIllegalChar("文件名测试") + "[" + pad(parseInt(videoElement.duration / 60), 2) + "：" + pad(parseInt(videoElement.duration % 60), 2) + "]";

                    //注意：参数值都需要base64编码，避免中文等字符乱码（URL Protocol调用程序m3u8dl.service.exe，会自动添加decode=true参数到URL头部）
                    let startup = "";//b64EncodeUnicode("tray")，默认normal
                    let config_name = "";//b64EncodeUnicode("aria2c.conf")，默认aria2c.conf
                    let aria2c_args_append = "";//b64EncodeUnicode("--referer=" + document.location.href)，参考Aria2c命令行参数
                    let max_speed_limit = "";//b64EncodeUnicode("1024")，2048、4096，整数
                    let name_url_string = b64EncodeUnicode(video_name) + ":" + b64EncodeUnicode(video_url);//key1:value1;key2:value2;...
                    let action_after_downloaded = "";//b64EncodeUnicode("休眠")，中文指令，退出程序、无操作、关机、睡眠...
                    //--------------------------------------------------主要修改区域 End--------------------------------------------------
                    let $a_link = $(`<a href='M3u8DL://startup=${startup}|config-name=${config_name}|aria2c-args-append=${aria2c_args_append}|max-speed-limit=${max_speed_limit}|name-url-string=${name_url_string}|action-after-downloaded=${action_after_downloaded}'>下载<br>视频</a>`);
                    $a_link.one("click", function () { $(this).click(function () { $(this).prop("disabled", true); return false; }); });

                    let $download_btn = $("<div id='div_download_btn'></div>");
                    $download_btn.css({
                        "display": "block",
                        "position": "absolute",
                        "top": ($(document).scrollTop() + 25) + "px",
                        "left": ($(document).width() - 70) + "px",
                        "width": "48px",
                        "height": "48px",
                        "color": "red",
                        "font-size": "16px",
                        "background-image": "url(data:img/jpg;base64,iVBORw0KGgoAAAANSUhEUgAAADAAAAAwCAMAAABg3Am1AAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAAAXVBMVEUYupwYtpwYloQhgnshinsYtpQhZWspPEopTVohhnsYqowYpowhVVohUVoYspQYrpQYinspRVIhfXMYmoQhbWsYoowhWVohWWMYqpQhXWMhaWspQVIYjoQYjnv///82KQBZAAAAAWJLR0QecgogKwAAAAd0SU1FB+QHFwkxFZq+mNQAAADUSURBVEjH7ZTbEoIgEIY5umumCGqa1vu/ZgczpSBlxouc8bvcfz9gBlhCdrYOZfyJkENFir7CqLM/ghdsKLGhErkMBujbAcc1JnDgvsNyiEMFvr5wSMIEeUxlkJAhZnOCAOETrGg8BZM+wYrc2MIC/ktQU0HN9+fa0EGgRuezQqGhlL0gS9DF/BYVwkk9BFUDVj8a37fTnAEZYpFC3XxEFuP9Zy10d6DNvqIp0xdmOoDOOCO3QC5aX0mIQJLAD7QkWldwjZngQRY8Kv3DOHYP451NcQMS0Qnr3TYZaQAAACV0RVh0ZGF0ZTpjcmVhdGUAMjAyMC0wNy0yM1QwOTo0OToxOCswMDowMI9K1dUAAAAldEVYdGRhdGU6bW9kaWZ5ADIwMjAtMDctMjNUMDk6NDk6MjErMDA6MDDlAC9ZAAAAAElFTkSuQmCC)",
                        "opacity": "1.0",
                        "z-index": "2147483647"
                    }).show("normal");

                    let $div_mask = "<div style='position: relative;width: 100%;height: 100%;background-color: rgba(255, 255, 255, 0.3);'></div>";
                    $download_btn.append($div_mask);

                    let $download_link = $("<div style='position: absolute;left: 0px;top: 0px;padding-left: 8px'></div>");
                    $download_link.append($a_link);
                    $download_btn.append($download_link);

                    $("body").append($download_btn);

                    $(document).unbind("scroll").scroll(function () {
                        let wScrollTop = parseInt($(document).scrollTop());
                        //let btnScrTop = $("#div_download_btn").position().top;
                        $("#div_download_btn").css({ "top": (wScrollTop + 25) + "px" });
                    });
                }
            }
        }, 3000); // 延时3秒
    }

    //解码
    function b64DecodeUnicode(str) {
        return decodeURIComponent(atob(str).split('').map(function (c) {
            return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
        }).join(''));
    }

    //编码
    function b64EncodeUnicode(str) {
        return btoa(encodeURIComponent(str).replace(/%([0-9A-F]{2})/g, function (match, p1) {
            return String.fromCharCode('0x' + p1);
        }));
    }

    function replaceIllegalChar(str) {
        str = str.replace(/(\r\n)|(\r)|(\n)/g, " ");
        return str.replace(/[\\\/:*?"<>| ]{1,}/g, " ");
    }

    function pad(num, n) {
        let len = num.toString().length;
        while (len < n) {
            num = "0" + num;
            len++;
        }
        return num;
    }

})();