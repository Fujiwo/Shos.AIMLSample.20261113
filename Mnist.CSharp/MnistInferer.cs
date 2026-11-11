// 機械学習で作成した ONNX モデル ファイルを利用して、手書き文字の認識を行う
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mnist.CSharp;

/// <summary>手書き数字推論器</summary>
class MnistInferer : IDisposable
{
    readonly InferenceSession session;

    // ONNX モデルを読み込み、複数回の推論で同じセッションを再利用する。
    public MnistInferer(string onnxModelPath) => session = new InferenceSession(onnxModelPath);

    public void Dispose() => session.Dispose();

    /// <summary>数字の予想</summary>
    /// <param name="input">0～1の値の配列 (28x28、左上原点)</param>
    /// <returns>0～9 (数字)</returns>
    // 28×28 の画素値をモデルへ渡し、最大スコアの添字を数字として返す。
    public int Infer(float[] input) => GetInference(input).MaximumIndex;

    IEnumerable<float> GetInference(float[] input)
        => Infer([
            NamedOnnxValue.CreateFromTensor(
                session.InputMetadata.First().Key,
                // モデルの入力メタデータに合わせた形状で、一次元の画素列をテンソル化する。
                new DenseTensor<float>(new Memory<float>(input), session.InputMetadata.First().Value.Dimensions)
            )
        ]);

    IEnumerable<float> Infer(IReadOnlyCollection<NamedOnnxValue> inputOnnxValues)
    {
        // 推論結果はセッションが破棄される前に配列へコピーして返す。
        using var results = session.Run(inputOnnxValues);
        return results.First().AsTensor<float>().ToArray();
    }
}
