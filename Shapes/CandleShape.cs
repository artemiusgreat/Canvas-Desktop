using Chart.ModelSpace;
using System;
using System.Collections.Generic;
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
    public override void CreateShape(int position, string series, IList<IInputModel> items)
    {
      var currentModel = GetModel(position, series, items);

      if (currentModel == null)
      {
        return;
      }

      var size = Math.Max(position - (position - 1.0), 0.0) / 4.0;
      var upSide = Math.Max(currentModel.Open, currentModel.Close);
      var downSide = Math.Min(currentModel.Open, currentModel.Close);
      var color = Brushes.Green.Color;

      if (currentModel.Open > currentModel.Close)
      {
        color = Brushes.Red.Color;
      }

      var shapeModel = new ShapeModel
      {
        Size = 1,
        Color = color
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
