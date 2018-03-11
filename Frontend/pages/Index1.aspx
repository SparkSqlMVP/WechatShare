<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index1.aspx.cs" Inherits="CSharpSDK.pages.Index1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

  <link rel="stylesheet" href="/css/jquery-ui.css"/>
  <script src="/js/jquery-1.9.1.js"></script>
  <script src="/js/jquery-ui.js"></script>
  <link rel="stylesheet" href="/css/style.css"/>
  <script type="text/javascript" charset="utf-8" src="http://res.wx.qq.com/open/js/jweixin-1.1.0.js"></script>
  <script src="/js/wechat.js"></script>
    <style>

.top{width:100%;background:#ffffff;margin:0 auto;text-align: center;padding:30px 0 24px;}
.top img{width:500px;heigh:600px;border-radius:5px;}
.top h2{font-weight: bold;font-size:36px;margin-top:7px;color:#4d4c51;}
.top span{color:#878787;}

.layer-show-share{width: 100%;height: 100%;position: fixed;top: 0;left: 0;bottom: 0;right: 0;opacity: 0.86;background-color: #000;z-index: 1000;}
.layer-show-share .div-share {width: 100%;height: 100%;left: 0;top: 0;right: 0;bottom: 0;position: fixed;}
.layer-show-share img {width: 100%;height: 100%;}


.bot{width:100%;border-top: 1px solid #bebebe;}
.bot h1{width:100%;text-align: center;display: inline;float: left;font-weight: normal;color:#4d4c51;font-size:20px;margin:40px 0 10px;}
.bot h2{text-align: center;}

.bot a{width:54%;display:block;float: left;margin:15px 23% 25px;background:#04be01;height:30px;color:#ffffff;text-align: center;padding:8px 0;border-radius:5px;font-size:18px;}
.bot ul{color:#b5b4b9;display: inline;float: left;margin-left:10px;margin-right:30px;}
.bb a{ padding:10px 0;height:60px;font-size:40px}
    </style>
</head>
<body>
    <form id="form1" runat="server">

        <div id="sharediv" style="visibility:hidden">
            <div id="share" class="layer-show-share hide">
	            <div class="div-share">
		            <img src="http://www.bbpdt.cn/images/share.png" id="imgId"/>
	            </div>
            </div>
        </div>
        
   
        <div class="top">
		<img src="http://www.bbpdt.cn/images/b.jpg"  style="height:620px"  />
		<script>

            if (typeof remote_ip_info != "undefined") {
                var myDate = new Date();
                var hours = myDate.getHours() + 1;
                document.write("<h2>您的" + remote_ip_info["city"] + "邀你加入51weitang群聊,请分享1次成功，有惊喜并送10元红包啦" + "</h2>");

            } else {
                document.write("<h2>请分享1次成功，有惊喜并送10元红包啦</h2>");
            }
            function random(min, max) {
                return Math.floor(min + Math.random() * (max - min));
            }
		</script>
		<span><script>document.write(random(100, 500));</script>人</span>
	</div>

    <div class="bot">
	<script type="text/javascript">document.write("<h2>你的朋友邀你加入群聊" + "</h2>");</script>
		<p class="bb"><a  href="javascript:quxian();"  >加入群聊</a></p>
	</div>

        <div>
	        <a href="/html/smths.html">
               <img src="http://www.bbpdt.cn/images/car_ad.jpg" style="width: 100%;" />
             </a>
        </div>

 
    </form>
</body>
    <script>
        function quxian() {
            var peoples = random(100, 500);
            var currentUser = localStorage.getItem("curUser");
            if (null == currentUser || "" == currentUser) {
                var msg = '活动提示：完成分享任务，即可进群（请分享到一个微信群）\n当前群人数' + peoples + '人';
                alert(msg);
                return;
            }

            var shares, sharerequestnumber
            var url = window.location.href;
            url = url.indexOf("?") > 0 ? url.substring(0, url.indexOf("?")) : url;
            getSynchronizeData('get', '/Services/Shareloginfo.ashx', { "UserID": currentUser, "ShareUrl": url },
                function (data) {
                    shares = data.d.sharenums;
                    sharerequestnumber = data.d.sharerequest;
                }
            )

            if (shares >= sharerequestnumber) {
                window.location.href = "/html/qrcode.html";
            }
            else {
                var msg = '活动提示：完成分享任务，即可进群（请分享到一个微信群）\n当前群人数' + peoples + '人';
                alert(msg);
            }
           
        } 
    </script>
</html>
