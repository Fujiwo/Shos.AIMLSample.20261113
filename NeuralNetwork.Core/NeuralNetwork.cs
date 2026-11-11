using System;
using System.Collections.Generic;
using System.Linq;

namespace NeuralNetwork.Core;

// 汎用拡張メンバー
public static class EnumerableExtension
{
    extension<TElement>(IEnumerable<TElement> source)
    {
        public void ForEach(Action<TElement> action)
        {
            foreach (var item in source)
                action(item);
        }

        public void For(Action<int, TElement> action)
        {
            var index = 0;
            foreach (var item in source)
                action(index++, item);
        }

        public IEnumerable<TResult> IndexSelect<TResult>(Func<int, TResult> selector)
        {
            var index = 0;
            foreach (var _ in source)
                yield return selector(index++);
        }
    }

    extension<TElement>(TElement[,] source)
    {
        public void For(Action<int, int, TElement> action)
        {
            for (var row = 0; row < source.GetLength(0); row++) {
                for (var column = 0; column < source.GetLength(1); column++)
                    action(row, column, source[row, column]);
            }
        }
    }

    extension(double[,] source)
    {
        public IEnumerable<double> GetColumn(int column)
        {
            for (var row = 0; row < source.GetLength(0); row++)
                yield return source[row, column];
        }
    }

    extension(int value)
    {
        public void Times(Action action)
        {
            for (var time = 0; time < value; time++)
                action();
        }
    }

    public static IEnumerable<double> Range(double minimum, double maximum, double step)
    {
        for (var value = minimum; value < maximum; value += step)
            yield return value;
    }
}

public static class Math // 数学
{
    //public static double Square(double value) => value * value;

    // シグモイド関数
    public static double Sigmoid(double x) => 1.0 / (1.0 + System.Math.Exp(-x));

    // シグモイド関数の微分
    public static double DifferentialSigmoid(double x) => Sigmoid(x) * (1.0 - Sigmoid(x));
}

public class Input // 入力
{
    public double Value { get; set; }
    public double Weight { get; set; } = 0.0; // 重み

    // 重み付けを行った値
    public double WeightingValue => Value * Weight;
}

public class Neuron // ニューロン
{
    double sum;

    public double Value { get; private set; } = 0.0;

    public void Input(IEnumerable<Input> inputData)
    {
        sum = 0;
        inputData.ForEach(input => Input(input.WeightingValue));
        Value = Math.Sigmoid(sum);
    }

    void Input(double value) => sum += value;
}

public class NeuralNetwork // ニューラル ネットワーク
{
    // 各層
    double[] inputLayer;
    Neuron[] middleLayer;
    Neuron outputLayer;

    // バイアス
    const double inputLayerBias = 1.0;
    const double middleLayerBias = 1.0;

    // 各層の重み
    // 入力層 → 中間層の重み
    double[,] inputWeight = { { RandomWeight, RandomWeight }, { RandomWeight, RandomWeight }, { RandomWeight, RandomWeight } };
    // 中間層 → 出力層の重み
    double[] middleWeight = [RandomWeight, RandomWeight, RandomWeight];

    const double weightRange = 10.0;
    static double RandomWeight => (Random.Shared.NextDouble() - 0.5) * weightRange;

    // 実行
    public double Commit((double, double) data)
    {
        // 各層
        inputLayer = [data.Item1, data.Item2, inputLayerBias];
        middleLayer = [new Neuron(), new Neuron()];
        outputLayer = new Neuron();

        // 入力層→中間層
        middleLayer.For((index, neuron) => middleLayer[index].Input(ToInputData(inputLayer, inputWeight.GetColumn(index).ToArray())));

        // 中間層→出力層
        outputLayer.Input([
            new Input { Value = middleLayer[0].Value, Weight = middleWeight[0] },
            new Input { Value = middleLayer[1].Value, Weight = middleWeight[1] },
            new Input { Value = middleLayerBias, Weight = middleWeight[2] }
        ]);

        return outputLayer.Value;
    }

    // 学習
    public void Learn(IEnumerable<(double, double, double)> dataCollection, int times)
        => times.Times(() => dataCollection.ForEach(data => Learn(data)));

    // 学習
    void Learn((double, double, double) data)
    {
        var outputData = Commit((data.Item1, data.Item2));
        var correctValue = data.Item3;

        // 学習係数
        var learningRate = 0.3;

        // 出力層→中間層
        // δmo = (出力値 - 正解値) × 出力の微分
        var daltaMO = (correctValue - outputData) * outputData * (1.0 - outputData);
        var oldMiddleWeight = middleWeight.Clone() as double[];
        // 修正量 = δmo × 中間層の値 × 学習係数
        middleLayer.For((index, neuron) => middleWeight[index] += neuron.Value * daltaMO * learningRate);
        middleWeight[2] += middleLayerBias * daltaMO * learningRate;

        // 中間層→入力層
        // δim = δmo × 中間出力の重み × 中間層の微分
        var deltaIM = middleLayer.IndexSelect(index => daltaMO * oldMiddleWeight[index] * middleLayer[index].Value * (1.0 - middleLayer[index].Value)).ToArray();
        // var deltaIM = new[] {
        //    daltaMO * oldMiddleWeight[0] * middleLayer[0].Value * (1.0 - middleLayer[0].Value),
        //    daltaMO * oldMiddleWeight[1] * middleLayer[1].Value * (1.0 - middleLayer[1].Value)
        // };

        // 修正量 = δim × 入力層の値 × 学習係数
        inputWeight.For((row, column, _) => inputWeight[row, column] += inputLayer[row] * deltaIM[column] * learningRate);
        // inputWeight[0, 0] += inputLayer[0] * deltaIM[0] * learningRate;
        // inputWeight[0, 1] += inputLayer[0] * deltaIM[1] * learningRate;
        // inputWeight[1, 0] += inputLayer[1] * deltaIM[0] * learningRate;
        // inputWeight[1, 1] += inputLayer[1] * deltaIM[1] * learningRate;
        // inputWeight[2, 0] += inputLayer[2] * deltaIM[0] * learningRate;
        // inputWeight[2, 1] += inputLayer[2] * deltaIM[1] * learningRate;
    }

    static IEnumerable<Input> ToInputData(double[] inputLayer, double[] inputWeight)
        => inputLayer.IndexSelect(index => new Input { Value = inputLayer[index], Weight = inputWeight[index] });
    // {
    //    for (var index = 0; index < inputLayer.Length; index++)
    //        yield return new Input { Value = inputLayer[index], Weight = inputWeight[index] };
    // }
}
