using Draw.ImageProcessing;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Draw.Extensions
{
    public static class CreateImageExtension
    {
        /// <summary>
        /// Создаем изображение из Canvas в Bitmap
        /// </summary>
        /// <param name="canvas">Поле Canvas</param>
        /// <returns></returns>
        public static Bitmap CreateImage(this Canvas canvas)
        {
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)canvas.Width, (int)canvas.Height, 96d, 96d, PixelFormats.Pbgra32);
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
            return (Bitmap)ImgOut; 
        }

        /// <summary>
        /// Изменяем размер изображения
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="width">ширина в px</param>
        /// <param name="height">высота в px</param>
        /// <returns></returns>
        public static Bitmap ScaleImage(this Bitmap source, int width, int height)
        {
            Bitmap dest = new Bitmap(width, height);
            using (Graphics gr = Graphics.FromImage(dest))
            {
                gr.FillRectangle(System.Drawing.Brushes.Black, 0, 0, width, height);  // Очищаем экран
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

        /// <summary>
        /// Инвертирует изображение в цвета изображение из БД MNIST (в ч/б)
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static BitmapSource ConvertToColorMnist(this BitmapSource image)
        {
            var img = image.GetBitmap();
            Bitmap output = new Bitmap(img.Width, img.Height);

            for (int j = 0; j < img.Height; j++)
            {
                for (int i = 0; i < img.Width; i++)
                {
                    // получаем (i, j) пиксель
                    var pixel = img.GetPixel(i, j);
                    output.SetPixel(i, j, PixelConvert.Invert(pixel));
                }
            }
            return output.BitmapToBitmapSource();
        }

        /// <summary>
        /// Преобразование изображения в цвета Ч/Б
        /// </summary>
        /// <param name="img">source</param>
        /// <returns></returns>
        public static Bitmap ConvertToBlackWhite(this Bitmap img)
        {
            // создаём BitmapSource из изображения, переданного в параметре
            Bitmap input = new Bitmap(img);
            // создаём BitmapSource для черно-белого изображения
            Bitmap output = new Bitmap(input.Width, input.Height);
            // перебираем в циклах все пиксели исходного изображения
            for (int j = 0; j < input.Height; j++)
            {
                for (int i = 0; i < input.Width; i++)
                {
                    // получаем (i, j) пиксель
                    UInt32 oldPixel = (UInt32)(input.GetPixel(i, j).ToArgb());
                    // получаем компоненты цветов пикселя
                    float R = (float)((oldPixel & 0x00FF0000) >> 16); // красный
                    float G = (float)((oldPixel & 0x0000FF00) >> 8); // зеленый
                    float B = (float)(oldPixel & 0x000000FF); // синий
                                                           // делаем цвет черно-белым (оттенки серого) - находим среднее арифметическое
                    R = G = B = (R + G + B) / 3.0f;
                    // собираем новый пиксель по частям (по каналам)
                    UInt32 newPixel = 0xFF000000 | ((UInt32)R << 16) | ((UInt32)G << 8) | ((UInt32)B);
                    // добавляем его в BitmapSource нового изображения
                    var pixel = System.Drawing.Color.FromArgb((int)newPixel);
                    pixel = PixelConvert.Invert(pixel);
                    output.SetPixel(i, j, pixel);
                }
            }
            // выводим черно-белый BitmapSource в pictureBox2
            return output;
        }

        /// <summary>
        /// Конвертация BitmapSource -> Bitmap
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Bitmap GetBitmap(this BitmapSource source)
        {
            Bitmap bmp = new Bitmap(source.PixelWidth, source.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            BitmapData data = bmp.LockBits(new Rectangle(System.Drawing.Point.Empty, bmp.Size), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            source.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
            bmp.UnlockBits(data);
            return bmp;
        }

        /// <summary>
        /// Конвертация Bitmap -> BitmapSource
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static BitmapSource BitmapToBitmapSource(this Bitmap source)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                source.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }        
    }
}