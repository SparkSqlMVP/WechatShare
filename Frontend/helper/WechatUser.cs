using System;


namespace weitang.Models
{
    /// <summary>
    /// 微信授权登陆回调获取的用户信息
    /// </summary>
    public class WechatUser: WechatError
    {
        /// <summary>
        /// 微信id
        /// </summary>
        public string Openid { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string Nickname { get; set; }
        /// <summary>
        /// 用户头像，最后一个数值代表正方形头像大小（有0、46、64、96、132数值可选，0代表640*640正方形头像），用户没有头像时该项为空
        /// </summary>
        public string Headimgurl { get; set; }
        /// <summary>
        /// 用户的性别，值为1时是男性，值为2时是女性，值为0时是未知
        /// </summary>
        public string Sex { get; set; }
        /// <summary>
        /// 省
        /// </summary>
        public string Province { get; set; }
        /// <summary>
        /// 市
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// 国家
        /// </summary>
        public string Country { get; set; }
        /// <summary>
        /// 用户特权信息，json 数组，如微信沃卡用户为（chinaunicom）
        /// 作者注：其实这个格式称不上JSON，只是个单纯数组。
        /// </summary>
        public string[] Privilege { get; set; }
        /// <summary>
        /// 只有在用户将公众号绑定到微信开放平台帐号后，才会出现该字段。详见：https://open.weixin.qq.com/cgi-bin/showdocument?action=dir_list&amp;t=resource/res_list&verify=1&amp;lang=zh_CN
        /// </summary>
        public string Unionid { get; set; }
    }

    public class WechatError
    {
        /// <summary>
        /// 错误码，例如40003
        /// "errcode":40003,"errmsg":"invalid openid"
        /// </summary>
        public string Errcode { get; set; }
        /// <summary>
        /// invalid openid
        /// "errcode":40003,"errmsg":"invalid openid"
        /// </summary>
        public string Errmsg { get; set; }
    }

    public class SignPackage
    {
        /*
        wx.config({
         debug: true, // 开启调试模式,调用的所有api的返回值会在客户端alert出来，若要查看传入的参数，可以在pc端打开，参数信息会通过log打出，仅在pc端时才会打印。
         appId: '', // 必填，公众号的唯一标识
         timestamp: , // 必填，生成签名的时间戳
         nonceStr: '', // 必填，生成签名的随机串
         signature: '',// 必填，签名，见附录1
         jsApiList: [] // 必填，需要使用的JS接口列表，所有JS接口列表见附录2
        }); 
         */
        public bool Debug { get; set; }
        public string AppId { get; set; } // 必填，公众号的唯一标识
        public long Timestamp { get; set; } // 必填，生成签名的时间戳
        public string NonceStr { get; set; } // 必填，生成签名的随机串
        public string Signature { get; set; }// 必填，签名，见附录1
        public string Url { get; set; }
        public string[] JsApiList { get; set; } // 必填，需要使用的JS接口列表，所有JS接口列表见附录2
    }
    public class TimeStamp
    {
        public static long Now()
        {
            return (long)((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds);
        }
        public static DateTime ToDateTime(long timestamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(timestamp);
        }
        public static string ToDateTimeString(long timestamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(timestamp).ToString();
        }
        public static string ToDateTimeString(long timestamp, string format)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(timestamp).ToString(format);
        }

    }
}
