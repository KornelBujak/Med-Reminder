using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Med_Reminder
{
    public class DaneOsobowe
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        [Column("adres_email")]
        public string AdresEmail { get; set; }

        [Required]
        [Column("haslo_szyfrowane")]
        public byte[] HasloSzyfrowane { get; set; }

        [Required]
        [MaxLength(50)]
        public string Imie { get; set; }

        [Required]
        [MaxLength(50)]
        public string Nazwisko { get; set; }

        [MaxLength(10)]
        public string Plec { get; set; }

        public int Wiek { get; set; }

        public double Waga { get; set; }

        public DateTime DataUrodzenia { get; set; }

        [MaxLength(20)]
        public string _numer_telefonu_ { get; set; }
        public ICollection<Lek> Leki { get; set; }
        public ICollection<Przypomnienie> Przypomnienia { get; set; }
    }
}


