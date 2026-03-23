namespace StudentMarksPredictor.NeuralNetwork;

// handles min-max normalization to scale values between 0 and 1
// we need this because the network works better with small numbers
public class Normalizer
{
    // minimum values for each input feature
    public double[] InputMin { get; set; } = new double[2];

    // maximum values for each input feature
    public double[] InputMax { get; set; } = new double[2];

    // min and max for the output column
    public double OutputMin { get; set; }
    public double OutputMax { get; set; }

    // scans the data to find min and max for each column
    // should only be called on training data to avoid data leakage
    public void Fit(double[][] inputs, double[] outputs)
    {
        InputMin[0] = double.MaxValue; InputMin[1] = double.MaxValue;
        InputMax[0] = double.MinValue; InputMax[1] = double.MinValue;
        OutputMin = double.MaxValue; OutputMax = double.MinValue;

        for (int i = 0; i < inputs.Length; i++)
        {
            for (int f = 0; f < 2; f++)
            {
                if (inputs[i][f] < InputMin[f]) InputMin[f] = inputs[i][f];
                if (inputs[i][f] > InputMax[f]) InputMax[f] = inputs[i][f];
            }
            if (outputs[i] < OutputMin) OutputMin = outputs[i];
            if (outputs[i] > OutputMax) OutputMax = outputs[i];
        }
    }

    // scales a raw input array to 0-1 range using stored min max
    public double[] NormalizeInput(double[] raw)
    {
        return new double[]
        {
            (raw[0] - InputMin[0]) / (InputMax[0] - InputMin[0]),
            (raw[1] - InputMin[1]) / (InputMax[1] - InputMin[1])
        };
    }

    // scales a raw output value to 0-1 range
    public double NormalizeOutput(double raw)
    {
        return (raw - OutputMin) / (OutputMax - OutputMin);
    }

    // converts a normalized output back to its original scale
    // used when showing predictions to the user
    public double DenormalizeOutput(double normalized)
    {
        return normalized * (OutputMax - OutputMin) + OutputMin;
    }

    // normalizes both inputs and outputs in one go
    // just a convenience method so we dont have to loop manually
    public (double[][] normX, double[] normY) Transform(double[][] inputs, double[] outputs)
    {
        var normX = new double[inputs.Length][];
        var normY = new double[outputs.Length];

        for (int i = 0; i < inputs.Length; i++)
        {
            normX[i] = NormalizeInput(inputs[i]);
            normY[i] = NormalizeOutput(outputs[i]);
        }

        return (normX, normY);
    }
}
