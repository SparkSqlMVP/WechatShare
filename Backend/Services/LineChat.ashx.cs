using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using WebAPI;
using weitang.Models;

namespace WechatBackend.Services
{
    /// <summary>
    /// Summary description for LineChat
    /// </summary>
    public class LineChat : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/json";
            string urlname = context.Request.Form["url"];
            string categories = "", ydata = "", datetime = "";
            ArrayList eventList = new ArrayList();
  

            // line chart
            try
            {
                string cmd = string.Format(@"  
                                    with line_data 
                                    as
                                    (
                                    select right(shareUrl, charindex('/',reverse(shareUrl))-1) as name,
                                    convert(varchar(10),InputDate,120) as datetime,
                                    count(1) as y
                                    from	[dbo].[ShareLog]
                                    group by right(shareUrl, charindex('/', reverse(shareUrl)) - 1),convert(varchar(10),InputDate,120)
                                    )
                                    SELECT  T1.name AS name ,
                                    stuff(( SELECT    ','+T2.datetime FROM      line_data T2
	                                    WHERE     T2.name = T1.name
	                                    FOR XML PATH('')
                                    ),1,1,'') AS datetime,
                                    stuff(( SELECT   ','+ltrim(T2.y) FROM      line_data T2
	                                    WHERE     T2.name = T1.name
	                                    FOR XML PATH('')
                                    ),1,1,'') AS y
                                    FROM    line_data T1  where  name='{0}'
                                    GROUP BY T1.name
                            "
                            , urlname);

                DataSet myset = new DataSet();
                myset = SQLServerHelper.GetDataSetBySql(cmd);

                categories = myset.Tables[0].Rows[0]["name"].ToString();
                ydata = myset.Tables[0].Rows[0]["y"].ToString();
                datetime = myset.Tables[0].Rows[0]["datetime"].ToString();


            }
            catch (Exception ex)
            {
                context.Response.Write(ex.ToString());

            }


            Hashtable ht = new Hashtable();
            ht.Add("categories", datetime);
            ht.Add("name", categories);
            ht.Add("data", ydata);
           
            eventList.Add(ht);

            JavaScriptSerializer ser = new JavaScriptSerializer();
            String jsonStr = ser.Serialize(eventList);
            context.Response.Write(jsonStr);

            //string result = JsonConvert.SerializeObject(new ApiResponse
            //{
            //    r = "1",
            //    d = new
            //    {
            //        name= urlname,
            //        data = ydata,
            //        time= datetime
            //    },
            //    m = "数据获取成功"
            //});
            //context.Response.Write(result);


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