# Shos.AIMLSample.20261113

.NET でニューラルネットワークの基本的な学習と推論を試すサンプルです。

## プロジェクト

- `NeuralNetwork.Core`: シグモイド関数と、小規模なニューラルネットワークの学習・推論処理を含むクラスライブラリです。
- `NeuralNetworkSample.WPF`: ニューラルネットワークや学習データをグラフで表示する WPF サンプルです。
- `Mnist.CSharp`: ONNX Runtime を使って MNIST の手書き数字を推論するコンソールサンプルです。

## 前提条件

- .NET 10 SDK
- `NeuralNetworkSample.WPF` のビルド・実行には Windows が必要です。

## ビルド

リポジトリのルートでソリューション全体をビルドします。

```powershell
dotnet build .\Shos.AIMLSample.20261113.sln --configuration Debug
```

## MNIST 推論サンプル

リポジトリのルートで次を実行します。

```powershell
dotnet run --project .\Mnist.CSharp\Mnist.CSharp.csproj --configuration Debug
```

推論には `Mnist.CSharp/assets/mnist_model.onnx` を使用します。サンプルは推論結果として予測数字を表示します。
