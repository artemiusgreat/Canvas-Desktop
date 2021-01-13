using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Chart.DecoratorSpace
{
  public class CrossDecorator : BaseDecorator, IDecorator
  {
    private Line _lineX = null;
    private Line _lineY = null;
    private Canvas _canvas = null;

    /// <summary>
    /// Create cross lines
    /// </summary>
    public override void CreateDelegate()
    {
      _canvas = Panel as Canvas;
      _canvas.MouseMove -= OnMouseMove;
      _canvas.MouseMove += OnMouseMove;
      _canvas.MouseLeave -= OnMouseLeave;
      _canvas.MouseLeave += OnMouseLeave;

      _lineX = new Line
      {
        X1 = 0,
        X2 = 0,
        Y1 = 0,
        Y2 = 0,
        StrokeThickness = 1,
        Stroke = Brushes.Black,
        StrokeDashArray = new DoubleCollection(new[] { 5.0, 5.0 })
      };

      _lineY = new Line
      {
        X1 = 0,
        X2 = 0,
        Y1 = 0,
        Y2 = 0,
        StrokeThickness = 1,
        Stroke = Brushes.Black,
        StrokeDashArray = new DoubleCollection(new[] { 5.0, 5.0 })
      };

      _canvas.Children.Clear();
      _canvas.Children.Add(_lineX);
      _canvas.Children.Add(_lineY);
    }

    /// <summary>
    /// Mouse leave event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnMouseLeave(object sender, MouseEventArgs e)
    {
      _lineX.X1 = _lineX.X2 = _lineX.Y1 = _lineX.Y2 = -1000;
      _lineY.X1 = _lineY.X2 = _lineY.Y1 = _lineY.Y2 = -1000;
    }

    /// <summary>
    /// Mouse move event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnMouseMove(object sender, MouseEventArgs e)
    {
      var position = e.GetPosition(_canvas);

      _lineX.X1 = position.X;
      _lineX.X2 = position.X;
      _lineX.Y1 = 0;
      _lineX.Y2 = _canvas.Height;

      _lineY.X1 = 0;
      _lineY.X2 = _canvas.Width;
      _lineY.Y1 = position.Y;
      _lineY.Y2 = position.Y;
    }
  }
}
