using Draw.AppContext;
using Draw.Entities;
using Draw.Extensions;
using Draw.ImageProcessing;
using Draw.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Draw.Service
{
    public class NumberService
    {
        private const int SIZE = 28;
        private const string path = @"temp/img/";
        private const string strFilePath = @"temp/numbers.csv";

        private Random random = new Random();
        private StorageProcessor st = new StorageProcessor();
        private SingletonContext context = SingletonContext.GetInstance;
        private object threadLock = new object();

        /// <summary>
        /// Создание изображения для нейросети
        /// </summary>
        /// <param name="canvas">Canvas</param>
        /// <param name="canvasWidth">Ширина сanvas</param>
        /// <param name="canvasHeight">Высота сanvas</param>
        /// <param name="sizeImg">Размер выходного изображения</param>
        /// <param name="rotate">Угол поворота</param>
        /// <param name="shiftX">Смещение по оси Х</param>
        /// <param name="shiftY">Смещение по оси У</param>
        /// <param name="scaleX">Масштабирование по оси Х</param>
        /// <param name="scaleY">Масштабирование по оси У</param>
        /// <param name="skewX">Наклон элемента вдоль оси Х</param>
        /// <param name="skewY">Наклон элемента вдоль оси У</param>
        /// <returns>Изображение в формате BitmapSource</returns>
        public BitmapSource GetBitmapNumber(
            Canvas canvas,
            int canvasWidth, 
            int canvasHeight,
            int sizeImg,
            int rotate,
            int shiftX, 
            int shiftY,
            double scaleX, 
            double scaleY,
            int skewX, 
            int skewY)
        {
            var newCanvas = canvas.GetCopy();
            TransformGroup group = new TransformGroup();

            if (rotate != 0) group.RotateTransform(canvasWidth, canvasHeight, rotate);
            if (shiftX != 0 || shiftY != 0) group.TranslateTransform(shiftX, shiftY);
            if (scaleX != 0 || scaleY != 0) group.ScaleTransform(canvasWidth, canvasHeight, scaleX, scaleY);
            if (skewX != 0 || skewY != 0) group.SkewTransform(canvasWidth, canvasHeight, skewX, skewY);

            newCanvas.TransformGroup(group);

            var img = newCanvas
                .CreateImage()
                .ConvertToBlackWhite()
                .ScaleImage(sizeImg, sizeImg)
                .BitmapToBitmapSource();

            return img;
        }

        /// <summary>
        /// Создания потоков и запуск генерации изображений
        /// </summary>
        /// <param name="list">Коллекция Canvas</param>
        /// <param name="scenario">Сценарий для генерации изображений</param>
        /// <param name="progressViewModel">Загрузка (ViewModel)</param>
        public void CreateThreads(List<Canvas> list, SettingsScenario scenario, ProgressViewModel progressViewModel)
        {
            progressViewModel.FinishTotalNumbers = scenario.countDigits.Sum();
            progressViewModel.TotalNumber = 0;

            for (int i = 0; i < list.Count; i++)
            {
                var canvas = XamlWriter.Save(list[i]);
                var canvasChildren = new List<string>();
                foreach (var child in list[i].Children)
                    canvasChildren.Add(XamlWriter.Save(child));

                var count = scenario.countDigits[i];
                Thread thread = new Thread(() => CreateMnistData(canvas, canvasChildren, count, scenario, progressViewModel));
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            }
        }

        /// <summary>
        /// Метод создания копий цифр с искажениями из стокового Canvas
        /// </summary>
        /// <param name="canvas">Стоковый Canvas</param>
        /// <param name="canvasChildren">Дочерние элементы передаваемого Canvas</param>
        /// <param name="count">Кол-во копий с преобразованиями</param>
        /// <param name="scenario">Сценарий генерации</param>
        /// <param name="progressViewModel">Загрузка (для изменения на view progressbar)</param>
        private void CreateMnistData(string canvas, List<string> canvasChildren, int count, SettingsScenario scenario, ProgressViewModel progressViewModel)
        {
            for (int k = 0; k < count; k++)
            {
                // делаем копию канваса из вьюхи, чтобы не затронуть шаблонный для последующий копий
                Canvas root = (UIElement)XamlReader.Parse(canvas) as Canvas;
                // преобразуем дочерние элементы canvas из string в UIElement, и добавляем в root
                canvasChildren.ForEach(x => root.Children.Add(XamlReader.Parse(x) as UIElement));

                var parametrs = new NumProperties
                {
                    Rotate = random.Next(scenario.RotateMin, scenario.RotateMax),
                    ShiftX = random.Next(scenario.ShiftMinX, scenario.ShiftMaxX),
                    ShiftY = random.Next(scenario.ShiftMinY, scenario.ShiftMaxY),
                    SkewX = random.Next(scenario.SkewMinX, scenario.SkewMaxX),
                    SkewY = random.Next(scenario.SkewMinY, scenario.SkewMaxY),
                    ScaleX = GetRandomNumber(scenario.ScaleMinX, scenario.ScaleMaxX),
                    ScaleY = GetRandomNumber(scenario.ScaleMinY, scenario.ScaleMaxY),
                };

                var img = GetBitmapNumber(
                    canvas: root,
                    canvasWidth: (int)root.Width,
                    canvasHeight: (int)root.Height,
                    sizeImg: SIZE,
                    rotate: parametrs.Rotate,
                    scaleX: parametrs.ScaleX,
                    scaleY: parametrs.ScaleY,
                    shiftX: parametrs.ShiftX,
                    shiftY: parametrs.ShiftY,
                    skewX: parametrs.SkewX,
                    skewY: parametrs.SkewY
                    );

                var nameNumber = ""; //string.Concat(root.Name.Last().ToString(), "-", Guid.NewGuid());
                var number = root.Name.Last().ToString();
                var line = st.ConvertToCsv(img, number, SIZE);

                var model = new Number
                {
                    Num = number,
                    Value = line,
                    NumProperties = parametrs
                };

                lock (threadLock)
                {
                    SingletonContext.Context.Numbers.Add(model);
                    SingletonContext.Context.SaveChanges();
                    nameNumber = model.Id.ToString();

                    st.SaveToCsvFile(strFilePath, line);

                    progressViewModel.TotalNumber++;
                }

                st.SaveImage(img, nameNumber, path);
            }
        }
        
        /// <summary>
        /// Рандомное дробное число
        /// </summary>
        /// <param name="minimum">минимум</param>
        /// <param name="maximum">Максимум</param>
        /// <returns></returns>
        public double GetRandomNumber(double minimum, double maximum)
        {
            var val = random.NextDouble() * (maximum - minimum) + minimum;
            return Math.Round(val, 2);
        }
    }
}