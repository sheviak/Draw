using System.Drawing;

namespace Draw.ImageProcessing
{
    public static class PixelConvert
    {
        /// <summary>
        /// Конвертирование пикселей в Luminance
        /// </summary>
        /// <param name="r">R</param>
        /// <param name="g">G</param>
        /// <param name="b">B</param>
        /// <returns></returns>
        public static double GetLuminance(byte r, byte g, byte b)
        {
            return 0.2126 * r + 0.7152 * g + 0.0722 * b;
        }

        /// <summary>
        /// Инвертирование цвета
        /// </summary>
        /// <param name="c">Цвет</param>
        /// <returns>инвертированный цвет (церный -> белый)</returns>
        public static Color Invert(Color c)
        {
            return Color.FromArgb(c.A, 0xFF - c.R, 0xFF - c.G, 0xFF - c.B);
        }
    }
}