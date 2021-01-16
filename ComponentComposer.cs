using Chart.ControlSpace;
using Chart.DecoratorSpace;
using Chart.EnumSpace;
using Chart.ModelSpace;
using Chart.ShapeSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Chart
{
  public class ComponentComposer
  {
    protected IList<int> _indexDomain = null;
    protected IList<double> _valueDomain = null;
    protected IList<IDecorator> _decorators = new List<IDecorator>();
    protected IList<IDecorator> _decoratorsUpdates = new List<IDecorator>();

    /// <summary>
    /// Properties
    /// </summary>
    public virtual string Name { get; set; }
    public virtual int StepSize { get; set; } = 5;
    public virtual int ValueCount { get; set; } = 5;
    public virtual int IndexCount { get; set; } = 100;
    public virtual int IndexLabelCount { get; set; } = 10;
    public virtual ChartControl Control { get; set; }
    public virtual IList<int> IndexDomain { get; set; }
    public virtual IList<double> ValueDomain { get; set; }
    public virtual IList<IInputModel> Items { get; set; } = new List<IInputModel>();
    public virtual IDictionary<string, IDictionary<string, IShape>> Groups { get; set; } = new Dictionary<string, IDictionary<string, IShape>>();
    public virtual Func<dynamic, dynamic> CreateLabel { get; set; }

    /// <summary>
    /// Getters
    /// </summary>
    public virtual int MinIndex => IndexDomain?.ElementAtOrDefault(0) ?? _indexDomain?.ElementAtOrDefault(0) ?? 0;
    public virtual int MaxIndex => IndexDomain?.ElementAtOrDefault(1) ?? _indexDomain?.ElementAtOrDefault(1) ?? IndexCount;
    public virtual int IndexStep => Math.Max((MaxIndex - MinIndex) / IndexLabelCount, 1);
    public virtual double MinValue => ValueDomain?.ElementAtOrDefault(0) ?? _valueDomain?.ElementAtOrDefault(0) ?? 0.0;
    public virtual double MaxValue => ValueDomain?.ElementAtOrDefault(1) ?? _valueDomain?.ElementAtOrDefault(1) ?? ValueCount;
    public virtual double ValueStep => MaxValue == MinValue ? 1 : (MaxValue - MinValue) / ValueCount;
    public virtual IList<int> AutoIndexDomain => _indexDomain;
    public virtual IList<double> AutoValueDomain => _valueDomain;

    /// <summary>
    /// Compose controls
    /// </summary>
    public virtual void Create()
    {
      var grid = new GridDecorator
      {
        Composer = this,
        Panel = Control.ViewArea.SetPanel("Grid", new CanvasImageControl()) as ICanvasControl
      };

      var axisT = new AxisDecorator
      {
        Composer = this,
        Position = PositionEnum.Top,
        Panel = Control.AxisT.SetPanel("AxisT", new CanvasImageControl()) as ICanvasControl
      };

      var axisB = new AxisDecorator
      {
        Composer = this,
        Position = PositionEnum.Bottom,
        Panel = Control.AxisB.SetPanel("AxisB", new CanvasImageControl()) as ICanvasControl
      };

      var axisL = new AxisDecorator
      {
        Composer = this,
        Position = PositionEnum.Left,
        Panel = Control.AxisL.SetPanel("AxisL", new CanvasImageControl()) as ICanvasControl
      };

      var axisR = new AxisDecorator
      {
        Composer = this,
        Position = PositionEnum.Right,
        Panel = Control.AxisR.SetPanel("AxisR", new CanvasImageControl()) as ICanvasControl
      };

      var labelsT = new LabelDecorator
      {
        Composer = this,
        Position = PositionEnum.Top,
        Panel = Control.AxisT.SetPanel("LabelsT", new CanvasImageControl()) as ICanvasControl
      };

      var labelsB = new LabelDecorator
      {
        Composer = this,
        Position = PositionEnum.Bottom,
        Panel = Control.AxisB.SetPanel("LabelsB", new CanvasImageControl()) as ICanvasControl
      };

      var labelsL = new LabelDecorator
      {
        Composer = this,
        Position = PositionEnum.Left,
        Panel = Control.AxisL.SetPanel("LabelsL", new CanvasImageControl()) as ICanvasControl
      };

      var labelsR = new LabelDecorator
      {
        Composer = this,
        Position = PositionEnum.Right,
        Panel = Control.AxisR.SetPanel("LabelsR", new CanvasImageControl()) as ICanvasControl
      };

      Control.ViewArea.SetPanel("Shapes", new CanvasImageControl());

      var cross = new CrossDecorator
      {
        Composer = this,
        Panel = Control.ViewArea.SetPanel("Cross", new CanvasControl()) as ICanvasControl,
        PanelL = Control.AxisL.SetPanel("CrossL", new CanvasControl()) as ICanvasControl,
        PanelR = Control.AxisR.SetPanel("CrossR", new CanvasControl()) as ICanvasControl,
        PanelT = Control.AxisT.SetPanel("CrossT", new CanvasControl()) as ICanvasControl,
        PanelB = Control.AxisB.SetPanel("CrossB", new CanvasControl()) as ICanvasControl
      };

      Control.ViewArea.GetPanel("Cross").Background = Brushes.Transparent;

      _decorators.Clear();
      _decorators.Add(grid);
      _decorators.Add(axisT);
      _decorators.Add(axisB);
      _decorators.Add(axisL);
      _decorators.Add(axisR);
      _decorators.Add(cross);
      _decoratorsUpdates.Add(labelsT);
      _decoratorsUpdates.Add(labelsB);
      _decoratorsUpdates.Add(labelsL);
      _decoratorsUpdates.Add(labelsR);

      foreach (var decorator in _decorators)
      {
        decorator.Panel.Clear();
        decorator.Create();
        decorator.Panel.Invalidate();
      }
    }

    /// <summary>
    /// Update delegate
    /// </summary>
    public virtual void Update()
    {
      var panel = Control.ViewArea.GetPanel("Shapes") as ICanvasControl;

      CreateIndexDomain();
      CreateValueDomain();

      foreach (var decorator in _decoratorsUpdates)
      {
        decorator.Panel.Clear();
        decorator.Update();
        decorator.Panel.Invalidate();
      }

      panel.Clear();

      foreach (var i in GetEnumerator())
      {
        var currentItem = Items.ElementAtOrDefault(i);

        if (currentItem == null)
        {
          continue;
        }

        Groups.TryGetValue(Name, out IDictionary<string, IShape> seriesItems);

        foreach (var series in seriesItems)
        {
          series.Value.Panel = panel;
          series.Value.Composer = this;
          series.Value.CreateShape(i, series.Key, Items);
        }
      }

      panel.Invalidate();
    }

    /// <summary>
    /// Enumerate
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerable<int> GetEnumerator()
    {
      var min = MinIndex;
      var max = MaxIndex;

      for (var i = min; i < max; i++)
      {
        yield return i;
      }
    }

    /// <summary>
    /// Create Min and Max domain 
    /// </summary>
    /// <returns></returns>
    public virtual IList<int> CreateIndexDomain()
    {
      _indexDomain ??= new int[2];
      _indexDomain[0] = 0;
      _indexDomain[1] = Math.Max(Items.Count, IndexCount);

      return _indexDomain;
    }

    /// <summary>
    /// Create Min and Max domain 
    /// </summary>
    /// <returns></returns>
    public virtual IList<double> CreateValueDomain()
    {
      var min = double.MaxValue;
      var max = double.MinValue;
      var panel = Control.ViewArea.GetPanel("Shapes") as ICanvasControl;

      foreach (var i in GetEnumerator())
      {
        var currentItem = Items.ElementAtOrDefault(i);

        if (currentItem == null)
        {
          continue;
        }

        Groups.TryGetValue(Name, out IDictionary<string, IShape> seriesItems);

        foreach (var series in seriesItems)
        {
          series.Value.Panel = panel;
          series.Value.Composer = this;

          var domain = series.Value.CreateDomain(i, series.Key, Items);

          if (domain != null)
          {
            min = Math.Min(min, domain[0]);
            max = Math.Max(max, domain[1]);
          }
        }
      }

      if (min > max)
      {
        return _valueDomain = null;
      }

      _valueDomain ??= new double[2];
      _valueDomain[0] = min;
      _valueDomain[1] = max;

      return _valueDomain;
    }

    /// <summary>
    /// Convert values to canvas coordinates
    /// </summary>
    /// <param name="panel"></param>
    /// <param name="input"></param>
    public virtual Point GetPixels(ICanvasControl panel, Point input)
    {
      // Clip

      input.X = Math.Min(Math.Max(MinIndex, input.X), MaxIndex);
      input.Y = Math.Min(Math.Max(MinValue, input.Y), MaxValue);

      // Convert to device pixels

      var deltaX = (input.X - MinIndex) / (MaxIndex - MinIndex);
      var deltaY = (input.Y - MinValue) / (MaxValue - MinValue);

      // Percentage to pixels, Y is inverted

      input.X = panel.W * deltaX;
      input.Y = panel.H - panel.H * deltaY;

      return input;
    }

    /// <summary>
    /// Transform coordinates
    /// </summary>
    /// <param name="panel"></param>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual Point GetPixels(ICanvasControl panel, double index, double value)
    {
      return GetPixels(panel, new Point(index, value));
    }

    /// <summary>
    /// Convert canvas coordinates to values
    /// </summary>
    /// <param name="panel"></param>
    /// <param name="input"></param>
    public virtual Point GetValues(ICanvasControl panel, Point input)
    {
      // Convert to values

      var deltaX = input.X / panel.W;
      var deltaY = input.Y / panel.H;

      // Percentage to values, Y is inverted

      input.X = MinIndex + (MaxIndex - MinIndex) * deltaX;
      input.Y = MaxValue - (MaxValue - MinValue) * deltaY;

      return input;
    }

    /// <summary>
    /// Format content
    /// </summary>
    public virtual string CreateContent(dynamic position)
    {
      return $"{position:0.00}";
    }
  }
}
