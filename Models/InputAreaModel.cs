using System.Collections.Generic;

namespace Chart.ModelSpace
{
  public interface IInputAreaModel : IModel
  {
    /// <summary>
    /// Name
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// Series
    /// </summary>
    IDictionary<string, IInputSeriesModel> Series { get; set; }
  }

  public class InputAreaModel : BaseModel, IInputAreaModel
  {
    /// <summary>
    /// Name
    /// </summary>
    public virtual string Name { get; set; }

    /// <summary>
    /// Series
    /// </summary>
    public virtual IDictionary<string, IInputSeriesModel> Series { get; set; } = new Dictionary<string, IInputSeriesModel>();
  }
}
