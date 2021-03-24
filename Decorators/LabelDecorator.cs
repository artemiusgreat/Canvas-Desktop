using Chart.EnumSpace;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Chart.DecoratorSpace
{
  public class LabelDecorator : BaseDecorator, IDecorator
  {
    /// <summary>
    /// Labels
    /// </summary>
    private IDictionary<int, TextBlock> _labels { get; set; } = new Dictionary<int, TextBlock>();

    /// <summary>
    /// Label size
    /// </summary>
    public virtual int FontSize { get; set; } = 10;

    /// <summary>
    /// Location of the axis, e.g. L(eft), R(ight), T(op), B(ottom)
    /// </summary>
    public virtual PositionEnum Position { get; set; }

    /// <summary>
    /// Custom label renderer
    /// </summary>
    public virtual Func<dynamic, dynamic> ShowIndex { get; set; }

    /// <summary>
    /// Custom label renderer
    /// </summary>
    public virtual Func<dynamic, dynamic> ShowValue { get; set; }

    /// <summary>
    /// Create component
    /// </summary>
    public override void CreateDelegate()
    {
      var canvas = Panel as Canvas;

      canvas.Children.Clear();

      _labels.Clear();

      CreateLabels();
    }

    /// <summary>
    /// Update component
    /// </summary>
    public override void UpdateDelegate()
    {
      CreateLabels();
    }

    /// <summary>
    /// Create component
    /// </summary>
    private void CreateLabels()
    {
      ShowIndex ??= Composer.ShowIndex;
      ShowValue ??= Composer.ShowValue;

      var minIndex = Composer.MinIndex;
      var maxIndex = Composer.MaxIndex;
      var minValue = Composer.MinValue;
      var maxValue = Composer.MaxValue;
      var stepSize = Composer.StepSize.Value;
      var indexStep = Composer.IndexStep;
      var valueStep = Composer.ValueStep;
      var indexCount = Composer.IndexLabelCount.Value;
      var valueCount = Composer.ValueCount.Value;
      var canvas = Panel as Canvas;
      var step = 0.0;

      switch (Position)
      {
        case PositionEnum.L:

          step = Panel.H / valueCount;

          for (var i = 1; i < valueCount; i++)
          {
            var label = CreateLabel(i, ShowValue(minValue + (valueCount - i) * valueStep));

            Canvas.SetTop(label, step * i - label.DesiredSize.Height / 2);
            Canvas.SetLeft(label, canvas.Width - label.DesiredSize.Width - stepSize * 2);
          }

          break;

        case PositionEnum.R:

          step = Panel.H / valueCount;

          for (var i = 1; i < valueCount; i++)
          {
            var label = CreateLabel(i, ShowValue(minValue + (valueCount - i) * valueStep));

            Canvas.SetTop(label, step * i - label.DesiredSize.Height / 2);
            Canvas.SetLeft(label, stepSize * 2);
          }

          break;

        case PositionEnum.T:

          step = Panel.W / indexCount;

          for (var i = 1; i < indexCount; i++)
          {
            var label = CreateLabel(i, ShowIndex(minIndex + i * indexStep));

            Canvas.SetTop(label, canvas.Height - label.DesiredSize.Height - stepSize);
            Canvas.SetLeft(label, step * i - label.DesiredSize.Width / 2);
          }

          break;

        case PositionEnum.B:

          step = Panel.W / indexCount;

          for (var i = 1; i < indexCount; i++)
          {
            var label = CreateLabel(i, ShowIndex(minIndex + i * indexStep));

            Canvas.SetTop(label, stepSize);
            Canvas.SetLeft(label, step * i - label.DesiredSize.Width / 2);
          }

          break;
      }
    }

    /// <summary>
    /// Create label
    /// </summary>
    /// <param name="i"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    private TextBlock CreateLabel(int i, dynamic content)
    {
      var canvas = Panel as Canvas;

      _labels.TryGetValue(i, out TextBlock label);

      if (label == null)
      {
        label = new TextBlock
        {
          Text = content,
          FontSize = FontSize,
          Foreground = new SolidColorBrush(Color),
          Margin = new Thickness(0),
          Padding = new Thickness(0)
        };

        canvas.Children.Add(label);

        _labels[i] = label;
      }

      label.Text = content;
      label.Measure(new Size(double.MaxValue, double.MaxValue));

      return label;
    }
  }
}
