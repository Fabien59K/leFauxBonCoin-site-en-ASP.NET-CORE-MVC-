using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FauxBonCoin.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FauxBonCoin.Controllers
{
    public class AnnoncesController : Controller
    {
        public IActionResult Index(int? idCategory)
        {
            UserConnect(ViewBag);
            ViewBag.categories = Category.GetCategories();
            List<Annonce> liste = Annonce.LoadAnnonces(idCategory);
            return View(liste);
        }

        [Route("[Controller]/{id?}")]
        public IActionResult Detail(int? id)
        {
            UserConnect(ViewBag);
            return View();
        }
        [Route("[Controller]/Deposer")]
        public IActionResult DeposerAnnonce()
        {
            UserConnect(ViewBag);
            bool connected = Convert.ToBoolean(HttpContext.Session.GetString("logged"));
            if(connected)
            {
                Annonce a = new Annonce();
                ViewBag.ListeCategories = Category.GetCategories();
                return View("Depot",a);
            }
            else
            {
                return RedirectToRoute(new { controller = "Utilisateurs", action = "Login" });
            }
            
        }

        [Route("[Controller]/DeposerAnnoncePost")]
        [HttpPost]
        public async Task<IActionResult> DeposerAnnoncePost(string titre, string description, decimal? prix, int? categorie, List<IFormFile> images)
        {
            Annonce a = new Annonce() { Titre = titre, Description = description, Prix = (decimal)prix, IdCategory = (int)categorie };
            a.IdUser = Convert.ToInt32(HttpContext.Session.GetString("idUser"));
            List<string> errors = new List<string>();
            if(titre == null)
            {
                errors.Add("Merci d'indiquer un titre d'annonce");
            }
            if (description == null)
            {
                errors.Add("Merci d'indiquer une description d'annonce");
            }
            if (prix == null)
            {
                errors.Add("Merci d'indiquer un prix à l'annonce");
            }
            if (categorie == null)
            {
                errors.Add("Merci d'indiquer une catégorie d'annonce");
            }
            if(errors.Count > 0)
            {
                
                ViewBag.error = true;
                ViewBag.errors = errors;
                return View("Depot", a);
            }
            else
            {
                foreach(IFormFile f in images)
                {
                    if(f.FileName.Contains(".png") || f.FileName.Contains(".jpg"))
                    {
                       
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/annonces", a.IdUser.ToString() + "-" + f.FileName);
                        var stream = new FileStream(path, FileMode.Create);
                        await f.CopyToAsync(stream);
                        a.Images.Add(new ImageAnnonce { Url = "images/annonces/" + a.IdUser.ToString() + "-" + f.FileName });
                    }
                    
                }
                
                a.Add();
                return RedirectToAction("Index");
            }
        }

        [Route("Categorie")]
        public IActionResult ListeCategories()
        {
            UserConnect(ViewBag);
            List<Category> liste = Category.GetCategories();
            return View("ListeCategories",liste);
        }
        [Route("Categorie/Add")]
        public IActionResult AjouterCategorie()
        {
            UserConnect(ViewBag);
            return View("AddCategorie");
        }

        [Route("Categorie/Register")]
        [HttpPost]
        public IActionResult RegisterCategory(string titre)
        {
            Category c = new Category { Titre = titre };
            c.Add();
            return RedirectToAction("ListeCategories");
        }

        public IActionResult DeleteCategory(int id)
        {
            Category c = new Category { Id = id };
            c.Delete();
            return RedirectToAction("ListeCategories");
        }

        private void UserConnect(dynamic v)
        {
            bool? logged = Convert.ToBoolean(HttpContext.Session.GetString("logged"));
            if(logged == true)
            {
                v.Logged = logged;
                v.Nom = HttpContext.Session.GetString("nom");
                v.Prenom = HttpContext.Session.GetString("prenom");
            }
        }
    }
}