using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;

namespace Draw.ImageConverters
{
    public class ImageConverter
    {
        /// <summary>
        /// Создаем изображение из Canvas в BitmapSource
        /// 1. создаем изображение
        /// 2. сохраняем в памяти
        /// 3. преобразуем в ч/б
        /// 4. изменяем размер
        /// </summary>
        /// <param name="canvas">поле Canvas</param>
        /// <param name="size">размер выходного изображения</param>
        /// <returns></returns>
        public Bitmap CreateImage(Canvas canvas, int size)
        {
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)canvas.RenderSize.Width, (int)canvas.RenderSize.Height, 96d, 96d, PixelFormats.Pbgra32);
            canvas.Measure(new System.Windows.Size((int)canvas.Width, (int)canvas.Height));
            canvas.Arrange(new Rect(new System.Windows.Size((int)canvas.Width, (int)canvas.Height)));
            rtb.Render(canvas);

            PngBitmapEncoder BufferSave = new PngBitmapEncoder();
            BufferSave.Frames.Add((BitmapFrame.Create(rtb)));

            // делаем сохранение в памяти (наверное хаха)
            MemoryStream fs = new MemoryStream();
            BufferSave.Save(fs);

            // преобразуем в тип Bitmap из MemoryStream
            System.Drawing.Image ImgOut = System.Drawing.Image.FromStream(fs);
            fs.Close();
            // преобразуем в Ч/Б
            var bwImage = ConvertToBlackWhite((Bitmap)ImgOut);
            // изменяем размер изображения
            return ScaleImage(bwImage, size, size);
        }

        // изменяем размер изображения
        public Bitmap ScaleImage(Bitmap source, int width, int height)
        {
            Bitmap dest = new Bitmap(width, height);
            using (Graphics gr = Graphics.FromImage(dest))
            {
                gr.FillRectangle(System.Drawing.Brushes.White, 0, 0, width, height);  // Очищаем экран
                gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                float srcwidth = source.Width;
                float srcheight = source.Height;
                float dstwidth = width;
                float dstheight = height;

                if (srcwidth <= dstwidth && srcheight <= dstheight)  // Исходное изображение меньше целевого
                {
                    int left = (width - source.Width) / 2;
                    int top = (height - source.Height) / 2;
                    gr.DrawImage(source, left, top, source.Width, source.Height);
                }
                else if (srcwidth / srcheight > dstwidth / dstheight)  // Пропорции исходного изображения более широкие
                {
                    float cy = srcheight / srcwidth * dstwidth;
                    float top = ((float)dstheight - cy) / 2.0f;
                    if (top < 1.0f) top = 0;
                    gr.DrawImage(source, 0, top, dstwidth, cy);
                }
                else  // Пропорции исходного изображения более узкие
                {
                    float cx = srcwidth / srcheight * dstheight;
                    float left = ((float)dstwidth - cx) / 2.0f;
                    if (left < 1.0f) left = 0;
                    gr.DrawImage(source, left, 0, cx, dstheight);
                }

                return dest;
            }
        }

        // преобразуем в Ч/Б
        public Bitmap ConvertToBlackWhite(Bitmap img)
        {
            // создаём BitmapSource из изображения, находящегося в pictureBox1
            Bitmap input = new Bitmap(img);
            // создаём BitmapSource для черно-белого изображения
            Bitmap output = new Bitmap(input.Width, input.Height);
            // перебираем в циклах все пиксели исходного изображения
            for (int j = 0; j < input.Height; j++)
                for (int i = 0; i < input.Width; i++)
                {
                    // получаем (i, j) пиксель
                    UInt32 pixel = (UInt32)(input.GetPixel(i, j).ToArgb());
                    // получаем компоненты цветов пикселя
                    float R = (float)((pixel & 0x00FF0000) >> 16); // красный
                    float G = (float)((pixel & 0x0000FF00) >> 8); // зеленый
                    float B = (float)(pixel & 0x000000FF); // синий
                                                           // делаем цвет черно-белым (оттенки серого) - находим среднее арифметическое
                    R = G = B = (R + G + B) / 3.0f;
                    // собираем новый пиксель по частям (по каналам)
                    UInt32 newPixel = 0xFF000000 | ((UInt32)R << 16) | ((UInt32)G << 8) | ((UInt32)B);
                    // добавляем его в BitmapSource нового изображения
                    output.SetPixel(i, j, System.Drawing.Color.FromArgb((int)newPixel));
                }
            // выводим черно-белый BitmapSource в pictureBox2
            return output;
        }

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
                    imgBitmap.SetPixel(i, j, someColor);
                    index++;
                }
            }

            return imgBitmap;
        }

        public BitmapSource BitmapToBitmapSource(Bitmap source)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                source.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
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
