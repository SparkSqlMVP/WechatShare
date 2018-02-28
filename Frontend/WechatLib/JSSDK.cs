using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using weitang.Models;

namespace weitang.Api.WechatLib
{


    public class JSSDK
    {
        public static string CACHE_OPENID = "CACHE_OPENID";
        public static string CACHE_NICKNAME = "CACHE_NICKNAME";
        public static string CACHE_HEADIMGURL = "CACHE_HEADIMGURL";
        private string appId;
        private string appSecret;
        private bool _debug;
        private SimpleCacheProvider _cache;
        private string code;
        const int TIME_OUT = 10000;
        const string URL_FORMAT_TOKEN = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}";
        const string URL_FORMAT_TICKET = "https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type=jsapi";
        const string URL_FORMAT_SQTOKEN = "https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code";
        const string URL_FORMAT_USERLIST = "https://api.weixin.qq.com/sns/userinfo?access_token={0}&openid={1}";
        const string URL_QRCODE_CREATE = "https://api.weixin.qq.com/cgi-bin/qrcode/create?access_token={0}";
        const string URL_QRCODE_DOWN = "https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket={0}";
        /// <summary>
        /// ͳһ֧���ӿ�
        /// </summary>
        const string UnifiedPayUrl = "https://api.mch.weixin.qq.com/pay/unifiedorder";

        /// <summary>
        /// ΢�Ŷ�����ѯ�ӿ�
        /// </summary>
        const string OrderQueryUrl = "https://api.mch.weixin.qq.com/pay/orderquery";
        const string ACTION_NAME = "QR_LIMIT_STR_SCENE";
        const string CACHE_TOKEN_KEY = "CACHE_TOKEN_KEY";
        const string CACHE_TICKET_KEY = "CACHE_TICKET_KEY";
        //��ȡ��ҳ��Ȩ�õĻ���key
        const string CACHE_SQTOKEN_KEY = "CACHE_SQTOKEN_KEY";
        const string CACHE_OPENID_KEY = "CACHE_OPENID_KEY";

        /// <summary>
        /// ����JDK����
        /// </summary>
        /// <param name="appId">΢�Ź����˺ŵ�AppId</param>
        /// <param name="appSecret">�����˺ŵ�appSecret</param>
        /// <param name="debug">�Ƿ����</param>
        public JSSDK(string appId, string appSecret, string code, bool debug = false)
        {
            this.appId = appId;
            this.appSecret = appSecret;
            this.code = code;
            this._cache = SimpleCacheProvider.GetInstance();
            this._debug = debug;
        }





        /// <summary>
        /// ����ǩ��������ַ���
        /// </summary>
        /// <param name="length">���ȣ�С��32λ��Ĭ��16λ</param>
        /// <returns>����ַ���</returns>
        private string CreateNonceStr(int length = 16)
        {
            return Guid.NewGuid().ToString().Replace("-", "").Substring(0, length > 32 ? 32 : length);
        }
        /// <summary>
        /// ��ȡAccessToken
        /// </summary>
        /// <returns>AccessToken</returns>
        private string GetAccessToken()
        {
            var token = this._cache.GetCache(CACHE_TOKEN_KEY);
            if (token != null)
                return token.ToString();

            try
            {
                string result = HttpGet(string.Format(URL_FORMAT_TOKEN, this.appId, this.appSecret));
                Dictionary<string, object> jsonObj = result.FromJson<Dictionary<string, object>>(); 
                if (jsonObj.ContainsKey("access_token"))
                {
                    token = jsonObj["access_token"].ToString();
                    this._cache.SetCache(CACHE_TOKEN_KEY, token.ToString(), 7000);
                }
                else
                {
                    //Ϊ�˳����������У����׳����󣬿��Լ�¼��־
                    token = jsonObj["errmsg"];
                }
            }
            catch
            {
                //Ϊ�˳����������У����׳����󣬿��Լ�¼��־
                token = "there_is_an_error_when_getting_token";
            }

            return token.ToString();
        }

        /// <summary>
        /// ��ȡApiTicket
        /// </summary>
        /// <returns>ApiTicket</returns>
        private string GetJsApiTicket()
        {
            var ticket = this._cache.GetCache(CACHE_TICKET_KEY);
            if (ticket != null)
                return ticket.ToString();
            try
            {
                string result = HttpGet(string.Format(URL_FORMAT_TICKET, this.GetAccessToken()));
                Dictionary<string, object> jsonObj = result.FromJson<Dictionary<string, object>>();
                if (jsonObj.ContainsKey("ticket"))
                {
                    ticket = jsonObj["ticket"].ToString();
                    this._cache.SetCache(CACHE_TICKET_KEY, ticket.ToString(), 7000);
                }
                else
                {
                    //Ϊ�˳����������У����׳����󣬿��Լ�¼��־
                    ticket = jsonObj["errmsg"];
                }
            }
            catch
            {
                //Ϊ�˳����������У����׳����󣬿��Լ�¼��־
                ticket = "there_is_an_error_when_getting_apiticket";
            }

            return ticket.ToString();
        }
        /// <summary>
        /// ��ȡjssdkǩ�����ö���
        /// </summary>
        /// <param name="url">��ǰҳ��url</param>
        /// <param name="jsapi">JsApiEnum,��:JsApiEnum.scanQRCode|JsApiEnum.onMenuShareQQ</param>
        /// <returns>΢�Ź���ƽ̨JsSdk�����ö���</returns>
        public SignPackage GetSignPackage(string url, JsApiEnum jsapi)
        {

            /*
             * ǩ���ֶ�
            noncestr=Wm3WZYTPz0wzccnW
            jsapi_ticket=sM4AOVdWfPE4DxkXGEs8VMCPGGVi4C3VM0P37wVUCFvkVAy_90u5h9nbSlYy3-Sl-HhTdfl2fzFy1AOcHKP7qg
            timestamp=1414587457
            url=http://mp.weixin.qq.com?params=value  
             */
            string noncestr = this.CreateNonceStr(16);
            string jsapi_tkcket = this.GetJsApiTicket();
            long timestamp = TimeStamp.Now();
            Dictionary<string, string> signData = new Dictionary<string, string>() { 
                {"noncestr",noncestr},
                {"jsapi_ticket",jsapi_tkcket},
                {"timestamp",timestamp.ToString()},
                {"url",url}
            };

            SignPackage result = new SignPackage()
            {
                AppId = this.appId,
                Timestamp = timestamp,
                NonceStr = noncestr,
                Debug = this._debug,
                Signature = new Signature().Sign(signData),
                Url = url,
                JsApiList = jsapi.ToString().Replace(" ", "").Split(',')
            };
            return result;
        }
        /// <summary>
        /// ��̨����http����
        /// </summary>
        /// <param name="url">����URL</param>
        /// <returns>�������ַ���</returns>
        private string HttpGet(string url)
        {
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            return client.DownloadString(url);
        }
        /// <summary>
        /// ��̨����http����
        /// </summary>
        /// <param name="url">����URL</param>
        /// <param name="paramStr">name=����&age=20</param>
        /// <returns>�������ַ���</returns>
        private string HttpPost(string url,string paramStr)
        {
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            // ��ȡPOST��ʽ����ӵ�Header
             client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
 
             byte[] postData = client.Encoding.GetBytes(paramStr);

             byte[] responseData = client.UploadData(url, "POST", postData); // �õ������ַ���
             return client.Encoding.GetString(responseData);// ����
        }
        public WechatUser GetWeChatUserList()
        {
            WechatUser userList = new WechatUser();
            try
            {
                string result = HttpGet(string.Format(URL_FORMAT_SQTOKEN, this.appId, this.appSecret, this.code));
                Dictionary<string, object> jsonObj = result.FromJson<Dictionary<string, object>>();
                var token = "";
                var openid = "";
                if (jsonObj.ContainsKey("access_token"))
                {
                    token = jsonObj["access_token"].ToString();
                }
                if (jsonObj.ContainsKey("openid"))
                {
                    openid = jsonObj["openid"].ToString();
                }
                result = HttpGet(string.Format(URL_FORMAT_USERLIST, token, openid));
                jsonObj = result.FromJson<Dictionary<string, object>>();
                if (jsonObj.ContainsKey("nickname"))
                {
                    userList.Openid = jsonObj["openid"].ToString();
                    userList.Nickname = jsonObj["nickname"].ToString();
                    userList.Headimgurl = jsonObj["headimgurl"].ToString();
                    this._cache.SetCache(CACHE_OPENID, userList.Openid);
                    this._cache.SetCache(CACHE_NICKNAME, userList.Nickname);
                    this._cache.SetCache(CACHE_HEADIMGURL, userList.Headimgurl);
                }
            }
            catch(Exception ex)
            {
            }
            return userList;
        }
    }
}