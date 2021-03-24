using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Chart.ControlSpace
{
  /// <summary>
  /// Interaction logic
  /// </summary>
  public partial class ChartControl : UserControl
  {
    /// <summary>
    /// Drag-N-Drop position
    /// </summary>
    private Point? _mousePosition = null;

    /// <summary>
    /// Current composer
    /// </summary>
    public virtual ComponentComposer Composer { get; set; }

    /// <summary>
    /// Composers
    /// </summary>
    public virtual IList<ComponentComposer> Composers { get; set; } = new List<ComponentComposer>();

    /// <summary>
    /// Constructor
    /// </summary>
    public ChartControl()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Size event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnSize(object sender, SizeChangedEventArgs e)
    {
      Composer?.Create();
      Composer?.Update();
    }

    /// <summary>
    /// Mouse wheel event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnWheel(object sender, MouseWheelEventArgs e)
    {
      SetIndexScale(e.Delta / 120, Keyboard.IsKeyDown(Key.LeftShift));
    }

    /// <summary>
    /// Vertical drag and resize event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnMouseMoveOnValueAxis(object sender, MouseEventArgs e)
    {
      if (e.LeftButton == MouseButtonState.Pressed)
      {
        SetValueScale(e.GetPosition(sender as Panel));
        return;
      }

      _mousePosition = null;
    }


    /// <summary>
    /// Horizontal drag and resize event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnMouseMoveOnIndexAxis(object sender, MouseEventArgs e)
    {
      if (e.LeftButton == MouseButtonState.Pressed)
      {
        var position = e.GetPosition(sender as Panel);

        if (_mousePosition == null)
        {
          _mousePosition = position;
          return;
        }

        var delta = _mousePosition.Value.X - position.X;

        _mousePosition = position;

        if (delta > 0)
        {
          SetIndexScale(-1, true);
          return;
        }

        if (delta < 0)
        {
          SetIndexScale(1, true);
          return;
        }

        return;
      }

      _mousePosition = null;
    }

    /// <summary>
    /// Double clck event in the view area
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnMouseDownView(object sender, MouseButtonEventArgs e)
    {
      if (e.ClickCount == 2)
      {
        Composer.ValueDomain = null;
        Composer.Update();
      }
    }

    /// <summary>
    /// Value scale
    /// </summary>
    /// <param name="position"></param>
    private void SetValueScale(Point position)
    {
      if (_mousePosition == null)
      {
        _mousePosition = position;
        return;
      }

      var delta = _mousePosition.Value.Y - position.Y;

      _mousePosition = position;

      Composer.ValueDomain ??= new List<double>(Composer.AutoValueDomain);

      if (delta < 0)
      {
        Composer.ValueDomain[0] -= Math.Abs(delta);
        Composer.ValueDomain[1] += Math.Abs(delta);
        Composer.Update();
        return;
      }

      var isNoFlip = Composer.ValueDomain[1] - Composer.ValueDomain[0] > Math.Abs(delta) * 2;

      if (delta > 0 && isNoFlip)
      {
        Composer.ValueDomain[0] += Math.Abs(delta);
        Composer.ValueDomain[1] -= Math.Abs(delta);
        Composer.Update();
        return;
      }
    }

    /// <summary>
    /// Index scale
    /// </summary>
    /// <param name="delta"></param>
    /// <param name="isZoom"></param>
    private void SetIndexScale(int delta, bool isZoom = false)
    {
      Composer.IndexDomain ??= new int[2];

      var increment = Composer.IndexLabelCount.Value / 2 * delta;
      var isInRange = Composer.IndexDomain[1] - Composer.IndexDomain[0] - increment * 2 > increment * 2;

      if (isZoom && isInRange)
      {
        Composer.IndexDomain[0] += increment;
        Composer.IndexDomain[1] -= increment;
        Composer.Update();
        Sync();
        return;
      }

      if (delta > 0)
      {
        Composer.IndexDomain[0] += Math.Abs(increment);
        Composer.IndexDomain[1] += Math.Abs(increment);
        Composer.Update();
        Sync();
        return;
      }

      if (delta < 0)
      {
        Composer.IndexDomain[0] -= Math.Abs(increment);
        Composer.IndexDomain[1] -= Math.Abs(increment);
        Composer.Update();
        Sync();
        return;
      }
    }

    /// <summary>
    /// Sync controls
    /// </summary>
    private void Sync()
    {
      foreach (var composer in Composers)
      {
        if (Equals(Composer.Name, composer.Name) == false)
        {
          composer.IndexDomain ??= new int[2];
          composer.IndexDomain[0] = Composer.IndexDomain[0];
          composer.IndexDomain[1] = Composer.IndexDomain[1];
          composer.Update();
        }
      }
    }
  }
}
