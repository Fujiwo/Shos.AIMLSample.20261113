using System;
using System.Collections.Generic;
using System.Linq;

namespace NeuralNetwork.Core;

// 列挙可能な値、配列、整数を、繰り返しや添字付きで扱う拡張メソッドをまとめる。
public static class EnumerableExtension
{
    extension<TElement>(IEnumerable<TElement> source)
    {
        // 要素の順序を保ったまま、各要素に同じ処理を適用する。
        public void ForEach(Action<TElement> action)
        {
            foreach (var item in source)
                action(item);
        }

        // 要素と 0 始まりの添字を一緒に渡し、添字ごとに異なる処理を可能にする。
        public void For(Action<int, TElement> action)
        {
            var index = 0;
            foreach (var item in source)
                action(index++, item);
        }

        // 元の要素は使わず、各要素の位置を使って新しい列を遅延生成する。
        public IEnumerable<TResult> IndexSelect<TResult>(Func<int, TResult> selector)
        {
            var index = 0;
            foreach (var _ in source)
                yield return selector(index++);
        }
    }

    extension<TElement>(TElement[,] source)
    {
        // 2 次元配列を行優先で走査し、値と行・列の添字を渡す。
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
        // 指定列を上から順に取り出し、行列の列データとして扱えるようにする。
        public IEnumerable<double> GetColumn(int column)
        {
            for (var row = 0; row < source.GetLength(0); row++)
                yield return source[row, column];
        }
    }

    extension(int value)
    {
        // 0 以上 value 未満の回数だけ、引数の処理を繰り返す。
        public void Times(Action action)
        {
            for (var time = 0; time < value; time++)
                action();
        }
    }

    public static IEnumerable<double> Range(double minimum, double maximum, double step)
    {
        // maximum は含めず、step ずつ増やしながら値を遅延生成する。
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

    // ニューロンへ渡す前に、入力値へ対応する重みを掛ける。
    public double WeightingValue => Value * Weight;
}

public class Neuron // ニューロン
{
    double sum;

    public double Value { get; private set; } = 0.0;

    public void Input(IEnumerable<Input> inputData)
    {
        // 前回の計算結果を持ち越さず、重み付き入力の合計から出力を計算する。
        sum = 0;
        inputData.ForEach(input => Input(input.WeightingValue));
        Value = Math.Sigmoid(sum);
    }

    void Input(double value) => sum += value;
}

public class NeuralNetwork // ニューラル ネットワーク
{
    // 入力 2 個とバイアス 1 個、中間層 2 個、出力層 1 個で構成する。
    double[] inputLayer;
    Neuron[] middleLayer;
    Neuron outputLayer;

    // バイアスは入力に依存しない定数入力として、各層の重みに含める。
    const double inputLayerBias = 1.0;
    const double middleLayerBias = 1.0;

    // 重み配列の行は入力、列は接続先の中間ニューロンに対応する。
    // 入力層 → 中間層の重み
    double[,] inputWeight = { { RandomWeight, RandomWeight }, { RandomWeight, RandomWeight }, { RandomWeight, RandomWeight } };
    // 中間層 → 出力層の重み
    double[] middleWeight = [RandomWeight, RandomWeight, RandomWeight];

    const double weightRange = 10.0;
    // 初期重みを -5 ～ 5 の範囲で一様に選ぶ。
    static double RandomWeight => (Random.Shared.NextDouble() - 0.5) * weightRange;

    // 1 件の入力を順方向に伝播し、出力層の予測値を返す。
    public double Commit((double, double) data)
    {
        // 入力とバイアスを設定し、今回の計算用にニューロンを作り直す。
        inputLayer = [data.Item1, data.Item2, inputLayerBias];
        middleLayer = [new Neuron(), new Neuron()];
        outputLayer = new Neuron();

        // 入力層の各値を重み付けして、中間層の各ニューロンへ渡す。
        middleLayer.For((index, neuron) => middleLayer[index].Input(ToInputData(inputLayer, inputWeight.GetColumn(index).ToArray())));

        // 中間層の出力とバイアスを重み付けし、最終的な予測値を求める。
        outputLayer.Input([
            new Input { Value = middleLayer[0].Value, Weight = middleWeight[0] },
            new Input { Value = middleLayer[1].Value, Weight = middleWeight[1] },
            new Input { Value = middleLayerBias, Weight = middleWeight[2] }
        ]);

        return outputLayer.Value;
    }

    // 教師データ全体を指定回数だけ繰り返し、1 件ずつ重みを更新する。
    public void Learn(IEnumerable<(double, double, double)> dataCollection, int times)
        => times.Times(() => dataCollection.ForEach(data => Learn(data)));

    // 1 件の教師データで予測誤差を逆方向へ伝え、各層の重みを更新する。
    void Learn((double, double, double) data)
    {
        var outputData = Commit((data.Item1, data.Item2));
        var correctValue = data.Item3;

        // 学習率は 1 回の更新で重みを動かす大きさを調整する。
        var learningRate = 0.3;

        // 出力層→中間層
        // δmo = (正解値 - 出力値) × 出力の微分
        var daltaMO = (correctValue - outputData) * outputData * (1.0 - outputData);
        // 中間層の誤差を計算するときに、更新前の重みを使うため保存する。
        var oldMiddleWeight = middleWeight.Clone() as double[];
        // 出力誤差に各入力値を掛け、出力層へつながる重みを更新する。
        middleLayer.For((index, neuron) => middleWeight[index] += neuron.Value * daltaMO * learningRate);
        middleWeight[2] += middleLayerBias * daltaMO * learningRate;

        // 中間層→入力層
        // δim = δmo × 中間出力の重み × 中間層の微分
        var deltaIM = middleLayer.IndexSelect(index => daltaMO * oldMiddleWeight[index] * middleLayer[index].Value * (1.0 - middleLayer[index].Value)).ToArray();
        // var deltaIM = new[] {
        //    daltaMO * oldMiddleWeight[0] * middleLayer[0].Value * (1.0 - middleLayer[0].Value),
        //    daltaMO * oldMiddleWeight[1] * middleLayer[1].Value * (1.0 - middleLayer[1].Value)
        // };

        // 中間層の誤差に入力値を掛け、入力層からの重みを更新する。
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
