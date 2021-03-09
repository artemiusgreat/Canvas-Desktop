using Chart.ModelSpace;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Chart.SeriesSpace
{
  public class BarSeries : BaseSeries, ISeries
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

      if (currentModel?.Point == null)
      {
        return;
      }

      var size = Math.Max(position - (position - 1.0), 0.0) / 4;

      var shapeModel = new InputShapeModel
      {
        Size = 1,
        Color = currentModel.Color ?? Color
      };

      var points = new Point[]
      {
        Composer.GetPixels(Panel, position - size, currentModel.Point),
        Composer.GetPixels(Panel, position + size, currentModel.Point),
        Composer.GetPixels(Panel, position + size, 0.0),
        Composer.GetPixels(Panel, position - size, 0.0),
        Composer.GetPixels(Panel, position - size, currentModel.Point)
      };

      Panel.CreateShape(points, shapeModel);
    }
  }
}
