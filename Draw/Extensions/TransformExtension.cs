using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Draw.Extensions
{
    public static class TransformExtension
    {
        public static BitmapSource RotateImageTransform(this BitmapSource image, double rotate)
        {
            // Selecting rotation Angle:
            RotateTransform rotation = new RotateTransform();
            rotation.Angle += rotate;
             // Computing Rotation Center:
            rotation.CenterX = image.Width / 2;
            rotation.CenterY = image.Height / 2;
            // Rotating current Layer's Image:
            image = CreateRenderTarget((int)image.Width, (int)image.Height,
                (visual, context) =>
                {
                    context.DrawImage(image, new Rect(0.0, 0.0, image.Width, image.Height));
                    visual.Transform = rotation;
                });
            
            return image;
        }

        public static BitmapSource ScaleImageTransform(this BitmapSource image, double scaleX, double scaleY)
        {
            ScaleTransform scaleTransform = new ScaleTransform();
            scaleTransform.ScaleX = scaleX;
            scaleTransform.ScaleY = scaleY;
            image = CreateRenderTarget((int)image.Width, (int)image.Height,
                (visual, context) =>
                {
                    context.DrawImage(image, new Rect(0.0, 0.0, image.Width, image.Height));
                    visual.Transform = scaleTransform;
                });

            return image;
        }

        public static BitmapSource TranslateImageTransform(this BitmapSource image, int shiftX, int shiftY)
        {
            TranslateTransform translateTransform = new TranslateTransform();
            translateTransform.X = shiftX;
            translateTransform.Y = shiftY;
            image = CreateRenderTarget((int)image.Width, (int)image.Height,
                (visual, context) =>
                {
                    context.DrawImage(image, new Rect(0.0, 0.0, image.Width, image.Height));
                    visual.Transform = translateTransform;
                });

            return image;
        }

        public static BitmapSource CreateRenderTarget(int width, int height, Action<DrawingVisual, DrawingContext> drawAction = null)
        {
            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext context = visual.RenderOpen())
            {
                if (drawAction != null)
                    drawAction(visual, context);
            }
            RenderTargetBitmap bitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(visual);
            return bitmap;
        }
    }
}