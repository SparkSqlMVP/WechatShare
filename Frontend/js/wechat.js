
var USERID = "USERID";//用户id
var WXOPENID = "WXOPENID";//微信OPENID
var WXNICKNAME = "WXNICKNAME";//微信昵称
var WXHEADIMGURL = "WXHEADIMGURL";//微信头像
var ACCESS_TOKEN = "ACCESS_TOKEN";//用户token
var REFRESH_TOKEN = "REFRESH_TOKEN";//刷新token
//获取网址参数
function getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]);
    return null;
}

//destiny是目标字符串，比如是http://www.huistd.com/?id=3&ttt=3 
//par是参数名，par_value是参数要更改的值，调用结果如下：
//changeURLPar(test, 'id', 99); // http://www.huistd.com/?id=99&ttt=3 
//changeURLPar(test, 'haha', 33); // http://www.huistd.com/?id=99&ttt=3&haha=33 

function changeURLPar(destiny, par, par_value) {
    var pattern = par + '=([^&]*)';
    var replaceText = par + '=' + par_value;
    if (destiny.match(pattern)) {
        var tmp = '/\\' + par + '=[^&]*/';
        tmp = destiny.replace(eval(tmp), replaceText);
        return (tmp);
    }
    else {
        if (destiny.match('[\?]')) {
            return destiny + '&' + replaceText;
        }
        else {
            return destiny + '?' + replaceText;
        }
    }
    return destiny + '\n' + par + '\n' + par_value;
} 
//LOCALSTOAGE存储

var DB = {
    set: function (key, val) {
        localStorage.setItem(key, val);
    },
    get: function (key) {
        var t = localStorage.getItem(key) || "";
        if (t == "undefined") {
            t = "";
        }
        return t;
    },
    getObj: function (key) {
        var t = localStorage.getItem(key) || "{}";
        return JSON.parse(t);
    },
    getArray: function (key) {
        var t = localStorage.getItem(key) || "[]";
        return JSON.parse(t);
    },
    remove: function (key) {
        localStorage.removeItem(key);
    },
    setS: function (key, val) {
        localStorage.setItem(key, val);
    },
    getS: function (key) {
        var t = localStorage.getItem(key) || "";
        if (t == "undefined") {
            t = "";
        }
        return t;
    },
    getSObj: function (key) {
        var t = localStorage.getItem(key) == "undefined" ? "" : localStorage.getItem(key) || "";
        return !!t ? JSON.parse(t) : '';
    },
    removes: function (key) {
        localStorage.removeItem(key);
    }
}
//通用请求
function getData(method, url, parms, _callback, _failback) {
    // console.log('TOKEN:' + DB.getS('TOKEN'));
    //alert(DB.get(ACCESS_TOKEN));
    $.ajax({
        headers: {'Authorization': 'Bearer ' + DB.get(ACCESS_TOKEN) },
        type: method, url: url, data: parms, dataType: "json",        
        success: function (data) {
            if (data.r == '1') {
                _callback(data);
            } else if (_failback) {
                if (data.r == '401') {
                    DB.removes(ACCESS_TOKEN);
                    DB.removes(REFRESH_TOKEN);
                    DB.removes(WXOPENID);
                    window.location.href = '/index.html';
                }
                _failback(data);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            //alert(XMLHttpRequest.status);
            if (XMLHttpRequest.status == '401') {
                $.ajax({
                    type: "POST", url: "/api/auth/refreshtoken", data: { refreshToken: DB.get(REFRESH_TOKEN) }, dataType: "json",
                    success: function (data) {
                        if (data.r == '1') {
                            DB.set(ACCESS_TOKEN, data.d.access_token);
                            DB.set(REFRESH_TOKEN, data.d.refresh_token);
                            DB.set(WXOPENID, data.d.openid);
                            $.ajax({
                                headers: { 'Authorization': 'Bearer ' + DB.get(ACCESS_TOKEN) },
                                type: method, url: url, data: parms, dataType: "json",
                                success: function (data) {
                                    if (data.r == '1') {
                                        _callback(data);
                                    } else if (_failback) {
                                        if (data.r == '401') {
                                            DB.removes(ACCESS_TOKEN);
                                            DB.removes(REFRESH_TOKEN);
                                            DB.removes(WXOPENID);
                                            //alert(3);
                                            //window.location.href = 'index.html';
                                        }
                                        _failback(data);
                                    }
                                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                                    _failback && _failback(XMLHttpRequest);
                                }
                            });
                        } else {
                            DB.removes(ACCESS_TOKEN);
                            DB.removes(REFRESH_TOKEN);
                            DB.removes(WXOPENID);
                            window.location.href = 'https://open.weixin.qq.com/connect/oauth2/authorize?appid=wx95c9eb18973660f6&redirect_uri=' + window.location.href + '&response_type=code&scope=snsapi_userinfo&state=1#wechat_redirect';
                        }
                    },
                    error: function (XMLHttpRequest) {
                        DB.removes(ACCESS_TOKEN);
                        DB.removes(REFRESH_TOKEN);
                        DB.removes(WXOPENID);
                        window.location.href = 'https://open.weixin.qq.com/connect/oauth2/authorize?appid=wx95c9eb18973660f6&redirect_uri=' + window.location.href + '&response_type=code&scope=snsapi_userinfo&state=1#wechat_redirect';
                    }
                });
            } else {
                _failback && _failback(XMLHttpRequest);
            }
           
        }
    });
}
//微信API初始化
function wxinit(appId, timestamp, nonceStr, signature, _callback) {
    if (typeof wx == 'undefined') {
        return false;
    }
    wx.config({
        debug: false, // 开启调试模式,调用的所有api的返回值会在客户端alert出来，若要查看传入的参数，可以在pc端打开，参数信息会通过log打出，仅在pc端时才会打印。
        appId: appId, // 必填，公众号的唯一标识
        timestamp: timestamp, // 必填，生成签名的时间戳
        nonceStr: nonceStr, // 必填，生成签名的随机串
        signature: signature,// 必填，签名，见附录1
        jsApiList: ["scanQRCode", "chooseWXPay", 'onMenuShareTimeline', 'onMenuShareAppMessage', 'onMenuShareQQ', 'onMenuShareWeibo', 'chooseImage', 'previewImage', 'uploadImage'] // 必填，需要使用的JS接口列表，所有JS接口列表见附录2
    });
    wx.ready(function () {
        if (_callback) {
            _callback();
        }
    });
    wx.error(function (res) {
        //alert(JSON.stringify(res));
    });
}





//微信分享
function wxShareInit(title, link, imgUrl, desc) {

    // 朋友圈
    var _suc1 = function () {

        var currentUser = localStorage.getItem("curUser");
        if (null == currentUser || "" == currentUser) {
            currentUser = parseInt(Date.now() + Math.random() * 100000000000);
            localStorage.setItem("curUser", currentUser);
            localStorage.setItem(currentUser, 1);
        } else {
            localStorage.setItem(currentUser, parseInt(localStorage.getItem(currentUser)) + 1);
        }
       
       
        if (parseInt(localStorage.getItem(currentUser)) >= 3) {
            window.location.href = "/html/qrcode.html";
        }
        else {
            alert('你分享成功了' + localStorage.getItem(currentUser) + '次，请再分享' + (3 - parseInt(localStorage.getItem(currentUser)))+'次群或者朋友圈!');
        }
        
        //getData('post', '/api/wechatshare/save', { "OpenId": DB.get(WXOPENID), "Postid": getQueryString("id"), "ShareAddress": "微信朋友圈", "Title": title, "Link": link, "Desc": desc, "ImgUrl": imgUrl }, function (data) {
        //    document.location.reload();
        //}, function (data) {
        //    })
    };
    //朋友
    var _suc2 = function () {
        var currentUser = localStorage.getItem("curUser");
        if (null == currentUser || "" == currentUser) {
            currentUser = parseInt(Date.now() + Math.random() * 100000000000);
            localStorage.setItem("curUser", currentUser);
            localStorage.setItem(currentUser, 1);
        } else {
            localStorage.setItem(currentUser, parseInt(localStorage.getItem(currentUser)) + 1);
        }

        if (parseInt(localStorage.getItem(currentUser)) >= 3) {
            window.location.href = "/html/qrcode.html";
        }
        else {
            alert('你分享成功了' + localStorage.getItem(currentUser) + '次，请再分享' + (3 - parseInt(localStorage.getItem(currentUser))) + '次群或者朋友圈!');
        }

    };
    //QQ
    var _suc3 = function () {
        var currentUser = localStorage.getItem("curUser");
        if (null == currentUser || "" == currentUser) {
            currentUser = parseInt(Date.now() + Math.random() * 100000000000);
            localStorage.setItem("curUser", currentUser);
            localStorage.setItem(currentUser, 1);
        } else {
            localStorage.setItem(currentUser, parseInt(localStorage.getItem(currentUser)) + 1);
        }

        if (parseInt(localStorage.getItem(currentUser)) >= 3) {
            window.location.href = "/html/qrcode.html";
        }
        else {
            alert('你分享成功了' + localStorage.getItem(currentUser) + '次，请再分享' + (3 - parseInt(localStorage.getItem(currentUser))) + '次群或者朋友圈!');
        }
    };
    // 微博
    var _suc4 = function () {
        var currentUser = localStorage.getItem("curUser");
        if (null == currentUser || "" == currentUser) {
            currentUser = parseInt(Date.now() + Math.random() * 100000000000);
            localStorage.setItem("curUser", currentUser);
            localStorage.setItem(currentUser, 1);
        } else {
            localStorage.setItem(currentUser, parseInt(localStorage.getItem(currentUser)) + 1);
        }

        if (parseInt(localStorage.getItem(currentUser)) >= 3) {
            window.location.href = "/html/qrcode.html";
        }
        else {
            alert('你分享成功了' + localStorage.getItem(currentUser) + '次，请再分享' + (3 - parseInt(localStorage.getItem(currentUser))) + '次群或者朋友圈!');
        }
    };


    wx.checkJsApi({
        jsApiList: ['onMenuShareTimeline', 'onMenuShareAppMessage', 'onMenuShareQQ', 'onMenuShareWeibo'], // 需要检测的JS接口列表，所有JS接口列表见附录2,
        success: function (res) {
            wx.onMenuShareTimeline({
                title: title, // 分享标题
                link: link, // 分享链接
                imgUrl: imgUrl, // 分享图标
                success: _suc1,
                cancel: function () {
                    // 用户取消分享后执行的回调函数
                }
            });

            wx.onMenuShareAppMessage({
                title: title, // 分享标题
                desc: desc, // 分享描述
                link: link, // 分享链接
                imgUrl: imgUrl, // 分享图标
                type: '', // 分享类型,music、video或link，不填默认为link
                dataUrl: '', // 如果type是music或video，则要提供数据链接，默认为空
                success: _suc2,
                cancel: function () {
                    // 用户取消分享后执行的回调函数
                }
            });

            wx.onMenuShareQQ({
                title: title, // 分享标题
                desc: desc, // 分享描述
                link: link, // 分享链接
                imgUrl: imgUrl, // 分享图标
                success: _suc3,
                cancel: function () {
                    // 用户取消分享后执行的回调函数
                }
            });

            wx.onMenuShareWeibo({
                title: title, // 分享标题
                desc: desc, // 分享描述
                link: link, // 分享链接
                imgUrl: imgUrl, // 分享图标
                success: _suc4,
                cancel: function () {
                    // 用户取消分享后执行的回调函数
                }
            });

        }
    });
}

// oneid 原始id  twoid 一直在变的id
var f = getQueryString("f");
var l = getQueryString("l");
function initall() {
    var url = window.location.href;
    var shareurl = "http://www.bbpdt.cn/index.aspx";
    getData('get', "/Services/ShareAPI.ashx", { "url": url, "shareurl": shareurl }, function (data) {
        wxinit(data.d.config.AppId, data.d.config.Timestamp, data.d.config.NonceStr, data.d.config.Signature, function () {
            wxShareInit(data.d.share.title, data.d.share.url, data.d.share.imags, data.d.share.describtion);
            });
    }, function (data) { });
}
initall();