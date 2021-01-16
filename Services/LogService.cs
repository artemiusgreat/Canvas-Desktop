using Serilog;

namespace Chart.ServiceSpace
{
  public sealed class LogService
  {
    /// <summary>
    /// Constructor call
    /// </summary>
    private static readonly LogService _setup = new LogService();

    /// <summary>
    /// Constructor
    /// </summary>
    static LogService()
    {
    }

    /// <summary>
    /// Constructor
    /// </summary>
    private LogService() => Log.Logger = new LoggerConfiguration()
      .MinimumLevel.Debug()
      .Enrich.FromLogContext()
      .WriteTo.Debug()
      .CreateLogger();

    /// <summary>
    /// Reference to logger's instance
    /// </summary>
    public static ILogger Instance => Log.Logger;
  }
}
