using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.Mvc;
using W05L03.Models;
using static System.Net.WebRequestMethods;

namespace W05L03.Controllers
{
    public class ScarpeController : Controller
    {
        // GET: Scarpe
        public ActionResult Index()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ScarpeCo"].ConnectionString.ToString();
            SqlConnection conn = new SqlConnection(connectionString);
            List<Scarpe> scarpeList = new List<Scarpe>();

            try
            {
                conn.Open();
                string query = "SELECT * FROM Scarpe";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Scarpe scarpa = new Scarpe()
                    {
                        IdProdotto = Convert.ToInt32(reader["IdProdotto"]),
                        NomeProdotto = reader["NomeProdotto"].ToString(),
                        Prezzo = Convert.ToDecimal(reader["Prezzo"]),
                        DescrizioneDettagliata = reader["DescrizioneDettagliata"].ToString(),
                        ImmagineCopertina = reader["ImmagineCopertina"].ToString(),
                        AltreImg1 = reader["AltreImg1"].ToString(),
                        AltreImg2 = reader["AltreImg2"].ToString(),
                        Disponibile = Convert.ToBoolean(reader["Disponibile"])
                    };
                    scarpeList.Add(scarpa);
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Si è verificato un errore durante il recupero dei dati delle scarpe: " + ex.Message;
            }
            finally
            {
                conn.Close();
            }

            return View(scarpeList);
        }

        // GET: Scarpe/Create
        public ActionResult CreateScarpe()
        {
            return View();
        }

        // POST: Scarpe/Create
        [HttpPost]
        public ActionResult CreateScarpe(Scarpe scarpa, HttpPostedFileBase ImmagineCopertina, HttpPostedFileBase AltreImg1, HttpPostedFileBase AltreImg2)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ScarpeCo"].ConnectionString.ToString();
            SqlConnection conn = new SqlConnection(connectionString);

            try
            {
                conn.Open();

                // Salvataggio delle immagini sul server
                string immagineCopertinaPath = "";
                if (ImmagineCopertina.ContentLength > 0)
                {
                    string _File = Path.GetFileName(ImmagineCopertina.FileName);
                    string _path = Path.Combine(Server.MapPath("~/UploadedFiles"), _File);
                    ImmagineCopertina.SaveAs(_path);
                    immagineCopertinaPath = "~/UploadedFiles/" + _File;
                }

                string altreImg1path = "";
                if (AltreImg1.ContentLength > 0)
                {
                    string _File = Path.GetFileName(AltreImg1.FileName);
                    string _path = Path.Combine(Server.MapPath("~/UploadedFiles"), _File);
                    AltreImg1.SaveAs(_path);
                    altreImg1path = "~/UploadedFiles/" + _File;
                }

                string altreImg2path = "";
                if (AltreImg2.ContentLength > 0)
                {
                    var _File = Path.GetFileName(AltreImg2.FileName);
                    var _path = Path.Combine(Server.MapPath("~/UploadedFiles"), _File);
                    AltreImg2.SaveAs(_path);
                    altreImg2path = "~/UploadedFiles/" + _File;
                }

                // Inserimento dei dati nel database
                string query = "INSERT INTO Scarpe (NomeProdotto, Prezzo, DescrizioneDettagliata, ImmagineCopertina, AltreImg1, AltreImg2, Disponibile) " +
                               "VALUES (@NomeProdotto, @Prezzo, @DescrizioneDettagliata, @ImmagineCopertina, @AltreImg1, @AltreImg2, @Disponibile)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@NomeProdotto", scarpa.NomeProdotto);
                cmd.Parameters.AddWithValue("@Prezzo", scarpa.Prezzo);
                cmd.Parameters.AddWithValue("@DescrizioneDettagliata", scarpa.DescrizioneDettagliata);
                cmd.Parameters.AddWithValue("@ImmagineCopertina", immagineCopertinaPath);
                cmd.Parameters.AddWithValue("@AltreImg1", altreImg1path);
                cmd.Parameters.AddWithValue("@AltreImg2", altreImg2path);
                cmd.Parameters.AddWithValue("@Disponibile", scarpa.Disponibile);

                cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                System.Diagnostics.Debug.WriteLine("Errore nella richiesta SQL");
                ViewBag.Message = "Si è verificato un errore di connessione durante l'inserimento: " + ex.Message;
            }
            finally
            {
                conn.Close();
            }

            return View("CreateScarpe");
        }
    }
}
