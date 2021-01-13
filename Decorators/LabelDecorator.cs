using Chart.EnumSpace;
using Chart.ModelSpace;
using System;
using System.Windows;
using System.Windows.Media;

namespace Chart.DecoratorSpace
{
  public class LabelDecorator : BaseDecorator, IDecorator
  {
    /// <summary>
    /// Label size
    /// </summary>
    public virtual int FontSize { get; set; } = 10;

    /// <summary>
    /// Location of the axis, e.g. L(eft), R(ight), T(op), B(ottom)
    /// </summary>
    public virtual PositionEnum Position { get; set; }

    /// <summary>
    /// Custom label renderer
    /// </summary>
    public virtual Func<dynamic, dynamic> ShowLabel { get; set; }

    /// <summary>
    /// Create label
    /// </summary>
    public virtual string ShowLabelDelegate(dynamic position)
    {
      return $"{position:0.00}";
    }

    /// <summary>
    /// Create component
    /// </summary>
    public override void CreateDelegate()
    {
      CreateLabel();
    }

    /// <summary>
    /// Update component
    /// </summary>
    public override void UpdateDelegate()
    {
      CreateLabel();
    }

    /// <summary>
    /// Update component
    /// </summary>
    protected virtual void CreateLabel()
    {
      ShowLabel ??= ShowLabelDelegate;

      var minIndex = Composer.MinIndex;
      var maxIndex = Composer.MaxIndex;
      var minValue = Composer.MinValue;
      var maxValue = Composer.MaxValue;
      var stepSize = Composer.StepSize;
      var indexStep = Composer.IndexStep;
      var valueStep = Composer.ValueStep;
      var indexCount = Composer.IndexLabelCount;
      var valueCount = Composer.ValueCount;
      var shapeModel = new ShapeModel { Size = 1, Color = Brushes.Black.Color };
      var contentModel = new ShapeModel { Size = FontSize, Color = Brushes.Black.Color };
      var point = new Point(0, 0);
      var step = 0.0;

      switch (Position)
      {
        case PositionEnum.Left:

          step = Panel.H / valueCount;
          contentModel.Position = TextAlignment.Right;
          point = new Point(Panel.W, 0);
          point.X -= stepSize * 2;

          for (var i = 1; i < valueCount; i++)
          {
            point.Y = step * i + contentModel.Size / 3;
            Panel.CreateLabel(point, ShowLabel(minValue + (valueCount - i) * valueStep), contentModel);
          }

          break;

        case PositionEnum.Right:

          step = Panel.H / valueCount;
          contentModel.Position = TextAlignment.Left;
          point = new Point(0, 0);
          point.X += stepSize * 2;

          for (var i = 1; i < valueCount; i++)
          {
            point.Y = step * i + contentModel.Size / 3;
            Panel.CreateLabel(point, ShowLabel(minValue + (valueCount - i) * valueStep), contentModel);
          }

          break;

        case PositionEnum.Top:

          step = Panel.W / indexCount;
          contentModel.Position = TextAlignment.Center;
          point = new Point(Panel.W, Panel.H);
          point.Y -= contentModel.Size;

          for (var i = 1; i < indexCount; i++)
          {
            point.X = step * i;
            Panel.CreateLabel(point, ShowLabel(minIndex + i * indexStep), contentModel);
          }

          break;

        case PositionEnum.Bottom:

          step = Panel.W / indexCount;
          contentModel.Position = TextAlignment.Center;
          point = new Point(Panel.W, 0);
          point.Y += contentModel.Size * 1.5;

          for (var i = 1; i < indexCount; i++)
          {
            point.X = step * i;
            Panel.CreateLabel(point, ShowLabel(minIndex + i * indexStep), contentModel);
          }

          break;
      }
    }
  }
}
