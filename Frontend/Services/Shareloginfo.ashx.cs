using CSharpSDK.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPI;
using weitang.Models;

namespace CSharpSDK.Services
{
    /// <summary>
    /// Summary description for Shareloginfo
    /// </summary>
    public class Shareloginfo : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";

            Pageinexinfo sharerindex = new Pageinexinfo();
            ShareLog mylog = new ShareLog();

            int sharecounts, shareinfoid;
            string UserID, ShareUrl;
            try
            {

                UserID = context.Request.Form["UserID"]; //context.Request.QueryString["UserID"]; //
                ShareUrl = context.Request.Form["ShareUrl"]; //context.Request.QueryString["ShareUrl"];//
                shareinfoid = int.Parse(weitang.StringExtensions.GetNumber(ShareUrl.Substring(ShareUrl.LastIndexOf('/') + 1)).ToString());
                mylog.userID = UserID;
                mylog.shareUrl = ShareUrl;
                mylog.shareinfoID = shareinfoid;
                mylog.BrowseType = DN.Framework.Utility.ClientHelper.GetUserAgent();
                mylog.ClientIp = DN.Framework.Utility.ClientHelper.ClientIP();
                mylog.IsMobile = DN.Framework.Utility.ClientHelper.GetIsMobileDevice() ? 1 : 0;
                mylog.OsName = DN.Framework.Utility.ClientHelper.GetOsName();
                mylog.BrowseName = DN.Framework.Utility.ClientHelper.GetBrowseName();
                mylog.BrowseVersion = DN.Framework.Utility.ClientHelper.GetBrowseVersion();

                var ipinfo = DN.WeiAd.Business.Services.IpTaoBaoHelper.GetIpResult(mylog.ClientIp);
                if (ipinfo != null)
                {
                    if (ipinfo.code == 0 && ipinfo.data != null)
                    {
                        mylog.Country = ipinfo.data.country;
                        mylog.Area = ipinfo.data.area;
                        mylog.City = ipinfo.data.city;
                        mylog.Region = ipinfo.data.region;
                        mylog.County = ipinfo.data.county;
                        mylog.Isp = ipinfo.data.isp;
                    }
                }
           
                    mylog.InputDate = System.DateTime.Now;
                }
                catch (Exception ex)
                {
                    DN.Framework.Utility.LogHelper.Write(ex.Message.ToString());
                    throw;
                }


                try
                {
                    string cmd = string.Format(@" insert into ShareLog(userID, shareUrl, shareinfoID, BrowseType, ClientIp, IsMobile, OsName, BrowseName, BrowseVersion, Country,
                                                                        Area, City, Region, County, Isp,InputDate ) 
                                               values (N'{0}',N'{1}',N'{2}',N'{3}',N'{4}',{5},N'{6}',N'{7}',N'{8}',N'{9}',N'{10}',N'{11}',N'{12}',N'{13}',N'{14}',N'{15}')
                                              ",
                                             mylog.userID, mylog.shareUrl, mylog.shareinfoID, mylog.BrowseType, mylog.ClientIp, mylog.IsMobile, mylog.OsName, mylog.BrowseName,
                                             mylog.BrowseVersion, mylog.Country, mylog.Area, mylog.City, mylog.Region, mylog.County, mylog.Isp, mylog.InputDate
                                          );

                    if (SQLServerHelper.ExcuteSql(cmd) > 0)
                    {
                    }

                }
                catch (Exception ex)
                {

                    DN.Framework.Utility.LogHelper.Write(ex.Message);
                }

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
                        sharerequest = sharerindex is null ? 0 : sharerindex.SharesRequirements,
                        shareimages = sharerindex is null ? "" : sharerindex.friendimages
                    },
                    m = "数据获取成功"
                });
                context.Response.Write(result);
            //context.Response.Write("OK");
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