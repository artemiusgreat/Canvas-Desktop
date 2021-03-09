using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Chart.DecoratorSpace
{
  public class GridDecorator : BaseDecorator, IDecorator
  {
    public override void CreateDelegate()
    {
      var canvas = Panel as Canvas;
      var pointMin = new Point(0, 0);
      var pointMax = new Point(0, Panel.H);
      var count = Composer.IndexLabelCount.Value;
      var step = Panel.W / count;

      canvas.Children.Clear();

      for (var i = 1; i < count; i++)
      {
        pointMin.X = step * i;
        pointMax.X = step * i;
        canvas.Children.Add(CreateLine(pointMin, pointMax));
      }

      pointMin = new Point(0, 0);
      pointMax = new Point(Panel.W, 0);
      count = Composer.ValueCount.Value;
      step = Panel.H / count;

      for (var i = 1; i < count; i++)
      {
        pointMin.Y = step * i;
        pointMax.Y = step * i;
        canvas.Children.Add(CreateLine(pointMin, pointMax));
      }
    }

    /// <summary>
    /// Create line
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    /// <returns></returns>
    private Line CreateLine(Point source, Point destination)
    {
      return new Line
      {
        X1 = source.X,
        Y1 = source.Y,
        X2 = destination.X,
        Y2 = destination.Y,
        StrokeThickness = 1,
        Stroke = new SolidColorBrush(Color),
        StrokeDashArray = new DoubleCollection(new[] { 5.0, 5.0 })
      };
    }
  }
}
