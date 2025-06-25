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
using System.Collections.Generic;
using System.Windows.Threading;

namespace WpfApp2.Views
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly DatabaseService _db = new();
        private DateTime _currentDate = DateTime.Now;
        private ObservableCollection<CalendarDay> _calendarDays = new();
        private ObservableCollection<Notes> _notes = new();
        int year;
        int month;
        string monthName;
        private Dictionary<int, string> monthsDictionary = new Dictionary<int, string>
        {
            {1, "Январь"},
            {2, "Февраль"},
            {3, "Март"},
            {4, "Апрель"},
            {5, "Май"},
            {6, "Июнь"},
            {7, "Июль"},
            {8, "Август"},
            {9, "Сентябрь"},
            {10, "Октябрь"},
            {11, "Ноябрь"},
            {12, "Декабрь"}
        };
        private System.Windows.Threading.DispatcherTimer _notificationTimer = new();
        private HashSet<int> _notifiedNotes = new(); // Для предотвращения повторных уведомлений

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
            year = _currentDate.Year;
            month = _currentDate.Month;
            monthName = monthsDictionary[month].ToString();
            MonthYearTextBlock.Text = $"{monthName} {year}";

            _notificationTimer.Interval = TimeSpan.FromSeconds(3); 
            _notificationTimer.Tick += NotificationTimer_Tick;
            _notificationTimer.Start();
        }

        private async void LoadNotes()
        {
            try
            {
                var notes = await _db.GetAllNotesAsync();
                Notes.Clear();

                foreach (var day in CalendarDays.Where(d => d.DayNumber > 0))
                {
                    day.DayNotes = notes.Where(n => n.Date.Date == day.Date.Date).ToList();
                }

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
                var holiday = _holidays.FirstOrDefault(h => h.Date.Date == date.Date);

                var calendarDay = new CalendarDay
                {
                    DayNumber = day,
                    Date = date,
                    ForegroundBrush = date.DayOfWeek == DayOfWeek.Sunday || date.DayOfWeek == DayOfWeek.Saturday
                        ? Brushes.Red
                        : Brushes.Black,
                    BackgroundBrush = holiday != null
                        ? holiday.BackgroundColor
                        : (date.Date == DateTime.Today ? Brushes.LightBlue : Brushes.White),
                    HolidayName = holiday?.Name
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
            LoadNotes(); 
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _currentDate = _currentDate.AddMonths(1);
            if (_currentDate.Year != year)
            {
                UpdateHolidaysForYear(_currentDate.Year);
            }
            year = _currentDate.Year;
            month = _currentDate.Month;
            monthName = monthsDictionary[month].ToString();

            MonthYearTextBlock.Text = $"{monthName} {year}";
            LoadCalendar();
            LoadNotes();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            _currentDate = _currentDate.AddMonths(-1);
            if (_currentDate.Year != year)
            {
                UpdateHolidaysForYear(_currentDate.Year);
            }
            year = _currentDate.Year;
            month = _currentDate.Month;
            monthName = monthsDictionary[month].ToString();

            MonthYearTextBlock.Text = $"{monthName} {year}";
            LoadCalendar();
            LoadNotes();
        }

        private void CalendarDay_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is CalendarDay day && day.DayNumber > 0)
            {
                var window = new AddTaskWindow(day.Date);
                window.ShowDialog();
                LoadNotes(); 
            }
        }

        private void NotificationTimer_Tick(object? sender, EventArgs e)
        {
            var now = DateTime.Now;
            foreach (var note in Notes)
            {
                var noteLocalTime = note.Date.Kind == DateTimeKind.Utc ? note.Date.ToLocalTime() : note.Date;
                var diff = (noteLocalTime - now).TotalSeconds;
                if (!_notifiedNotes.Contains(note.Id) && diff < 30 && diff >= 0)
                {
                    MessageBox.Show($"Напоминание: {note.Title}\n{note.Description}", "Задача", MessageBoxButton.OK, MessageBoxImage.Information);
                    _notifiedNotes.Add(note.Id);
                }
            }
        }

        private async void DeleteNoteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int noteId)
            {
                var result = MessageBox.Show("Удалить эту задачу?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    using (var db = new DatabaseService())
                    {
                        await db.DeleteNoteAsync(noteId);
                    }
                    LoadNotes();
                }
            }
        }
        private readonly List<Holiday> _holidays = new List<Holiday> { };

        private void UpdateHolidaysForYear(int year)
        {
            _holidays.Clear();
            _holidays.AddRange(new List<Holiday>
            {
                new Holiday { Date = new DateTime(year, 1, 1), Name = "Новый год" },
                new Holiday { Date = new DateTime(year, 1, 7), Name = "Рождество" },
                new Holiday { Date = new DateTime(year, 2, 23), Name = "День защитника Отечества" },
                new Holiday { Date = new DateTime(year, 3, 8), Name = "Международный женский день" },
                new Holiday { Date = new DateTime(year, 5, 1), Name = "Праздник весны и труда" },
                new Holiday { Date = new DateTime(year, 5, 9), Name = "День Победы" },
                new Holiday { Date = new DateTime(year, 6, 12), Name = "День России" },
                new Holiday { Date = new DateTime(year, 11, 4), Name = "День народного единства" }
            });
        }
    }
}
