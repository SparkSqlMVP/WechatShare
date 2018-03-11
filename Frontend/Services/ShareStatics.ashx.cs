using CSharpSDK.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using weitang.Models;

namespace CSharpSDK.Services
{
    /// <summary>
    /// Summary description for ShareStatics
    /// </summary>
    public class ShareStatics : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";

            string UserID, ShareUrl;
            int shareinfoid;
            UserID = context.Request.Form["UserID"]; //context.Request.QueryString["UserID"]; //
            ShareUrl = context.Request.Form["ShareUrl"]; //context.Request.QueryString["ShareUrl"];//
            shareinfoid = int.Parse(weitang.StringExtensions.GetNumber(ShareUrl.Substring(ShareUrl.LastIndexOf('/') + 1)).ToString());

            Pageinexinfo sharerindex = new Pageinexinfo();
            int sharecounts;
            WechatEntities wechat = new WechatEntities();
            sharecounts = wechat.ShareLogs.Where(a => a.userID == UserID && a.shareUrl == ShareUrl).ToList().Count;
            sharerindex = wechat.Pageinexinfo.Where(a => a.shareinfoID == shareinfoid).FirstOrDefault();

            //   
            string result = JsonConvert.SerializeObject(new ApiResponse
            {
                r = "1",
                d = new
                {
                    sharenums = sharecounts,
                    sharerequest = sharerindex is null ? 0 : sharerindex.SharesRequirements
                },
                m = "数据获取成功"
            });
            context.Response.Write(result);
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