using NeuralNetworkSample.WPF.Models;
using OxyPlot;
using OxyPlot.Series;
using System.Collections.Generic;
using System.Windows.Media;

namespace NeuralNetworkSample.WPF.ViewModels;

public class LineSeriesViewModel
{
    public List<DataPoint> Coordinates { get; set; }
    public string Title { get; set; } = "";
    public Color Color { get; set; } = Colors.Black;
    public MarkerType MarkerType { get; set; } = MarkerType.Plus;
    public LineStyle LineStyle { get; set; } = LineStyle.None;
    public double MarkerSize { get; set; } = 3;
    public double MarkerStrokeThickness { get; set; } = 2;
}

public class MathViewModel
{
    public LineSeriesViewModel SigmoidFunctionGraph { get; } = new() {
        Coordinates = MathModel.SigmoidFunction,
        Title = "シグモイド関数 (y = 1 /  (1 + e^-x))",
        Color = Colors.DarkOrange,
        MarkerType = MarkerType.Circle,
        LineStyle = LineStyle.Solid,
        MarkerSize = 2,
        MarkerStrokeThickness = 1
    };

    public LineSeriesViewModel DifferentialSigmoidFunctionGraph { get; } = new() {
        Coordinates = MathModel.DifferentialSigmoidFunction,
        Title = "シグモイド関数の微分 (y = sigmoid(x) * (1 - sigmoid(x)))",
        Color = Colors.DarkGreen,
        MarkerType = MarkerType.Circle,
        LineStyle = LineStyle.Solid,
        MarkerSize = 2,
        MarkerStrokeThickness = 1
    };
}

public class SampleDataViewModel
{
    public LineSeriesViewModel Data { get; } = new() { Coordinates = new SampleDataModel().Coordinates, Title = "座標データ", Color = Colors.DarkBlue };
}

public class NeuralNetworkViewModel
{
    readonly NeuralNetworkModel neuralNetworkModel = new();

    public LineSeriesViewModel Fukui { get; private set; }
    public LineSeriesViewModel Others { get; private set; }

    public NeuralNetworkViewModel()
    {
        Fukui = new LineSeriesViewModel { Coordinates = neuralNetworkModel.FukuiCoordinates, Title = "福井", Color = Colors.Red };
        Others = new LineSeriesViewModel { Coordinates = neuralNetworkModel.OthersCoordinates, Title = "他都道府県", Color = Colors.Blue };
    }
}

public class TrainingDataViewModel
{
    readonly TrainingDataModel trainingDataModelModel = new();

    public LineSeriesViewModel Fukui { get; private set; }
    public LineSeriesViewModel Others { get; private set; }

    public TrainingDataViewModel()
    {
        Fukui = new LineSeriesViewModel { Coordinates = trainingDataModelModel.LearningFukuiCoordinates, Title = "福井", Color = Colors.Red };
        Others = new LineSeriesViewModel { Coordinates = trainingDataModelModel.LearningOthersCoordinates, Title = "他都道府県", Color = Colors.Blue };
    }
}

public class MachineLearningViewModel
{
    readonly MachineLearningModel machineLearningModel = new();

    public LineSeriesViewModel TrainingFukui { get; private set; }
    public LineSeriesViewModel TrainingOthers { get; private set; }
    public LineSeriesViewModel TestFukui { get; private set; }
    public LineSeriesViewModel TestOthers { get; private set; }

    public MachineLearningViewModel()
    {
        TrainingFukui = new LineSeriesViewModel { Coordinates = machineLearningModel.TrainingFukuiCoordinates, Title = "福井 教師データ", Color = Colors.Red, MarkerType = MarkerType.Plus };
        TrainingOthers = new LineSeriesViewModel { Coordinates = machineLearningModel.TrainingOthersCoordinates, Title = "他都道府県 教師データ", Color = Colors.Blue, MarkerType = MarkerType.Plus };
        TestFukui = new LineSeriesViewModel { Coordinates = machineLearningModel.TestFukuiCoordinates, Title = "福井 テストデータ", Color = Colors.DarkRed, MarkerType = MarkerType.Circle };
        TestOthers = new LineSeriesViewModel { Coordinates = machineLearningModel.TestOthersCoordinates, Title = "他都道府県 テストデータ", Color = Colors.DarkBlue, MarkerType = MarkerType.Circle };
    }
}

public class MainWindowViewModel
{
    public MathViewModel Math { get; } = new();
    public SampleDataViewModel SampleData { get; } = new();
    public NeuralNetworkViewModel NeuralNetwork { get; } = new();
    public TrainingDataViewModel TrainingData { get; } = new();
    public MachineLearningViewModel MachineLearning { get; } = new();

    public PlotModel MathPlotModel { get; }
    public PlotModel SampleDataPlotModel { get; }
    public PlotModel NeuralNetworkPlotModel { get; }
    public PlotModel TrainingDataPlotModel { get; }
    public PlotModel MachineLearningPlotModel { get; }

    public MainWindowViewModel()
    {
        MathPlotModel = CreatePlotModel(Math.SigmoidFunctionGraph, Math.DifferentialSigmoidFunctionGraph);
        SampleDataPlotModel = CreatePlotModel(SampleData.Data);
        NeuralNetworkPlotModel = CreatePlotModel(NeuralNetwork.Fukui, NeuralNetwork.Others);
        TrainingDataPlotModel = CreatePlotModel(TrainingData.Fukui, TrainingData.Others);
        MachineLearningPlotModel = CreatePlotModel(MachineLearning.TrainingFukui, MachineLearning.TrainingOthers, MachineLearning.TestFukui, MachineLearning.TestOthers);
    }

    static PlotModel CreatePlotModel(params LineSeriesViewModel[] source)
    {
        var model = new PlotModel();

        foreach (var item in source) {
            model.Series.Add(new LineSeries {
                Title = item.Title,
                ItemsSource = item.Coordinates,
                DataFieldX = nameof(DataPoint.X),
                DataFieldY = nameof(DataPoint.Y),
                LineStyle = item.LineStyle,
                Color = item.LineStyle == LineStyle.None ? OxyColors.Transparent : ToOxyColor(item.Color),
                MarkerType = item.MarkerType,
                MarkerSize = item.MarkerSize,
                MarkerStroke = ToOxyColor(item.Color),
                MarkerStrokeThickness = item.MarkerStrokeThickness,
                MarkerFill = ToOxyColor(item.Color)
            });
        }

        return model;
    }

    static OxyColor ToOxyColor(Color color) => OxyColor.FromArgb(color.A, color.R, color.G, color.B);
}
