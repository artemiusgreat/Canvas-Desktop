using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using Avalonia.Threading;
using Canvas.Source;
using Canvas.Source.ControlSpace;
using Canvas.Source.DecoratorSpace;
using Canvas.Source.EnumSpace;
using Canvas.Source.ModelSpace;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;

namespace Canvas.Views.Avalonia
{
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
    /// Immediate width
    /// </summary>
    public virtual double CurrentWidth => Bounds.Width;

    /// <summary>
    /// Immediate height
    /// </summary>
    public virtual double CurrentHeight => Bounds.Height;

    /// <summary>
    /// Visuals
    /// </summary>
    protected bool _setup = false;
    protected double _canvasWidth = 0;
    protected double _canvasHeight = 0;
    protected SKPaint _paint = null;
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
    /// Skia context
    /// </summary>
    class CanvasContext : ICustomDrawOperation
    {
      public virtual Rect Bounds => View.Bounds;
      public virtual CanvasView View { get; set; }
      public virtual bool HitTest(Point p) => false;
      public virtual bool Equals(ICustomDrawOperation other) => false;

      /// <summary>
      /// Custom render
      /// </summary>
      /// <param name="context"></param>
      public virtual void Render(IDrawingContextImpl context)
      {
        var index = View.CurrentWidth;
        var value = View.CurrentHeight;

        if (Equals(View._canvasWidth, index) is false || Equals(View._canvasHeight, value) is false)
        {
          View._canvasWidth = index;
          View._canvasHeight = value;
          View.Create();
        }

        var exView = new SKRect(0, 0,
          (float)Math.Floor(View.CurrentWidth),
          (float)Math.Floor(View.CurrentHeight));

        var inView = new SKRect(
          (float)View.Composer.IndexSpace,
          (float)View.Composer.ValueSpace,
          (float)(exView.Width + View.Composer.IndexSpace),
          (float)(exView.Height + View.Composer.ValueSpace));

        var ctx = context as ISkiaDrawingContextImpl;

        if (ctx is not null)
        {
          ctx.SkCanvas.DrawBitmap(View._imageLines, exView, exView, View._paint);
          ctx.SkCanvas.DrawBitmap(View._imageLabels, exView, exView, View._paint);
          ctx.SkCanvas.DrawBitmap(View._imageGroups, exView, inView, View._paint);
          ctx.SkCanvas.DrawBitmap(View._imageSingles, exView, inView, View._paint);
          ctx.SkCanvas.DrawBitmap(View._imageCross, exView, exView, View._paint);
        }
      }

      /// <summary>
      /// Dispose
      /// </summary>
      public virtual void Dispose()
      {
      }
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public CanvasView()
    {
      AvaloniaXamlLoader.Load(this);

      _paint = new SKPaint 
      { 
        FilterQuality = SKFilterQuality.High
      };

      Dispatcher.UIThread.InvokeAsync(() => 
      {
        PointerMoved += OnMouseMove;
        PointerWheelChanged += OnWheel;

      }, DispatcherPriority.ContextIdle);
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
    /// Render event
    /// </summary>
    /// <param name="context"></param>
    public override void Render(DrawingContext context)
    {
      if (_setup is false)
      {
        return;
      }

      context.Custom(new CanvasContext
      {
        View = this
      });

      context.DrawLine(new Pen(Brushes.Black), new Point(0, 0), new Point(0, 0));

      base.Render(context);
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
    public void OnUnload(object sender, RoutedEventArgs e)
    {
      Dispose();
    }

    /// <summary>
    /// Size event
    /// </summary>
    /// <param name="availableSize"></param>
    /// <returns></returns>
    protected override Size MeasureOverride(Size availableSize)
    {
      //var response = base.MeasureOverride(availableSize);

      //_currentWidth = availableSize.Width;
      //_currentHeight = availableSize.Height;

      //Create();

      return availableSize;
    }

    /// <summary>
    /// Mouse wheel event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnWheel(object sender, PointerWheelEventArgs e)
    {
      var isZoom = Equals(e.KeyModifiers, KeyModifiers.Shift);

      switch (true)
      {
        case true when e.Delta.Y > 0: _ = isZoom ? Composer.ZoomIndexScale(1) : Composer.PanIndexScale(-1); break;
        case true when e.Delta.Y < 0: _ = isZoom ? Composer.ZoomIndexScale(-1) : Composer.PanIndexScale(1); break;
      }

      Update();

      Domains.OnNext(Composer);
    }

    /// <summary>
    /// Horizontal drag and resize event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnMouseMove(object sender, PointerEventArgs e)
    {
      var point = e.GetPosition(this);

      var position = new PointModel
      {
        Index = point.X,
        Value = point.Y
      };

      _crossDecorator.Update(position.Index.Value, position.Value);

      if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
      {
        if (_mouse is null)
        {
          _mouse = position;
          return;
        }

        var deltaX = _mouse.Index - position.Index;
        var deltaY = _mouse.Value - position.Value;
        var isZoom = Equals(e.KeyModifiers, KeyModifiers.Shift);

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
    protected void OnMouseDown(object sender, PointerEventArgs e)
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
    protected void OnMouseLeave(object sender, PointerEventArgs e)
    {
      _crossDecorator.Panel.Clear();
    }

    /// <summary>
    /// Create canvas
    /// </summary>
    protected void CreateCanvas()
    {
      Dispose();

      var exW = (int)Math.Max(1, Math.Floor(CurrentWidth));
      var exH = (int)Math.Max(1, Math.Floor(CurrentHeight));
      var inW = (int)Math.Max(1, Math.Floor(CurrentWidth - Composer.IndexSpace * 2));
      var inH = (int)Math.Max(1, Math.Floor(CurrentHeight - Composer.ValueSpace * 2));

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