using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace W05L03.Models
{
    public class Scarpe
    {
        public int IdProdotto { get; set; }
        public string NomeProdotto { get; set; }

        // roba del prezzo per farlo decimale bene:
        [Required(ErrorMessage = "Il prezzo dell'articolo è obbligatorio.")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal Prezzo { get; set; }

        [Display(Name = "Descrizione dettagliata:")]
        public string DescrizioneDettagliata { get; set; }

        [Display(Name = "Immagine vetrina:")]
        public string ImmagineCopertina { get; set; }

        [Display(Name = "Altre immagini:")]
        public string AltreImg1 { get; set; }

        [Display(Name = "Altre immagini:")]
        public string AltreImg2 { get; set; }

        public bool Disponibile { get; set; }

       public Scarpe() { }

      public Scarpe(string nomeprodotto, decimal prezzo, string dd, string mainimg, string img1, string img2, bool disponibile)
        {
            NomeProdotto = nomeprodotto;
            Prezzo = prezzo;
            DescrizioneDettagliata = dd;
            ImmagineCopertina = mainimg;
            AltreImg1 = img1;
            AltreImg2 = img2;
            Disponibile = disponibile;
        }
    }
}
