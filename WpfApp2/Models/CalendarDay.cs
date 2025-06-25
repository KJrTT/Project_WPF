using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WpfApp2.Models
{
    public class CalendarDay
    {
        public int id { get; set; }
        public int DayNumber { get; set; }

        [NotMapped]
        public Brush ForegroundBrush { get; set; } = Brushes.Black;

        [NotMapped]
        public Brush BackgroundBrush { get; set; } = Brushes.White;

        public DateTime Date { get; set; }

        [NotMapped]
        public List<Notes> DayNotes { get; set; } = new List<Notes>();
    }
}
