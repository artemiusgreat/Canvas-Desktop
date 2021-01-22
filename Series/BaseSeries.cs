using Chart.ControlSpace;
using Chart.ModelSpace;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace Chart.SeriesSpace
{
  public interface ISeries
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
    /// Get Min and Max for the current point
    /// </summary>
    /// <param name="position"></param>
    /// <param name="series"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    double[] CreateDomain(int position, string series, IList<IInputModel> items);

    /// <summary>
    /// Create the shape
    /// </summary>
    /// <param name="position"></param>
    /// <param name="series"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    void CreateItem(int position, string series, IList<IInputModel> items);
  }

  public abstract class BaseSeries : ISeries
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
    /// Get data model
    /// </summary>
    /// <param name="position"></param>
    /// <param name="series"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public virtual dynamic GetModel(int position, string series, IList<IInputModel> items)
    {
      var pointModel = items.ElementAtOrDefault(position);

      if (pointModel == null || pointModel.Areas == null)
      {
        return null;
      }

      pointModel.Areas.TryGetValue(Composer.Name, out IAreaModel areaModel);

      if (areaModel == null || areaModel.Series == null)
      {
        return null;
      }

      areaModel.Series.TryGetValue(series, out ISeriesModel seriesModel);

      if (seriesModel == null || seriesModel.Model == null)
      {
        return null;
      }

      return seriesModel.Model;
    }

    /// <summary>
    /// Get Min and Max for the current point
    /// </summary>
    /// <param name="position"></param>
    /// <param name="series"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public virtual double[] CreateDomain(int position, string series, IList<IInputModel> items)
    {
      var currentModel = GetModel(position, series, items);

      if (currentModel == null)
      {
        return null;
      }

      return new double[]
      {
        currentModel.Point,
        currentModel.Point
      };
    }

    /// <summary>
    /// Create the shape
    /// </summary>
    /// <param name="position"></param>
    /// <param name="series"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public virtual void CreateItem(int position, string series, IList<IInputModel> items)
    {
    }
  }
}
