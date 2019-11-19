using Draw.Command;
using Draw.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Draw.ViewModel
{
    public class GenerateNumbersViewModel : ViewModelBase
    {
        private const int SIZE = 28;
        private object threadLock = new object();
        string writePath = @"file.csv";

        private Random random = new Random();
        private List<Canvas> list = new List<Canvas>();
        public DelegateCommand<IEnumerable<Canvas>> Test { get; set; }

        public GenerateNumbersViewModel()
        {
            // получаем все Canvas с вьюхи с типизируем
            Test = new DelegateCommand<IEnumerable<Canvas>>((canvas) =>
            {
                foreach (var item in canvas as IEnumerable<Canvas>)
                {
                    // тут в зависимости от Канваса будет передаваться 
                    // определенное число копий этого элемента
                    CreateImage(item, 10);
                }
            });
        }
        void CreateImage(Canvas canvas, int count)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                for (int k = 0; k < count; k++)
                {
                    // делаем копию канваса из вьюхи, чтобы не затронуть шаблонный для последующий копий
                    Canvas root = Clone(canvas);
                    root.Children.Clear(); // очищаем на всякий случай 
                    // копируем все дочерние элементы из родителя канваса в дочерний root, т.к. метод Clone не копирует дочерние
                    foreach (UIElement child in canvas.Children)
                    {
                        var xaml = System.Windows.Markup.XamlWriter.Save(child);
                        var deepCopy = System.Windows.Markup.XamlReader.Parse(xaml) as UIElement;
                        root.Children.Add(deepCopy);
                    }

                    // в этом методе вращаем дочерние элементы
                    root.ChangeCanvas(random.Next(-45, 45));
                    root.UpdateLayout(); // обновляем

                    var img = root
                        .CreateImage() // создаем из канваса изображение
                        .ScaleImage(28, 28) // кардируем в масштаб 28х28
                        .ConvertToBlackWhite() // преобразуем в ч/б
                        .BitmapToBitmapSource(); // в BitmapSource

                    // потом нашу картинку нужно конвертнуть в csv формат
                    // то есть выделить светосилу каждого пикселя
                    // для этого создаем билдер и далее...
                    var sbOutput = new StringBuilder();
                    var strSeperator = ",";
                    var imgOut = img.GetBitmap();
                    for (var i = 0; i < SIZE; i++)
                    {
                        for (var j = 0; j < SIZE; j++)
                        {
                            // i j change место
                            var pixel = imgOut.GetPixel(j, i);
                            var lum = (int)GetLuminance(pixel.R, pixel.G, pixel.B);
                            sbOutput.AppendFormat(string.Concat(lum, (i == SIZE - 1 && j == SIZE - 1) ? "" : strSeperator));
                        }
                    }
                    // записываем построчно в файл нашу картинку 784 числа разделенных запятыми и с в начале
                    WriteToFile(sbOutput.ToString());
                    //SaveImage(img, root.Name);
                }
            }));
        }

        public void WriteToFile(string line)
        {
            // Используем маркер блокировки
            lock (threadLock)
            {
                using (StreamWriter sw = new StreamWriter(writePath, true, Encoding.UTF8))
                {
                    sw.WriteLine(line);
                }
            }
        }

        public double GetLuminance(byte r, byte g, byte b)
        {
            return 0.2126 * r + 0.7152 * g + 0.0722 * b;
        }

        public async void SaveImage(BitmapSource image, string aboutNumber)
        {
            /*
            var savedialog = new SaveFileDialog
            {
                Title = "Сохранить файла",
                Filter = "Graphics Redactor|* .png",
                FileName = aboutNumber
            };

            if (savedialog.ShowDialog() != DialogResult.OK) return;
            */

            var img = image.GetBitmap();
            var mass = new MemoryStream();
            using (var files = new FileStream("temp\\" + random.Next(0, Int32.MaxValue) + ".png", FileMode.Create, FileAccess.ReadWrite))
            {
                img.Save(mass, System.Drawing.Imaging.ImageFormat.Png);

                byte[] matric = mass.ToArray();
                await files.WriteAsync(matric, 0, matric.Length);

                mass.Close();
                files.Close();
            }
        }

        public T Clone<T>(T controlToClone) where T : Canvas
        {
            PropertyInfo[] controlProperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            T instance = Activator.CreateInstance<T>();

            foreach (PropertyInfo propInfo in controlProperties)
            {
                if (propInfo.CanWrite)
                {
                    if (propInfo.Name != "WindowTarget")
                        propInfo.SetValue(instance, propInfo.GetValue(controlToClone, null), null);
                }
            }

            return instance;
        }
    }
}
