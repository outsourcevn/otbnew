﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using WebTMDT.Helpers;
using WebTMDT.Models;
using Newtonsoft.Json;
namespace WebTMDT.Controllers
{
    public class HomeController : Controller
    {
        private langson12Entities db = new langson12Entities();

        public ActionResult Index(int? status)
        {
            var Cat = db.Categories.ToList();
            ViewBag.Category = Cat;
            var LocalData = db.Locals.ToList();
            ViewBag.LocalData = LocalData;
            ViewBag.ProductHumanType = Configs.CreateListProductHumanType();
            if (status==null)
            {
                ViewBag.status=0;
            }else{
                ViewBag.status=status;
            }
            return View();
        }
        [HttpGet]
        public string getF22()
        {
            return JsonConvert.SerializeObject(Configs.CreateListProductHumanType());
        }
        #region LoadDanhMuc
        [ChildActionOnly]
        public IEnumerable<CatViewModel> CreateVM(int parentid, IEnumerable<Category> source)
        {
            var data = from men in source
                       where men.F3 == parentid
                       select new CatViewModel()
                       {
                           CatId = men.F1,
                           CatName = men.F2
                           //// other properties
                           //ChildrenCat = CreateVM(men.F1, source)
                       };
            return data;
        }

        [ChildActionOnly]
        public IEnumerable<LocalViewModel> CreateVMLocal(int parentid, IEnumerable<Local> source)
        {
            var data = from men in source
                       where men.F3 == parentid
                       select new LocalViewModel()
                       {
                           LocalId = men.F1,
                           LocalName = men.F2
                           //// other properties
                           //ChildrenLocal = CreateVMLocal(men.F1, source)
                       };
            return data;
        }

        [HttpPost]
        public ActionResult SelectDanhMuc3(int id)
        {
            var data = from men in db.Categories
                       where men.F3 == id
                       select new DanhMucCon()
                       {
                           Id = men.F1,
                           Name = men.F2
                       };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult NotFound()
        {
            var x = new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            return View(x);
        }

        //[ChildActionOnly]
        //public ActionResult ShowCat1()
        //{
        //    var data = db.Categories.Where(x => x.F3 == null).ToList();
        //    return PartialView("ShowCat1", data);
        //}

        //[HttpPost]
        //public ActionResult ShowCat2(int? id)
        //{
        //    var data = db.Categories.Where(x => x.F3 == id).ToList();
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        [ChildActionOnly]
        public ActionResult _DanhMucSanPhamPartial()
        {
            var data = db.Categories.Where(x => x.F3 == null).ToList();
            
            return PartialView("_DanhMucSanPhamPartial", data);
        }
        //[ChildActionOnly]
        public ActionResult _DanhMucSanPhamPartial2(int? status)
        {
            var data = Configs.CreateListProductHumanType();
            if (status == null)
            {
                ViewBag.status = 0;
            }
            else
            {
                ViewBag.status = status;
            }
            return PartialView("_DanhMucSanPhamPartial2", data);
        }
        public ActionResult listComment(long? product_id)
        {
            try {
                if (Configs.getCookie("user_type") == "3" || Configs.getCookie("user_type") == "2")
                { 
                    var data = db.comments.Where(o => o.product_id == product_id).OrderBy(o => o.id).Take(100).ToList();
                    return PartialView("listComment", data);
                }
                else
                {
                    string user_id = Configs.getCookie("user_id");
                    var data = db.comments.Where(o => o.product_id == product_id && o.user_id == user_id).OrderBy(o => o.id).Take(100).ToList();
                    return PartialView("listComment", data);
                }
                
            }catch{
                return PartialView("listComment", null);
            }
        }
        //_ProductWithCatelog
        [ChildActionOnly]
        public ActionResult _ProductWithCatelog(int? id)
        {

            //var products = GetProductOfCat(id).OrderByDescending(x => x.F10).Take(5).ToList();
            var _cat = db.Categories.Where(x => x.F1 == id).FirstOrDefault();
            List<Product> _products = new List<Product>();
            if (_cat != null)
            {
                if (_cat.Category1.Count > 0)
                {
                    SetProducts(_cat.Category1, _products);
                    //foreach (var c1 in _cat.Category1)
                    //{
                    //    if (c1.Products.Count > 0)
                    //    {
                    //         _products.AddRange(c1.Products);
                    //    }
                    //    if (c1.Category1.Count > 0)
                    //    {
                    //        foreach (var c2 in c1.Category1)
                    //        {
                    //            if (c2.Products.Count > 0)
                    //            {
                    //                _products.AddRange(c2.Products);
                    //            }
                    //        }
                    //    }
                    //}
                }
                else
                {
                    _products.AddRange(_cat.Products);
                }
            }
            else
            {
                _products = null;
            }
            return PartialView("_ProductWithCatelog", _products.OrderByDescending(x=>x.F10).Take(4).ToList());
        }
        [ChildActionOnly]
        public ActionResult _ProductWithCatelog2(string cat,int? status)
        {
            if (status == null) { 
                //var products = GetProductOfCat(id).OrderByDescending(x => x.F10).Take(5).ToList();
                var _products = db.Products.Where(o => o.F22.Contains(cat) && o.status!=1).OrderByDescending(o => o.F1).Take(5).ToList();
                return PartialView("_ProductWithCatelog2", _products);
            }
            else
            {
                var _products = db.Products.Where(o => o.F22.Contains(cat) && o.status == status).OrderByDescending(o => o.F1).Take(5).ToList();
                return PartialView("_ProductWithCatelog2", _products);
            }
        }
        //public void SetProducts2(ICollection<Category> ic, List<Product> _products)
        //{
        //    foreach (var c1 in ic)
        //    {
        //        if (c1.Products.Count > 0)
        //        {
        //            _products.AddRange(c1.Products);
        //        }
        //        if (c1.Category1.Count > 0)
        //        {
        //            SetProducts(c1.Category1, _products);
        //        }
        //    }
        //}
        public void SetProducts(ICollection<Category> ic, List<Product> _products)
        {
            foreach (var c1 in ic)
            {
                if (c1.Products.Count > 0)
                {
                    _products.AddRange(c1.Products);
                }
                if (c1.Category1.Count > 0)
                {
                    SetProducts(c1.Category1, _products);
                }
            }
        }


        public IEnumerable<Product> GetProductOfCat(int? id)
        {
            var _cat = db.Categories.Where(x => x.F1 == id).FirstOrDefault();
            List<Product> _products = new List<Product>();
            if (_cat != null)
            {
                if (_cat.Category1.Count > 0)
                {
                    SetProducts(_cat.Category1, _products);
                }
                else
                {
                    _products.AddRange(_cat.Products);
                }
            }
            else
            {
                _products = null;
            }
            return _products;
        }
        public bool GetProductOfCat2(string F22)
        {
            try
            {
                return db.Products.Any(o => o.F22.Contains(F22));
            }
            catch
            {
                return false;
            }
            //var _cat = db.Categories.Where(x => x.F1 == id).FirstOrDefault();
            //List<Product> _products = new List<Product>();
            //if (_cat != null)
            //{
            //    if (_cat.Category1.Count > 0)
            //    {
            //        SetProducts(_cat.Category1, _products);
            //    }
            //    else
            //    {
            //        _products.AddRange(_cat.Products);
            //    }
            //}
            //else
            //{
            //    _products = null;
            //}
            //return _products;
        }
        public string generateSiteMap()
        {

            try
            {

                XmlWriterSettings settings = null;
                string xmlDoc = null;
                var p = db.Products.Where(o=>o.status!=1).ToList();
                settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.Encoding = Encoding.UTF8;
                xmlDoc = HttpRuntime.AppDomainAppPath + "sitemap.xml";//HttpContext.Server.MapPath("../") + 
                float percent = 0.85f;

                string urllink = "";
                using (XmlTextWriter writer = new XmlTextWriter(xmlDoc, Encoding.UTF8))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("urlset");
                    writer.WriteAttributeString("xmlns", "http://www.sitemaps.org/schemas/sitemap/0.9");

                    writer.WriteStartElement("url");
                    writer.WriteElementString("loc", "http://langson12.net");
                    writer.WriteElementString("changefreq", "always");
                    writer.WriteElementString("priority", "1");
                    writer.WriteEndElement();

                    for (int i = 0; i < p.Count; i++)
                    {
                        try
                        {
                            writer.WriteStartElement("url");
                            urllink = "http://langson12.net/" + Configs.unicodeToNoMark(db.Categories.Find(p[i].F15).F2) + "/" + Configs.unicodeToNoMark(p[i].F2) + "-" + p[i].F1+".html";
                            writer.WriteElementString("loc", urllink);
                            //writer.WriteElementString("lastmod", DR["datetime"].ToString());
                            try
                            {
                                if (i < 500)
                                {
                                    writer.WriteElementString("changefreq", "hourly");
                                    percent = 0.85f;
                                }
                                else
                                {
                                    writer.WriteElementString("changefreq", "monthly");
                                    percent = 0.70f;
                                }
                            }
                            catch (Exception ex1)
                            {
                            }

                            writer.WriteElementString("priority", percent.ToString("0.00"));
                            writer.WriteEndElement();
                        }
                        catch (Exception ex2)
                        {
                        }
                    }

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }

            }
            catch (Exception extry)
            {
                //StreamWriter sw = new StreamWriter();
            }
            return "ok";
        }
    }
}