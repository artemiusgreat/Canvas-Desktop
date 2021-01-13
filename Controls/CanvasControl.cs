using Chart.ModelSpace;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Chart.ControlSpace
{
  public interface ICanvasControl
  {
    /// <summary>
    /// Width
    /// </summary>
    double W { get; }

    /// <summary>
    /// Height
    /// </summary>
    double H { get; }

    /// <summary>
    /// Create line
    /// </summary>
    /// <param name="pointSource"></param>
    /// <param name="pointDestination"></param>
    /// <param name="shapeModel"></param>
    void CreateLine(Point pointSource, Point pointDestination, IShapeModel shapeModel);

    /// <summary>
    /// Create circle
    /// </summary>
    /// <param name="point"></param>
    /// <param name="shapeModel"></param>
    void CreateCircle(Point point, IShapeModel shapeModel);

    /// <summary>
    /// Create shape
    /// </summary>
    /// <param name="points"></param>
    /// <param name="shapeModel"></param>
    void CreateShape(IList<Point> points, IShapeModel shapeModel);

    /// <summary>
    /// Create label
    /// </summary>
    /// <param name="point"></param>
    /// <param name="content"></param>
    /// <param name="shapeModel"></param>
    void CreateLabel(Point point, string content, IShapeModel shapeModel);

    /// <summary>
    /// Invalidate canvas
    /// </summary>
    void Invalidate();

    /// <summary>
    /// Clear canvas
    /// </summary>
    void Clear();
  }

  public class CanvasControl : Canvas, ICanvasControl
  {
    /// <summary>
    /// Width
    /// </summary>
    public double W => Width;

    /// <summary>
    /// Height
    /// </summary>
    public double H => Height;

    /// <summary>
    /// Create line
    /// </summary>
    /// <param name="pointSource"></param>
    /// <param name="pointDestination"></param>
    /// <param name="shapeModel"></param>
    public virtual void CreateLine(Point pointSource, Point pointDestination, IShapeModel shapeModel)
    {
    }

    /// <summary>
    /// Create circle
    /// </summary>
    /// <param name="point"></param>
    /// <param name="shapeModel"></param>
    public virtual void CreateCircle(Point point, IShapeModel shapeModel)
    {
    }

    /// <summary>
    /// Create shape
    /// </summary>
    /// <param name="points"></param>
    /// <param name="shapeModel"></param>
    public virtual void CreateShape(IList<Point> points, IShapeModel shapeModel)
    {
    }

    /// <summary>
    /// Create label
    /// </summary>
    /// <param name="point"></param>
    /// <param name="content"></param>
    /// <param name="shapeModel"></param>
    public virtual void CreateLabel(Point point, string content, IShapeModel shapeModel)
    {
    }

    /// <summary>
    /// Invalidate canvas
    /// </summary>
    public virtual void Invalidate()
    {
    }

    /// <summary>
    /// Clear canvas
    /// </summary>
    public virtual void Clear()
    {
    }
  }
}
