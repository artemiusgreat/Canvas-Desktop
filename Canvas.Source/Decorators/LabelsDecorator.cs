using Canvas.Source.EnumSpace;
using Canvas.Source.ModelSpace;
using SkiaSharp;
using System;

namespace Canvas.Source.DecoratorSpace
{
  public class LabelsDecorator : BaseDecorator, IDecorator
  {
    /// <summary>
    /// Custom label renderer
    /// </summary>
    public virtual Func<dynamic, dynamic> ShowIndex { get; set; }

    /// <summary>
    /// Custom label renderer
    /// </summary>
    public virtual Func<dynamic, dynamic> ShowValue { get; set; }

    /// <summary>
    /// Reusable shape model
    /// </summary>
    protected IShapeModel _shape = null;

    /// <summary>
    /// Reusable points model
    /// </summary>
    protected IPointModel _point = null;

    /// <summary>
    /// Constructor
    /// </summary>
    public LabelsDecorator()
    {
      _point = new PointModel
      {
        Index = 0,
        Value = 0
      };

      _shape = new ShapeModel
      {
        Size = 10,
        Location = LocationEnum.Center,
        Color = new SKColor(50, 50, 50)
      };
    }

    /// <summary>
    /// Create component
    /// </summary>
    public override void CreateDelegate()
    {
      _shape = Composer.LabelShape ?? _shape;

      CreateIndex();
      CreateValue();
    }

    /// <summary>
    /// Update component
    /// </summary>
    public override void UpdateDelegate()
    {
      CreateIndex();
      CreateValue();
    }

    /// <summary>
    /// Create index labels
    /// </summary>
    protected virtual void CreateIndex()
    {
      ShowIndex ??= Composer.ShowIndex;

      var min = Composer.MinIndex;
      var max = Composer.MaxIndex;
      var count = Composer.IndexLabelCount;
      var step = (Panel.IndexSize - Composer.IndexSpace * 2) / count;
      var change = (Composer.MaxIndex - Composer.MinIndex) / count;

      for (var i = 1; i < count; i++)
      {
        var index = Composer.IndexSpace + step * i;
        var content = ShowIndex(min + i * change);

        // T

        _point.Index = index;
        _point.Value = Composer.ValueSpace - Composer.ValueAxisSpace * 2;
        _shape.Location = LocationEnum.Center;

        Panel.CreateLabel(_point, _shape, content);

        // B

        _point.Index = index;
        _point.Value = Panel.ValueSize - Composer.ValueSpace + _shape.Size + Composer.ValueAxisSpace * 2;
        _shape.Location = LocationEnum.Center;

        Panel.CreateLabel(_point, _shape, content);
      }
    }

    /// <summary>
    /// Create index labels
    /// </summary>
    protected virtual void CreateValue()
    {
      ShowValue ??= Composer.ShowValue;

      var min = Composer.MinValue;
      var max = Composer.MaxValue;
      var count = Composer.ValueLabelCount;
      var step = (Panel.ValueSize - Composer.ValueSpace * 2) / count;
      var change = (Composer.MaxValue - Composer.MinValue) / count;

      for (var i = 1; i < count; i++)
      {
        var value = Composer.ValueSpace + step * i + _shape.Size / 2;
        var content = ShowValue(min + (count - i) * change);

        // L

        _point.Index = Composer.IndexSpace - Composer.IndexAxisSpace * 2;
        _point.Value = value;
        _shape.Location = LocationEnum.R;

        Panel.CreateLabel(_point, _shape, content);

        // R

        _point.Index = Panel.IndexSize - Composer.IndexSpace + Composer.IndexAxisSpace * 2;
        _point.Value = value;
        _shape.Location = LocationEnum.L;

        Panel.CreateLabel(_point, _shape, content);
      }
    }
  }
}
