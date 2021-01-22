using Chart.ModelSpace;
using System.Collections.Generic;
using System.Windows;

namespace Chart.ShapeSpace
{
  public class LineShape : BaseShape, IShape
  {
    public virtual IList<int> IndexLevels { get; set; } = new List<int>();
    public virtual IList<double> ValueLevels { get; set; } = new List<double>();

    public override void UpdateShape()
    {
      var shapeModel = new ShapeModel { Size = 1, Color = Color };
      var pointMin = new Point(0, 0);
      var pointMax = new Point(0, 0);

      foreach (var level in IndexLevels)
      {
        var pixelLevel = Composer.GetPixels(Panel, level, 0);

        pointMin.X = pointMax.X = pixelLevel.X;
        pointMin.Y = 0;
        pointMax.Y = Panel.H;

        Panel.CreateLine(pointMin, pointMax, shapeModel);
      }

      foreach (var level in ValueLevels)
      {
        var pixelLevel = Composer.GetPixels(Panel, 0, level);

        pointMin.Y = pointMax.Y = pixelLevel.Y;
        pointMin.X = 0;
        pointMax.X = Panel.W;

        Panel.CreateLine(pointMin, pointMax, shapeModel);
      }
    }
  }
}
