using Chart.ModelSpace;
using System.Collections.Generic;
using System.Windows;

namespace Chart.SeriesSpace
{
  public class AreaSeries : BaseSeries, ISeries
  {
    /// <summary>
    /// Render the shape
    /// </summary>
    /// <param name="position"></param>
    /// <param name="series"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public override void CreateItem(int position, string series, IList<IPointModel> items)
    {
      var currentModel = GetModel(position, series, items);
      var previousModel = GetModel(position - 1, series, items);

      if (currentModel?.Point == null || previousModel?.Point == null)
      {
        return;
      }

      var shapeModel = new ShapeModel
      {
        Size = 1,
        Color = currentModel.Color ?? Color
      };

      var points = new Point[]
      {
        Composer.GetPixels(Panel, position - 1, previousModel.Point),
        Composer.GetPixels(Panel, position, currentModel.Point),
        Composer.GetPixels(Panel, position, 0.0),
        Composer.GetPixels(Panel, position - 1, 0.0),
        Composer.GetPixels(Panel, position - 1, previousModel.Point)
      };

      Panel.CreateShape(points, shapeModel);
    }
  }
}
