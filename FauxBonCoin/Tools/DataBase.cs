using FauxBonCoin.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FauxBonCoin.Tools
{
    public class DataBase
    {
        private static DataBase _instance = null;
        private static object _lock = new object();

        public static DataBase Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new DataBase();
                    }
                    return _instance;
                }
            }
        }
        private DataBase()
        {

        }

        public List<Category> LoadCategories()
        {
            List<Category> liste = new List<Category>();
            SqlCommand command = new SqlCommand("SELECT * FROM Categorie", Connection.Instance);
            Connection.Instance.Open();
            SqlDataReader reader = command.ExecuteReader();
            while(reader.Read())
            {
                Category c = new Category { Id = reader.GetInt32(0), Titre = reader.GetString(1) };
                liste.Add(c);
            }
            reader.Close();
            command.Dispose();
            Connection.Instance.Close();
            return liste;
        }
        public void AddCategory(Category c)
        {
            SqlCommand command = new SqlCommand("INSERT INTO Categorie(Titre) OUTPUT INSERTED.ID VALUES(@Titre)", Connection.Instance);
            command.Parameters.Add(new SqlParameter("@Titre", c.Titre));
            Connection.Instance.Open();
            c.Id = (int)command.ExecuteScalar();
            command.Dispose();
            Connection.Instance.Close();
        }
        public void DeleteCategory(Category c)
        {
            SqlCommand command = new SqlCommand("DELETE FROM Categorie WHERE Id = @id", Connection.Instance);
            command.Parameters.Add(new SqlParameter("@id", c.Id));
            Connection.Instance.Open();
            command.ExecuteNonQuery();
            Connection.Instance.Close();
        }

        static string GetMd5Hash(MD5 md5Hash, string input)
        {

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

        public bool LookUser(Utilisateur u)
        {
            bool retour = false;
            if (u.Email != null)
            {
                SqlCommand command = new SqlCommand("SELECT * FROM Utilisateur where Email=@email", Connection.Instance);
                command.Parameters.Add(new SqlParameter("@email", u.Email));
                Connection.Instance.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    u.Id = reader.GetInt32(0);
                    retour = true;
                }
                else
                {
                    retour = false;
                }
                reader.Close();
                command.Dispose();
                Connection.Instance.Close();
            }
            else
            {
                retour = false;
            }

            return retour;
        }

        public bool LookUser(string email, string password, Utilisateur u)
        {
            bool retour = false;
            if (email != null)
            {
                MD5 md5Hash = MD5.Create();
                string passwordHash = GetMd5Hash(md5Hash, password);
                SqlCommand command = new SqlCommand("SELECT Id, Nom, Prenom FROM Utilisateur where Email=@email and Password = @password", Connection.Instance);
                command.Parameters.Add(new SqlParameter("@email", email));
                command.Parameters.Add(new SqlParameter("@password", passwordHash));
                Connection.Instance.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    u.Id = reader.GetInt32(0);
                    u.Nom = reader.GetString(1);
                    u.Prenom = reader.GetString(2);
                    retour = true;
                }
                else
                {
                    retour = false;
                }
                reader.Close();
                command.Dispose();
                Connection.Instance.Close();
            }
            else
            {
                retour = false;
            }

            return retour;
        }



        public void AddUser(Utilisateur u)
        {
            SqlCommand command = new SqlCommand("INSERT INTO Utilisateur(Nom,Prenom,Email,Telephone, Password) OUTPUT INSERTED.ID VALUES(@Nom,@Prenom,@Email,@Telephone,@Password)", Connection.Instance);
            MD5 md5Hash = MD5.Create();
            string passwordHash = GetMd5Hash(md5Hash, u.Password);
            command.Parameters.Add(new SqlParameter("@Nom", u.Nom));
            command.Parameters.Add(new SqlParameter("@Prenom", u.Prenom));
            command.Parameters.Add(new SqlParameter("@Email", u.Email));
            command.Parameters.Add(new SqlParameter("@Telephone", u.Telephone));
            command.Parameters.Add(new SqlParameter("@Password", passwordHash));
            Connection.Instance.Open();
            u.Id = (int)command.ExecuteScalar();
            command.Dispose();
            Connection.Instance.Close();
        }


        public void AddAnnonce(Annonce a)
        {
            SqlCommand command = new SqlCommand("INSERT INTO Annonce(Titre,Prix,Description,IdCategory,IdUser,DateAjout) OUTPUT INSERTED.ID VALUES(@Titre,@Prix,@Description,@IdCategory,@IdUser,@DateAjout)", Connection.Instance);
            command.Parameters.Add(new SqlParameter("@Titre", a.Titre));
            command.Parameters.Add(new SqlParameter("@Prix", a.Prix));
            command.Parameters.Add(new SqlParameter("@Description", a.Description));
            command.Parameters.Add(new SqlParameter("@IdCategory", a.IdCategory));
            command.Parameters.Add(new SqlParameter("@IdUser", a.IdUser));
            command.Parameters.Add(new SqlParameter("@DateAjout", a.DateAjout));
            Connection.Instance.Open();
            a.Id = (int)command.ExecuteScalar();
            command.Dispose();
            foreach(ImageAnnonce img in a.Images)
            {
                command = new SqlCommand("INSERT INTO ImageAnnonce (Url,IdAnnonce) values(@Url,@IdAnnonce)", Connection.Instance);
                command.Parameters.Add(new SqlParameter("@Url", img.Url));
                command.Parameters.Add(new SqlParameter("@IdAnnonce", a.Id));
                command.ExecuteNonQuery();
                command.Dispose();
            }
            Connection.Instance.Close();
        }

        public List<Annonce> GetAnnonces(int? idCategory)
        {
            List<Annonce> liste = new List<Annonce>();
            SqlCommand command;
            if(idCategory == null)
            {
                command = new SqlCommand("SELECT Id, Titre, Prix, Description, IdCategory, IdUser, DateAjout From Annonce",Connection.Instance);
            }
            else
            {
                command = new SqlCommand("SELECT Id, Titre, Prix, Description, IdCategory, IdUser, DateAjout From Annonce WHERE IdCategory = @idcategory", Connection.Instance);
                command.Parameters.Add(new SqlParameter("@idcategory", idCategory));
            }
            Connection.Instance.Open();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Annonce a = new Annonce { Id = reader.GetInt32(0), Titre = reader.GetString(1), Description = reader.GetString(3), IdCategory = reader.GetInt32(4), Prix = reader.GetDecimal(2), IdUser = reader.GetInt32(5), DateAjout = reader.GetDateTime(6) };
                liste.Add(a);
            }
            
            reader.Close();
            command.Dispose();
            for(int i=0; i < liste.Count; i++)
            {
                command = new SqlCommand("SELECT Id, Url from ImageAnnonce WHERE idAnnonce = @idAnnonce", Connection.Instance);
                command.Parameters.Add(new SqlParameter("@idAnnonce", liste[i].Id));
                reader = command.ExecuteReader();
                while(reader.Read())
                {
                    liste[i].Images.Add(new ImageAnnonce { Id = reader.GetInt32(0), Url = reader.GetString(1) });
                }
                reader.Close();
                command.Dispose();
            }
            Connection.Instance.Close();
            return liste;
        }
    }
}
