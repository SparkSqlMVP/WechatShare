using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WechatBackend.Models;

namespace DatatableCRUD.Controllers
{
    public class HomeController : Controller
    {        
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetShareinfos()
        {
            //don't 处理
            //using (MyDatabaseEntities dc = new MyDatabaseEntities())
            //{
            //    var shareinfos = dc.ShareInfoes.OrderBy(a => a.Title).ToList();
            //    return Json(new { data = shareinfos }, JsonRequestBehavior.AllowGet);
            //}
            List<ShareInfo> shareinfolist = new List<ShareInfo>();
            //需要改变值，所有用循环
            WechatEntities dc = new WechatEntities();
            for (int i = 0; i < dc.ShareInfoes.ToList().Count; i++)
            {
                ShareInfo myshareinfo = new ShareInfo();
                myshareinfo.Id = dc.ShareInfoes.Local[i].Id;
                myshareinfo.Title = dc.ShareInfoes.Local[i].Title;
                myshareinfo.Description = dc.ShareInfoes.Local[i].Description;
                myshareinfo.Image = string.Format(@"<img src='{0}' alt='' border=3 height=30 width=40>", dc.ShareInfoes.Local[i].Image);

                var pageinfo = dc.Pageinexinfoes.Where(a => a.shareinfoID == myshareinfo.Id).FirstOrDefault();

                myshareinfo.ShareURL = pageinfo !=null ? "http://www.bbpdt.cn/pages/index"+ pageinfo.shareinfoID.ToString()+".html" : "";
                myshareinfo.AuthorID = dc.ShareInfoes.Local[i].AuthorID;
                myshareinfo.InputDate = dc.ShareInfoes.Local[i].InputDate;
                shareinfolist.Add(myshareinfo);
            }
            //return Json(shareinfolist);
            return Json(new { data = shareinfolist }, JsonRequestBehavior.AllowGet);
        }
        
        [HttpGet]
        public ActionResult Save(int id)
        {
            using (WechatEntities dc = new WechatEntities())
            {
                var v = dc.ShareInfoes.Where(a => a.Id == id).FirstOrDefault();
                return View(v);
            }
        }


        [HttpGet]
        public ActionResult GeneratePage(int id)
        {
            using (WechatEntities dc = new WechatEntities())
            {
                var v = dc.Pageinexinfoes.Where(a => a.shareinfoID == id).FirstOrDefault();
                return View(v);
            }
        }

        [HttpPost]
        public ActionResult GeneratePage(Pageinexinfo pageindex, HttpPostedFileBase file)
        {

            bool status = false;
            if (ModelState.IsValid)
            {
                using (WechatEntities dc = new WechatEntities())
                {
              
                    string FriendsImages = "";
                    string adimages = "", friendimages = "";

                    pageindex.AuthorID = 100;
                    pageindex.InputDate = System.DateTime.Now;


                    // pageindex.Id :  Pageindex.shareinfoID 是否存在
                    var PageinfoDB = dc.Pageinexinfoes.Where(a => a.shareinfoID == pageindex.Id).FirstOrDefault();
                    if (PageinfoDB != null) {
                        //edit 
                        var v = dc.Pageinexinfoes.Where(a => a.Id == PageinfoDB.Id).FirstOrDefault();
                        if (v != null)
                        {
                            if (Request.Files[0].FileName != "")
                            {
                                FriendsImages = System.Guid.NewGuid().ToString() + System.IO.Path.GetExtension(Request.Files[0].FileName);
                                savefile(Request.Files[0], FriendsImages);
                                v.FriendsImages = "images/upload/" + FriendsImages;
                            }
                            else
                            {
                                v.FriendsImages = PageinfoDB.FriendsImages;
                            }


                            v.Description = pageindex.Description;
                            v.SharesRequirements = pageindex.SharesRequirements;


                            if (Request.Files[1].FileName != "")
                            {
                                adimages = System.Guid.NewGuid().ToString() + System.IO.Path.GetExtension(Request.Files[1].FileName);
                                savefile(Request.Files[1], adimages);
                                v.AdImages = "images/upload/" + adimages;
                            }
                            else
                            {
                                v.AdImages = PageinfoDB.AdImages;
                            }


                            if (Request.Files[2].FileName != "")
                            {
                                friendimages = System.Guid.NewGuid().ToString() + System.IO.Path.GetExtension(Request.Files[2].FileName);
                                savefile(Request.Files[2], friendimages);
                                v.friendimages = "images/upload/" + friendimages;
                            }
                            else
                            {
                                v.friendimages = PageinfoDB.friendimages;
                            }

                            v.AdURL = pageindex.AdURL; // test


                            string generateFile = System.IO.File.ReadAllText(System.AppDomain.CurrentDomain.BaseDirectory + "/template/" + "index.tl", Encoding.GetEncoding("gb2312"));
                            generateFile = generateFile.Replace("{0}", string.Format("<img src='http://www.bbpdt.cn/{0}'  style='height:620px' />", v.FriendsImages));
                            generateFile = generateFile.Replace("{1}", pageindex.Description);
                            generateFile = generateFile.Replace("{2}", pageindex.AdURL);
                            generateFile = generateFile.Replace("{3}", string.Format("<img src='http://www.bbpdt.cn/{0}'   style='width:100%;'  />", v.AdImages));
                            Log(generateFile, "index" + pageindex.Id.ToString() + ".html");

                        }

                         

                       
                    }
                    else {

                        //Save

                        if (Request.Files[0].FileName != "")
                        {
                            FriendsImages = System.Guid.NewGuid().ToString() + System.IO.Path.GetExtension(Request.Files[0].FileName);
                            savefile(Request.Files[0], FriendsImages);
                            pageindex.FriendsImages= "images/upload/" + FriendsImages;
                        }
                        if (Request.Files[1].FileName != "")
                        {
                            adimages = System.Guid.NewGuid().ToString() + System.IO.Path.GetExtension(Request.Files[1].FileName);
                            savefile(Request.Files[1], adimages);
                            pageindex.AdImages = "images/upload/" + adimages;
                        }
                        if (Request.Files[2].FileName != "")
                        {
                            friendimages = System.Guid.NewGuid().ToString() + System.IO.Path.GetExtension(Request.Files[2].FileName);
                            savefile(Request.Files[2], friendimages);
                            pageindex.friendimages = "images/upload/" + friendimages;
                        }
                        pageindex.shareinfoID = pageindex.Id;

                        // Save
                        dc.Pageinexinfoes.Add(pageindex);

                        string generateFile = System.IO.File.ReadAllText(System.AppDomain.CurrentDomain.BaseDirectory + "/template/" + "index.tl", Encoding.GetEncoding("gb2312"));
                        generateFile = generateFile.Replace("{0}", string.Format("<img src='http://www.bbpdt.cn/{0}'  style='height:620px' />", pageindex.FriendsImages));
                        generateFile = generateFile.Replace("{1}", pageindex.Description);
                        generateFile = generateFile.Replace("{2}", pageindex.AdURL);
                        generateFile = generateFile.Replace("{3}", string.Format("<img src='http://www.bbpdt.cn/{0}'   style='width:100%;'  />", pageindex.AdImages));
                       
                        Log(generateFile, "index" + pageindex.Id.ToString() + ".html");
                       
                    }


                  
                    dc.SaveChanges();
                    status = true;
                }
            }
            //return new JsonResult { Data = new { status = status} };
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Save(ShareInfo share, HttpPostedFileBase file)
        {
            string filename = "";
            bool status = false;
            if (ModelState.IsValid)
            {
                using (WechatEntities dc = new WechatEntities())
                {
                   
                    if (share.Id > 0)
                    {
                        //Edit 
                        var v = dc.ShareInfoes.Where(a => a.Id == share.Id).FirstOrDefault();
                        if (v != null)
                        {
                           
                            if (file != null)
                            {
                                filename = System.Guid.NewGuid().ToString() + System.IO.Path.GetExtension(file.FileName);
                                v.Image = "images/upload/" + filename;
                                savefile(file, filename);
                            }
                            else
                            {
                                if (share.Image != null)
                                {
                                    v.Image = share.Image;
                                }
                            }

                            v.Title = share.Title;
                            v.ShareURL = share.ShareURL;
                            v.Description = share.Description;
                            v.AuthorID = 100; // test
                        }
                    }
                    else
                    {
                        //Save
                        dc.ShareInfoes.Add(share);
                    }

                    dc.SaveChanges();
                    status = true;
                }
            }
            //return new JsonResult { Data = new { status = status} };
              return RedirectToAction("Index");

        }

        private void savefile(HttpPostedFileBase File,string fileName) {
            String FileExtension = System.IO.Path.GetExtension(File.FileName);
            File.SaveAs(Server.MapPath("~/images/upload/") + fileName);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            using (WechatEntities dc = new WechatEntities())
            {
                var v = dc.ShareInfoes.Where(a => a.Id == id).FirstOrDefault();
                if (v != null)
                {
                    return View(v);
                }
                else
                {
                    return HttpNotFound();
                }
            }
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteShareinfo(int id)
        {
            bool status = false;
            using (WechatEntities dc = new WechatEntities())
            {
                var v = dc.ShareInfoes.Where(a => a.Id == id).FirstOrDefault();
                if (v != null)
                {
                    dc.ShareInfoes.Remove(v);
                    dc.SaveChanges();
                    status = true;
                }
            }
            return new JsonResult { Data = new { status = status} };
        }

        public void Log(string content, string filename)
        {
            try
            {
                //string filename = DateTime.Now.ToString("yyyyMMdd") + ".txt";
                if (!Directory.Exists(Server.MapPath("~/Pages/")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Pages/"));
                }
                FileInfo file = new FileInfo(Server.MapPath("~/Pages/") + filename);
              
                StreamWriter sw = null;
                if (!file.Exists)
                {
                    sw = file.CreateText();
                    sw.WriteLine(content.ToString());
                }
                else
                {
                    file.Delete();
                    sw = file.AppendText();
                    sw.WriteLine(content.ToString());
                }
                sw.Close();
                sw.Flush();
                sw.Dispose();
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.ToString());
            }
        }
    }
}