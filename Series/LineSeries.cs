using Chart.ModelSpace;
using System.Collections.Generic;

namespace Chart.SeriesSpace
{
  public class LineSeries : BaseSeries, ISeries
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
        Color = currentModel.Color ?? Color
      };

      Panel.CreateLine(
        Composer.GetPixels(Panel, position - 1, previousModel.Point),
        Composer.GetPixels(Panel, position, currentModel.Point),
        shapeModel);
    }
  }
}
