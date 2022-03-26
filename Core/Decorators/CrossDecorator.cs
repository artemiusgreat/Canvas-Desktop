using Core.EnumSpace;
using Core.ModelSpace;
using SkiaSharp;
using System;
using System.Collections.Generic;

namespace Core.DecoratorSpace
{
  public class CrossDecorator : BaseDecorator, IDecorator
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
    /// Reusable line shape model
    /// </summary>
    protected IShapeModel _shapeLine = null;

    /// <summary>
    /// Reusable label shape model
    /// </summary>
    protected IShapeModel _shapeLabel = null;

    /// <summary>
    /// Reusable label points model
    /// </summary>
    protected IPointModel _pointLabel = null;

    /// <summary>
    /// Reusable points model
    /// </summary>
    protected IList<IPointModel> _points = null;

    /// <summary>
    /// Constructor
    /// </summary>
    public CrossDecorator()
    {
      _shapeLine = new ShapeModel
      {
        Size = 1,
        Location = LocationEnum.Center,
        LineShape = LineShapeEnum.Dashes,
        Color = new SKColor(70, 70, 70)
      };

      _shapeLabel = new ShapeModel
      {
        Size = 10,
        Location = LocationEnum.Center,
        Color = new SKColor(255, 255, 255)
      };

      _pointLabel = new PointModel
      {
        Index = 0,
        Value = 0
      };

      _points = new IPointModel[]
      {
        new PointModel { Index = 0, Value = 0 },
        new PointModel { Index = 0, Value = 0 }
      };
    }

    /// <summary>
    /// Create cross decorator
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    public new void Update(double index, double value)
    {
      Panel.Clear();

      var isInRange =
        index > Composer.IndexSpace &&
        index < Panel.IndexSize - Composer.IndexSpace &&
        value > Composer.ValueSpace &&
        value < Panel.ValueSize - Composer.ValueSpace;

      if (isInRange)
      {
        ShowIndex ??= Composer.ShowIndex;
        ShowValue ??= Composer.ShowValue;

        _shapeLine = Composer.LineShape ?? _shapeLine;
        _shapeLabel = Composer.LabelShape ?? _shapeLabel;

        _pointLabel.Index = index;
        _pointLabel.Value = value;

        var scale = Composer.GetValues(Panel, _pointLabel);

        CreateLines(index, value);
        CreateBoxes(index, value, scale);
        CreateLabels(index, value, scale);
      }
    }

    /// <summary>
    /// Create lines
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    protected virtual void CreateLines(double index, double value)
    {
      // Index

      _points[0].Index = Composer.IndexSpace;
      _points[0].Value = value;
      _points[1].Index = Panel.IndexSize - Composer.IndexSpace;
      _points[1].Value = value;

      Panel.CreateLine(_points, _shapeLine);

      // Value

      _points[0].Index = index;
      _points[0].Value = Composer.ValueSpace;
      _points[1].Index = index;
      _points[1].Value = Panel.ValueSize - Composer.ValueSpace;

      Panel.CreateLine(_points, _shapeLine);
    }

    /// <summary>
    /// Create lines
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="scale"></param>
    protected virtual void CreateBoxes(double index, double value, IPointModel scale)
    {
      var content = ShowIndex(scale.Index);
      var contentMeasure = Panel.GetContentMeasure(content, _shapeLabel.Size.Value);
      var indexSize = _shapeLabel.Size * content.Length / 2;
      var valueSize = contentMeasure.Value / 2;

      // L

      _points[0].Index = 0;
      _points[0].Value = value - valueSize;
      _points[1].Index = Composer.IndexSpace;
      _points[1].Value = value + valueSize;

      Panel.CreateBox(_points, _shapeLine);

      // R

      _points[0].Index = Panel.IndexSize - Composer.IndexSpace;
      _points[0].Value = value - valueSize;
      _points[1].Index = Panel.IndexSize;
      _points[1].Value = value + valueSize;

      Panel.CreateBox(_points, _shapeLine);

      // T

      var baseT = Composer.ValueSpace - Composer.ValueAxisSpace * 2 - _shapeLabel.Size / 2;

      _points[0].Index = index - indexSize;
      _points[0].Value = baseT - valueSize;
      _points[1].Index = index + indexSize;
      _points[1].Value = baseT + valueSize;

      Panel.CreateBox(_points, _shapeLine);

      // B

      var baseB = Panel.ValueSize - Composer.ValueSpace + Composer.ValueAxisSpace * 2 + _shapeLabel.Size / 2;

      _points[0].Index = index - indexSize;
      _points[0].Value = baseB - valueSize;
      _points[1].Index = index + indexSize;
      _points[1].Value = baseB + valueSize;

      Panel.CreateBox(_points, _shapeLine);
    }

    /// <summary>
    /// Create labels
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="scale"></param>
    protected virtual void CreateLabels(double index, double value, IPointModel scale)
    {
      // L

      _points[0].Index = Composer.IndexSpace - Composer.IndexAxisSpace * 2;
      _points[0].Value = value + _shapeLabel.Size / 2;
      _shapeLabel.Location = LocationEnum.R;

      Panel.CreateLabel(_points[0], _shapeLabel, ShowValue(scale.Value));

      // R

      _points[0].Index = Panel.IndexSize - Composer.IndexSpace + Composer.IndexAxisSpace * 2;
      _points[0].Value = value + _shapeLabel.Size / 2;
      _shapeLabel.Location = LocationEnum.L;

      Panel.CreateLabel(_points[0], _shapeLabel, ShowValue(scale.Value));

      // T 

      _points[0].Index = index;
      _points[0].Value = Composer.ValueSpace - Composer.ValueAxisSpace * 2;
      _shapeLabel.Location = LocationEnum.Center;

      Panel.CreateLabel(_points[0], _shapeLabel, ShowIndex(scale.Index));

      // B

      _points[0].Index = index;
      _points[0].Value = Panel.ValueSize - Composer.ValueSpace + Composer.ValueAxisSpace * 2 + _shapeLabel.Size;
      _shapeLabel.Location = LocationEnum.Center;

      Panel.CreateLabel(_points[0], _shapeLabel, ShowIndex(scale.Index));
    }
  }
}
