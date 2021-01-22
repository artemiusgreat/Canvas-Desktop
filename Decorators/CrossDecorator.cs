using Chart.ControlSpace;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Chart.DecoratorSpace
{
  public class CrossDecorator : BaseDecorator, IDecorator
  {
    private Line _lineX = null;
    private Line _lineY = null;
    private Border _boxL = null;
    private Border _boxR = null;
    private Border _boxT = null;
    private Border _boxB = null;

    /// <summary>
    /// Label background color
    /// </summary>
    public virtual Color ColorBack { get; set; } = Brushes.DarkGray.Color;

    /// <summary>
    /// Label color
    /// </summary>
    public virtual Color ColorFront { get; set; } = Brushes.White.Color;

    /// <summary>
    /// Label size
    /// </summary>
    public virtual int FontSize { get; set; } = 10;

    /// <summary>
    /// Custom label renderer
    /// </summary>
    public virtual Func<dynamic, dynamic> ShowIndex { get; set; }

    /// <summary>
    /// Custom label renderer
    /// </summary>
    public virtual Func<dynamic, dynamic> ShowValue { get; set; }

    /// <summary>
    /// Panels
    /// </summary>
    public virtual ICanvasControl PanelL { get; set; }
    public virtual ICanvasControl PanelR { get; set; }
    public virtual ICanvasControl PanelT { get; set; }
    public virtual ICanvasControl PanelB { get; set; }

    /// <summary>
    /// Create cross lines
    /// </summary>
    public override void CreateDelegate()
    {
      ShowIndex ??= Composer.ShowIndex;
      ShowValue ??= Composer.ShowValue;

      // Cross lines

      var canvas = Panel as Canvas;

      canvas.MouseMove -= OnMouseMove;
      canvas.MouseMove += OnMouseMove;
      canvas.MouseLeave -= OnMouseLeave;
      canvas.MouseLeave += OnMouseLeave;

      canvas.Children.Clear();
      canvas.Children.Add(_lineX = CreateLine());
      canvas.Children.Add(_lineY = CreateLine());

      // Cross labels

      var canvasL = PanelL as Canvas;
      var canvasR = PanelR as Canvas;
      var canvasT = PanelT as Canvas;
      var canvasB = PanelB as Canvas;

      canvasL.Children.Clear();
      canvasR.Children.Clear();
      canvasT.Children.Clear();
      canvasB.Children.Clear();
      canvasL.Children.Add(_boxL = CreateLabel(PanelL, HorizontalAlignment.Right) as Border);
      canvasR.Children.Add(_boxR = CreateLabel(PanelR, HorizontalAlignment.Left) as Border);
      canvasT.Children.Add(_boxT = CreateLabel(null, HorizontalAlignment.Center) as Border);
      canvasB.Children.Add(_boxB = CreateLabel(null, HorizontalAlignment.Center) as Border);

      Canvas.SetTop(_boxL, -1000);
      Canvas.SetTop(_boxR, -1000);
      Canvas.SetTop(_boxT, canvasT.Height - _boxT.Height - Composer.StepSize);
      Canvas.SetTop(_boxB, Composer.StepSize);

      Canvas.SetLeft(_boxT, -1000);
      Canvas.SetLeft(_boxB, -1000);
    }

    /// <summary>
    /// Create line
    /// </summary>
    /// <returns></returns>
    private Line CreateLine()
    {
      return new Line
      {
        X1 = 0,
        X2 = 0,
        Y1 = 0,
        Y2 = 0,
        StrokeThickness = 1,
        Stroke = new SolidColorBrush(Color),
        StrokeDashArray = new DoubleCollection(new[] { 5.0, 5.0 })
      };
    }

    /// <summary>
    /// Create label
    /// </summary>
    /// <param name="panel"></param>
    /// <param name="posH"></param>
    /// <returns></returns>
    private FrameworkElement CreateLabel(ICanvasControl panel, HorizontalAlignment posH)
    {
      var step = Composer.StepSize;
      var label = new TextBlock
      {
        Text = "0.0",
        FontSize = FontSize,
        VerticalAlignment = VerticalAlignment.Center,
        HorizontalAlignment = posH,
        Foreground = new SolidColorBrush(ColorFront),
        Margin = new Thickness(0),
        Padding = new Thickness(0)
      };

      var container = new Border
      {
        Height = FontSize + step,
        Child = label,
        ClipToBounds = true,
        Margin = new Thickness(0),
        Padding = new Thickness(0),
        BorderThickness = new Thickness(0),
        Background = new SolidColorBrush(ColorBack)
      };

      if (panel != null)
      {
        container.Width = panel.W;
      }

      switch (posH)
      {
        case HorizontalAlignment.Left: container.Padding = new Thickness(step * 2, 0, 0, 0); break;
        case HorizontalAlignment.Right: container.Padding = new Thickness(0, 0, step * 2, 0); break;
        case HorizontalAlignment.Center: container.Padding = new Thickness(step, 0, step, 0); break;
      }

      return container;
    }

    /// <summary>
    /// Mouse leave event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnMouseLeave(object sender, MouseEventArgs e)
    {
      _lineX.X1 = _lineX.X2 = _lineX.Y1 = _lineX.Y2 = -1000;
      _lineY.X1 = _lineY.X2 = _lineY.Y1 = _lineY.Y2 = -1000;

      Canvas.SetTop(_boxL, -1000);
      Canvas.SetTop(_boxR, -1000);
      Canvas.SetLeft(_boxT, -1000);
      Canvas.SetLeft(_boxB, -1000);
    }

    /// <summary>
    /// Mouse move event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnMouseMove(object sender, MouseEventArgs e)
    {
      var canvas = Panel as Canvas;
      var position = e.GetPosition(canvas);
      var values = Composer.GetValues(Panel, position);

      _lineX.X1 = position.X;
      _lineX.X2 = position.X;
      _lineX.Y1 = 0;
      _lineX.Y2 = canvas.Height;

      _lineY.X1 = 0;
      _lineY.X2 = canvas.Width;
      _lineY.Y1 = position.Y;
      _lineY.Y2 = position.Y;

      (_boxL.Child as TextBlock).Text = ShowValue(values.Y);
      (_boxR.Child as TextBlock).Text = ShowValue(values.Y);
      (_boxT.Child as TextBlock).Text = ShowIndex(values.X);
      (_boxB.Child as TextBlock).Text = ShowIndex(values.X);

      Canvas.SetTop(_boxL, position.Y - _boxL.ActualHeight / 2);
      Canvas.SetTop(_boxR, position.Y - _boxR.ActualHeight / 2);
      Canvas.SetLeft(_boxT, position.X - _boxT.ActualWidth / 2);
      Canvas.SetLeft(_boxB, position.X - _boxB.ActualWidth / 2);
    }
  }
}
