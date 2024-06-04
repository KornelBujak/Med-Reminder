using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Med_Reminder
{
    public class Lek
    {

        [Required]
        [Column("id_leku")]
        public int id_leku{  get; set; }
        [Required]
        [MaxLength(100)]
        [Column("nazwa_leku")]
        public string NazwaLeku { get; set; }

        [Required]
        [Column("dane_osobowe_id")]
        public int DaneOsoboweId { get; set; }
        public DaneOsobowe DaneOsobowe { get; set; }
        public ICollection<Przypomnienie> Przypomnienia { get; set; }


    }
    }

