using System.Collections.Generic;

namespace Chart.ModelSpace
{
  public interface IAreaModel : IModel
  {
    /// <summary>
    /// Name
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// Series
    /// </summary>
    IDictionary<string, ISeriesModel> Series { get; set; }
  }

  public class AreaModel : BaseModel, IAreaModel
  {
    /// <summary>
    /// Name
    /// </summary>
    public virtual string Name { get; set; }

    /// <summary>
    /// Series
    /// </summary>
    public virtual IDictionary<string, ISeriesModel> Series { get; set; } = new Dictionary<string, ISeriesModel>();
  }
}
