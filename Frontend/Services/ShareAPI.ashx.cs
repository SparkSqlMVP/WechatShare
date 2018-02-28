using CSharpSDK.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using weitang.Api.WechatLib;
using weitang.Models;

namespace CSharpSDK.Services
{
    /// <summary>
    /// Summary description for ShareAPI
    /// </summary>
    public class ShareAPI : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            //数据验证
    
            //获取微信配置信息
            string url = context.Request.QueryString["url"];
            string shareurl = context.Request.QueryString["shareurl"];
            JSSDK sdk = new JSSDK("wxf89835502774e9c2", "385fa8177eabd63abd74236ef9b1f684", "", false);
            var qianming = HttpUtility.UrlDecode(url, System.Text.Encoding.GetEncoding(65001));
            shareinfo share = new shareinfo();

            int id = int.Parse(GetNumber(url.Substring(url.LastIndexOf('/') + 1)).ToString());
            WechatEntities dc = new WechatEntities();
            var v = dc.ShareInfo.Where(a => a.Id == id).FirstOrDefault();
            share.url = v.ShareURL;
            share.title = v.Title;
            share.describtion = v.Description;
            share.imags = "http://www.bbpdt.cn/" + v.Image;

            SignPackage config = sdk.GetSignPackage(qianming, JsApiEnum.chooseWXPay | JsApiEnum.onMenuShareTimeline | JsApiEnum.onMenuShareAppMessage | JsApiEnum.onMenuShareQQ | JsApiEnum.onMenuShareWeibo);
            // return new ApiResponse { r = "1", d = new { config = config }, m = "数据获取成功" };
            context.Response.Write(JsonConvert.SerializeObject(new ApiResponse {
                r = "1", d = new {config = config,share= share },
                m = "数据获取成功" }));
        }

        public static decimal GetNumber(string str)
        {
            decimal result = 0;
            if (str != null && str != string.Empty)
            {
                // 正则表达式剔除非数字字符（不包含小数点.） 
                str = Regex.Replace(str, @"[^\d.\d]", "");
                // 如果是数字，则转换为decimal类型 
                if (Regex.IsMatch(str, @"^[+-]?\d*[.]?\d*$"))
                {
                    result = decimal.Parse(str);
                }
            }
            return result;
        }

        public class shareinfo
        {
            public string url { get; set; }
            public string title { set; get; }
            public string describtion { set; get; }
            public string imags { set; get; }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}