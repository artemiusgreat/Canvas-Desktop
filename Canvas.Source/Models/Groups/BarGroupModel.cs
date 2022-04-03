using Canvas.Source.ControlSpace;
using SkiaSharp;
using System;
using System.Collections.Generic;

namespace Canvas.Source.ModelSpace
{
  public class BarGroupModel : GroupModel, IGroupModel
  {
    /// <summary>
    /// Render the shape
    /// </summary>
    /// <param name="position"></param>
    /// <param name="name"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public override void CreateShape(int position, string name, IList<IGroupModel> items)
    {
      var currentModel = Composer.GetGroup(position, name, items);

      if (currentModel?.Point is null)
      {
        return;
      }

      var size = 1.0 / 4.0;

      var points = new IPointModel[]
      {
        Composer.GetPixels(Panel, position - size, currentModel.Point),
        Composer.GetPixels(Panel, position + size, currentModel.Point),
        Composer.GetPixels(Panel, position + size, 0.0),
        Composer.GetPixels(Panel, position - size, 0.0),
        Composer.GetPixels(Panel, position - size, currentModel.Point)
      };

      Color = currentModel.Color ?? Color;

      Panel.CreateShape(points, this);
    }
  }
}
