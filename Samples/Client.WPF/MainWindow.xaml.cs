using Canvas.Views.WPF;
using Canvas.Source;
using Canvas.Source.ModelSpace;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows;

namespace Client.WPF
{
  public partial class MainWindow : Window
  {
    public int Count = 0;
    public double CurrentOpen = 0;
    public Timer Clock = new(100);
    public Random Generator = new();
    public DateTime Time = DateTime.UtcNow;
    public IList<CanvasView> Panels = new List<CanvasView>();
    public IList<IGroupModel> Points = new List<IGroupModel>();

    /// <summary>
    /// Constructor
    /// </summary>
    public MainWindow()
    {
      InitializeComponent();

      ViewBars.Composer = new Composer { Name = "Bars" };
      ViewLines.Composer = new Composer { Name = "Lines" };
      ViewAreas.Composer = new Composer { Name = "Areas" };
      ViewCandles.Composer = new Composer { Name = "Candles" };
      ViewDeltas.Composer = new Composer { Name = "Deltas" };

      Panels.Add(ViewBars);
      Panels.Add(ViewLines);
      Panels.Add(ViewAreas);
      Panels.Add(ViewCandles);
      Panels.Add(ViewDeltas);

      Panels.ForEach(panel =>
      {
        panel.Create();
        panel.Domains.Subscribe(composer =>
          Panels.ForEach(o =>
          {
            o.Composer.IndexDomain = composer.IndexDomain;
            o.Update();
          }));
      });

      Clock.Enabled = true;
      Clock.Elapsed += (sender, e) => Dispatcher.Invoke(() => Counter(sender, e));
    }

    /// <summary>
    /// Process
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Counter(object sender, ElapsedEventArgs e)
    {
      if (Points.Count > 150)
      {
        Clock.Stop();
      }

      var candle = GetCandle();
      var point = new Model { ["Point"] = candle.Close };
      var pointDelta = new Model { ["Point"] = Generator.Next(2000, 5000) };
      var pointMirror = new Model { ["Point"] = Generator.Next(2000, 5000) };
      var arrow = new Model { ["Point"] = candle.Close, ["Direction"] = 1 };

      if (Points.Count == 0 || IsNextFrame())
      {
        Time = DateTime.UtcNow;
        CurrentOpen = candle.Close;
        Points.Add(new GroupModel
        {
          Index = Time.Ticks,
          Groups = new Dictionary<string, IGroupModel>
          {
            ["Bars"] = new GroupModel { Groups = new Dictionary<string, IGroupModel> { ["V1"] = new BarGroupModel { Value = point } } },
            ["Areas"] = new GroupModel { Groups = new Dictionary<string, IGroupModel> { ["V1"] = new AreaGroupModel { Value = point } } },
            ["Lines"] = new GroupModel
            {
              Groups = new Dictionary<string, IGroupModel>
              {
                ["V1"] = new LineGroupModel { Value = point },
                ["V2"] = new LineGroupModel { Value = pointMirror }
              }
            },
            ["Candles"] = new GroupModel
            {
              Groups = new Dictionary<string, IGroupModel>
              {
                ["V1"] = new CandleGroupModel { Value = candle },
                ["V2"] = new ArrowGroupModel { Value = arrow }
              }
            },
            ["Deltas"] = new GroupModel { Groups = new Dictionary<string, IGroupModel> { ["V1"] = new BarGroupModel { Value = pointDelta } } }
          }
        });
      }

      var currentDelta = Points.Last().Groups["Deltas"].Groups["V1"];
      var currentCandle = Points.Last().Groups["Candles"].Groups["V1"];

      currentCandle.Value.Low = candle.Low;
      currentCandle.Value.High = candle.High;
      currentCandle.Value.Close = candle.Close;
      currentCandle.Color = currentCandle.Value.Close > currentCandle.Value.Open ? SKColors.LimeGreen : SKColors.OrangeRed;

      currentDelta.Value.Point = currentCandle.Value.Close > currentCandle.Value.Open ? candle.Close : -candle.Close;
      currentDelta.Color = currentCandle.Color;

      Panels.ForEach(panel =>
      {
        var composer = panel.Composer;
        composer.Groups = Points;
        composer.IndexDomain ??= new int[2];
        composer.IndexDomain[0] = composer.Groups.Count - composer.IndexCount;
        composer.IndexDomain[1] = composer.Groups.Count;
        panel.Update();
      });
    }

    /// <summary>
    /// Generate candle
    /// </summary>
    /// <returns></returns>
    protected dynamic GetCandle()
    {
      var point = (double)Generator.Next(1000, 5000);
      var shadow = (double)Generator.Next(500, 1000);
      var candle = new Model
      {
        ["Low"] = point - shadow,
        ["High"] = point + shadow,
        ["Open"] = CurrentOpen,
        ["Close"] = point
      };

      return candle;
    }

    /// <summary>
    /// Create new bar when it's time
    /// </summary>
    /// <returns></returns>
    protected bool IsNextFrame()
    {
      return DateTime.UtcNow.Ticks - Time.Ticks >= TimeSpan.FromMilliseconds(100).Ticks;
    }
  }
}
