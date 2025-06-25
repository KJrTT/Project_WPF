using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using WpfApp2.Models;

public class CalendarDay : INotifyPropertyChanged
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

    [NotMapped]
    public string? HolidayName { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}