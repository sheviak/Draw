using System.Windows.Controls;
using Draw.Command;
using System.Windows.Forms;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Media.Imaging;

namespace Draw.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private const int SIZE = 28;
        private ImageConverters.ImageConverter imageConverters;

        private Bitmap _bitmap;
        public Bitmap Bitmap
        {
            get { return this._bitmap; }
            set
            {
                this._bitmap = value;
                OnPropertyChanged("Bitmap");
            }
        }

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

        private string _aboutNumber = "1, поворот на 45 градусов, сдвиг по оси Х";
        public string AboutNumber
        {
            get { return _aboutNumber; }
            set
            {
                _aboutNumber = value;
                OnPropertyChanged("AboutNumber");
            }
        }

        public DelegateCommand<Canvas> ClearCommand { get; set; }
        public DelegateCommand<Canvas> CreateCommand { get; set; }
        public ICommand SavePngCommand => new RelayCommand(() => SaveImage(Bitmap));
        public ICommand SaveCsvCommand => new RelayCommand(() => SaveCSV(Bitmap));
        public ICommand LoadCommand => new RelayCommand(() => LoadFromCsv());

        public MainWindowViewModel()
        {
            imageConverters = new ImageConverters.ImageConverter();

            ClearCommand = new DelegateCommand<Canvas>(canvas => ClearFields((Canvas)canvas));
            CreateCommand = new DelegateCommand<Canvas>(canvas =>
            {
                Bitmap = imageConverters.CreateImage((Canvas) canvas, SIZE);
                //BitmapSource t = (BitmapSource) System.Drawing.BitmapSource.FromFile(@" C:\Users\Kyrylo_Sheviak\Desktop\9.png", true);
                BitmapSource = imageConverters.BitmapToBitmapSource(Bitmap);
            });
        }

        private void ClearFields(Canvas canvas)
        {
            ((Canvas) canvas).Children.Clear();
            BitmapSource = null;
        }

        private void LoadFromCsv()
        {
            var openDialog = new OpenFileDialog
            {
                FileName = "Открыть файл", Filter = "CSV reader|* .csv"
            };

            if (openDialog.ShowDialog() != DialogResult.OK) return;

            using (var reader = new StreamReader(openDialog.FileName))
            {
                int index = 0;
                string line = "";
                while (!reader.EndOfStream)
                {
                    if (index == 0)
                        AboutNumber = reader.ReadLine();
                    if (index == 1)
                        line = reader.ReadLine();
                    index++;
                }
                var temp = imageConverters.CSVtoBitmap(line, SIZE);
                BitmapSource = imageConverters.BitmapToBitmapSource(temp);
            }
        }

        private void SaveCSV(Bitmap image)
        {
            var sbOutput = new StringBuilder();
            var strFilePath = @"testfile.csv";
            var strSeperator = ",";
            sbOutput.AppendLine(AboutNumber);
            for (var i = 0; i < SIZE; i++)
            {
                for (var j = 0; j < SIZE; j++)
                {
                    var pixel = image.GetPixel(i, j);
                    var lum = (int)imageConverters.GetLuminance(pixel.R, pixel.G, pixel.B);
                    sbOutput.AppendFormat(string.Concat(lum, (i == SIZE-1 && j == SIZE-1) ? "" : strSeperator));
                }
            }
            // Create and write the csv file
            File.WriteAllText(strFilePath, sbOutput.ToString(), Encoding.UTF8);
        }

        private void SaveImage(Bitmap image)
        {
            var savedialog = new SaveFileDialog
            {
                Title = "Сохранить файла",
                Filter = "Graphics Redactor|* .png",
                FileName = AboutNumber
            };

            if (savedialog.ShowDialog() != DialogResult.OK) return;

            var mass = new MemoryStream();
            var files = new FileStream(savedialog.FileName, FileMode.Create, FileAccess.ReadWrite);
            image.Save(mass, System.Drawing.Imaging.ImageFormat.Png);

            byte[] matric = mass.ToArray();
            files.Write(matric, 0, matric.Length);

            mass.Close();
            files.Close();
        }
    }
}