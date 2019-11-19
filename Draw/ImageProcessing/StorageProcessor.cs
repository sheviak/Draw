using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Draw.Extensions;

namespace Draw.ImageProcessing
{
    public class StorageProcessor
    {
        public Bitmap CSVtoBitmap(string csv, int size)
        {
            var split = csv.Split(',');
            var arr = split.Select(item => Convert.ToInt32(item)).ToList();
            var imgBitmap = new Bitmap(size, size);
            int index = 0;
            for (var i = 0; i < size; i++)
            {
                for (var j = 0; j < size; j++)
                {
                    var someColor = Color.FromArgb(arr[index], arr[index], arr[index]);
                    // на оборот тут i и j
                    imgBitmap.SetPixel(j, i, someColor);
                    index++;
                }
            }

            return imgBitmap;
        }

        public (BitmapSource, string) LoadFromCsv(int size)
        {
            var openDialog = new OpenFileDialog
            {
                FileName = "Открыть файл",
                Filter = "CSV reader|* .csv"
            };

            if (openDialog.ShowDialog() != DialogResult.OK) return (null, null);

            using (var reader = new StreamReader(openDialog.FileName))
            {
                int index = 0;
                string line = "", aboutNumber = "";
                while (!reader.EndOfStream)
                {
                    if (index == 0)
                        aboutNumber = reader.ReadLine();
                    if (index == 1)
                        line = reader.ReadLine();
                    index++;
                }
                var temp = CSVtoBitmap(line, size).BitmapToBitmapSource();
                return (temp, aboutNumber);
            }
        }

        public void SaveCsv(BitmapSource image, string aboutNumber, int size)
        {
            var sbOutput = new StringBuilder();
            var strFilePath = @"testfile.csv";
            var strSeperator = ",";
            sbOutput.AppendLine(aboutNumber);
            var img = image.GetBitmap();
            for (var i = 0; i < size; i++)
            {
                for (var j = 0; j < size; j++)
                {
                    // i j change место
                    var pixel = img.GetPixel(j, i);
                    var lum = (int)GetLuminance(pixel.R, pixel.G, pixel.B);
                    sbOutput.AppendFormat(string.Concat(lum, (i == size - 1 && j == size - 1) ? "" : strSeperator));
                }
            }
            // Create and write the csv file
            File.WriteAllText(strFilePath, sbOutput.ToString(), Encoding.UTF8);
        }

        public void SaveImage(BitmapSource image, string aboutNumber)
        {
            var savedialog = new SaveFileDialog
            {
                Title = "Сохранить файла",
                Filter = "Graphics Redactor|* .png",
                FileName = aboutNumber
            };

            if (savedialog.ShowDialog() != DialogResult.OK) return;

            /*
            var im = image.GetBitmap();
            Bitmap output = new Bitmap(im.Width, im.Height);

            for (int j = 0; j < im.Height; j++)
            {
                for (int i = 0; i < im.Width; i++)
                {
                    // получаем (i, j) пиксель
                    var pixel = im.GetPixel(i, j);
                    output.SetPixel(i, j, CreateImageExtension.invert(pixel));
                }
            }
            */
            var img = image.GetBitmap();
            var mass = new MemoryStream();
            var files = new FileStream(savedialog.FileName, FileMode.Create, FileAccess.ReadWrite);
            img.Save(mass, System.Drawing.Imaging.ImageFormat.Png);

            byte[] matric = mass.ToArray();
            files.Write(matric, 0, matric.Length);

            mass.Close();
            files.Close();
        }

        /// <summary>
        /// Конвертирование пикселей в Luminance
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public double GetLuminance(byte r, byte g, byte b)
        {
            return 0.2126 * r + 0.7152 * g + 0.0722 * b;
        }
    }
}
