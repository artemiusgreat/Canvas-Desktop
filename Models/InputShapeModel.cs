using System.Windows;
using System.Windows.Media;

namespace Chart.ModelSpace
{
  public interface IInputShapeModel : IModel
  {
    /// <summary>
    /// Size
    /// </summary>
    double Size { get; set; }

    /// <summary>
    /// Color
    /// </summary>
    Color Color { get; set; }

    /// <summary>
    /// Position
    /// </summary>
    TextAlignment Position { get; set; }
  }

  public class InputShapeModel : BaseModel, IInputShapeModel
  {
    /// <summary>
    /// Size
    /// </summary>
    public virtual double Size { get; set; }

    /// <summary>
    /// Color
    /// </summary>
    public virtual Color Color { get; set; }

    /// <summary>
    /// Position
    /// </summary>
    public virtual TextAlignment Position { get; set; }
  }
}
