using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2.Models
{
    public class Notes
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string? Priority { get; set; }
        public string? Category { get; set; }
        public string? Description { get; set; }
        [NotMapped]
        public DateTime DateLocal => Date.ToLocalTime();
    }
}
