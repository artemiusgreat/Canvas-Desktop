using Chart.ModelSpace;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Chart.SeriesSpace
{
  public class ArrowSeries : BaseSeries, ISeries
  {
    /// <summary>
    /// Negative color
    /// </summary>
    public virtual Color ColorUp { get; set; } = Brushes.Black.Color;

    /// <summary>
    /// Positive color
    /// </summary>
    public virtual Color ColorDown { get; set; } = Brushes.Blue.Color;

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
        Color = currentModel.Direction < 0 ? ColorDown : ColorUp
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
