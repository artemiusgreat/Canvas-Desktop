using Chart.ModelSpace;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Chart.ShapeSpace
{
  public class ArrowShape : BaseShape, IShape
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

      var size = Math.Max(position - (position - 1.0), 0.0) / 4.0;
      var color = Brushes.Blue.Color;
      var direction = 1.0;

      if (currentModel.Open > currentModel.Close)
      {
        color = Brushes.Black.Color;
        direction = -1.0;
      }

      var shapeModel = new ShapeModel
      {
        Size = 1,
        Color = color
      };

      var points = new Point[]
      {
        Composer.GetPixels(Panel, position, currentModel.Point),
        Composer.GetPixels(Panel, position + size, currentModel.Point - size * direction * 4.0),
        Composer.GetPixels(Panel, position - size, currentModel.Point - size * direction * 4.0),
      };

      Panel.CreateShape(points, shapeModel);
    }
  }
}
