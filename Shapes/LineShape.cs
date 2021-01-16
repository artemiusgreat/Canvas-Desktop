using Chart.ModelSpace;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace Chart.ShapeSpace
{
  public class LineShape : BaseShape, IShape
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
      var previousModel = GetModel(position - 1, series, items);

      if (currentModel == null || previousModel == null)
      {
        return;
      }

      var shapeModel = new ShapeModel
      {
        Size = 1,
        Color = Brushes.Black.Color
      };

      Panel.CreateLine(
        Composer.GetPixels(Panel, position - 1, previousModel.Point),
        Composer.GetPixels(Panel, position, currentModel.Point),
        shapeModel);
    }
  }
}
