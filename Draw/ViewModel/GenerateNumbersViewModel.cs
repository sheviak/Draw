using Draw.Command;
using System.Collections.Generic;
using System.Windows.Controls;
using Draw.Service;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Draw.Entities;
using Draw.ImageProcessing;

namespace Draw.ViewModel
{
    public class GenerateNumbersViewModel : ViewModelBase
    {
        private List<Canvas> _list = new List<Canvas>();
        private NumberService _service = new NumberService();
        private StorageProcessor _storage = new StorageProcessor();

        // для задания количества каждой цифры
        private SettingsScenario _SettingsScenario = new SettingsScenario();
        public SettingsScenario SettingsScenario { get => _SettingsScenario; set { _SettingsScenario = value; OnPropertyChanged(); } }

        // для view // прогресс на форме
        private ProgressViewModel _progressViewModel = new ProgressViewModel();
        public ProgressViewModel ProgressViewModel { get => _progressViewModel; set { _progressViewModel = value; OnPropertyChanged(); } }

        // заполнить combobox для выбора количества цифр
        public List<int> CountData { get; set; } = Enumerable.Range(0, 11).ToList();

        public DelegateCommand<IEnumerable<Canvas>> Create { get; set; }

        public GenerateNumbersViewModel()
        {
            // получаем все Canvas с вьюхи с типизируем
            Create = new DelegateCommand<IEnumerable<Canvas>>((canvas) =>
            {
                _list.Clear();

                ProgressViewModel.Load = true;

                foreach (var item in canvas as IEnumerable<Canvas>)
                    _list.Add(item);

                _service.CreateThreads(_list, SettingsScenario, ProgressViewModel);
            });
        }

        public ICommand LoadScenario => new RelayCommand(() => SettingsScenario = _storage.LoadScenario());
        public ICommand SaveScenario => new RelayCommand(() => _storage.SaveScenario(SettingsScenario));

        public ICommand ClearAll => new DelegateCommand<IEnumerable<Canvas>>((canvas) => 
        {
            foreach (var item in canvas as IEnumerable<Canvas>)
                item.Children.Clear();
        });
    }
}