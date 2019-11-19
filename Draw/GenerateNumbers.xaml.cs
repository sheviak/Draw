using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Draw
{
    /// <summary>
    /// Логика взаимодействия для GenerateNumbers.xaml
    /// </summary>
    public partial class GenerateNumbers : Window
    {
        PathFigure currentFigure;
        bool isDrawing = false;
        Canvas Canvas;

        public GenerateNumbers()
        {
            InitializeComponent();
        }

        void DrawingMouseDown(object sender, MouseButtonEventArgs e)
        {
            Canvas = (Canvas)sender;
            Mouse.Capture(Canvas);
            isDrawing = true;
            StartFigure(e.GetPosition(Canvas));
        }

        void DrawingMouseMove(object sender, MouseEventArgs e)
        {
            if (!isDrawing)
                return;
            AddFigurePoint(e.GetPosition(Canvas));
        }

        void DrawingMouseUp(object sender, MouseButtonEventArgs e)
        {
            AddFigurePoint(e.GetPosition(Canvas));
            EndFigure();
            isDrawing = false;
            Mouse.Capture(null);
        }

        void StartFigure(Point start)
        {
            currentFigure = new PathFigure() { StartPoint = start };
            var currentPath =
                new System.Windows.Shapes.Path()
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 10,
                    Data = new PathGeometry() { Figures = { currentFigure } }
                };
            Canvas.Children.Add(currentPath);
        }

        void AddFigurePoint(Point point)
        {
            currentFigure.Segments.Add(new LineSegment(point, isStroked: true));
        }

        void EndFigure()
        {
            currentFigure = null;
        }
    }
}
