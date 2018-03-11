
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DN.WeiAd.Business.Services
{
    /// <summary>
    /// 淘宝IP检查
    /// </summary>
    public class IpTaoBaoHelper
    {
        static Dictionary<string, IpResult> m_list = new Dictionary<string, IpResult>();
        const string m_cachehashid = "IpTaoBaoHelper";
        static IpResult GetIpResultByCache(string ip,IpResult ipinfo)
        {
            if (m_list.ContainsKey(ip))
            {
                ipinfo = m_list[ip];
            }
            else
            {
                if (ipinfo != null)
                {
                    m_list.Add(ip, ipinfo);
                }
            }
            return ipinfo;
        }


        /// <summary>
        /// 检查是否腾讯ISP
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool CheckTenIsp(string ip)
        {
            var info = GetIpResult(ip);
            if (info == null) return false;
            if(info.code ==1)
            {
                //只是包含有“腾讯”就认为是腾讯过来检查的
                if(info.data.isp.IndexOf("腾讯")!=-1)
                {
                    return true;
                }
                //是否去掉北上广深圳
            }
            return false;
        }

        /// <summary>
        /// 获取IP信息
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static IpResult GetIpResult(string ip)
        {
            IpResult info = null;
            try
            {
                info = GetIpResultByCache(ip, info);
                if (info == null)
                {
                    //替换原有的淘宝服务
                    info = GeIpResultByApiTB(ip);
                    if (info == null)
                    {
                        string url = string.Format("http://ip.taobao.com/service/getIpInfo.php?ip={0}", ip);
                        string json = DN.Framework.Utility.WebClientHelper.GetSend(url);
                        info = DN.Framework.Utility.Serializer.DeserializeObject<IpResult>(json);
                        if (info.code == 0)
                        {
                            GetIpResultByCache(ip, info);
                        }
                    }
                    else
                    {
                        GetIpResultByCache(ip, info);
                    }
                }
            }
            catch (Exception ex)
            {
                DN.Framework.Utility.LogHelper.Write(ex.Message, "error");
            }

            return info;
        }

        private const String host = "https://dm-81.data.aliyun.com";
        private const String path = "/rest/160601/ip/getIpInfo.json";
        private const String method = "GET";
        private const String appcode = "1734d33de9764d7d8be994b3f0daafb2";

        public static IpResult GeIpResultByApiTB(string ip)
        {
            IpResult result = null;

            try
            {
                String querys = "ip=" + ip;
                String bodys = "";
                String url = host + path;

                HttpWebRequest httpRequest = null;
                HttpWebResponse httpResponse = null;

                if (0 < querys.Length)
                {
                    url = url + "?" + querys;
                }

                if (host.Contains("https://"))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                    httpRequest = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
                }
                else
                {
                    httpRequest = (HttpWebRequest)WebRequest.Create(url);
                }
                httpRequest.Method = method;
                httpRequest.Headers.Add("Authorization", "APPCODE " + appcode);
                if (0 < bodys.Length)
                {
                    byte[] data = Encoding.UTF8.GetBytes(bodys);
                    using (Stream stream = httpRequest.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }
                try
                {
                    httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                }
                catch (WebException ex)
                {
                    httpResponse = (HttpWebResponse)ex.Response;
                }

                Stream st = httpResponse.GetResponseStream();
                StreamReader reader = new StreamReader(st, Encoding.GetEncoding("utf-8"));
                string json = reader.ReadToEnd();
                if (!string.IsNullOrEmpty(json))
                {
                    result = DN.Framework.Utility.Serializer.DeserializeObject<IpResult>(json);
                }
            }
            catch (Exception ex)
            {
                DN.Framework.Utility.LogHelper.Write(string.Format("{0}，转换错误，：{1}", ip, ex.Message), "iperror");
            }
            return result;
        }

        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }


        public class IpResult
        {
            /// <summary>
            /// 结果 0正确，1错误
            /// </summary>
            public int code { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public IpInfo data { get; set; }
        }


        /// <summary>
        /// IP信息
        /// </summary>
        public class IpInfo
        {
            /// <summary>
            /// IP地址
            /// </summary>
            public string ip { get; set; }
            /// <summary>
            /// 中国
            /// </summary>
            public string country { get; set; }
            /// <summary>
            /// 华北
            /// </summary>
            public string area { get; set; }
            /// <summary>
            /// 北京市
            /// </summary>
            public string region { get; set; }
            /// <summary>
            /// 北京市
            /// </summary>
            public string city { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string county { get; set; }
            /// <summary>
            /// 电信
            /// </summary>
            public string isp { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string country_id { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string area_id { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string region_id { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string city_id { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string county_id { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string isp_id { get; set; }
        }
    }
}
