# Shos.AIMLSample.20261113

[日本語](./README.md)

This is a collection of samples for learning neural network fundamentals by running code in C# and Python. You can explore training a small network, graphing training results, and inference on handwritten digits using a trained model.

## Key Points

- **Learn the same fundamentals in different ways.** Follow C# and Python code to see forward calculations from inputs to outputs and training that updates weights using error.
- **View the training process in graphs.** The WPF sample lets you compare the sigmoid function, coordinate data, classification before training, training data, and classification after training in separate tabs.
- **Try inference with a model.** The MNIST sample uses ONNX Runtime to load a model and infer the digit represented by a 28×28-pixel sample image.
- **The small-scale examples are easy to explore in code.** Each sample is intended for learning; they are not production-ready models and do not guarantee accuracy.

## Sample Projects

| Project | Description | Main file |
|---|---|---|
| `NeuralNetwork.Core` | A small neural network that uses the sigmoid function. It implements forward calculations and weight updates based on error. | `NeuralNetwork.Core/NeuralNetwork.cs` |
| `NeuralNetwork.Core.Tests` | Checks that a neuron does not carry over its previous total when inputs are provided repeatedly. | `NeuralNetwork.Core.Tests/NeuronTests.cs` |
| `NeuralNetworkSample.WPF` | Displays graphs of functions and coordinate data, as well as classification results before and after network training. | `NeuralNetworkSample.WPF/Views/MainWindow.xaml` |
| `NeuralNetwork.Python` | Classifies whether latitude and longitude coordinates are in Fukui Prefecture, and displays the training data and inference results in a scatter plot. Includes a script and a notebook. | `NeuralNetwork.Python/classification.py` |
| `Mnist.CSharp` | Uses ONNX Runtime to infer a handwritten digit from a sample image. | `Mnist.CSharp/Program.cs` |

## Prerequisites

- .NET 10 SDK
- Windows to build and run the WPF sample
- Python and Matplotlib to run the Python sample

The MNIST model is included at `Mnist.CSharp/assets/mnist_model.onnx`. The Python training data is in `NeuralNetwork.Python/data/locations.training.csv`.

## Build and Test

Run the following command from the repository root.

```powershell
dotnet build .\Shos.AIMLSample.20261113.sln --configuration Debug
```

Run the neural network unit tests.

```powershell
dotnet test .\NeuralNetwork.Core.Tests\NeuralNetwork.Core.Tests.csproj
```

## Run the Samples

### MNIST Handwritten Digit Inference

Run from the repository root.

```powershell
dotnet run --project .\Mnist.CSharp\Mnist.CSharp.csproj --configuration Debug
```

The sample passes the 28×28-pixel image provided in `Mnist.CSharp/Program.cs` to the model and displays the inferred digit. This sample demonstrates how to use a model; it does not train one.

### WPF Graphs

On Windows, run from the repository root.

```powershell
dotnet run --project .\NeuralNetworkSample.WPF\NeuralNetworkSample.WPF.csproj --configuration Debug
```

Switch between tabs to view the sigmoid function and its derivative, coordinate data, classification with random initial weights, the training data, and classification after training on that data. The coordinate data is loaded from `NeuralNetworkSample.WPF/locations.csv`.

### Python Coordinate Classification

Install Matplotlib, then run the script with the `NeuralNetwork.Python` folder as the working directory so the relative path to the CSV file resolves correctly.

```powershell
python -m pip install matplotlib
Push-Location .\NeuralNetwork.Python
python .\classification.py
Pop-Location
```

A window displays the training data and classification results as scatter plots. In the training data, label `0` means Fukui Prefecture and `1` means elsewhere.

To explore the same process cell by cell, open `NeuralNetwork.Python/classification.ipynb` in a Jupyter-compatible environment, select a Python kernel with Matplotlib available, and run it.

## Directory Structure

```text
Shos.AIMLSample.20261113.sln       Solution
NeuralNetwork.Core/                Basic neural network operations
NeuralNetwork.Core.Tests/          Tests for the basic operations
NeuralNetworkSample.WPF/           WPF app for visualizing states before and after training
  locations.csv                    Coordinate data loaded by WPF
Mnist.CSharp/                      Inference sample using an ONNX model
  assets/mnist_model.onnx           MNIST inference model
NeuralNetwork.Python/              Python coordinate classification sample
  classification.py                Script
  classification.ipynb             Notebook
  data/locations.training.csv       Training data
```

## Learning Notes

- The initial weights in `NeuralNetwork.Core` are random, so classification results before training may differ between runs.
- Python and WPF are coordinate classification samples using their respective data and implementations. MNIST is a separate inference example that uses a trained ONNX model.
- These are minimal examples for learning how things work. They do not guarantee classification results or inference accuracy and are not suitable for production use.