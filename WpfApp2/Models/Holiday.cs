using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WpfApp2.Models
{
    internal class Holiday
    {
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public Brush BackgroundColor { get; set; } = Brushes.LightPink;
    }
}
