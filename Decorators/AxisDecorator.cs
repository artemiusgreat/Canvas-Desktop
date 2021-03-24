using Chart.EnumSpace;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Chart.DecoratorSpace
{
  public class AxisDecorator : BaseDecorator, IDecorator
  {
    /// <summary>
    /// Location of the axis, e.g. L(eft), R(ight), T(op), B(ottom)
    /// </summary>
    public virtual PositionEnum Position { get; set; }

    /// <summary>
    /// Create component
    /// </summary>
    public override void CreateDelegate()
    {
      var canvas = Panel as Canvas;
      var stepSize = Composer.StepSize.Value;
      var indexCount = Composer.IndexLabelCount.Value;
      var valueCount = Composer.ValueCount.Value;
      var pointMin = new Point(0, 0);
      var pointMax = new Point(0, 0);
      var step = 0.0;

      canvas.Children.Clear();

      switch (Position)
      {
        case PositionEnum.L:

          pointMin = new Point(Panel.W, 0);
          pointMax = new Point(Panel.W, Panel.H);

          break;

        case PositionEnum.R:

          pointMin = new Point(0, 0);
          pointMax = new Point(0, Panel.H);

          break;

        case PositionEnum.T:

          pointMin = new Point(0, Panel.H);
          pointMax = new Point(Panel.W, Panel.H);

          break;

        case PositionEnum.B:

          pointMin = new Point(0, 0);
          pointMax = new Point(Panel.W, 0);

          break;
      }

      canvas.Children.Add(CreateLine(pointMin, pointMax));

      switch (Position)
      {
        case PositionEnum.L:

          step = Panel.H / valueCount;
          pointMin = new Point(Panel.W, 0);
          pointMax = new Point(Panel.W - stepSize, 0);

          for (var i = 1; i < valueCount; i++)
          {
            pointMin.Y += step;
            pointMax.Y += step;
            canvas.Children.Add(CreateLine(pointMin, pointMax));
          }

          break;

        case PositionEnum.R:

          step = Panel.H / valueCount;
          pointMin = new Point(0, 0);
          pointMax = new Point(stepSize, 0);

          for (var i = 1; i < valueCount; i++)
          {
            pointMin.Y += step;
            pointMax.Y += step;
            canvas.Children.Add(CreateLine(pointMin, pointMax));
          }

          break;

        case PositionEnum.T:

          step = Panel.W / indexCount;
          pointMin = new Point(0, Panel.H);
          pointMax = new Point(0, Panel.H - stepSize);

          for (var i = 1; i < indexCount; i++)
          {
            pointMin.X += step;
            pointMax.X += step;
            canvas.Children.Add(CreateLine(pointMin, pointMax));
          }

          break;

        case PositionEnum.B:

          step = Panel.W / indexCount;
          pointMin = new Point(0, 0);
          pointMax = new Point(0, stepSize);

          for (var i = 1; i < indexCount; i++)
          {
            pointMin.X += step;
            pointMax.X += step;
            canvas.Children.Add(CreateLine(pointMin, pointMax));
          }

          break;
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
