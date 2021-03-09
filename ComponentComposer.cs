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
    public virtual double? ValueCenter { get; set; }
    public virtual ChartControl Control { get; set; }
    public virtual IInputAreaModel Group { get; set; } = new InputAreaModel();
    public virtual IList<int> IndexDomain { get; set; }
    public virtual IList<double> ValueDomain { get; set; }
    public virtual IList<IInputModel> Items { get; set; } = new List<IInputModel>();
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
        Panel = Control.ViewArea.SetPanel("Grid", new CanvasControl()) as ICanvasControl
      };

      var axisT = new AxisDecorator
      {
        Composer = this,
        Position = PositionEnum.Top,
        Color = Brushes.DarkGray.Color,
        Panel = Control.AxisT.SetPanel("AxisT", new CanvasControl()) as ICanvasControl
      };

      var axisB = new AxisDecorator
      {
        Composer = this,
        Position = PositionEnum.Bottom,
        Color = Brushes.DarkGray.Color,
        Panel = Control.AxisB.SetPanel("AxisB", new CanvasControl()) as ICanvasControl
      };

      var axisL = new AxisDecorator
      {
        Composer = this,
        Position = PositionEnum.Left,
        Color = Brushes.DarkGray.Color,
        Panel = Control.AxisL.SetPanel("AxisL", new CanvasControl()) as ICanvasControl
      };

      var axisR = new AxisDecorator
      {
        Composer = this,
        Position = PositionEnum.Right,
        Color = Brushes.DarkGray.Color,
        Panel = Control.AxisR.SetPanel("AxisR", new CanvasControl()) as ICanvasControl
      };

      var labelsT = new LabelDecorator
      {
        Composer = this,
        Position = PositionEnum.Top,
        Panel = Control.AxisT.SetPanel("LabelsT", new CanvasControl()) as ICanvasControl
      };

      var labelsB = new LabelDecorator
      {
        Composer = this,
        Position = PositionEnum.Bottom,
        Panel = Control.AxisB.SetPanel("LabelsB", new CanvasControl()) as ICanvasControl
      };

      var labelsL = new LabelDecorator
      {
        Composer = this,
        Position = PositionEnum.Left,
        Panel = Control.AxisL.SetPanel("LabelsL", new CanvasControl()) as ICanvasControl
      };

      var labelsR = new LabelDecorator
      {
        Composer = this,
        Position = PositionEnum.Right,
        Panel = Control.AxisR.SetPanel("LabelsR", new CanvasControl()) as ICanvasControl
      };

      var crossView = Control.ViewArea.SetPanel("Cross", new CanvasControl());

      var cross = new CrossDecorator
      {
        Composer = this,
        Panel = crossView as ICanvasControl,
        PanelL = Control.AxisL.SetPanel("CrossL", new CanvasControl()) as ICanvasControl,
        PanelR = Control.AxisR.SetPanel("CrossR", new CanvasControl()) as ICanvasControl,
        PanelT = Control.AxisT.SetPanel("CrossT", new CanvasControl()) as ICanvasControl,
        PanelB = Control.AxisB.SetPanel("CrossB", new CanvasControl()) as ICanvasControl
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
      Control.ViewArea.SetPanel("View", new CanvasImageControl());
    }

    /// <summary>
    /// Create shapes
    /// </summary>
    public virtual void CreateShapes()
    {
      if (_shapes.Any())
      {
        Control.ViewArea.GetPanel("Levels");

        return;
      }

      // Create once and keep state

      _shapes.Clear();

      var levels = new LineShape
      {
        Composer = this,
        Panel = Control.ViewArea.SetPanel("Levels", new CanvasImageControl()) as ICanvasControl
      };

      _shapes.Add(levels);
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
    /// Update series
    /// </summary>
    public virtual void UpdateSeries()
    {
      var panel = Control.ViewArea.SetPanel("View", new CanvasImageControl()) as ICanvasControl;

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
    /// Update shapes
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
      var min = double.MaxValue;
      var max = double.MinValue;
      var panel = Control.ViewArea.GetPanel("Shapes") as ICanvasControl;

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

          var domain = series.Value.Shape.CreateDomain(i, series.Key, Items);

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

      var step = (max - min) / ValueCount.Value;

      _valueDomain ??= new double[2];
      _valueDomain[0] = min - step;
      _valueDomain[1] = max + step;

      if (ValueCenter.HasValue)
      {
        var domain = Math.Max(Math.Abs(max), Math.Abs(min)) + ValueCenter.Value;

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
