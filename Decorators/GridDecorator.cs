using Chart.ModelSpace;
using System.Windows;
using System.Windows.Media;

namespace Chart.DecoratorSpace
{
  public class GridDecorator : BaseDecorator, IDecorator
  {
    public override void CreateDelegate()
    {
      var shapeModel = new ShapeModel { Size = 1, Color = Brushes.LightGray.Color };

      var pointMin = new Point(0, 0);
      var pointMax = new Point(0, Panel.H);
      var count = Composer.IndexLabelCount;
      var step = Panel.W / count;

      for (var i = 1; i < count; i++)
      {
        pointMin.X = step * i;
        pointMax.X = step * i;
        Panel.CreateLine(pointMin, pointMax, shapeModel);
      }

      pointMin = new Point(0, 0);
      pointMax = new Point(Panel.W, 0);
      count = Composer.ValueCount;
      step = Panel.H / count;

      for (var i = 1; i < count; i++)
      {
        pointMin.Y = step * i;
        pointMax.Y = step * i;
        Panel.CreateLine(pointMin, pointMax, shapeModel);
      }
    }
  }
}
