using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp2.Models;
using WpfApp2.Services;
using Microsoft.EntityFrameworkCore;
using WpfApp2.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WpfApp2.ViewsModel;

namespace WpfApp2.Views
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly DatabaseService _db = new();
        private DateTime _currentDate = DateTime.Now;
        private ObservableCollection<CalendarDay> _calendarDays = new();
        private ObservableCollection<Notes> _notes = new();

        public ObservableCollection<CalendarDay> CalendarDays
        {
            get => _calendarDays;
            set
            {
                _calendarDays = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Notes> Notes
        {
            get => _notes;
            set
            {
                _notes = value;
                OnPropertyChanged();
            }
        }

        public string MonthYearText => _currentDate.ToString("MMMM yyyy");

        public MainWindow()
        {
            InitializeComponent();
            LoadCalendar();
            LoadNotes();
        }

        private async void LoadNotes()
        {
            try
            {
                var notes = await _db.GetAllNotesAsync();
                Notes.Clear();
                foreach (var note in notes.OrderByDescending(n => n.Date))
                {
                    Notes.Add(note);
                }
                NotesListBox.ItemsSource = Notes;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заметок: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadCalendar()
        {
            CalendarDays.Clear();
            
            var firstDayOfMonth = new DateTime(_currentDate.Year, _currentDate.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            var firstDayOfWeek = (int)firstDayOfMonth.DayOfWeek;
            
            for (int i = 0; i < firstDayOfWeek; i++)
            {
                CalendarDays.Add(new CalendarDay { DayNumber = 0, ForegroundBrush = Brushes.LightGray });
            }
            
            for (int day = 1; day <= lastDayOfMonth.Day; day++)
            {
                var date = new DateTime(_currentDate.Year, _currentDate.Month, day);
                var calendarDay = new CalendarDay
                {
                    DayNumber = day,
                    Date = date,
                    ForegroundBrush = date.DayOfWeek == DayOfWeek.Sunday || date.DayOfWeek == DayOfWeek.Saturday 
                        ? Brushes.Red 
                        : Brushes.Black,
                    BackgroundBrush = date.Date == DateTime.Today ? Brushes.LightBlue : Brushes.White
                };
                CalendarDays.Add(calendarDay);
            }
            
            var remainingCells = 42 - CalendarDays.Count; 
            for (int i = 0; i < remainingCells; i++)
            {
                CalendarDays.Add(new CalendarDay { DayNumber = 0, ForegroundBrush = Brushes.LightGray });
            }
            
            CalendarDaysControl.ItemsSource = CalendarDays;
            OnPropertyChanged(nameof(MonthYearText));
        }

        private void AddNoteButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddTaskWindow();
            window.ShowDialog();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _currentDate = _currentDate.AddMonths(1);
            LoadCalendar();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            _currentDate = _currentDate.AddMonths(-1);
            LoadCalendar();
        }
        
        
    }
}