﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using W05L03.Models;

namespace W05L03.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ScarpeCo"].ConnectionString.ToString();
            SqlConnection conn = new SqlConnection(connectionString);

            // Creo una lista di Scarpe
            List<Scarpe> scarpeList = new List<Scarpe>();

            try
            {
                conn.Open();
                // Seleziono solo le scarpe con disponibile true da mostrare
                string query = "SELECT * FROM Scarpe WHERE Disponibile = 1";
                SqlCommand command = new SqlCommand(query, conn);
                SqlDataReader reader = command.ExecuteReader();

                // Ciclo while per leggere i dati dal database
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
                        Disponibile = Convert.ToBoolean(reader["Disponibile"]),
                    };
                    scarpeList.Add(scarpa);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Errore:" + ex.Message);
            }
            finally
            {
                conn.Close();
            }

            // Restituisco la vista con la lista di scarpe
            return View(scarpeList);
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}