using NeuralNetwork.Core;

namespace NeuralNetwork.Core.Tests;

[TestClass]
public class NeuronTests
{
    [TestMethod]
    public void Input_ResetsPreviousSum_OnRepeatedCalls()
    {
        // 同じ入力を連続して与えても、前回の合計値を持ち越さないことを確認する。
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
