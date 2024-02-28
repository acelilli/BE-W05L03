using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using W05L03.Models;

namespace W05L03.Controllers
{
    public class ScarpeController : Controller
    {
        // GET: Scarpe
        public ActionResult Index()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DipendentiPagamenti"].ConnectionString.ToString();
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
                    Scarpe scarpa = new Scarpe
                    {
                        IdProdotto = Convert.ToInt32(reader["IdProdotto"]),
                        NomeProdotto = reader["NomeProdotto"].ToString(),
                        Prezzo = Convert.ToDecimal(reader["Prezzo"]),
                        DescrizioneDettagliata = reader["DescrizioneDettagliata"].ToString(),
                        ImmagineCopertina = reader["ImmagineCopertina"].ToString(),
                        AltreImg1 = reader["AltreImg1"].ToString(),
                        AltreImg2 = reader["AltreImg2"].ToString()
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
        //Create
        //Edit & Soft Delete
    }
}