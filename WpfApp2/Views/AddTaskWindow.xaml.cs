using System.Windows;
using System.Windows.Controls;
using WpfApp2.ViewsModel;
using System.Collections.Generic;
using System.Linq;

namespace WpfApp2.Views
{
    public partial class AddTaskWindow : Window
    {
        private readonly AddTaskViewModel _viewModel;
        private readonly DateTime _selectedDate;


        public AddTaskWindow(DateTime selectedDate)
        {
            InitializeComponent();
            _viewModel = new AddTaskViewModel();
            DataContext = _viewModel;

           
            DatePicker.SelectedDate = selectedDate;
            _viewModel.Date = selectedDate;

            HourComboBox.ItemsSource = Enumerable.Range(0, 24).Select(i => i.ToString("D2"));
            MinuteComboBox.ItemsSource = Enumerable.Range(0, 60).Select(i => i.ToString("D2"));
            HourComboBox.SelectedIndex = 12;
            MinuteComboBox.SelectedIndex = 0;

            MyComboBox.ItemsSource = new List<string> { "Высокий", "Средний", "Низкий" };
            MyComboBox2.ItemsSource = new List<string> { "Работа", "Учеба", "Личное", "Другое" };
            CancelButton.Command = _viewModel.CancelCommand;
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        public AddTaskWindow()
        {
            InitializeComponent();
            _viewModel = new AddTaskViewModel();
            DataContext = _viewModel;

            HourComboBox.ItemsSource = Enumerable.Range(0, 24).Select(i => i.ToString("D2"));
            MinuteComboBox.ItemsSource = Enumerable.Range(0, 60).Select(i => i.ToString("D2"));
            HourComboBox.SelectedIndex = 12;
            MinuteComboBox.SelectedIndex = 0;

            CancelButton.Command = _viewModel.CancelCommand;

            MyComboBox.ItemsSource = new List<string> { "Высокий", "Средний", "Низкий" };
            MyComboBox2.ItemsSource = new List<string> { "Работа", "Учеба", "Личное", "Другое" };

            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AddTaskViewModel.Title) && string.IsNullOrEmpty(_viewModel.Title))
            {
                Close();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, введите заголовок заметки", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(MyComboBox.Text))
            {
                MessageBox.Show("Пожалуйста, выберите уровень приоритета", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(MyComboBox2.Text))
            {
                MessageBox.Show("Пожалуйста, выберите вид задачи", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            _viewModel.Title = TitleTextBox.Text;
            _viewModel.Description = DescriptionTextBox.Text;
            _viewModel.Category = MyComboBox.Text;
            _viewModel.Priority = MyComboBox2.Text;
            var date = DatePicker.SelectedDate ?? System.DateTime.Now;
            int hour = HourComboBox.SelectedIndex >= 0 ? HourComboBox.SelectedIndex : 0;
            int minute = MinuteComboBox.SelectedIndex >= 0 ? MinuteComboBox.SelectedIndex : 0;
            _viewModel.Date = new System.DateTime(date.Year, date.Month, date.Day, hour, minute, 0);
            _viewModel.SaveCommand.Execute(null);
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
} 
