using System.Windows.Controls;
using Draw.Command;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using System.Windows.Media.Imaging;
using Draw.ImageProcessing;
using Draw.Extensions;
using System.Collections.Generic;

namespace Draw.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private const int SIZE = 28;

        private BitmapSource _bitmapSource;
        public BitmapSource BitmapSource
        {
            get { return this._bitmapSource; }
            set
            {
                this._bitmapSource = value;
                OnPropertyChanged("BitmapSource");
            }
        }

        private string _aboutNumber;
        public string AboutNumber
        {
            get { return _aboutNumber; }
            set
            {
                _aboutNumber = value;
                OnPropertyChanged("AboutNumber");
            }
        }

        private int _rotate;
        public int Rotate
        {
            get { return _rotate; }
            set
            {
                _rotate = value;
                OnPropertyChanged("Rotate");
            }
        }

        private int _shiftX;
        public int ShiftX
        {
            get { return _shiftX; }
            set
            {
                _shiftX = value;
                OnPropertyChanged("ShiftX");
            }
        }

        private int _shiftY;
        public int ShiftY
        {
            get { return _shiftY; }
            set
            {
                _shiftY = value;
                OnPropertyChanged("ShiftY");
            }
        }

        private double _scaleY = 1;
        public double ScaleY
        {
            get { return _scaleY; }
            set
            {
                _scaleY = value;
                OnPropertyChanged("ScaleY");
            }
        }

        private double _scaleX = 1;
        public double ScaleX
        {
            get { return _scaleX; }
            set
            {
                _scaleX = value;
                OnPropertyChanged("ScaleX");
            }
        }

        private int _number = -1;
        public int Number
        {
            get { return _number; }
            set
            {
                _number = value;
                OnPropertyChanged("Number");
            }
        }

        public List<int> NumberList { get; set; } = new List<int>();

        private StorageProcessor storage = new StorageProcessor();

        public DelegateCommand<Canvas> ClearCommand { get; set; }
        public DelegateCommand<Canvas> CreateCommand { get; set; }
        public ICommand SavePngCommand => new RelayCommand(() => storage.SaveImage(BitmapSource, AboutNumber));
        public ICommand SaveCsvCommand => new RelayCommand(() => storage.SaveCsv(BitmapSource, AboutNumber, SIZE));
        public ICommand LoadCommand => new RelayCommand(() =>
        {
            var temp = storage.LoadFromCsv(SIZE);
            BitmapSource = temp.Item1;
            AboutNumber = temp.Item2;
        });

        public MainWindowViewModel()
        {
            ClearCommand = new DelegateCommand<Canvas>(canvas => ClearFields((Canvas)canvas));
            CreateCommand = new DelegateCommand<Canvas>(canvas => CreateImage((Canvas)canvas));

            Initialize();
        }

        private void Initialize()
        {
            for (int i = 0; i <= 9; i++)
                NumberList.Add(i);
            //TODO: сделать выпадающий список на масштабирование и сдвиги
            // сделать правльное соотношение или порядок выполнения функций

        }

        private void CreateImage(Canvas canvas)
        {
            //TODO: доделать поворот - сделать более красивый код
            canvas.ChangeCanvas(45);
            canvas.UpdateLayout();
            var img = canvas
                .CreateImage()
                .ConvertToBlackWhite()
                .ScaleImage(SIZE, SIZE)
                .BitmapToBitmapSource()
                //.RotateImageTransform(Rotate)
                .ScaleImageTransform(ScaleX, ScaleY)
                .TranslateImageTransform(ShiftX, ShiftY);

            BitmapSource = img;
        }

        private void ClearFields(Canvas canvas)
        {
            canvas.Children.Clear();
            BitmapSource = null;
            Rotate = ShiftX = ShiftY = 0;
            AboutNumber = "";
        }
    }
}