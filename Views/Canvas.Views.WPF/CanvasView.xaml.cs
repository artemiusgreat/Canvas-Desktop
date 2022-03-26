using Core;
using Core.ControlSpace;
using Core.DecoratorSpace;
using Core.EnumSpace;
using Core.ModelSpace;
using SkiaSharp;
using SkiaSharp.Views.WPF;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Canvas.Views.WPF
{
  /// <summary>
  /// Interaction logic
  /// </summary>
  public partial class CanvasView : UserControl
  {
    /// <summary>
    /// Composer
    /// </summary>
    public virtual Composer Composer { get; set; }

    /// <summary>
    /// Observable domain changes
    /// </summary>
    public virtual Subject<Composer> Domains { get; protected set; } = new Subject<Composer>();

    /// <summary>
    /// Visuals
    /// </summary>
    protected bool _setup = false;
    protected double _canvasWidth = 0;
    protected double _canvasHeight = 0;
    protected SKBitmap _imageCross = null;
    protected SKBitmap _imageLines = null;
    protected SKBitmap _imageLabels = null;
    protected SKBitmap _imageGroups = null;
    protected SKBitmap _imageSingles = null;
    protected SKCanvas _canvasCross = null;
    protected SKCanvas _canvasLines = null;
    protected SKCanvas _canvasLabels = null;
    protected SKCanvas _canvasGroups = null;
    protected SKCanvas _canvasSingles = null;
    protected CrossDecorator _crossDecorator = null;
    protected IPointModel _mouse = null;

    /// <summary>
    /// Constructor
    /// </summary>
    public CanvasView()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Create component
    /// </summary>
    public virtual void Create()
    {
      _setup = true;

      CreateCanvas();
      Composer.Create();
      Composer.Update();
      InvalidateVisual();
    }

    /// <summary>
    /// Create component
    /// </summary>
    public virtual void Update()
    {
      _canvasLabels.Clear();
      _canvasGroups.Clear();
      _canvasSingles.Clear();

      Composer.Update();
      InvalidateVisual();
    }

    /// <summary>
    /// Load event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnLoad(object sender, RoutedEventArgs e)
    {
    }

    /// <summary>
    /// Render event
    /// </summary>
    /// <param name="context"></param>
    protected override void OnRender(DrawingContext context)
    {
      if (_setup is false)
      {
        return;
      }

      base.OnRender(context);

      var index = ActualWidth;
      var value = ActualHeight;

      if (Equals(_canvasWidth, index) is false || Equals(_canvasHeight, value) is false)
      {
        _canvasWidth = index;
        _canvasHeight = value;
        Create();
      }

      var exView = new Rect(0, 0,
        Math.Floor(index),
        Math.Floor(value));

      var inView = new Rect(
        Composer.IndexSpace,
        Composer.ValueSpace,
        exView.Width - Composer.IndexSpace * 2,
        exView.Height - Composer.ValueSpace * 2);

      context.DrawImage(_imageLines.ToWriteableBitmap(), exView);
      context.DrawImage(_imageLabels.ToWriteableBitmap(), exView);
      context.DrawImage(_imageGroups.ToWriteableBitmap(), inView);
      context.DrawImage(_imageSingles.ToWriteableBitmap(), inView);
      context.DrawImage(_imageCross.ToWriteableBitmap(), exView);
    }

    /// <summary>
    /// Dispose
    /// </summary>
    protected void Dispose()
    {
      _imageCross?.Dispose();
      _imageLines?.Dispose();
      _imageLabels?.Dispose();
      _imageGroups?.Dispose();
      _imageSingles?.Dispose();
      _canvasCross?.Dispose();
      _canvasLines?.Dispose();
      _canvasLabels?.Dispose();
      _canvasGroups?.Dispose();
      _canvasSingles?.Dispose();

      _imageCross = null;
      _imageLines = null;
      _imageLabels = null;
      _imageGroups = null;
      _imageSingles = null;
      _canvasCross = null;
      _canvasLines = null;
      _canvasLabels = null;
      _canvasGroups = null;
      _canvasSingles = null;
    }

    /// <summary>
    /// Unload event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnUnload(object sender, RoutedEventArgs e)
    {
      Dispose();
    }

    /// <summary>
    /// Size event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnSize(object sender, SizeChangedEventArgs e)
    {
      //Create();
    }

    /// <summary>
    /// Mouse wheel event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnWheel(object sender, MouseWheelEventArgs e)
    {
      var isZoom = Keyboard.IsKeyDown(Key.LeftShift);

      switch (true)
      {
        case true when e.Delta > 0: _ = isZoom ? Composer.ZoomIndexScale(1) : Composer.PanIndexScale(-1); break;
        case true when e.Delta < 0: _ = isZoom ? Composer.ZoomIndexScale(-1) : Composer.PanIndexScale(1); break;
      }

      Update();
      
      Domains.OnNext(Composer);
    }

    /// <summary>
    /// Horizontal drag and resize event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnMouseMove(object sender, MouseEventArgs e)
    {
      var point = e.GetPosition(this);

      var position = new PointModel 
      {
        Index = point.X,
        Value = point.Y
      };

      _crossDecorator.Update(position.Index.Value, position.Value);

      if (e.LeftButton == MouseButtonState.Pressed)
      {
        if (_mouse is null)
        {
          _mouse = position;
          return;
        }

        var deltaX = _mouse.Index - position.Index;
        var deltaY = _mouse.Value - position.Value;
        var isZoom = Keyboard.IsKeyDown(Key.LeftShift);

        _mouse = position;

        switch (true)
        {
          case true when deltaX > 0: _ = isZoom ? Composer.ZoomIndexScale(-1) : Composer.PanIndexScale(1); break;
          case true when deltaX < 0: _ = isZoom ? Composer.ZoomIndexScale(1) : Composer.PanIndexScale(-1); break;
        }

        switch (true)
        {
          case true when deltaY > 0: Composer.ZoomValueScale(-1); break;
          case true when deltaY < 0: Composer.ZoomValueScale(1); break;
        }
      }
      else
      {
        _mouse = null;
      }

      Update();
      
      Domains.OnNext(Composer);
    }

    /// <summary>
    /// Double click event in the view area
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
      Composer.ValueDomain = null;

      Update();

      Domains.OnNext(Composer);
    }

    /// <summary>
    /// Mouse leave event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnMouseLeave(object sender, MouseEventArgs e)
    {
      _crossDecorator.Panel.Clear();
    }

    /// <summary>
    /// Create canvas
    /// </summary>
    protected void CreateCanvas()
    {
      Dispose();

      var exW = (int)Math.Max(1, Math.Floor(ActualWidth));
      var exH = (int)Math.Max(1, Math.Floor(ActualHeight));
      var inW = (int)Math.Max(1, Math.Floor(ActualWidth - Composer.IndexSpace * 2));
      var inH = (int)Math.Max(1, Math.Floor(ActualHeight - Composer.ValueSpace * 2));

      _imageCross = new SKBitmap(exW, exH);
      _imageLines = new SKBitmap(exW, exH);
      _imageLabels = new SKBitmap(exW, exH);
      _imageGroups = new SKBitmap(inW, inH);
      _imageSingles = new SKBitmap(inW, inH);
      _canvasCross = new SKCanvas(_imageCross);
      _canvasLines = new SKCanvas(_imageLines);
      _canvasLabels = new SKCanvas(_imageLabels);
      _canvasGroups = new SKCanvas(_imageGroups);
      _canvasSingles = new SKCanvas(_imageSingles);

      Composer.Panels = new Dictionary<PanelEnum, ICanvasControl>
      {
        [PanelEnum.Cross] = new CanvasPanelControl { Name = Name, Panel = _canvasCross, IndexSize = exW, ValueSize = exH },
        [PanelEnum.Lines] = new CanvasPanelControl { Name = Name, Panel = _canvasLines, IndexSize = exW, ValueSize = exH },
        [PanelEnum.Labels] = new CanvasPanelControl { Name = Name, Panel = _canvasLabels, IndexSize = exW, ValueSize = exH },
        [PanelEnum.Groups] = new CanvasPanelControl { Name = Name, Panel = _canvasGroups, IndexSize = inW, ValueSize = inH },
        [PanelEnum.Singles] = new CanvasPanelControl { Name = Name, Panel = _canvasSingles, IndexSize = inW, ValueSize = inH }
      };

      _crossDecorator = new CrossDecorator
      {
        Composer = Composer,
        Panel = Composer.Panels[PanelEnum.Cross]
      };
    }
  }
}
