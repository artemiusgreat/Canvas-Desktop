using Chart.ModelSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Chart.ShapeSpace
{
  public class CandleShape : BaseShape, IShape
  {
    /// <summary>
    /// Get Min and Max for the current point
    /// </summary>
    /// <param name="position"></param>
    /// <param name="series"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public override double[] CreateDomain(int position, string series, IList<IPointModel> items)
    {
      var currentItem = items.ElementAtOrDefault(position);

      if (currentItem == null)
      {
        return null;
      }

      return new double[]
      {
        currentItem.Areas[Composer.Name].Series[series].Model.Low ?? currentItem.Areas[Composer.Name].Series[series].Model.Point,
        currentItem.Areas[Composer.Name].Series[series].Model.High ?? currentItem.Areas[Composer.Name].Series[series].Model.Point
      };
    }

    /// <summary>
    /// Render the shape
    /// </summary>
    /// <param name="position"></param>
    /// <param name="series"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public override void CreateShape(int position, string series, IList<IPointModel> items)
    {
      var currentItem = items.ElementAtOrDefault(position);

      if (currentItem == null)
      {
        return;
      }

      var size = Math.Max(position - (position - 1.0), 0.0) / 4.0;
      var currentModel = currentItem.Areas[Composer.Name].Series[series].Model;
      var upSide = Math.Max(currentModel.Open, currentModel.Close);
      var downSide = Math.Min(currentModel.Open, currentModel.Close);

      var shapeModel = new ShapeModel
      {
        Size = 1,
        Color = currentModel.Open < currentModel.Close ? Brushes.Green.Color : Brushes.Red.Color
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
        Composer.GetPixels(Panel, position, currentModel.Low),
        Composer.GetPixels(Panel, position, currentModel.High),
        shapeModel);
    }
  }
}
