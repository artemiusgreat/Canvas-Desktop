using Chart.ControlSpace;
using Chart.ModelSpace;
using System.Collections.Generic;
using System.Linq;

namespace Chart.ShapeSpace
{
  public interface IShape
  {
    /// <summary>
    /// Reference to composer
    /// </summary>
    ComponentComposer Composer { get; set; }

    /// <summary>
    /// Panel
    /// </summary>
    ICanvasControl Panel { get; set; }

    /// <summary>
    /// Get Min and Max for the current point
    /// </summary>
    /// <param name="position"></param>
    /// <param name="series"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    double[] CreateDomain(int position, string series, IList<IPointModel> items);

    /// <summary>
    /// Create the shape
    /// </summary>
    /// <param name="position"></param>
    /// <param name="series"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    void CreateShape(int position, string series, IList<IPointModel> items);
  }

  public abstract class BaseShape : IShape
  {
    /// <summary>
    /// Reference to composer
    /// </summary>
    public virtual ComponentComposer Composer { get; set; }

    /// <summary>
    /// Panel
    /// </summary>
    public virtual ICanvasControl Panel { get; set; }

    /// <summary>
    /// Get Min and Max for the current point
    /// </summary>
    /// <param name="position"></param>
    /// <param name="series"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public virtual double[] CreateDomain(int position, string series, IList<IPointModel> items)
    {
      var currentItem = items.ElementAtOrDefault(position);

      if (currentItem == null)
      {
        return null;
      }

      return new double[]
      {
        currentItem.Areas[Composer.Name].Series[series].Model.Point,
        currentItem.Areas[Composer.Name].Series[series].Model.Point
      };
    }

    /// <summary>
    /// Create the shape
    /// </summary>
    /// <param name="position"></param>
    /// <param name="series"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public virtual void CreateShape(int position, string series, IList<IPointModel> items)
    {
    }
  }
}
