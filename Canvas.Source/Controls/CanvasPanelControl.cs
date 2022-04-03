using Canvas.Source.EnumSpace;
using Canvas.Source.ModelSpace;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Canvas.Source.ControlSpace
{
  public class CanvasPanelControl : CanvasControl, ICanvasControl
  {
    /// <summary>
    /// Reusable shape geometry
    /// </summary>
    protected SKPath _shapeRoute = null;

    /// <summary>
    /// Reusable pen for lines
    /// </summary>
    protected SKPaint _penLine = null;

    /// <summary>
    /// Reusable pen for boxes
    /// </summary>
    protected SKPaint _penBox = null;

    /// <summary>
    /// Reusable pen for custom shapes
    /// </summary>
    protected SKPaint _penShape = null;

    /// <summary>
    /// Reusable pen for labels
    /// </summary>
    protected SKPaint _penLabel = null;

    /// <summary>
    /// Reusable pen for circles
    /// </summary>
    protected SKPaint _penCircle = null;

    /// <summary>
    /// Reusable pen for measures
    /// </summary>
    protected SKPaint _penMeasure = null;

    /// <summary>
    /// Line styles
    /// </summary>
    protected IList<SKPathEffect> _shapeStyles = null;

    /// <summary>
    /// Drawing surface
    /// </summary>
    public SKCanvas Panel { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public CanvasPanelControl()
    {
      _shapeRoute = new SKPath();

      _shapeStyles = new List<SKPathEffect>
      {
        SKPathEffect.CreateDash(new float[] { 1, 3 }, 0),
        SKPathEffect.CreateDash(new float[] { 3, 3 }, 0)
      };

      _penLine = new SKPaint
      {
        Color = SKColors.Black,
        Style = SKPaintStyle.Stroke,
        FilterQuality = SKFilterQuality.High,
        IsAntialias = true,
        IsStroke = false,
        IsDither = false,
        StrokeWidth = 1
      };

      _penCircle = new SKPaint
      {
        Color = SKColors.Black,
        Style = SKPaintStyle.Fill,
        FilterQuality = SKFilterQuality.High,
        IsAntialias = false,
        IsStroke = false,
        IsDither = false
      };

      _penBox = new SKPaint
      {
        Color = SKColors.Black,
        Style = SKPaintStyle.Fill,
        FilterQuality = SKFilterQuality.High,
        IsAntialias = false,
        IsStroke = false,
        IsDither = false
      };

      _penShape = new SKPaint
      {
        Color = SKColors.Black,
        Style = SKPaintStyle.Fill,
        FilterQuality = SKFilterQuality.High,
        IsAntialias = false,
        IsStroke = false,
        IsDither = false
      };

      _penLabel = new SKPaint
      {
        Color = SKColors.Black,
        TextAlign = SKTextAlign.Center,
        FilterQuality = SKFilterQuality.High,
        IsAntialias = true,
        IsStroke = false,
        IsDither = false,
        TextSize = 10
      };

      _penMeasure = new SKPaint
      {
        TextSize = 10
      };
    }

    /// <summary>
    /// Create line
    /// </summary>
    /// <param name="points"></param>
    /// <param name="shape"></param>
    public override void CreateLine(IList<IPointModel> points, IShapeModel shape)
    {
      _penLine.Color = shape.Color.Value;
      _penLine.Style = SKPaintStyle.Stroke;
      _penLine.StrokeWidth = (float)shape.Size;

      switch (shape.LineShape)
      {
        case LineShapeEnum.Dots: _penLine.PathEffect = _shapeStyles[0]; break;
        case LineShapeEnum.Dashes: _penLine.PathEffect = _shapeStyles[1]; break;
      }

      Panel.DrawLine(
        (float)points[0].Index,
        (float)points[0].Value,
        (float)points[1].Index,
        (float)points[1].Value,
        _penLine);
    }

    /// <summary>
    /// Create circle
    /// </summary>
    /// <param name="point"></param>
    /// <param name="shape"></param>
    public override void CreateCircle(IPointModel point, IShapeModel shape)
    {
      _penCircle.Color = shape.Color.Value;
      _penCircle.Style = SKPaintStyle.Fill;

      Panel.DrawCircle(
        (float)point.Index,
        (float)point.Value,
        (float)shape.Size,
        _penCircle);
    }

    /// <summary>
    /// Create box
    /// </summary>
    /// <param name="points"></param>
    /// <param name="shape"></param>
    public override void CreateBox(IList<IPointModel> points, IShapeModel shape)
    {
      _penCircle.Color = shape.Color.Value;
      _penCircle.Style = SKPaintStyle.Fill;

      Panel.DrawRect(
        (float)points[0].Index,
        (float)points[0].Value,
        (float)(points[1].Index - points[0].Index),
        (float)(points[1].Value - points[0].Value),
        _penCircle);
    }

    /// <summary>
    /// Create shape
    /// </summary>
    /// <param name="points"></param>
    /// <param name="shape"></param>
    public override void CreateShape(IList<IPointModel> points, IShapeModel shape)
    {
      var origin = points.ElementAtOrDefault(0);

      if (origin is null)
      {
        return;
      }

      _shapeRoute.Reset();
      _shapeRoute.MoveTo((float)origin.Index.Value, (float)origin.Value);

      for (var i = 1; i < points.Count; i++)
      {
        _shapeRoute.LineTo((float)points[i].Index.Value, (float)points[i].Value);
      }

      _shapeRoute.Close();

      _penShape.Color = shape.Color.Value;
      _penShape.Style = SKPaintStyle.Fill;

      Panel.DrawPath(_shapeRoute, _penShape);
    }

    /// <summary>
    /// Create label
    /// </summary>
    /// <param name="point"></param>
    /// <param name="shape"></param>
    /// <param name="content"></param>
    public override void CreateLabel(IPointModel point, IShapeModel shape, string content)
    {
      _penLabel.Color = shape.Color.Value;
      _penLabel.TextSize = (float)shape.Size;
      _penLabel.TextAlign = SKTextAlign.Center;

      switch (shape.Location)
      {
        case LocationEnum.L: _penLabel.TextAlign = SKTextAlign.Left; break;
        case LocationEnum.R: _penLabel.TextAlign = SKTextAlign.Right; break;
      }

      var space = (_penLabel.FontSpacing - _penLabel.TextSize) / 2;

      Panel.DrawText(
        content,
        (float)point.Index,
        (float)(point.Value - space),
        _penLabel);
    }

    /// <summary>
    /// Measure content
    /// </summary>
    /// <param name="content"></param>
    /// <param name="size"></param>
    public override IPointModel GetContentMeasure(string content, double size)
    {
      _penMeasure.TextSize = (float)size;

      return new PointModel
      {
        Index = content.Length * _penMeasure.FontMetrics.MaxCharacterWidth,
        Value = _penMeasure.FontSpacing
      };
    }

    /// <summary>
    /// Clear canvas
    /// </summary>
    public override void Clear()
    {
      Panel.Clear(SKColors.Transparent);
    }

    /// <summary>
    /// Dispose
    /// </summary>
    public override void Dispose()
    {
      _penLine?.Dispose();
      _penLabel?.Dispose();
      _penShape?.Dispose();
      _penCircle?.Dispose();
      _penMeasure?.Dispose();
      _shapeRoute?.Dispose();
      _shapeStyles.ForEach(x => x.Dispose());

      _penLine = null;
      _penLabel = null;
      _penShape = null;
      _penCircle = null;
      _penMeasure = null;
      _shapeRoute = null;
      _shapeStyles = null;

      base.Dispose();
    }
  }
}
