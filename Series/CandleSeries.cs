using Chart.ModelSpace;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Chart.SeriesSpace
{
  public class CandleSeries : BaseSeries, ISeries
  {
    /// <summary>
    /// Get Min and Max for the current point
    /// </summary>
    /// <param name="position"></param>
    /// <param name="series"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public override double[] CreateDomain(int position, string series, IList<IInputModel> items)
    {
      var currentModel = GetModel(position, series, items);

      if (currentModel == null)
      {
        return null;
      }

      return new double[]
      {
        currentModel.Low ?? currentModel.Point,
        currentModel.High ?? currentModel.Point
      };
    }

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

      var L = currentModel.Low ?? currentModel.Point;
      var H = currentModel.High ?? currentModel.Point;
      var O = currentModel.Open ?? currentModel.Point;
      var C = currentModel.Close ?? currentModel.Point;
      var size = Math.Max(position - (position - 1.0), 0.0) / 4.0;
      var upSide = Math.Max(O, C);
      var downSide = Math.Min(O, C);

      var shapeModel = new InputShapeModel
      {
        Size = 1,
        Color = currentModel.Color ?? Color
      };

      var points = new Point[]
      {
        Composer.GetPixels(Panel, position - size, upSide),
        Composer.GetPixels(Panel, position + size, upSide),
        Composer.GetPixels(Panel, position + size, downSide),
        Composer.GetPixels(Panel, position - size, downSide),
        Composer.GetPixels(Panel, position - size, upSide)
      };

      Panel.CreateShape(points, shapeModel);
      Panel.CreateLine(
        Composer.GetPixels(Panel, position, L),
        Composer.GetPixels(Panel, position, H),
        shapeModel);
    }
  }
}
