using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FauxBonCoin.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FauxBonCoin.Controllers
{
    public class UtilisateursController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Route("[Controller]/Login")]
        public IActionResult Login()
        {
            Utilisateur u = new Utilisateur();
            return View(u);
        }

        [HttpPost]
        public IActionResult LoginPost(string email, string password)
        {
            Utilisateur u = new Utilisateur { Email = email, Password = password };
            List<string> errors = new List<string>();
            if(email == null)
            {
                errors.Add("Merci de saisir un email");
            }
            if(password == null)
            {
                errors.Add("Merci de saisir un mot de passe");
            }
            if (!u.ExistWithPassword())
            {
                errors.Add("Aucun utilisateur avec cette email et mot de passe");
            }
            if(errors.Count > 0)
            {
                ViewBag.errors = errors;
                return View("Login",u);
            }
            else
            {
                HttpContext.Session.SetString("logged", "true");
                HttpContext.Session.SetString("nom", u.Nom);
                HttpContext.Session.SetString("prenom", u.Prenom);
                HttpContext.Session.SetString("idUser", u.Id.ToString());
                return RedirectToRoute(new { controller = "Annonces", action = "Index" });
            }
        }

        [HttpPost]
        public IActionResult Register(string nom, string prenom, string email, string telephone, string password, string cPassword)
        {
            List<string> errors = new List<string>();
            Utilisateur u = new Utilisateur { Nom = nom, Prenom = prenom, Email = email, Telephone = telephone, Password = password };
            if(nom == null)
            {
                errors.Add("Merci de saisir un Nom");
            }
            if(prenom == null)
            {
                errors.Add("Merci de saisir un prénom");
            }
            if(email  == null)
            {
                errors.Add("Merci de saisir un email");
            }
            if(telephone == null)
            {
                errors.Add("Merci de saisir un numéro de téléphone");
            }
            if(password == null)
            {
                errors.Add("Merci de saisir un mot de passe");
            }
            if(password != cPassword)
            {
                errors.Add("Merci de saisir le même mot de passe");
            }
            if (u.Exist())
            {
                errors.Add("Cette adresse email est déjà utilisée");
            }
            ViewBag.errors = errors;
            if(errors.Count > 0)
            {
                
            }
            else
            {
                u.Add();
                ViewBag.Inscription = true;
                ViewBag.Message = "Inscription reussie merci de vous connecter";
            }
            return View("Login", u);
        }

        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return RedirectToRoute(new { controller = "Annonces", action = "Index" });
        }
    }
}