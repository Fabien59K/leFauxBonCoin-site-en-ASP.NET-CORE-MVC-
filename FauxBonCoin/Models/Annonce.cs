using FauxBonCoin.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FauxBonCoin.Models
{
    public class Annonce
    {
        private int id;
        private string titre;
        private string description;
        private int idCategory;
        private int idUser;
        private decimal prix;
        private DateTime dateAjout;
        private List<ImageAnnonce> images;

        public int Id { get => id; set => id = value; }
        public string Titre { get => titre; set => titre = value; }
        public string Description { get => description; set => description = value; }
        public int IdCategory { get => idCategory; set => idCategory = value; }
        public int IdUser { get => idUser; set => idUser = value; }
        public decimal Prix { get => prix; set => prix = value; }
        public DateTime DateAjout { get => dateAjout; set => dateAjout = value; }
        public List<ImageAnnonce> Images { get => images; set => images = value; }

        public Annonce()
        {
            Images = new List<ImageAnnonce>();
            DateAjout = DateTime.Now;
        }

        public void Add()
        {
            DataBase.Instance.AddAnnonce(this);
        }

        public static List<Annonce> LoadAnnonces(int? idCategory)
        {
            return DataBase.Instance.GetAnnonces(idCategory);
        }
    }
}
