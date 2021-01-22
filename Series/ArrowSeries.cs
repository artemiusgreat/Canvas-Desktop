using Chart.ModelSpace;
using System.Collections.Generic;
using System.Windows;

namespace Chart.SeriesSpace
{
  public class ArrowSeries : BaseSeries, ISeries
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

      if (currentModel == null)
      {
        return;
      }

      var size = Panel.W / (Composer.MaxIndex - Composer.MinIndex) / 3;

      var shapeModel = new ShapeModel
      {
        Size = 1,
        Color = currentModel.Color ?? Color
      };

      var points = new Point[]
      {
        Composer.GetPixels(Panel, position, currentModel.Point),
        Composer.GetPixels(Panel, position, currentModel.Point),
        Composer.GetPixels(Panel, position, currentModel.Point)
      };

      points[0].Y -= size * currentModel.Direction;
      points[1].X += size;
      points[2].X -= size;

      Panel.CreateShape(points, shapeModel);
    }
  }
}
