﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using WebTMDT.Models;

namespace WebTMDT.Helpers
{
    public static class Configs
    {
        public const int ImageMinimumBytes = 512;
        private static langson12Entities db = new langson12Entities();
        public static string getImageFromId(string id)
        {
            try
            {
                return db.AspNetUsers.Find(id).Photo1;
            }
            catch
            {
                return "";
            }
        }
        public static bool IsImage(HttpPostedFileBase postedFile)
        {
            //-------------------------------------------
            //  Check the image mime types
            //-------------------------------------------
            if (postedFile.ContentType.ToLower() != "image/jpg" &&
                        postedFile.ContentType.ToLower() != "image/jpeg" &&
                        postedFile.ContentType.ToLower() != "image/pjpeg" &&
                        postedFile.ContentType.ToLower() != "image/gif" &&
                        postedFile.ContentType.ToLower() != "image/x-png" &&
                        postedFile.ContentType.ToLower() != "image/png")
            {
                return false;
            }

            //-------------------------------------------
            //  Check the image extension
            //-------------------------------------------
            if (System.IO.Path.GetExtension(postedFile.FileName).ToLower() != ".jpg"
                && System.IO.Path.GetExtension(postedFile.FileName).ToLower() != ".png"
                && System.IO.Path.GetExtension(postedFile.FileName).ToLower() != ".gif"
                && System.IO.Path.GetExtension(postedFile.FileName).ToLower() != ".jpeg")
            {
                return false;
            }

            //-------------------------------------------
            //  Attempt to read the file and check the first bytes
            //-------------------------------------------
            try
            {
                if (!postedFile.InputStream.CanRead)
                {
                    return false;
                }

                if (postedFile.ContentLength < ImageMinimumBytes)
                {
                    return false;
                }

                byte[] buffer = new byte[512];
                postedFile.InputStream.Read(buffer, 0, 512);
                string content = System.Text.Encoding.UTF8.GetString(buffer);
                if (Regex.IsMatch(content, @"<script|<html|<head|<title|<body|<pre|<table|<a\s+href|<img|<plaintext|<cross\-domain\-policy",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline))
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

            //-------------------------------------------
            //  Try to instantiate new Bitmap, if .NET will throw exception
            //  we can assume that it's not a valid image
            //-------------------------------------------

            try
            {
                using (var bitmap = new System.Drawing.Bitmap(postedFile.InputStream))
                {
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public static string unicodeToNoMark(string input)
        {
            input = input.ToLowerInvariant().Trim();
            if (input == null) return "";
            string noMark = "a,a,a,a,a,a,a,a,a,a,a,a,a,a,a,a,a,a,e,e,e,e,e,e,e,e,e,e,e,e,u,u,u,u,u,u,u,u,u,u,u,u,o,o,o,o,o,o,o,o,o,o,o,o,o,o,o,o,o,o,i,i,i,i,i,i,y,y,y,y,y,y,d,A,A,E,U,O,O,D";
            string unicode = "a,á,à,ả,ã,ạ,â,ấ,ầ,ẩ,ẫ,ậ,ă,ắ,ằ,ẳ,ẵ,ặ,e,é,è,ẻ,ẽ,ẹ,ê,ế,ề,ể,ễ,ệ,u,ú,ù,ủ,ũ,ụ,ư,ứ,ừ,ử,ữ,ự,o,ó,ò,ỏ,õ,ọ,ơ,ớ,ờ,ở,ỡ,ợ,ô,ố,ồ,ổ,ỗ,ộ,i,í,ì,ỉ,ĩ,ị,y,ý,ỳ,ỷ,ỹ,ỵ,đ,Â,Ă,Ê,Ư,Ơ,Ô,Đ";
            string[] a_n = noMark.Split(',');
            string[] a_u = unicode.Split(',');
            for (int i = 0; i < a_n.Length; i++)
            {
                input = input.Replace(a_u[i], a_n[i]);
            }
            input = input.Replace("  ", " ");
            input = Regex.Replace(input, "[^a-zA-Z0-9% ._]", string.Empty);
            input = removeSpecialChar(input);
            input = input.Replace(" ", "-");
            input = input.Replace("--", "-");
            return input;
        }

        public static string ConvertToStringNoUnicode(string input)
        {
            input = input.ToLowerInvariant().Trim();
            if (input == null) return "";
            string noMark = "a,a,a,a,a,a,a,a,a,a,a,a,a,a,a,a,a,a,e,e,e,e,e,e,e,e,e,e,e,e,u,u,u,u,u,u,u,u,u,u,u,u,o,o,o,o,o,o,o,o,o,o,o,o,o,o,o,o,o,o,i,i,i,i,i,i,y,y,y,y,y,y,d,A,A,E,U,O,O,D";
            string unicode = "a,á,à,ả,ã,ạ,â,ấ,ầ,ẩ,ẫ,ậ,ă,ắ,ằ,ẳ,ẵ,ặ,e,é,è,ẻ,ẽ,ẹ,ê,ế,ề,ể,ễ,ệ,u,ú,ù,ủ,ũ,ụ,ư,ứ,ừ,ử,ữ,ự,o,ó,ò,ỏ,õ,ọ,ơ,ớ,ờ,ở,ỡ,ợ,ô,ố,ồ,ổ,ỗ,ộ,i,í,ì,ỉ,ĩ,ị,y,ý,ỳ,ỷ,ỹ,ỵ,đ,Â,Ă,Ê,Ư,Ơ,Ô,Đ";
            string[] a_n = noMark.Split(',');
            string[] a_u = unicode.Split(',');
            for (int i = 0; i < a_n.Length; i++)
            {
                input = input.Replace(a_u[i], a_n[i]);
            }
            input = input.Replace("  ", " ");
            input = Regex.Replace(input, "[^a-zA-Z0-9% ._]", string.Empty);
            input = removeSpecialChar(input);
            return input;
        }

        public static bool ContainsPunctuation(string text, string Punctuation)
        {
            return text.IndexOfAny(Punctuation.ToCharArray()) >= 0;
        }

        public static string removeSpecialChar(string input)
        {
            input = input.Replace("-", "").Replace(":", "").Replace(",", "").Replace("_", "").Replace("'", "").Replace("\"", "").Replace(";", "").Replace("”", "").Replace(".", "").Replace("%", "");
            return input;
        }

        public static string splitString(string input)
        {
            string strNew = unicodeToNoMark(input);
            return strNew.Replace("-", " ");
        }

        public static List<sortOrder> KhoangGia()
        {
            var numbersArray = Enumerable.Range(1, 20).ToArray();
            List<int> List_int1 = new List<int>();
            for (int i = 1; i <= numbersArray.Length; i += 2)
            {
                List_int1.Add(i);
            }
            List_int1.Add(20);

            List<sortOrder> myList = new List<sortOrder>();

            for (int i = 0; i < List_int1.Count-1; i++)
            {
                //if (i == 10)
                //{
                //    break;
                //}
                var _x = string.Format("{0}-{1}", List_int1[i], List_int1[i + 1]);
                CultureInfo vi_VN = new CultureInfo("vi-VN");
                var _y = string.Format(vi_VN.NumberFormat, "Từ {0:n0}- đến {1:n0}", List_int1[i] * 1000000, List_int1[i + 1] * 1000000);
                myList.Add(new sortOrder() { NameSort = _y, TypeSort = _x });
            }
            var _x1 = string.Format("{0}-{1}", List_int1[List_int1.Count - 1], 10000000000);
            CultureInfo vi_VN2 = new CultureInfo("vi-VN");
            var _y1 = string.Format(vi_VN2.NumberFormat, "Từ {0:n0}- đến {1:n0}", List_int1[List_int1.Count - 1] * 1000000, 10000000000);
            myList.Add(new sortOrder() { NameSort = _y1, TypeSort = _x1 });
            return myList;
        }

        public class TwoNumber
        {
            public long Number1 { get; set; }
            public long Number2 { get; set; }
        }

        public static TwoNumber GetNumber(string textNumber)
        {
            // textNumber = "3-5";
            string[] x = textNumber.Split('-');
            TwoNumber _tw = new TwoNumber();
            _tw.Number1 = long.Parse(x[0]) * 1000000;
            _tw.Number2 = long.Parse(x[1]) * 1000000;
            return _tw;
        }

        public static string CookieName { get; set; }
        public static void setCookie(string cookieName, string cookieValue) {
            HttpCookie myCookie = new HttpCookie(cookieName);
            myCookie.Name = cookieName;
            myCookie.Value = cookieValue;
            myCookie.Expires = DateTime.Now.AddDays(7);
            HttpContext.Current.Response.SetCookie(myCookie);
        }

        public static string getCookie(string name)
        {
            if (HttpContext.Current.Request.Cookies.Get(name) != null)
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies.Get(name);
                return cookie.Value;
            }
            else
            {
                return "";
            }           
        }
        public static void removieCookie(string field)
        {
            HttpCookie MyCookie = new HttpCookie(field);
            MyCookie.Value = "";
            MyCookie.Expires = DateTime.Now.AddDays(-1);
            HttpContext.Current.Response.Cookies.Add(MyCookie);
            //Response.Cookies.Add(MyCookie);   

        }
         //ViewBag.ProductType = new List<ProductType>() {
         //       new ProductType() { ProductTypeName = "Hàng chính hãng" },
         //       new ProductType() { ProductTypeName = "Hàng xách tay" },
         //       new ProductType() { ProductTypeName = "Hàng lỗi" },
         //       new ProductType() { ProductTypeName = "Hàng xuất khẩu" },
         //       new ProductType() { ProductTypeName = "Hàng khác" },
         //   };
         //   ViewBag.ProductStatus = new List<ProductStatus>() { 
         //       new ProductStatus() { ProductStatusName = "Mới 100%" },
         //       new ProductStatus() { ProductStatusName = "Mới 90%" },
         //       new ProductStatus() { ProductStatusName = "Mới 80%" },
         //       new ProductStatus() { ProductStatusName = "Hàng like new" },
         //       new ProductStatus() { ProductStatusName = "Hàng cũ" },
         //       new ProductStatus() { ProductStatusName = "Hàng thanh lý" },
         //       new ProductStatus() { ProductStatusName = "Hàng cho không" },
         //       new ProductStatus() { ProductStatusName = "Hàng khác" }
         //   };
        public static IEnumerable<ProductType> CreateListProductType()
        {
            List<ProductType> p = new List<ProductType>();
            p.Add(new ProductType() {ProductTypeName = "Xe nhập khẩu chính hãng"});
            p.Add(new ProductType() {ProductTypeName = "Xe lắp ráp trong nước"});
            p.Add(new ProductType() {ProductTypeName = "Xe ngoại giao"});           
            p.Add(new ProductType() {ProductTypeName = "khác"});
            return p;
        }

        public static IEnumerable<ProductKg> CreateListProductKg()
        {
            List<ProductKg> p = new List<ProductKg>();
            p.Add(new ProductKg() { ProductKgName = "1 tấn", ProductKgValue = 1 });
            p.Add(new ProductKg() { ProductKgName = "3 tấn", ProductKgValue = 3 });
            p.Add(new ProductKg() { ProductKgName = "5 tấn", ProductKgValue = 5 });
            p.Add(new ProductKg() { ProductKgName = "10 tấn", ProductKgValue = 10 });
            p.Add(new ProductKg() { ProductKgName = "15 tấn", ProductKgValue = 15 });
            p.Add(new ProductKg() { ProductKgName = "20 tấn", ProductKgValue = 20 });
            p.Add(new ProductKg() { ProductKgName = "Trên 20 tấn", ProductKgValue = 25 });
            return p;
        }
        public static IEnumerable<ProductHumanType> CreateListProductHumanType()
        {
            List<ProductHumanType> p = new List<ProductHumanType>();
            p.Add(new ProductHumanType() { ProductHumanTypeName = "xe tìm hàng"});
            p.Add(new ProductHumanType() { ProductHumanTypeName = "hàng tìm xe" });
            p.Add(new ProductHumanType() { ProductHumanTypeName = "xe tìm người" });
            p.Add(new ProductHumanType() { ProductHumanTypeName = "người tìm xe" });
            p.Add(new ProductHumanType() { ProductHumanTypeName = "đi chung xe" });
            p.Add(new ProductHumanType() { ProductHumanTypeName = "hợp tác vận tải" });
            return p;
        }
        public static IEnumerable<ProductStatus> CreateListProductStatus()
        {
            List<ProductStatus> p = new List<ProductStatus>();
            p.Add(new ProductStatus() { ProductStatusName = "Mới 100%" });
            p.Add(new ProductStatus() { ProductStatusName = "Mới 90%" });
            p.Add(new ProductStatus() { ProductStatusName = "Mới 80%" });
            p.Add(new ProductStatus() { ProductStatusName = "Xe gara" });
            p.Add(new ProductStatus() { ProductStatusName = "Xe cũ" });
            p.Add(new ProductStatus() { ProductStatusName = "Xe thanh lý" });            
            p.Add(new ProductStatus() { ProductStatusName = "Xe khác" });
            return p;
        }

        public static string GetMd5Hash(string input)
        {
            MD5 md5Hash = MD5.Create();
            if (input == "" || input == null) input = "chanhniem";
            // Convert the input string to a byte array and compute the hash. 
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes 
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string. 
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string. 
            return sBuilder.ToString();
        }
    }
}