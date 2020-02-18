using System.Windows.Controls;
using Draw.Command;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using System.Windows.Media.Imaging;
using Draw.ImageProcessing;
using System.Collections.Generic;
using System.Linq;
using Draw.Service;
using Draw.AppContext;
using Draw.Entities;
using System;
using System.Diagnostics;

namespace Draw.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private const int SIZE = 28;
        private const string link = "https://mssg.me/sheviak.k";
        private const string strFilePath = @"temp/single-number.csv";
        private SingletonContext context = SingletonContext.GetInstance;

        private BitmapSource _bitmapSource;
        public BitmapSource BitmapSource
        {
            get { return this._bitmapSource; }
            set
            {
                this._bitmapSource = value;
                OnPropertyChanged();
            }
        }

        private string _aboutNumber;
        public string AboutNumber
        {
            get { return _aboutNumber; }
            set
            {
                _aboutNumber = value;
                OnPropertyChanged();
            }
        }

        private int _rotate = 0;
        public int Rotate
        {
            get { return _rotate; }
            set
            {
                _rotate = value;
                OnPropertyChanged();
            }
        }

        private int _shiftX = 0;
        public int ShiftX
        {
            get { return _shiftX; }
            set
            {
                _shiftX = value;
                OnPropertyChanged();
            }
        }

        private int _shiftY = 0;
        public int ShiftY
        {
            get { return _shiftY; }
            set
            {
                _shiftY = value;
                OnPropertyChanged();
            }
        }

        private double _scaleY = 1;
        public double ScaleY
        {
            get { return _scaleY; }
            set
            {
                _scaleY = value;
                OnPropertyChanged();
            }
        }

        private double _scaleX = 1;
        public double ScaleX
        {
            get { return _scaleX; }
            set
            {
                _scaleX = value;
                OnPropertyChanged();
            }
        }

        private int _skewX = 0;
        public int SkewX
        {
            get { return _skewX; }
            set
            {
                _skewX = value;
                OnPropertyChanged();
            }
        }

        private int _skewY = 0;
        public int SkewY
        {
            get { return _skewY; }
            set
            {
                _skewY = value;
                OnPropertyChanged();
            }
        }

        private int _selectedNumber = -1;
        public int SelectedNumber
        {
            get { return _selectedNumber; }
            set
            {
                _selectedNumber = value;
                OnPropertyChanged();
            }
        }

        public List<int> NumberList { get; set; } = Enumerable.Range(0, 10).ToList();

        private StorageProcessor storage = new StorageProcessor();

        public MainWindowViewModel()
        {
            ClearCommand = new DelegateCommand<Canvas>(canvas => ClearFields((Canvas)canvas));
            CreateCommand = new DelegateCommand<Canvas>(canvas => CreateImage((Canvas)canvas));
        }
       
        public DelegateCommand<Canvas> ClearCommand { get; set; }
        public DelegateCommand<Canvas> CreateCommand { get; set; }

        public ICommand SaveDbCommand => new RelayCommand(() =>
        {
            if (BitmapSource == null) return;

            var number = SelectedNumber.ToString();
            var line = storage.ConvertToCsv(BitmapSource, number, SIZE);

            var model = new Number
            {
                Num = number,
                Value = line,
                NumProperties = new NumProperties
                {
                    Rotate = this.Rotate,
                    ScaleX = this.ScaleX,
                    ScaleY = this.ScaleY,
                    ShiftX = this.ShiftX,
                    ShiftY = this.ShiftY,
                    SkewX = this.SkewX,
                    SkewY = this.SkewY
                }
            };

            SingletonContext.Context.Numbers.Add(model);
            SingletonContext.Context.SaveChanges();
        });

        public ICommand SavePngCommand => new RelayCommand(() => storage.SaveImage(BitmapSource, SelectedNumber.ToString()));
        public ICommand SaveCsvCommand => new RelayCommand(() =>
        {
            var str = storage.ConvertToCsv(BitmapSource, SelectedNumber.ToString(), SIZE);
            storage.SaveToCsvFile(strFilePath, str);
        });

        public ICommand LoadCommand => new RelayCommand(() =>
        {
            var temp = storage.LoadFromCsv(SIZE);
            BitmapSource = temp.Item1;
            AboutNumber = temp.Item2;
        });

        public ICommand GenerateWindow => new RelayCommand(() =>
        {
            var generate = new GenerateNumbers();
            generate.ShowDialog();
        });

        public ICommand CloseProgram => new RelayCommand(() =>
        {
            Environment.Exit(0);
        });

        public ICommand OpenWebSite => new RelayCommand(() => Process.Start(link));

        private void CreateImage(Canvas canvas)
        {
            var service = new NumberService();
            var img = service
                .GetBitmapNumber(
                    canvas: canvas,
                    canvasWidth: (int)canvas.Width,
                    canvasHeight: (int)canvas.Height,
                    sizeImg: SIZE,
                    rotate: Rotate,
                    scaleX: ScaleX,
                    scaleY: ScaleY,
                    shiftX: ShiftX,
                    shiftY: ShiftY,
                    skewX: SkewX,
                    skewY: SkewY
                );

            BitmapSource = img;
        }

        private void ClearFields(Canvas canvas)
        {
            canvas.Children.Clear();
            SelectedNumber = -1;
            BitmapSource = null;
            Rotate = ShiftX = ShiftY = 0;
            AboutNumber = "";
        }
    }
}