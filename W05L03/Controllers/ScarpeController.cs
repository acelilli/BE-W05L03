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
        /////////////////////////////////////////
        //METODI PER LA CREAZIONE DELLE SCARPE
        ////////////////////////////////////////

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

                // Salvataggio delle immagini sul server -> metodo separato SaveImage sotto
                string immagineCopertinaPath = SaveImage(ImmagineCopertina);
                string altreImg1path = SaveImage(AltreImg1);
                string altreImg2path = SaveImage(AltreImg2);


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

        // Metodo per salvare un'immagine sul server e restituire il percorso del file per non riscriverlo 3 volte
        private string SaveImage(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                string fileName = Path.GetFileName(file.FileName);
                string filePath = Path.Combine(Server.MapPath("~/UploadedFiles"), fileName);
                file.SaveAs(filePath);
                return "~/UploadedFiles/" + fileName;
            }
            return null;
        }
        /////////////////////////////////////////
        //METODI PER L'EDIT DELLE SCARPE
        ////////////////////////////////////////
        // GET
        public ActionResult EditScarpe(int id)
        {
            Scarpe scarpa = null;
            string connectionString = ConfigurationManager.ConnectionStrings["ScarpeCo"].ConnectionString.ToString();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Scarpe WHERE IdProdotto = @IdProdotto";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@IdProdotto", id);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    scarpa = new Scarpe()
                    {
                        IdProdotto = (int)reader["IdProdotto"],
                        NomeProdotto = reader["NomeProdotto"].ToString(),
                        Prezzo = Convert.ToDecimal(reader["Prezzo"]),
                        DescrizioneDettagliata = reader["DescrizioneDettagliata"].ToString(),
                        ImmagineCopertina = reader["ImmagineCopertina"].ToString(),
                        AltreImg1 = reader["AltreImg1"].ToString(),
                        AltreImg2 = reader["AltreImg2"].ToString(),
                        Disponibile = Convert.ToBoolean(reader["Disponibile"])
                    };
                }

                reader.Close();
            }

            if (scarpa == null)
            {
                return HttpNotFound(); // Restituisci 404 se la scarpa non è trovata
            }

            return View(scarpa);
        }

        // POST
        [HttpPost]
        public ActionResult EditScarpe(int id, Scarpe scarpa)
        {
            if (ModelState.IsValid)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ScarpeCo"].ConnectionString.ToString();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sqlQuery = "UPDATE Scarpe SET NomeProdotto = @NomeProdotto, Prezzo = @Prezzo, DescrizioneDettagliata = @DescrizioneDettagliata, ImmagineCopertina = @ImmagineCopertina, AltreImg1 = @AltreImg1, AltreImg2 = @AltreImg2, Disponibile = @Disponibile WHERE IdProdotto = @IdProdotto";

                    SqlCommand cmd = new SqlCommand(sqlQuery, conn);
                    cmd.Parameters.AddWithValue("@IdProdotto", scarpa.IdProdotto);
                    cmd.Parameters.AddWithValue("@NomeProdotto", scarpa.NomeProdotto);
                    cmd.Parameters.AddWithValue("@Prezzo", scarpa.Prezzo);
                    cmd.Parameters.AddWithValue("@DescrizioneDettagliata", scarpa.DescrizioneDettagliata);
                    cmd.Parameters.AddWithValue("@ImmagineCopertina", scarpa.ImmagineCopertina);
                    cmd.Parameters.AddWithValue("@AltreImg1", scarpa.AltreImg1);
                    cmd.Parameters.AddWithValue("@AltreImg2", scarpa.AltreImg2);
                    cmd.Parameters.AddWithValue("@Disponibile", scarpa.Disponibile);

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Si è verificato un errore durante l'aggiornamento delle scarpe: " + ex.Message);
                    }
                }
            }
            return View(scarpa);
        }
        /////////////////////////////////////////
        //METODI PER I DETTAGLI DELLE SCARPE
        ////////////////////////////////////////
        // GET: Scarpe/Details/5
        public ActionResult DettagliScarpa(int id)
        {
            Scarpe scarpa = null;
            string connectionString = ConfigurationManager.ConnectionStrings["ScarpeCo"].ConnectionString.ToString();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Scarpe WHERE IdProdotto = @IdProdotto";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@IdProdotto", id);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    scarpa = new Scarpe()
                    {
                        IdProdotto = (int)reader["IdProdotto"],
                        NomeProdotto = reader["NomeProdotto"].ToString(),
                        Prezzo = Convert.ToDecimal(reader["Prezzo"]),
                        DescrizioneDettagliata = reader["DescrizioneDettagliata"].ToString(),
                        ImmagineCopertina = reader["ImmagineCopertina"].ToString(),
                        AltreImg1 = reader["AltreImg1"].ToString(),
                        AltreImg2 = reader["AltreImg2"].ToString(),
                        Disponibile = Convert.ToBoolean(reader["Disponibile"])
                    };
                }

                reader.Close();
            }

            if (scarpa == null)
            {
                return HttpNotFound(); // Restituisci 404 se la scarpa non è trovata
            }

            return View(scarpa);
        }
    }
}
