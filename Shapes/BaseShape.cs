using Chart.ControlSpace;
using System.Windows.Media;

namespace Chart.ShapeSpace
{
  public interface IShape
  {
    /// <summary>
    /// Color
    /// </summary>
    Color Color { get; set; }

    /// <summary>
    /// Reference to composer
    /// </summary>
    ComponentComposer Composer { get; set; }

    /// <summary>
    /// Panel
    /// </summary>
    ICanvasControl Panel { get; set; }

    /// <summary>
    /// Create the shape
    /// </summary>
    /// <returns></returns>
    void UpdateShape();
  }

  public abstract class BaseShape : IShape
  {
    /// <summary>
    /// Color
    /// </summary>
    public virtual Color Color { get; set; } = Brushes.Black.Color;

    /// <summary>
    /// Reference to composer
    /// </summary>
    public virtual ComponentComposer Composer { get; set; }

    /// <summary>
    /// Panel
    /// </summary>
    public virtual ICanvasControl Panel { get; set; }

    /// <summary>
    /// Create the shape
    /// </summary>
    /// <returns></returns>
    public virtual void UpdateShape()
    {
    }
  }
}
