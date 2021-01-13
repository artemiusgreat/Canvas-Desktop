namespace Chart.ModelSpace
{
  public interface ISeriesModel : IModel
  {
    /// <summary>
    /// Name
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// Model that may contain arbitrary data needed to draw the shape
    /// </summary>
    dynamic Model { get; set; }
  }

  public class SeriesModel : BaseModel, ISeriesModel
  {
    /// <summary>
    /// Name
    /// </summary>
    public virtual string Name { get; set; }

    /// <summary>
    /// Model that may contain arbitrary data needed to draw the shape
    /// </summary>
    public virtual dynamic Model { get; set; } = new BaseModel();
  }
}
