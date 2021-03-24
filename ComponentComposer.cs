using Chart.ControlSpace;
using Chart.DecoratorSpace;
using Chart.EnumSpace;
using Chart.ModelSpace;
using Chart.ShapeSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Chart
{
  public class ComponentComposer
  {
    protected IList<int> _indexDomain = null;
    protected IList<double> _valueDomain = null;
    protected IList<IShape> _shapes = new List<IShape>();
    protected IList<IDecorator> _decorators = new List<IDecorator>();
    protected IList<IDecorator> _decoratorsUpdates = new List<IDecorator>();

    /// <summary>
    /// Properties
    /// </summary>
    public virtual string Name { get; set; }
    public virtual int? StepSize { get; set; } = 5;
    public virtual int? ValueCount { get; set; } = 6;
    public virtual int? IndexCount { get; set; } = 100;
    public virtual int? IndexLabelCount { get; set; } = 10;
    public virtual double? ValueSpace { get; set; }
    public virtual ChartControl Control { get; set; }
    public virtual IAreaModel Group { get; set; } = new AreaModel();
    public virtual IList<int> IndexDomain { get; set; }
    public virtual IList<double> ValueDomain { get; set; }
    public virtual IList<IPointModel> Items { get; set; } = new List<IPointModel>();
    public virtual Func<dynamic, dynamic> ShowIndexAction { get; set; }
    public virtual Func<dynamic, dynamic> ShowValueAction { get; set; }

    /// <summary>
    /// Getters
    /// </summary>
    public virtual int MinIndex => IndexDomain?.ElementAtOrDefault(0) ?? _indexDomain?.ElementAtOrDefault(0) ?? 0;
    public virtual int MaxIndex => IndexDomain?.ElementAtOrDefault(1) ?? _indexDomain?.ElementAtOrDefault(1) ?? IndexCount.Value;
    public virtual int IndexStep => Math.Max((MaxIndex - MinIndex) / IndexLabelCount.Value, 1);
    public virtual double MinValue => ValueDomain?.ElementAtOrDefault(0) ?? _valueDomain?.ElementAtOrDefault(0) ?? 0.0;
    public virtual double MaxValue => ValueDomain?.ElementAtOrDefault(1) ?? _valueDomain?.ElementAtOrDefault(1) ?? ValueCount.Value;
    public virtual double ValueStep => MaxValue == MinValue ? 1 : (MaxValue - MinValue) / ValueCount.Value;
    public virtual IList<int> AutoIndexDomain => _indexDomain;
    public virtual IList<double> AutoValueDomain => _valueDomain;

    /// <summary>
    /// Compose controls
    /// </summary>
    public virtual void Create()
    {
      CreateDecorators();
      CreateSeries();
      CreateShapes();
    }

    /// <summary>
    /// Create decorators
    /// </summary>
    public virtual void CreateDecorators()
    {
      var grid = new GridDecorator
      {
        Composer = this,
        Color = Brushes.LightGray.Color,
        Panel = Control.View.GetPanel(nameof(PanelEnum.Grid), new CanvasControl()) as ICanvasControl
      };

      var axisT = new AxisDecorator
      {
        Composer = this,
        Position = PositionEnum.T,
        Color = Brushes.DarkGray.Color,
        Panel = Control.AxisT.GetPanel(nameof(PanelEnum.AxisT), new CanvasControl()) as ICanvasControl
      };

      var axisB = new AxisDecorator
      {
        Composer = this,
        Position = PositionEnum.B,
        Color = Brushes.DarkGray.Color,
        Panel = Control.AxisB.GetPanel(nameof(PanelEnum.AxisB), new CanvasControl()) as ICanvasControl
      };

      var axisL = new AxisDecorator
      {
        Composer = this,
        Position = PositionEnum.L,
        Color = Brushes.DarkGray.Color,
        Panel = Control.AxisL.GetPanel(nameof(PanelEnum.AxisL), new CanvasControl()) as ICanvasControl
      };

      var axisR = new AxisDecorator
      {
        Composer = this,
        Position = PositionEnum.R,
        Color = Brushes.DarkGray.Color,
        Panel = Control.AxisR.GetPanel(nameof(PanelEnum.AxisR), new CanvasControl()) as ICanvasControl
      };

      var labelsT = new LabelDecorator
      {
        Composer = this,
        Position = PositionEnum.T,
        Panel = Control.AxisT.GetPanel(nameof(PanelEnum.LabelT), new CanvasControl()) as ICanvasControl
      };

      var labelsB = new LabelDecorator
      {
        Composer = this,
        Position = PositionEnum.B,
        Panel = Control.AxisB.GetPanel(nameof(PanelEnum.LabelB), new CanvasControl()) as ICanvasControl
      };

      var labelsL = new LabelDecorator
      {
        Composer = this,
        Position = PositionEnum.L,
        Panel = Control.AxisL.GetPanel(nameof(PanelEnum.LabelL), new CanvasControl()) as ICanvasControl
      };

      var labelsR = new LabelDecorator
      {
        Composer = this,
        Position = PositionEnum.R,
        Panel = Control.AxisR.GetPanel(nameof(PanelEnum.LabelR), new CanvasControl()) as ICanvasControl
      };

      var crossView = Control
        .View
        .GetPanel(nameof(PanelEnum.Cross), new CanvasControl());

      var cross = new CrossDecorator
      {
        Composer = this,
        Panel = crossView as ICanvasControl,
        PanelL = Control.AxisL.GetPanel(nameof(PanelEnum.CrossL), new CanvasControl()) as ICanvasControl,
        PanelR = Control.AxisR.GetPanel(nameof(PanelEnum.CrossR), new CanvasControl()) as ICanvasControl,
        PanelT = Control.AxisT.GetPanel(nameof(PanelEnum.CrossT), new CanvasControl()) as ICanvasControl,
        PanelB = Control.AxisB.GetPanel(nameof(PanelEnum.CrossB), new CanvasControl()) as ICanvasControl
      };

      crossView.Background = Brushes.Transparent;

      Panel.SetZIndex(crossView, 100);

      _decorators.Clear();
      _decorators.Add(grid);
      _decorators.Add(axisT);
      _decorators.Add(axisB);
      _decorators.Add(axisL);
      _decorators.Add(axisR);
      _decorators.Add(cross);
      _decorators.Add(labelsT);
      _decorators.Add(labelsB);
      _decorators.Add(labelsL);
      _decorators.Add(labelsR);
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
    /// Create shapes
    /// </summary>
    public virtual void CreateSeries()
    {
      Control.View.GetPanel(nameof(PanelEnum.Series), new CanvasImageControl());
    }

    /// <summary>
    /// Create shapes
    /// </summary>
    public virtual void CreateShapes()
    {
      if (_shapes.Any())
      {
        Control.View.GetPanel(nameof(PanelEnum.Levels));
        return;
      }

      // Create once and keep state

      _shapes.Clear();
      _shapes.Add(new LineShape
      {
        Composer = this,
        Panel = Control.View.GetPanel(nameof(PanelEnum.Levels), new CanvasImageControl()) as ICanvasControl
      });
    }

    /// <summary>
    /// Update delegate
    /// </summary>
    public virtual void Update()
    {
      CreateIndexDomain();
      CreateValueDomain();
      UpdateDecorators();
      UpdateSeries();
      UpdateShapes();
    }

    /// <summary>
    /// Update decorators
    /// </summary>
    public virtual void UpdateDecorators()
    {
      foreach (var decorator in _decoratorsUpdates)
      {
        decorator.Panel.Clear();
        decorator.Update();
        decorator.Panel.Invalidate();
      }
    }

    /// <summary>
    /// Update series and collections
    /// </summary>
    public virtual void UpdateSeries()
    {
      var panel = Control.View.GetPanel(nameof(PanelEnum.Series), new CanvasImageControl()) as ICanvasControl;

      panel.Clear();

      foreach (var i in GetEnumerator())
      {
        var currentItem = Items.ElementAtOrDefault(i);

        if (currentItem == null || Group.Series == null)
        {
          continue;
        }

        foreach (var series in Group.Series)
        {
          series.Value.Shape.Panel = panel;
          series.Value.Shape.Composer = this;
          series.Value.Shape.CreateItem(i, series.Key, Items);
        }
      }

      panel.Invalidate();
    }

    /// <summary>
    /// Update static shapes
    /// </summary>
    public virtual void UpdateShapes()
    {
      foreach (var shape in _shapes)
      {
        shape.Panel.Clear();
        shape.UpdateShape();
        shape.Panel.Invalidate();
      }
    }

    /// <summary>
    /// Update levels
    /// </summary>
    public virtual void UpdateLevels(IList<int> indexLevels, IList<double> valueLevels)
    {
      var levels = _shapes.Where(o => o is LineShape);

      foreach (LineShape level in levels)
      {
        level.IndexLevels = indexLevels ?? new int[0];
        level.ValueLevels = valueLevels ?? new double[0];
      }

      UpdateShapes();
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
      _indexDomain[1] = Math.Max(Items.Count, IndexCount.Value);

      return _indexDomain;
    }

    /// <summary>
    /// Create Min and Max domain 
    /// </summary>
    /// <returns></returns>
    public virtual IList<double> CreateValueDomain()
    {
      var average = 0.0;
      var min = double.MaxValue;
      var max = double.MinValue;

      foreach (var i in GetEnumerator())
      {
        var currentItem = Items.ElementAtOrDefault(i);

        if (currentItem == null || Group.Series == null)
        {
          continue;
        }

        foreach (var series in Group.Series)
        {
          series.Value.Shape.Composer = this;

          var domain = series.Value.Shape.CreateDomain(i, series.Key, Items);

          if (domain != null)
          {
            min = Math.Min(min, domain[0]);
            max = Math.Max(max, domain[1]);
            average += max - min;
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

      if (ValueSpace.HasValue)
      {
        average = average / Math.Max(MaxIndex - MinIndex, 1) * ValueSpace.Value / 100;

        _valueDomain[0] = min - average;
        _valueDomain[1] = max + average;
      }

      if (min < 0 && max > 0)
      {
        var domain = Math.Max(Math.Abs(max), Math.Abs(min));

        _valueDomain[0] = -domain;
        _valueDomain[1] = domain;
      }

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
      var deltaW = panel.W;
      var deltaH = panel.H;

      // Percentage to pixels, Y is inverted

      input.X = deltaW * deltaX;
      input.Y = deltaH - deltaH * deltaY;

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
    /// Format index label
    /// </summary>
    public virtual string ShowIndex(dynamic input)
    {
      return ShowIndexAction == null ? $"{input:0.00}" : ShowIndexAction(input);
    }

    /// <summary>
    /// Format value label
    /// </summary>
    public virtual string ShowValue(dynamic input)
    {
      return ShowValueAction == null ? $"{input:0.00}" : ShowValueAction(input);
    }
  }
}
