using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using WebAPI;

namespace WechatBackend.Services
{
    /// <summary>
    /// Summary description for Urlpie
    /// </summary>
    public class Urlpie : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/json";

            ArrayList eventList = new ArrayList();
        
            // pie chart
            try
            {
                string cmd = string.Format(@"  
                        select right(shareUrl, charindex('/',reverse(shareUrl))-1) as name,
		                                    round(count(1)*1.0 / (
                                                select count(1)
                                                from[dbo].[ShareLog]
                                            ),2)*100 as y
                            from[dbo].[ShareLog]
                            group by right(shareUrl, charindex('/', reverse(shareUrl)) - 1)
                                              "
                            );

                DataSet myset = new DataSet();
                myset = SQLServerHelper.GetDataSetBySql(cmd);

                for (int i = 0; i < myset.Tables[0].Rows.Count; i++)
                {
                    Hashtable ht = new Hashtable();
                    ht.Add("name",myset.Tables[0].Rows[i]["name"]);
                    ht.Add("y", myset.Tables[0].Rows[i]["y"]);
                    eventList.Add(ht);
                }


            }
            catch (Exception )
            {
               

            }
    



            JavaScriptSerializer ser = new JavaScriptSerializer();
            String jsonStr = ser.Serialize(eventList);
            context.Response.Write(jsonStr);
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