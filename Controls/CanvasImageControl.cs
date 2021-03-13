using Chart.ModelSpace;
using SkiaSharp;
using SkiaSharp.Views.WPF;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Chart.ControlSpace
{
  public class CanvasImageControl : CanvasControl
  {
    protected SKBitmap _bmp = null;
    protected SKCanvas _canvas = null;

    /// <summary>
    /// Canvas extention
    /// </summary>
    protected Visual _panel = new DrawingVisual();
    protected override int VisualChildrenCount => 1;
    protected override Visual GetVisualChild(int index) => _panel;

    /// <summary>
    /// Constructor
    /// </summary>
    public CanvasImageControl()
    {
      Loaded += (object sender, RoutedEventArgs e) =>
      {
        // Ensure that canvas doesn't exist

        RemoveVisualChild(_panel);
        RemoveLogicalChild(_panel);

        // Create canvas

        AddVisualChild(_panel);
        AddLogicalChild(_panel);
      };

      Unloaded += (object sender, RoutedEventArgs e) =>
      {
        RemoveVisualChild(_panel);
        RemoveLogicalChild(_panel);
      };
    }

    /// <summary>
    /// Create line
    /// </summary>
    /// <param name="pointSource"></param>
    /// <param name="pointDestination"></param>
    /// <param name="shapeModel"></param>
    public override void CreateLine(Point pointSource, Point pointDestination, IInputShapeModel shapeModel)
    {
      CreateCanvas();

      var pen = new SKPaint
      {
        Style = SKPaintStyle.Stroke,
        Color = shapeModel.Color.ToSKColor(),
        StrokeWidth = (float)shapeModel.Size
      };

      _canvas.DrawLine(
        (float)pointSource.X,
        (float)pointSource.Y,
        (float)pointDestination.X,
        (float)pointDestination.Y,
        pen);

      pen.Dispose();
    }

    /// <summary>
    /// Create circle
    /// </summary>
    /// <param name="point"></param>
    /// <param name="shapeModel"></param>
    public override void CreateCircle(Point point, IInputShapeModel shapeModel)
    {
      CreateCanvas();

      var pen = new SKPaint
      {
        Style = SKPaintStyle.Fill,
        Color = shapeModel.Color.ToSKColor()
      };

      _canvas.DrawCircle(
        (float)point.X,
        (float)point.Y,
        (float)shapeModel.Size,
        pen);

      pen.Dispose();
    }

    /// <summary>
    /// Create shape
    /// </summary>
    /// <param name="points"></param>
    /// <param name="shapeModel"></param>
    public override void CreateShape(IList<Point> points, IInputShapeModel shapeModel)
    {
      CreateCanvas();

      var shape = new SKPath();
      var origin = points.ElementAtOrDefault(0);

      if (origin == default)
      {
        return;
      }

      shape.MoveTo((float)origin.X, (float)origin.Y);

      for (var i = 1; i < points.Count; i++)
      {
        shape.LineTo((float)points[i].X, (float)points[i].Y);
      }

      shape.Close();

      var pen = new SKPaint
      {
        Style = SKPaintStyle.Fill,
        Color = shapeModel.Color.ToSKColor()
      };

      _canvas.DrawPath(shape, pen);

      pen.Dispose();
      shape.Dispose();
    }

    /// <summary>
    /// Create label
    /// </summary>
    /// <param name="point"></param>
    /// <param name="content"></param>
    /// <param name="shapeModel"></param>
    public override void CreateLabel(Point point, string content, IInputShapeModel shapeModel)
    {
      CreateCanvas();

      var position = SKTextAlign.Center;

      switch (shapeModel.Position)
      {
        case TextAlignment.Left: position = SKTextAlign.Left; break;
        case TextAlignment.Right: position = SKTextAlign.Right; break;
      }

      var pen = new SKPaint
      {
        TextAlign = position,
        TextSize = (float)shapeModel.Size,
        Color = shapeModel.Color.ToSKColor()
      };

      _canvas.DrawText(
        content,
        (float)point.X,
        (float)point.Y,
        pen);

      pen.Dispose();
    }

    /// <summary>
    /// Clear canvas
    /// </summary>
    public override void Clear()
    {
      CreateCanvas();

      _bmp.Erase(SKColors.Transparent);
      _canvas.Clear(SKColors.Transparent);
    }

    /// <summary>
    /// Invalidate
    /// </summary>
    public override void Invalidate()
    {
      CreateCanvas();

      using (var ctx = (_panel as DrawingVisual).RenderOpen())
      {
        ctx.DrawImage(_bmp.ToWriteableBitmap(), new Rect(0, 0, W, H));
      }
    }

    /// <summary>
    /// Create canvas
    /// </summary>
    protected void CreateCanvas()
    {
      var width = (int)(W + 1);
      var height = (int)(H + 1);

      if (width != _bmp?.Width || height != _bmp?.Height)
      {
        _canvas = new SKCanvas(_bmp = new SKBitmap(width, height));
      }
    }
  }
}
