using Core.ModelSpace;
using SkiaSharp;
using System.Collections.Generic;

namespace Core.DecoratorSpace
{
  public class LinesDecorator : BaseDecorator, IDecorator
  {
    /// <summary>
    /// Reusable shape model
    /// </summary>
    protected IShapeModel _shape = null;

    /// <summary>
    /// Reusable points model
    /// </summary>
    protected IList<IPointModel> _points = null;

    /// <summary>
    /// Constructor
    /// </summary>
    public LinesDecorator()
    {
      _shape = new ShapeModel
      {
        Size = 1,
        Color = new SKColor(230, 230, 230)
      };

      _points = new IPointModel[]
      {
        new PointModel { Index = 0, Value = 0 },
        new PointModel { Index = 0, Value = 0 }
      };
    }

    /// <summary>
    /// Create component
    /// </summary>
    public override void CreateDelegate()
    {
      _shape = Composer.LineShape ?? _shape;

      CreateIndex();
      CreateValue();
    }

    /// <summary>
    /// Create index
    /// </summary>
    protected virtual void CreateIndex()
    {
      var count = Composer.IndexLabelCount;
      var step = (Panel.IndexSize - Composer.IndexSpace * 2) / count;

      _points[0].Value = Composer.ValueSpace;
      _points[1].Value = Panel.ValueSize - Composer.ValueSpace;

      _points[0].Index = Composer.IndexSpace;
      _points[1].Index = Composer.IndexSpace;

      Panel.CreateLine(_points, _shape);

      _points[0].Index = Composer.IndexSpace + step * count;
      _points[1].Index = Composer.IndexSpace + step * count;

      Panel.CreateLine(_points, _shape);

      _points[0].Value = Composer.ValueSpace - Composer.ValueAxisSpace;
      _points[1].Value = Panel.ValueSize - Composer.ValueSpace + Composer.ValueAxisSpace;

      for (var i = 1; i < count; i++)
      {
        _points[0].Index = Composer.IndexSpace + step * i;
        _points[1].Index = Composer.IndexSpace + step * i;

        Panel.CreateLine(_points, _shape);
      }
    }

    /// <summary>
    /// Create value
    /// </summary>
    protected virtual void CreateValue()
    {
      var count = Composer.ValueLabelCount;
      var step = (Panel.ValueSize - Composer.ValueSpace * 2) / count;

      _points[0].Index = Composer.IndexSpace;
      _points[1].Index = Panel.IndexSize - Composer.IndexSpace;

      _points[0].Value = Composer.ValueSpace;
      _points[1].Value = Composer.ValueSpace;

      Panel.CreateLine(_points, _shape);

      _points[0].Value = Composer.ValueSpace + step * count;
      _points[1].Value = Composer.ValueSpace + step * count;

      Panel.CreateLine(_points, _shape);

      _points[0].Index = Composer.IndexSpace - Composer.IndexAxisSpace;
      _points[1].Index = Panel.IndexSize - Composer.IndexSpace + Composer.IndexAxisSpace;

      for (var i = 1; i < count; i++)
      {
        _points[0].Value = Composer.ValueSpace + step * i;
        _points[1].Value = Composer.ValueSpace + step * i;

        Panel.CreateLine(_points, _shape);
      }
    }
  }
}
