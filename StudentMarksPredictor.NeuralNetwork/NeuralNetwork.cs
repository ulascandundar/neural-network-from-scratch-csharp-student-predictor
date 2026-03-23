using StudentMarksPredictor.Shared.Exceptions;

namespace StudentMarksPredictor.NeuralNetwork;

// basic feedforward neural network with one hidden layer
// uses relu activation and trains with stochastic gradient descent
public class NeuralNetwork
{
    // how many inputs we have
    private readonly int _inputSize;

    // number of neurons in the hidden layer
    private readonly int _hiddenSize;

    // random number generator for shuffling and weight init
    private readonly Random _rng;

    // weight matrix between input and hidden layer
    private double[,] _weightsIH;

    // bias values for hidden neurons
    private double[] _biasesH;

    // weight matrix between hidden and output layer
    private double[,] _weightsHO;

    // single bias for the output neuron
    private double _biasO;

    // raw values before activation in hidden layer
    private double[] _hiddenRaw;

    // activated values after relu in hidden layer
    private double[] _hiddenAct;

    // cached output value from the last forward pass
    private double _outputVal;

    public int InputSize => _inputSize;
    public int HiddenSize => _hiddenSize;

    // sets up the network with given layer sizes and initializes weights
    public NeuralNetwork(int inputSize, int hiddenSize, int seed = 42)
    {
        _inputSize = inputSize;
        _hiddenSize = hiddenSize;
        _rng = new Random(seed);

        _weightsIH = new double[inputSize, hiddenSize];
        _biasesH = new double[hiddenSize];
        _weightsHO = new double[hiddenSize, 1];
        _biasO = 0;
        _hiddenRaw = new double[hiddenSize];
        _hiddenAct = new double[hiddenSize];

        InitializeWeights();
    }

    // he initialization so relu neurons dont start dead
    private void InitializeWeights()
    {
        double scaleIH = Math.Sqrt(2.0 / _inputSize);
        double scaleHO = Math.Sqrt(2.0 / _hiddenSize);

        for (int i = 0; i < _inputSize; i++)
            for (int j = 0; j < _hiddenSize; j++)
                _weightsIH[i, j] = (_rng.NextDouble() * 2 - 1) * scaleIH;

        for (int j = 0; j < _hiddenSize; j++)
            _weightsHO[j, 0] = (_rng.NextDouble() * 2 - 1) * scaleHO;
    }

    // relu just returns x if positive otherwise zero
    private static double ReLU(double x) => x > 0 ? x : 0;

    // derivative of relu is 1 when positive 0 when not
    private static double ReLUDerivative(double x) => x > 0 ? 1 : 0;

    // runs the input through the network and returns the predicted value
    // also caches intermediate values for backprop
    public double Forward(double[] input)
    {
        if (input.Length != _inputSize)
            throw new InputSizeMismatchException(_inputSize, input.Length);

        // calculate hidden layer activations
        for (int j = 0; j < _hiddenSize; j++)
        {
            _hiddenRaw[j] = _biasesH[j];
            for (int i = 0; i < _inputSize; i++)
                _hiddenRaw[j] += input[i] * _weightsIH[i, j];
            _hiddenAct[j] = ReLU(_hiddenRaw[j]);
        }

        // output layer is linear so no activation function here
        _outputVal = _biasO;
        for (int j = 0; j < _hiddenSize; j++)
            _outputVal += _hiddenAct[j] * _weightsHO[j, 0];

        return _outputVal;
    }

    // backpropagation step that adjusts weights based on the error
    // computes gradients and updates weights using gradient descent
    private void Backward(double[] input, double expected, double learningRate)
    {
        // error at output is just predicted minus expected for mse
        double dOutput = _outputVal - expected;

        // update weights from hidden to output
        for (int j = 0; j < _hiddenSize; j++)
            _weightsHO[j, 0] -= learningRate * _hiddenAct[j] * dOutput;
        _biasO -= learningRate * dOutput;

        // propagate error back to hidden layer and update those weights too
        for (int j = 0; j < _hiddenSize; j++)
        {
            double dHidden = dOutput * _weightsHO[j, 0] * ReLUDerivative(_hiddenRaw[j]);
            for (int i = 0; i < _inputSize; i++)
                _weightsIH[i, j] -= learningRate * input[i] * dHidden;
            _biasesH[j] -= learningRate * dHidden;
        }
    }

    // trains the network for a given number of epochs
    // shuffles data each epoch using fisher yates to avoid ordering bias
    // returns the final mse value
    public double Train(double[][] inputs, double[] outputs, int epochs, double learningRate)
    {
        int n = inputs.Length;
        var indices = new int[n];
        for (int i = 0; i < n; i++) indices[i] = i;

        double lastMSE = 0;
        for (int epoch = 1; epoch <= epochs; epoch++)
        {
            // shuffle training order each epoch
            for (int i = n - 1; i > 0; i--)
            {
                int j = _rng.Next(i + 1);
                (indices[i], indices[j]) = (indices[j], indices[i]);
            }

            double totalLoss = 0;
            for (int k = 0; k < n; k++)
            {
                int idx = indices[k];
                double predicted = Forward(inputs[idx]);
                double error = predicted - outputs[idx];
                totalLoss += error * error;
                Backward(inputs[idx], outputs[idx], learningRate);
            }
            lastMSE = totalLoss / n;
        }
        return lastMSE;
    }

    // just a wrapper around forward for cleaner api usage
    public double Predict(double[] input) => Forward(input);

    // dumps all weights and biases into a flat list so we can save them to db
    public List<WeightEntry> ExportWeights()
    {
        var weights = new List<WeightEntry>();

        // input to hidden weights
        for (int i = 0; i < _inputSize; i++)
            for (int j = 0; j < _hiddenSize; j++)
                weights.Add(new WeightEntry("IH", i, j, _weightsIH[i, j]));

        // hidden biases
        for (int j = 0; j < _hiddenSize; j++)
            weights.Add(new WeightEntry("BiasH", j, 0, _biasesH[j]));

        // hidden to output weights
        for (int j = 0; j < _hiddenSize; j++)
            weights.Add(new WeightEntry("HO", j, 0, _weightsHO[j, 0]));

        // output bias
        weights.Add(new WeightEntry("BiasO", 0, 0, _biasO));

        return weights;
    }

    // loads weights from a list back into the network matrices
    // used when restoring a trained model from the database
    public void ImportWeights(List<WeightEntry> weights)
    {
        foreach (var w in weights)
        {
            switch (w.Layer)
            {
                case "IH": _weightsIH[w.Row, w.Col] = w.Value; break;
                case "BiasH": _biasesH[w.Row] = w.Value; break;
                case "HO": _weightsHO[w.Row, w.Col] = w.Value; break;
                case "BiasO": _biasO = w.Value; break;
            }
        }
    }
}
