using Chart.SeriesSpace;

namespace Chart.ModelSpace
{
  public interface IInputSeriesModel : IModel
  {
    /// <summary>
    /// Name
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// Chart type
    /// </summary>
    ISeries Shape { get; set; }

    /// <summary>
    /// Model that may contain arbitrary data needed to draw the shape
    /// </summary>
    dynamic Model { get; set; }
  }

  public class InputSeriesModel : BaseModel, IInputSeriesModel
  {
    /// <summary>
    /// Name
    /// </summary>
    public virtual string Name { get; set; }

    /// <summary>
    /// Chart type
    /// </summary>
    public virtual ISeries Shape { get; set; }

    /// <summary>
    /// Model that may contain arbitrary data needed to draw the shape
    /// </summary>
    public virtual dynamic Model { get; set; } = new BaseModel();
  }
}
