using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatatableCRUD.Services
{
    /// <summary>
    /// Summary description for UploadImages
    /// </summary>
    public class UploadImages : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            HttpPostedFile file = context.Request.Files[0];
            //String fileName = System.IO.Path.GetFileName(file.FileName);
            String FileExtension = System.IO.Path.GetExtension(file.FileName);
            string fileName = System.Guid.NewGuid().ToString();
            file.SaveAs(context.Server.MapPath("~/images/upload/") + fileName+FileExtension);
            context.Response.Write("OK");
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