using Chart.ModelSpace;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Chart.ShapeSpace
{
  public class BarShape : BaseShape, IShape
  {
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

      var size = Math.Max(position - (position - 1.0), 0.0) / 4;

      var shapeModel = new ShapeModel
      {
        Size = 1,
        Color = Brushes.Black.Color
      };

      var points = new Point[]
      {
        Composer.GetPixels(Panel, position - size, currentModel.Point),
        Composer.GetPixels(Panel, position + size, currentModel.Point),
        Composer.GetPixels(Panel, position + size, Composer.MinValue),
        Composer.GetPixels(Panel, position - size, Composer.MinValue),
        Composer.GetPixels(Panel, position - size, currentModel.Point)
      };

      Panel.CreateShape(points, shapeModel);
    }
  }
}
