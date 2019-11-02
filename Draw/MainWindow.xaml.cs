using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Draw
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PathFigure currentFigure;
        bool isDrawing = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        void DrawingMouseDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(canvas);
            isDrawing = true;
            StartFigure(e.GetPosition(canvas));
        }

        void DrawingMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!isDrawing)
                return;
            AddFigurePoint(e.GetPosition(canvas));
        }

        void DrawingMouseUp(object sender, MouseButtonEventArgs e)
        {
            AddFigurePoint(e.GetPosition(canvas));
            EndFigure();
            isDrawing = false;
            Mouse.Capture(null);
        }

        void StartFigure(System.Windows.Point start)
        {
            currentFigure = new PathFigure() { StartPoint = start };
            var currentPath =
                new System.Windows.Shapes.Path()
                {
                    Stroke = System.Windows.Media.Brushes.Red,
                    StrokeThickness = 8,
                    Data = new PathGeometry() { Figures = { currentFigure } }
                };
            canvas.Children.Add(currentPath);
        }

        void AddFigurePoint(System.Windows.Point point)
        {
            currentFigure.Segments.Add(new LineSegment(point, isStroked: true));
        }

        void EndFigure()
        {
            currentFigure = null;
        }

        // Стоковое изображение
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog save = new System.Windows.Forms.SaveFileDialog();
            save.Filter = "Graphics Redactor|* .png";
            save.Title = "Сохранение файла";
            save.ShowDialog();

            var rtb = new RenderTargetBitmap((int)canvas.Width, (int)canvas.Height, 96d, 96d, PixelFormats.Pbgra32);
            // needed otherwise the image output is black
            canvas.Measure(new System.Windows.Size((int)canvas.Width, (int)canvas.Height));
            canvas.Arrange(new Rect(new System.Windows.Size((int)canvas.Width, (int)canvas.Height)));
            rtb.Render(canvas);

            PngBitmapEncoder BufferSave = new PngBitmapEncoder();
            BufferSave.Frames.Add((BitmapFrame.Create(rtb)));
            using (var fs = File.OpenWrite(save.FileName))
            {
                BufferSave.Save(fs);
            }
        }       
    }
}
