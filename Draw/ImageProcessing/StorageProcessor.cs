using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Draw.Entities;
using Draw.Extensions;
using Newtonsoft.Json;

namespace Draw.ImageProcessing
{
    public class StorageProcessor
    {
        /// <summary>
        /// Конвертирвание строки CSV в изображение Bitmap
        /// </summary>
        /// <param name="csv">CSV строка</param>
        /// <param name="size">Размер изображения</param>
        /// <returns></returns>
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

        /// <summary>
        /// Считывание 1й строки CSV из файла
        /// </summary>
        /// <param name="size">Размер изображения</param>
        /// <returns></returns>
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
                var line = reader.ReadLine();
                var temp = CSVtoBitmap(line.Remove(0, 2), size).BitmapToBitmapSource();
                return (temp, line.Remove(1));
            }
        }

        /// <summary>
        /// Конвертирвоание изображения BitmapSource в CSV строку
        /// </summary>
        /// <param name="image">Изображение</param>
        /// <param name="number">Цифра</param>
        /// <param name="size">Размер в px</param>
        /// <returns></returns>
        public string ConvertToCsv(BitmapSource image, string number, int size)
        {
            var sbOutput = new StringBuilder();
            var strSeperator = ",";
            sbOutput.AppendFormat(number+strSeperator);

            var img = image.GetBitmap();
            for (var i = 0; i < size; i++)
            {
                for (var j = 0; j < size; j++)
                {
                    // i j change место
                    var pixel = img.GetPixel(j, i);
                    //pixel = PixelConvert.Invert(pixel); // инвертируем в нужную картинку для БД MNIST
                    var lum = (int)PixelConvert.GetLuminance(pixel.R, pixel.G, pixel.B);
                    sbOutput.AppendFormat(string.Concat(lum, (i == size - 1 && j == size - 1) ? "" : strSeperator));
                }
            }

            return sbOutput.ToString();
        }

        /// <summary>
        /// Сохранение в CSV файл
        /// </summary>
        /// <param name="strFilePath">Путь к файлу</param>
        /// <param name="line">Строка CSV</param>
        public void SaveToCsvFile(string strFilePath, string line)
        {
            // File.WriteAllText(strFilePath, sbOutput.ToString(), Encoding.UTF8);
            using (StreamWriter sw = new StreamWriter(strFilePath, true, Encoding.UTF8))
            {
                sw.WriteLine(line);
            }
        }

        /// <summary>
        /// Сохранение изображения
        /// </summary>
        /// <param name="image">Изображение</param>
        /// <param name="nameNumber">Цифра</param>
        /// <param name="path">Путь сохранения</param>
        public async void SaveImage(BitmapSource image, string nameNumber, string path = "temp")
        {
            var img = image.GetBitmap();
            var mass = new MemoryStream();
            using (var files = new FileStream(string.Concat(path, "//", nameNumber, ".png"), FileMode.Create, FileAccess.ReadWrite))
            {
                img.Save(mass, System.Drawing.Imaging.ImageFormat.Png);

                byte[] matric = mass.ToArray();
                await files.WriteAsync(matric, 0, matric.Length);

                mass.Close();
                files.Close();
            }
        }

        /// <summary>
        /// Загрузка сценария
        /// </summary>
        /// <returns></returns>
        public SettingsScenario LoadScenario()
        {
            var openDialog = new OpenFileDialog
            {
                FileName = "Открыть сценарий",
                Filter = "JSON data|* .json",
                InitialDirectory = Application.StartupPath + "\\temp\\scenario"
            };

            if (openDialog.ShowDialog() != DialogResult.OK) return new SettingsScenario();

            string json = File.ReadAllText(openDialog.FileName);
            return JsonConvert.DeserializeObject<SettingsScenario>(json);
        }

        /// <summary>
        /// Сохранение сценария
        /// </summary>
        /// <param name="scenario">Объект типа SettingsScenario</param>
        public void SaveScenario(SettingsScenario scenario)
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = "JSON data|* .json",
                InitialDirectory = Application.StartupPath + "\\temp\\scenario"
            };

            if (saveDialog.ShowDialog() != DialogResult.OK) return;

            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter sw = new StreamWriter(saveDialog.FileName))
            {
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, scenario);
                }
            }
        }
    }
}