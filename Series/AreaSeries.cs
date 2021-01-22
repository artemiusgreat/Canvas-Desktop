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
    public override void CreateItem(int position, string series, IList<IInputModel> items)
    {
      var currentModel = GetModel(position, series, items);
      var previousModel = GetModel(position - 1, series, items);

      if (currentModel == null || previousModel == null)
      {
        return;
      }

      var shapeModel = new ShapeModel
      {
        Size = 1,
        Color = Color
      };

      var points = new Point[]
      {
        Composer.GetPixels(Panel, position - 1, previousModel.Point),
        Composer.GetPixels(Panel, position, currentModel.Point),
        Composer.GetPixels(Panel, position, Composer.MinValue),
        Composer.GetPixels(Panel, position - 1, Composer.MinValue),
        Composer.GetPixels(Panel, position - 1, previousModel.Point)
      };

      Panel.CreateShape(points, shapeModel);
    }
  }
}
