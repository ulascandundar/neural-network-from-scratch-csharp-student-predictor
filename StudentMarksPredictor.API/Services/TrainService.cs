using StudentMarksPredictor.API.DTOs;
using StudentMarksPredictor.API.Validators;
using StudentMarksPredictor.Shared.Exceptions;
using StudentMarksPredictor.Data;
using NN = StudentMarksPredictor.NeuralNetwork;

namespace StudentMarksPredictor.API.Services;

public class TrainService
{
    private readonly Repository _repo;

    public TrainService(Repository repo)
    {
        _repo = repo;
    }

    public async Task<TrainResponse> TrainModelAsync(TrainRequest request)
    {
        TrainRequestValidator.Validate(request);

        var records = await _repo.GetAllTrainingRecordsAsync();
        if (records.Count == 0)
            throw new TrainingDataNotFoundException();

        var allInputs = records.Select(r => new double[] { r.NumberCourses, r.TimeStudy }).ToArray();
        var allOutputs = records.Select(r => r.Marks).ToArray();

        // shuffle before split for randomness
        var rng = new Random(42);
        var indices = Enumerable.Range(0, records.Count).ToArray();
        for (int i = indices.Length - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (indices[i], indices[j]) = (indices[j], indices[i]);
        }

        int testCount = (int)(records.Count * request.TestSplitRatio);
        int trainCount = records.Count - testCount;

        var trainInputs = indices.Take(trainCount).Select(i => allInputs[i]).ToArray();
        var trainOutputs = indices.Take(trainCount).Select(i => allOutputs[i]).ToArray();
        var testInputs = indices.Skip(trainCount).Select(i => allInputs[i]).ToArray();
        var testOutputs = indices.Skip(trainCount).Select(i => allOutputs[i]).ToArray();

        // fit normalizer only on training data
        var normalizer = new NN.Normalizer();
        normalizer.Fit(trainInputs, trainOutputs);
        var (normTrainX, normTrainY) = normalizer.Transform(trainInputs, trainOutputs);

        var network = new NN.NeuralNetwork(2, request.HiddenSize);
        double trainMSE = network.Train(normTrainX, normTrainY, request.Epochs, request.LearningRate);

        // evaluate on test set
        double testMSE = 0;
        if (testCount > 0)
        {
            var (normTestX, normTestY) = normalizer.Transform(testInputs, testOutputs);
            testMSE = network.CalculateMSE(normTestX, normTestY);
        }

        var session = await _repo.SaveSessionAsync(network, normalizer,
            request.Epochs, request.LearningRate, trainMSE, testMSE);

        return new TrainResponse
        {
            SessionId = session.Id,
            TrainMSE = Math.Round(trainMSE, 4),
            TestMSE = Math.Round(testMSE, 4),
            Epochs = request.Epochs,
            LearningRate = request.LearningRate,
            HiddenSize = request.HiddenSize,
            TrainRecords = trainCount,
            TestRecords = testCount,
            CreatedAt = session.CreatedAt
        };
    }

    public async Task TrainIfNoModelExistsAsync()
    {
        var session = await _repo.GetLatestSessionAsync();
        if (session != null)
            return;

        await TrainModelAsync(new TrainRequest());
    }
}
