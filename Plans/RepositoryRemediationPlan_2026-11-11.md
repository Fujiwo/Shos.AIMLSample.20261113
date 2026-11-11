# Repository Review Remediation Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 2026-11-11 のリポジトリレビューで未解決となったリソース解放、README の欠落、Neuron の累積値の3項目を修正し、検証する。

**Architecture:** 指摘対象の既存ファイルを最小限変更し、`NeuralNetwork.Core` の再入力動作は新しい MSTest プロジェクトで回帰テストする。ONNX 推論器はセッションと推論結果の双方を所有スコープ内で破棄し、README はソリューションから参照済みのルートパスに追加する。

**Tech Stack:** C#、.NET 10、MSTest、Microsoft.ML.OnnxRuntime、Visual Studio Solution

---

## 状況と方針

- 対象レビュー: [`Reviews/RepositoryReview_2026-11-11.md`](../Reviews/RepositoryReview_2026-11-11.md)
- **対応済み:** `Prism.Wpf` はコミット `90af146` で `NeuralNetworkSample.WPF.csproj` から削除済み。計画対象から除外し、実装時に再追加しない。
- **今回対応する項目:** ONNX リソース解放（High）、README とソリューション参照の整合（Medium）、Neuron の累積値リセット（Low）。
- README の不整合は、ソリューションに既に登録されているルート `README.md` を作成して解消する。
- 現在テストプロジェクトがないため、Neuron の回帰テストと合わせて `NeuralNetwork.Core.Tests` を追加する。

## ファイル構成

| ファイル | 変更内容 |
|---|---|
| `NeuralNetwork.Core.Tests/NeuralNetwork.Core.Tests.csproj` | .NET 10 の MSTest プロジェクト |
| `NeuralNetwork.Core.Tests/NeuronTests.cs` | Neuron に同一入力を繰り返した場合の回帰テスト |
| `Shos.AIMLSample.20261113.sln` | 新しいテストプロジェクトを登録。既存の README ソリューション項目は維持 |
| `NeuralNetwork.Core/NeuralNetwork.cs` | `Neuron.Input` の計算開始時に前回の内部和をリセット |
| `Mnist.CSharp/MnistInferer.cs` | `IDisposable` 実装、推論結果の破棄、および破棄前の出力値コピー |
| `Mnist.CSharp/Program.cs` | `MnistInferer` を `using` スコープで使用 |
| `README.md` | プロジェクト概要、前提条件、ビルド・実行方法を追加 |

## Task 1: ONNX 推論リソースを確実に解放する

**Files:**
- Modify: `Mnist.CSharp/MnistInferer.cs`
- Modify: `Mnist.CSharp/Program.cs`

- [ ] **Step 1: `MnistInferer` に `IDisposable` を実装する**

クラス宣言を `class MnistInferer : IDisposable` に変更し、保持する `InferenceSession` を解放するメソッドを追加する。

```csharp
public void Dispose() => session.Dispose();
```

- [ ] **Step 2: 推論結果を破棄し、出力値を破棄前に配列へコピーする**

`Infer(IReadOnlyCollection<NamedOnnxValue> inputOnnxValues)` を次の形にする。戻り値を `ToArray()` で実体化し、結果オブジェクトの破棄後も列挙できるようにする。

```csharp
IEnumerable<float> Infer(IReadOnlyCollection<NamedOnnxValue> inputOnnxValues)
{
    using var results = session.Run(inputOnnxValues);
    return results.First().AsTensor<float>().ToArray();
}
```

- [ ] **Step 3: 呼び出し側で推論器を破棄する**

`Mnist.CSharp/Program.cs` の推論器生成を `using var` に変更する。

```csharp
using var mnistInference = new MnistInferer(@$"assets\{dataFileName}");
```

- [ ] **Step 4: MNIST サンプルの実行と出力形式を確認する**

実行:

```powershell
$output = dotnet run --project .\Mnist.CSharp\Mnist.CSharp.csproj --configuration Debug
$exitCode = $LASTEXITCODE
if ($exitCode -ne 0 -or ($output -join "`n") -notmatch '(?m)^The digit is probably [0-9]\.$') { throw "MNIST inference failed: $output" }
```

期待結果: プロセスが正常終了し、標準出力に `The digit is probably 0.` から `The digit is probably 9.` の形式の行が含まれる。

## Task 2: Neuron の回帰テストを追加する

**Files:**
- Create: `NeuralNetwork.Core.Tests/NeuralNetwork.Core.Tests.csproj`
- Create: `NeuralNetwork.Core.Tests/NeuronTests.cs`
- Modify: `Shos.AIMLSample.20261113.sln`

- [ ] **Step 1: MSTest プロジェクトを作成してソリューションに登録する**

```powershell
dotnet new mstest --framework net10.0 --name NeuralNetwork.Core.Tests --output NeuralNetwork.Core.Tests
dotnet add .\NeuralNetwork.Core.Tests\NeuralNetwork.Core.Tests.csproj reference .\NeuralNetwork.Core\NeuralNetwork.Core.csproj
dotnet sln .\Shos.AIMLSample.20261113.sln add .\NeuralNetwork.Core.Tests\NeuralNetwork.Core.Tests.csproj
```

- [ ] **Step 2: 同じ入力の再投入で値が変わらないテストを書く**

`NeuralNetwork.Core.Tests/NeuronTests.cs`:

```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeuralNetwork.Core;

namespace NeuralNetwork.Core.Tests;

[TestClass]
public class NeuronTests
{
    [TestMethod]
    public void Input_ResetsPreviousSum_OnRepeatedCalls()
    {
        var neuron = new Neuron();
        var inputs = new[]
        {
            new Input { Value = 2.0, Weight = 3.0 },
            new Input { Value = 1.0, Weight = -1.0 }
        };

        neuron.Input(inputs);
        var firstValue = neuron.Value;

        neuron.Input(inputs);

        Assert.AreEqual(firstValue, neuron.Value, 1e-12);
    }
}
```

- [ ] **Step 3: 変更前のテスト失敗を確認する**

実行:

```powershell
dotnet test .\NeuralNetwork.Core.Tests\NeuralNetwork.Core.Tests.csproj --configuration Debug
```

期待結果: `Input_ResetsPreviousSum_OnRepeatedCalls` が失敗する。現状の実装は2回目の入力でも内部和を加算する。

## Task 3: Neuron の内部和を入力ごとに初期化する

**Files:**
- Modify: `NeuralNetwork.Core/NeuralNetwork.cs`

- [ ] **Step 1: `Neuron.Input` の冒頭で内部和をリセットする**

`Neuron.Input(IEnumerable<Input> inputData)` の先頭に `sum = 0;` を置き、既存の加算と活性化関数の処理はそのまま残す。

```csharp
public void Input(IEnumerable<Input> inputData)
{
    sum = 0;
    inputData.ForEach(input => Input(input.WeightingValue));
    Value = Math.Sigmoid(sum);
}
```

- [ ] **Step 2: 回帰テストの成功を確認する**

実行:

```powershell
dotnet test .\NeuralNetwork.Core.Tests\NeuralNetwork.Core.Tests.csproj --configuration Debug
```

期待結果: `Input_ResetsPreviousSum_OnRepeatedCalls` を含む全テストが成功する。

## Task 4: README を追加してソリューション参照を解決する

**Files:**
- Create: `README.md`
- Verify: `Shos.AIMLSample.20261113.sln` の既存 `README.md` ソリューション項目

- [ ] **Step 1: リポジトリの利用に必要な README を作成する**

ルート `README.md` に次の節を記載する。

1. サンプルリポジトリの目的と3プロジェクト（`NeuralNetwork.Core`、`NeuralNetworkSample.WPF`、`Mnist.CSharp`）の役割
2. ビルド前提条件（.NET 10 SDK、WPF サンプル実行時は Windows）
3. ソリューション全体の Debug ビルドコマンド
4. MNIST 推論サンプルの実行コマンドと `Mnist.CSharp/assets/mnist_model.onnx` の用途

ビルド・実行の節には次のコマンドをそのまま掲載する。

```powershell
dotnet build .\Shos.AIMLSample.20261113.sln --configuration Debug
dotnet run --project .\Mnist.CSharp\Mnist.CSharp.csproj --configuration Debug
```

ソリューションにはルート `README.md` が既に登録されているため、参照先の変更は行わない。

- [ ] **Step 2: README の存在と参照先を確認する**

実行:

```powershell
Test-Path .\README.md
Select-String -Path .\Shos.AIMLSample.20261113.sln -Pattern 'README\.md = README\.md'
```

期待結果: `Test-Path` は `True` を返し、ソリューション内にルート `README.md` の登録が1件見つかる。

## Task 5: 全体検証を行う

**Files:**
- Verify: `Shos.AIMLSample.20261113.sln`
- Verify: `NeuralNetwork.Core.Tests/NeuronTests.cs`
- Verify: `README.md`
- Verify: `Mnist.CSharp/MnistInferer.cs` と `Mnist.CSharp/Program.cs`

- [ ] **Step 1: ソリューション全体を Debug 構成でビルドする**

実行:

```powershell
dotnet build .\Shos.AIMLSample.20261113.sln --configuration Debug
```

期待結果: ビルドが成功し、エラーがない。

- [ ] **Step 2: ソリューションのテストを実行する**

実行:

```powershell
dotnet test .\Shos.AIMLSample.20261113.sln --configuration Debug
```

期待結果: `NeuralNetwork.Core.Tests` の全テストが成功する。

- [ ] **Step 3: MNIST サンプルを再実行する**

Task 1 Step 4 の出力確認コマンドを実行する。

期待結果: 推論プロセスが正常終了し、予測値が 0～9 の範囲で表示される。

- [ ] **Step 4: Prism の再追加がないことを確認する**

実行:

```powershell
Select-String -Path .\NeuralNetworkSample.WPF\NeuralNetworkSample.WPF.csproj -Pattern 'Prism\.Wpf'
```

期待結果: 一致するパッケージ参照がない。

## 完了条件

- `NeuralNetwork.Core.Tests` の回帰テストが成功し、同じ入力を繰り返しても Neuron の出力値が変わらない。
- `InferenceSession` と `session.Run(...)` の結果がそれぞれ確実に破棄され、推論器の利用箇所で破棄漏れがない。
- `README.md` が追加され、ソリューション内の既存参照が解決する。
- `Prism.Wpf` は削除済み状態を維持する。
- Debug 構成のソリューションビルド、テスト、および MNIST 推論実行がすべて成功する。
