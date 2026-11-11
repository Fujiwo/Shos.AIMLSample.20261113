using NeuralNetworkSample.WPF.Models;
using OxyPlot;
using OxyPlot.Series;
using System.Collections.Generic;
using System.Windows.Media;

namespace NeuralNetworkSample.WPF.ViewModels;

public class LineSeriesViewModel
{
    // OxyPlot の 1 系列を描くために必要な座標、凡例、色、線、マーカーの設定。
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
    // 関数と微分のサンプル点を、別々の系列としてグラフへ渡す。
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
        // 学習前のネットワークが各座標をどちらのクラスに分類するかを表示する。
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
        // 教師ラベルで分けた学習用の座標を表示する。
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
        // 教師データと、学習後に別途推論したテストデータを系列で区別する。
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
        // 各タブに対応する系列をまとめ、それぞれ独立したプロットモデルを作る。
        MathPlotModel = CreatePlotModel(Math.SigmoidFunctionGraph, Math.DifferentialSigmoidFunctionGraph);
        SampleDataPlotModel = CreatePlotModel(SampleData.Data);
        NeuralNetworkPlotModel = CreatePlotModel(NeuralNetwork.Fukui, NeuralNetwork.Others);
        TrainingDataPlotModel = CreatePlotModel(TrainingData.Fukui, TrainingData.Others);
        MachineLearningPlotModel = CreatePlotModel(MachineLearning.TrainingFukui, MachineLearning.TrainingOthers, MachineLearning.TestFukui, MachineLearning.TestOthers);
    }

    static PlotModel CreatePlotModel(params LineSeriesViewModel[] source)
    {
        var model = new PlotModel();

        // 各系列の座標、色、線、マーカー設定を OxyPlot の系列へ写す。
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
