using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WpfApp2.Models
{
    public class CalendarDay
    {
        public int DayNumber { get; set; }
        public Brush ForegroundBrush { get; set; } = Brushes.Black;
        public Brush BackgroundBrush { get; set; } = Brushes.White;
        public DateTime Date { get; set; }
    }
}
