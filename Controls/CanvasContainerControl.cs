using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Chart.ControlSpace
{
  public class CanvasContainerControl : Canvas
  {
    /// <summary>
    /// Cache
    /// </summary>
    protected IDictionary<string, Panel> _panels = new Dictionary<string, Panel>();

    /// <summary>
    /// Clear
    /// </summary>
    public void ClearPanels()
    {
      _panels.ForEach(o => o.Value.Children.Clear());
      _panels.Clear();
    }

    /// <summary>
    /// Get panel by name
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Panel GetPanel(string index)
    {
      if (_panels.TryGetValue(index, out Panel control))
      {
        return UpdateSize(control, ActualWidth, ActualHeight);
      }

      return null;
    }

    /// <summary>
    /// Create new layer
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public Panel SetPanel(string index, Panel item)
    {
      if (_panels.TryGetValue(index, out Panel control))
      {
        // Delete

        if (item == null)
        {
          _panels.Remove(index);

          if (control != null)
          {
            Children.Remove(control);
          }

          return null;
        }

        return UpdateSize(control, ActualWidth, ActualHeight);
      }

      // Create

      item.VerticalAlignment = VerticalAlignment.Stretch;
      item.HorizontalAlignment = HorizontalAlignment.Stretch;

      Children.Add(_panels[index] = item);

      UpdateSize(item, ActualWidth, ActualHeight);

      return item;
    }

    /// <summary>
    /// Update size
    /// </summary>
    /// <param name="element"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    protected Panel UpdateSize(Panel element, double width, double height)
    {
      if (Equals(element.Width, width) && Equals(element.Height, height))
      {
        return element;
      }

      SetTop(element, 0);
      SetLeft(element, 0);

      element.Width = width;
      element.Height = height;

      return element;
    }
  }
}
