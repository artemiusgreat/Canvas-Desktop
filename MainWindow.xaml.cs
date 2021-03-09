using Chart.ControlSpace;
using Chart.ModelSpace;
using Chart.SeriesSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Chart
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private int _index = 0;
    private DateTime _time = DateTime.Now;
    private Random _generator = new Random();
    private IList<IInputModel> _items = new List<IInputModel>();
    private IList<IInputAreaModel> _groups = new List<IInputAreaModel>();
    private IList<ComponentComposer> _composers = new List<ComponentComposer>();

    /// <summary>
    /// Constructor
    /// </summary>
    public MainWindow()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Data updates
    /// </summary>
    private void OnData()
    {
      var pointModel = new InputModel
      {
        Time = _time,
      };

      _time = _time.AddMinutes(5);

      foreach (var area in _groups)
      {
        pointModel.Areas[area.Name] = new InputAreaModel { Series = new Dictionary<string, IInputSeriesModel>() };
        pointModel.Areas[area.Name].Name = area.Name;

        foreach (var series in area.Series)
        {
          pointModel.Areas[area.Name].Series[series.Key] = new InputSeriesModel();
          pointModel.Areas[area.Name].Series[series.Key].Name = series.Key;

          dynamic input = new BaseModel();

          input.Low = _generator.Next(1, 10);
          input.Open = input.Low + _generator.Next(1, 10);
          input.Close = input.Low + _generator.Next(1, 10);
          input.High = Math.Max(input.Open, input.Close) + _generator.Next(1, 10);
          input.Direction = input.Close > input.Open ? 1.0 : -1.0;
          input.Point = input.Close;
          input.Color = input.Close > input.Open || input.Direction > 0 ? Brushes.LimeGreen.Color : Brushes.OrangeRed.Color;

          switch (series.Value)
          {
            case BarSeries o: input.Point *= input.Direction; break;
            case LineSeries o: input.Color = Brushes.Black.Color; break;
            case ArrowSeries o: input.Color = Brushes.Black.Color; break;
            case AreaSeries o: input.Color = Brushes.DarkGray.Color; break;
          }

          pointModel.Areas[area.Name].Series[series.Key].Model = input;
        }
      }

      _items.Add(pointModel);

      Dispatcher.BeginInvoke(new Action(() =>
      {
        foreach (var composer in _composers)
        {
          composer.Items = _items;
          composer.Update();
        }
      }));
    }

    private void OnRender(object sender, EventArgs e)
    {
      _groups = CreateGroups();

      Dispatcher.BeginInvoke(new Action(() =>
      {
        var index = 0;

        foreach (var area in _groups)
        {
          var chartControl = new ChartControl
          {
            Composers = _composers,
            Background = Brushes.Transparent,
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch
          };

          var composer = new ComponentComposer
          {
            Group = area,
            Name = area.Name,
            Control = chartControl,
            ShowIndexAction = (i) =>
            {
              var date =
                _items.ElementAtOrDefault((int)i)?.Time ??
                _items.ElementAtOrDefault(0)?.Time ??
                DateTime.Now;

              return $"{date:yyyy-MM-dd HH:mm}";
            }
          };

          _composers.Add(chartControl.Composer = composer);

          ChartAreas.RowDefinitions.Add(new RowDefinition());
          ChartAreas.Children.Add(chartControl);
          Grid.SetRow(chartControl, index++);
        }

        Dispatcher.BeginInvoke(new Action(() =>
        {
          foreach (var composer in _composers)
          {
            composer.Create();
            composer.Update();
          }

        }), DispatcherPriority.ApplicationIdle);

        var clock = new Timer();

        clock.Interval = 100;
        clock.Enabled = true;
        clock.Elapsed += (object sender, ElapsedEventArgs e) =>
        {
          if (_index++ >= 200)
          {
            clock.Stop();
          }

          foreach (var composer in _composers)
          {
            composer.IndexDomain ??= new int[2];
            composer.IndexDomain[0] = composer.Items.Count - composer.IndexCount.Value;
            composer.IndexDomain[1] = composer.Items.Count;
          }

          OnData();
        };

      }), DispatcherPriority.ApplicationIdle, null);
    }

    /// <summary>
    /// Define chart areas
    /// </summary>
    /// <returns></returns>
    private IList<IInputAreaModel> CreateGroups()
    {
      return new List<IInputAreaModel>
      {
        new InputAreaModel
        {
          Name = "Candles",
          Series = new Dictionary<string, IInputSeriesModel>
          {
            ["Candles X"] = new InputSeriesModel { Name = "Candles X", Shape = new CandleSeries() },
            ["Lines X"] = new InputSeriesModel { Name = "Lines X", Shape = new LineSeries() },
            ["Arrows X"] = new InputSeriesModel { Name = "Arrows X", Shape = new ArrowSeries() }
          }
        },
        new InputAreaModel
        {
          Name = "Lines",
          Series = new Dictionary<string, IInputSeriesModel>
          {
            ["Lines Y"] = new InputSeriesModel { Name = "Lines Y", Shape = new LineSeries() },
            ["Lines Z"] = new InputSeriesModel { Name = "Lines Z", Shape = new LineSeries() }
          }
        },
        new InputAreaModel
        {
          Name = "Bars",
          Series = new Dictionary<string, IInputSeriesModel>
          {
            ["Bars X"] = new InputSeriesModel { Name = "Bars X", Shape = new BarSeries() }
          }
        },
        new InputAreaModel
        {
          Name = "Areas",
          Series = new Dictionary<string, IInputSeriesModel>
          {
            ["Areas X"] = new InputSeriesModel { Name = "Areas X", Shape = new AreaSeries() }
          }
        }
      };
    }
  }
}
