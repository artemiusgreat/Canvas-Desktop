using Chart.EnumSpace;
using Chart.ModelSpace;
using System;
using System.Windows;
using System.Windows.Media;

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
      var minIndex = Composer.MinIndex;
      var maxIndex = Composer.MaxIndex;
      var minValue = Composer.MinValue;
      var maxValue = Composer.MaxValue;
      var stepSize = Composer.StepSize;
      var indexCount = Composer.IndexLabelCount;
      var valueCount = Composer.ValueCount;

      var step = 0.0;
      var pointMin = new Point(0, 0);
      var pointMax = new Point(0, 0);
      var shapeModel = new ShapeModel { Size = 1, Color = Brushes.Black.Color };

      switch (Position)
      {
        case PositionEnum.Left:

          pointMin = new Point(maxIndex, minValue);
          pointMax = new Point(maxIndex, maxValue);

          break;

        case PositionEnum.Right:

          pointMin = new Point(minIndex, minValue);
          pointMax = new Point(minIndex, maxValue);

          break;

        case PositionEnum.Top:

          pointMin = new Point(minIndex, minValue);
          pointMax = new Point(maxIndex, minValue);

          break;

        case PositionEnum.Bottom:

          pointMin = new Point(minIndex, maxValue);
          pointMax = new Point(maxIndex, maxValue);

          break;
      }

      Panel.CreateLine(Composer.GetPixels(Panel, pointMin), Composer.GetPixels(Panel, pointMax), shapeModel);

      switch (Position)
      {
        case PositionEnum.Left:

          step = Panel.H / valueCount;
          pointMin = new Point(Panel.W, 0);
          pointMax = new Point(Panel.W - stepSize, 0);

          for (var i = 1; i < valueCount; i++)
          {
            pointMin.Y += step;
            pointMax.Y += step;
            Panel.CreateLine(pointMin, pointMax, shapeModel);
          }

          break;

        case PositionEnum.Right:

          step = Panel.H / valueCount;
          pointMin = new Point(0, 0);
          pointMax = new Point(stepSize, 0);

          for (var i = 1; i < valueCount; i++)
          {
            pointMin.Y += step;
            pointMax.Y += step;
            Panel.CreateLine(pointMin, pointMax, shapeModel);
          }

          break;

        case PositionEnum.Top:

          step = Panel.W / indexCount;
          pointMin = new Point(0, Panel.H);
          pointMax = new Point(0, Panel.H - stepSize);

          for (var i = 1; i < indexCount; i++)
          {
            pointMin.X += step;
            pointMax.X += step;
            Panel.CreateLine(pointMin, pointMax, shapeModel);
          }

          break;

        case PositionEnum.Bottom:

          step = Panel.W / indexCount;
          pointMin = new Point(0, 0);
          pointMax = new Point(0, stepSize);

          for (var i = 1; i < indexCount; i++)
          {
            pointMin.X += step;
            pointMax.X += step;
            Panel.CreateLine(pointMin, pointMax, shapeModel);
          }

          break;
      }
    }
  }
}
