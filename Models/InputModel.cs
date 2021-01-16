using System;
using System.Collections.Generic;

namespace Chart.ModelSpace
{
  public interface IInputModel : IModel
  {
    /// <summary>
    /// Time
    /// </summary>
    DateTime Time { get; set; }

    /// <summary>
    /// Areas
    /// </summary>
    IDictionary<string, IAreaModel> Areas { get; set; }
  }

  public class InputModel : BaseModel, IInputModel
  {
    /// <summary>
    /// Time
    /// </summary>
    public virtual DateTime Time { get; set; }

    /// <summary>
    /// Areas
    /// </summary>
    public virtual IDictionary<string, IAreaModel> Areas { get; set; } = new Dictionary<string, IAreaModel>();
  }
}
