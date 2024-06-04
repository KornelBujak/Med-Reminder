using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Med_Reminder
{
    public class Przypomnienie
    {

        public int Id { get; set; }
        public int dane_osobowe_id { get; set; }
        public TimeSpan godzina_przypomnienia { get; set; }

        [ForeignKey("dane_osobowe_id")]

        public DateTime? data_początkowa {  get; set; }
        public DateTime? data_końcowa { get; set; }
        public bool _czywyslano { get; set; } 
        public Lek Lek { get; set; }
        public string nazwa_leku { get; set; }
        public DaneOsobowe DaneOsobowe { get; set; }
        public int id_leku { get; set; }

    }
}
