using Core.ControlSpace;
using SkiaSharp;
using System;

namespace Core.DecoratorSpace
{
  public interface IDecorator : IDisposable
  {
    /// <summary>
    /// Create shape
    /// </summary>
    Action Create { get; set; }

    /// <summary>
    /// Update shape
    /// </summary>
    Action Update { get; set; }

    /// <summary>
    /// Composer
    /// </summary>
    Composer Composer { get; set; }

    /// <summary>
    /// Panel
    /// </summary>
    ICanvasControl Panel { get; set; }

    /// <summary>
    /// Create delegate
    /// </summary>
    void CreateDelegate();

    /// <summary>
    /// Update delegate
    /// </summary>
    void UpdateDelegate();
  }

  public abstract class BaseDecorator : IDecorator
  {
    /// <summary>
    /// Create shape
    /// </summary>
    public virtual Action Create { get; set; }

    /// <summary>
    /// Update shape
    /// </summary>
    public virtual Action Update { get; set; }

    /// <summary>
    /// Composer
    /// </summary>
    public virtual Composer Composer { get; set; }

    /// <summary>
    /// Panel
    /// </summary>
    public virtual ICanvasControl Panel { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public BaseDecorator()
    {
      Create = CreateDelegate;
      Update = UpdateDelegate;
    }

    /// <summary>
    /// Create delegate
    /// </summary>
    public virtual void CreateDelegate()
    {
    }

    /// <summary>
    /// Update delegate
    /// </summary>
    public virtual void UpdateDelegate()
    {
    }

    /// <summary>
    /// Dispose
    /// </summary>
    public virtual void Dispose()
    {
    }
  }
}
