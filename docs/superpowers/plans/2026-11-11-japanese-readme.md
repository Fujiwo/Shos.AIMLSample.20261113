# 初学者向け日本語 README 更新計画

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 初学者がリポジトリの学習ポイントを理解し、C#・WPF・Python のサンプルを実行できる日本語 README に更新する。

**Architecture:** ルートの `README.md` だけを編集し、概要から各サンプルの説明、実行手順、ファイル構成、注意事項へと読み進められる構成にする。既存のコードとプロジェクト設定で確認できる事実だけを説明し、依存関係やコードは変更しない。

**Tech Stack:** Markdown、.NET 10、PowerShell、Python、Matplotlib、WPF。

---

### Task 1: README を初学者向けに書き直す

**Files:**
- Modify: `README.md`
- Reference: `Shos.AIMLSample.20261113.sln`
- Reference: `NeuralNetwork.Core/NeuralNetwork.cs`
- Reference: `NeuralNetwork.Core.Tests/NeuronTests.cs`
- Reference: `Mnist.CSharp/MnistInferer.cs`
- Reference: `Mnist.CSharp/Program.cs`
- Reference: `NeuralNetworkSample.WPF/Views/MainWindow.xaml`
- Reference: `NeuralNetworkSample.WPF/Models/Model.cs`
- Reference: `NeuralNetwork.Python/classification.py`
- Reference: `NeuralNetwork.Python/data/locations.training.csv`

- [ ] README の構成を「概要」「このリポジトリのポイント」「サンプル一覧」「前提条件」「ビルドとテスト」「サンプルの実行」「ディレクトリ構成」「学習時の注意」の順にする。
- [ ] プロジェクト一覧では、Core の小規模な順伝播・学習処理、WPF のシグモイド関数・座標・学習前後のグラフ、Python の座標分類と Matplotlib による可視化、MNIST の ONNX Runtime 推論を区別して説明する。
- [ ] 前提条件には .NET 10 SDK、WPF 実行時の Windows 要件、Python サンプルで使う Matplotlib を記載する。MNIST は `Mnist.CSharp/assets/mnist_model.onnx` を使うと説明する。
- [ ] 以下の実在するコマンドを PowerShell のコードブロックで掲載する。

```powershell
dotnet build .\Shos.AIMLSample.20261113.sln --configuration Debug
dotnet test .\NeuralNetwork.Core.Tests\NeuralNetwork.Core.Tests.csproj
dotnet run --project .\Mnist.CSharp\Mnist.CSharp.csproj --configuration Debug
dotnet run --project .\NeuralNetworkSample.WPF\NeuralNetworkSample.WPF.csproj --configuration Debug
python -m pip install matplotlib
Push-Location .\NeuralNetwork.Python
python .\classification.py
Pop-Location
```

- [ ] Jupyter Notebook を使う場合は `NeuralNetwork.Python/classification.ipynb` を開き、Matplotlib を利用できる Python 環境でセルを実行する、と案内する。
- [ ] CSV、ONNX モデル、ソリューション、各プロジェクト、テストの相対パスを示す。コードから読み取れない推論結果・精度・データ出典は断定しない。
- [ ] 学習用サンプルであり本番用途を想定しないこと、C# の初期重みが乱数で初期化されること、各サンプルの目的に沿って結果を見ることを簡潔に補足する。

### Task 2: README の記述と Markdown を検証する

**Files:**
- Verify: `README.md`

- [ ] README に書いた相対パスが存在することを確認する。PowerShell では次を実行し、すべて `True` になることを確認する。

```powershell
Test-Path .\Shos.AIMLSample.20261113.sln
Test-Path .\NeuralNetwork.Core\NeuralNetwork.cs
Test-Path .\NeuralNetwork.Core.Tests\NeuralNetwork.Core.Tests.csproj
Test-Path .\Mnist.CSharp\assets\mnist_model.onnx
Test-Path .\NeuralNetworkSample.WPF\NeuralNetworkSample.WPF.csproj
Test-Path .\NeuralNetwork.Python\classification.py
Test-Path .\NeuralNetwork.Python\classification.ipynb
Test-Path .\NeuralNetwork.Python\data\locations.training.csv
```

- [ ] README のコードフェンスと見出しを確認し、`git diff --check` が成功することを確認する。
- [ ] README 以外のファイルに意図しない差分がないことを確認する。ドキュメントのみの変更なので、ビルドとテストは実行しない。

### Task 3: README 更新をコミットする

**Files:**
- Commit: `README.md`

- [ ] `git add -- README.md` で README だけをステージし、`git diff --cached --check` を実行する。
- [ ] 次のメッセージでコミットする。

```text
docs: improve Japanese README for beginners

Co-authored-by: Copilot <223556219+Copilot@users.noreply.github.com>
```
