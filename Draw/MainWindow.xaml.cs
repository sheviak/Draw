using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

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

        void DrawingMouseMove(object sender, MouseEventArgs e)
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
            canvas.Children.Add(currentPath);
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
