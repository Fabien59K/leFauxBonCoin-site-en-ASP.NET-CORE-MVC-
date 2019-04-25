using FauxBonCoin.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FauxBonCoin.Models
{
    public class Utilisateur
    {
        private int id;
        private string nom;
        private string prenom;
        private string email;
        private string telephone;
        private string password;

        public int Id { get => id; set => id = value; }
        public string Nom { get => nom; set => nom = value; }
        public string Prenom { get => prenom; set => prenom = value; }
        public string Email { get => email; set => email = value; }
        public string Telephone { get => telephone; set => telephone = value; }
        public string Password { get => password; set => password = value; }

        public bool Exist()
        {
            return DataBase.Instance.LookUser(this);
        }

        public bool ExistWithPassword()
        {
            return DataBase.Instance.LookUser(Email, Password, this);
        }

        public void Add()
        {
            DataBase.Instance.AddUser(this);
        }

    }
}
