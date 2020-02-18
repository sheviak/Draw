using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace Draw.Extensions
{
    public static class TransformExtension
    {
        /// <summary>
        /// Копирование элемента
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <returns></returns>
        public static T GetCopy<T>(this T element) where T : UIElement
        {
            using (var ms = new MemoryStream())
            {
                XamlWriter.Save(element, ms);
                ms.Seek(0, SeekOrigin.Begin);
                return (T)XamlReader.Load(ms);
            }
        }

        /// <summary>
        /// Трансформация всех дочерних элементов переданной TransformGroup
        /// </summary>
        /// <param name="canvas">Source canvas</param>
        /// <param name="group">Группа методов преобразования</param>
        /// <returns></returns>
        public static Canvas TransformGroup(this Canvas canvas, TransformGroup group)
        {
            foreach (UIElement item in canvas.Children)
            {
                item.RenderTransform = group;
            }

            return canvas;
        }

        /// <summary>
        /// Поворачивает элемент вокруг оси на определенное количество градусов
        /// </summary>
        /// <param name="group">Source TransformGroup</param>
        /// <param name="canvasWidth">Ширина поля Canvas</param>
        /// <param name="canvasHeight">Высота поля Canvas</param>
        /// <param name="rotate">Угол поворота в градусах</param>
        /// <returns></returns>
        public static TransformGroup RotateTransform(this TransformGroup group, int canvasWidth, int canvasHeight, int rotate)
        {
            group.Children.Add(new RotateTransform
            {
                Angle = rotate,
                CenterX = canvasWidth / 2,
                CenterY = canvasHeight / 2,
            });

            return group;
        }

        // TranslateTransform п
        /// <summary>
        /// Смещение элемента по оси X и Y
        /// </summary>
        /// <param name="group">Source TransformGroup</param>
        /// <param name="shiftX">Смещение по оси Х в px</param>
        /// <param name="shiftY">Смещение по оси Y в px</param>
        /// <returns></returns>
        public static TransformGroup TranslateTransform(this TransformGroup group, int shiftX, int shiftY)
        {
            group.Children.Add(new TranslateTransform
            {
                X = shiftX,
                Y = shiftY,
            });

            return group;
        }

        /// <summary>
        /// Масштабирование элемента
        /// </summary>
        /// <param name="group">Source TransformGroup</param>
        /// <param name="canvasWidth">Ширина Canvas</param>
        /// <param name="canvasHeight">Высота Canvas</param>
        /// <param name="scaleX">Увеличение по осо Х</param>
        /// <param name="scaleY">Увеличение по оси У</param>
        /// <returns></returns>
        public static TransformGroup ScaleTransform(this TransformGroup group, int canvasWidth, int canvasHeight, double scaleX, double scaleY)
        {
            group.Children.Add(new ScaleTransform
            {
                CenterX = canvasWidth / 2,
                CenterY = canvasHeight / 2,
                ScaleX = scaleX,
                ScaleY = scaleY,
            });

            return group;
        }

        /// <summary>
        /// Наклон элемента
        /// </summary>
        /// <param name="group">Source TransformGroup</param>
        /// <param name="canvasWidth">Ширина Canvas</param>
        /// <param name="canvasHeight">Высота Canvas</param>
        /// <param name="skewX">Наклон элемента вдоль оси X</param>
        /// <param name="skewY">Наклон элемента вдоль оси У</param>
        /// <returns></returns>
        public static TransformGroup SkewTransform(this TransformGroup group, int canvasWidth, int canvasHeight, int skewX, int skewY)
        {
            group.Children.Add(new SkewTransform
            {
                CenterX = canvasWidth / 2,
                CenterY = canvasHeight / 2,
                AngleX = skewX,
                AngleY = skewY
            });

            return group;
        }
    }
}