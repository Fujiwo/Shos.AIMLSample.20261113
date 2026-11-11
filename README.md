# Shos.AIMLSample.20261113

[English](./README.en.md)

ニューラルネットワークの基本を、C# と Python で実行しながら学ぶためのサンプル集です。小規模なネットワークの学習処理、学習結果のグラフ表示、学習済みモデルを使った手書き数字の推論を試せます。

## このリポジトリのポイント

- **同じ基礎を異なる形で学べます。** C# と Python のコードで、入力から出力を計算する順方向の処理と、誤差を使って重みを更新する学習処理を追えます。
- **学習の様子をグラフで確認できます。** WPF サンプルでは、シグモイド関数、座標データ、学習前の分類、教師データ、学習後の分類をタブで比較できます。
- **モデルを使った推論も試せます。** MNIST サンプルは ONNX Runtime でモデルを読み込み、28×28 ピクセルのサンプル画像が表す数字を推論します。
- **小さな構成なので、コードを読みながら試せます。** 各サンプルは学習目的のものであり、実運用向けのモデルや精度を提供するものではありません。

## サンプル一覧

| プロジェクト | 内容 | 主なファイル |
|---|---|---|
| `NeuralNetwork.Core` | シグモイド関数を使う小規模なニューラルネットワーク。順方向の計算と、誤差に基づく重みの更新を実装しています。 | `NeuralNetwork.Core/NeuralNetwork.cs` |
| `NeuralNetwork.Core.Tests` | ニューロンに入力を繰り返し与えたとき、前回の合計値を持ち越さないことを確認します。 | `NeuralNetwork.Core.Tests/NeuronTests.cs` |
| `NeuralNetworkSample.WPF` | 関数と座標データ、およびネットワークの学習前後の分類結果をグラフで表示します。 | `NeuralNetworkSample.WPF/Views/MainWindow.xaml` |
| `NeuralNetwork.Python` | 緯度・経度から福井県内かどうかを分類し、教師データと推論結果を散布図に表示します。スクリプトとノートブックがあります。 | `NeuralNetwork.Python/classification.py` |
| `Mnist.CSharp` | ONNX Runtime を使って、サンプルの手書き数字画像を推論します。 | `Mnist.CSharp/Program.cs` |

## 前提条件

- .NET 10 SDK
- WPF サンプルをビルド・実行する場合は Windows
- Python サンプルを実行する場合は Python と Matplotlib

MNIST モデルは `Mnist.CSharp/assets/mnist_model.onnx` に含まれています。Python の教師データは `NeuralNetwork.Python/data/locations.training.csv` にあります。

## ビルドとテスト

以下のコマンドはリポジトリのルートで実行します。

```powershell
dotnet build .\Shos.AIMLSample.20261113.sln --configuration Debug
```

ニューラルネットワークの単体テストを実行します。

```powershell
dotnet test .\NeuralNetwork.Core.Tests\NeuralNetwork.Core.Tests.csproj
```

## サンプルを実行する

### MNIST の手書き数字推論

リポジトリのルートから実行します。

```powershell
dotnet run --project .\Mnist.CSharp\Mnist.CSharp.csproj --configuration Debug
```

`Mnist.CSharp/Program.cs` に用意された 28×28 ピクセルの画像をモデルに渡し、推論した数字を表示します。このサンプルはモデルの利用方法を示すもので、モデルの学習は行いません。

### WPF のグラフ表示

Windows 上でリポジトリのルートから実行します。

```powershell
dotnet run --project .\NeuralNetworkSample.WPF\NeuralNetworkSample.WPF.csproj --configuration Debug
```

タブを切り替えると、シグモイド関数とその微分、座標データ、ランダムな初期重みによる分類、教師データ、教師データで学習した後の分類を確認できます。座標データは `NeuralNetworkSample.WPF/locations.csv` から読み込みます。

### Python の座標分類

Matplotlib をインストールし、CSV ファイルへの相対パスが合うよう `NeuralNetwork.Python` フォルダーを作業ディレクトリにして実行します。

```powershell
python -m pip install matplotlib
Push-Location .\NeuralNetwork.Python
python .\classification.py
Pop-Location
```

ウィンドウに、教師データと分類結果が散布図で表示されます。教師データのラベルは `0` が福井県、`1` がそれ以外です。

同じ処理をセル単位で確認したい場合は `NeuralNetwork.Python/classification.ipynb` を Jupyter 対応環境で開き、Matplotlib が使える Python カーネルを選択して実行してください。

## ディレクトリ構成

```text
Shos.AIMLSample.20261113.sln       ソリューション
NeuralNetwork.Core/                ニューラルネットワークの基本処理
NeuralNetwork.Core.Tests/          基本処理のテスト
NeuralNetworkSample.WPF/           学習前後の状態を可視化する WPF アプリ
  locations.csv                    WPF が読み込む座標データ
Mnist.CSharp/                      ONNX モデルを使う推論サンプル
  assets/mnist_model.onnx           MNIST の推論モデル
NeuralNetwork.Python/              Python の座標分類サンプル
  classification.py                スクリプト
  classification.ipynb             ノートブック
  data/locations.training.csv       教師データ
```

## 学習時の注意

- `NeuralNetwork.Core` の初期重みは乱数で決まるため、学習前の分類結果は実行ごとに異なることがあります。
- Python と WPF はそれぞれのデータ・実装で座標を分類するサンプルです。MNIST は別の学習済み ONNX モデルを使った推論例です。
- いずれも仕組みを学ぶための最小限の例です。分類結果や推論精度を保証するものではなく、本番用途には適していません。
