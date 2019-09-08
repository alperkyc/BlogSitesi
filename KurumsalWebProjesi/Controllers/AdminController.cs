using KurumsalWebProjesi.Models.DataContext;
using KurumsalWebProjesi.Models.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace KurumsalWebProjesi.Controllers
{
    public class AdminController : Controller
    {
        KurumsalDB1Context db = new KurumsalDB1Context();
        // GET: Admin
        [Route("yonetimpaneli")]
        public ActionResult Index()
        {
            ViewBag.BlogSayi = db.Blog.Count();
            ViewBag.KategoriSayi = db.Kategori.Count();
            ViewBag.HizmetSayi = db.Hizmet.Count();
            ViewBag.YorumSayi = db.Yorum.Count();
            ViewBag.YorumOnay = db.Yorum.Where(x => x.Onay == false).Count();
            var sorgu = db.Kategori.ToList();
            return View(sorgu);
        }

        [Route("yonetimpaneli/giris")]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(Admin admin)
        {
            string md5pass = Crypto.Hash(admin.Sifre,"MD5");
            var login = db.Admin.Where(x => x.Eposta.Equals(admin.Eposta) && x.Sifre.Equals(md5pass)).FirstOrDefault();

            if(login!=null)
            {
                Session["adminid"] = login.AdminId;
                Session["eposta"] = login.Eposta;
                Session["yetki"] = login.Yetki;
                return RedirectToAction("Index","Admin");
            }
            ViewBag.Uyari = "Eposta ya da şifre yanlış..";
            return View(admin);
        }

        public ActionResult Logout()
        {
            Session["adminid"] = null;
            Session["eposta"] = null;
            Session["yetki"] = null;
            Session.Abandon();
            return RedirectToAction("Login", "Admin");
            
        }

        public ActionResult SifremiUnuttum()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SifremiUnuttum(string eposta)
        {
            var mail = db.Admin.Where(x => x.Eposta == eposta).SingleOrDefault();

            if (mail !=null)
            {
                Random rnd = new Random();
                int yenisifre = rnd.Next();

                Admin admin = new Admin();
                mail.Sifre = Crypto.Hash(Convert.ToString(yenisifre), "MD5");
                db.SaveChanges();

                

               
                WebMail.SmtpServer = "smtp.gmail.com";
                WebMail.EnableSsl = true;
                WebMail.UserName = "kurumsalwebtest@gmail.com";
                WebMail.Password = "ALP114274x";
                WebMail.SmtpPort = 587;
                WebMail.Send(eposta, "Admin Panel Giriş Şifreniz",  "<br>" + "Şifreniz :" + yenisifre);
                ViewBag.Uyari = "Şifreniz mailinize başarıyla gönderilmiştir.";
            }

            else
            {
                ViewBag.Uyari = "Sistemde kayıtlı böyle bir mail adresi bulunamamaktadir.";
            }
            return View();
        }

        public ActionResult Adminler()
        {
            return View(db.Admin.ToList());
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Admin admin,string sifre,string eposta)
        {
            if(ModelState.IsValid)
            {
                admin.Sifre = Crypto.Hash(sifre, "MD5");
                db.Admin.Add(admin);
                db.SaveChanges();
                return RedirectToAction("Adminler");
            }
            return View(admin);
        }
        

        public ActionResult Edit(int id)
        {
            var a = db.Admin.Where(x => x.AdminId == id).SingleOrDefault();
            return View(a);
        }

        [HttpPost]
        public ActionResult Edit(int id,Admin admin,string sifre,string eposta)
        {
            
            if(ModelState.IsValid)
            {
                var a = db.Admin.Where(x => x.AdminId == id).SingleOrDefault();
                a.Sifre = Crypto.Hash(sifre, "MD5");
                a.Eposta = admin.Eposta;    
                a.Yetki = admin.Yetki;
                db.SaveChanges();
                return RedirectToAction("Adminler");
            }
            return View(admin);
        }

        public ActionResult Delete(int id)
        {
            var a = db.Admin.Where(x => x.AdminId == id).SingleOrDefault();
           

            if(a!=null)
            {
                db.Admin.Remove(a);
                db.SaveChanges();
                return RedirectToAction("Adminler");
            }
            return View();
        }
    }
}