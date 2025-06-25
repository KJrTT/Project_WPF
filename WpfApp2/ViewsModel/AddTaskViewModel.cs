using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;
using WpfApp2.Services;
using WpfApp2.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfApp2.ViewsModel
{
    internal class AddTaskViewModel : INotifyPropertyChanged
    {
        private string _title = string.Empty;
        private string _description = string.Empty;
        private string? _Category = string.Empty;
        private string? _Priority = string.Empty;
        private DateTime _date = DateTime.Now;
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        public string Category
        {
            get => _Category;
            set
            {
                _Category = value;
                OnPropertyChanged();
            }
        }
        public string Priority
        {
            get => _Priority;
            set
            {
                _Priority = value;
                OnPropertyChanged();
            }
        }

        public DateTime Date
        {
            get => _date;
            set
            {
                _date = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public AddTaskViewModel()
        {
            SaveCommand = new RelayCommand(SaveNote);
            CancelCommand = new RelayCommand(Cancel);
        }

        private async void SaveNote()
        {
            if (string.IsNullOrWhiteSpace(Title))
                return;

            var note = new Notes
            {
                Title = Title,
                Description = Description,
                Date = DateTime.SpecifyKind(Date, DateTimeKind.Local).ToUniversalTime(),
                Priority = Priority,
                Category = Category
            };

            using (var db = new DatabaseService())
            {
                await db.AddNoteAsync(note);
            }
            Cancel();
        }

        private void Cancel()
        {
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;

        public void Execute(object? parameter) => _execute();
    }
}
