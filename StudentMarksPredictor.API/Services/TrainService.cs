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

        var inputs = records.Select(r => new double[] { r.NumberCourses, r.TimeStudy }).ToArray();
        var outputs = records.Select(r => r.Marks).ToArray();

        var normalizer = new NN.Normalizer();
        normalizer.Fit(inputs, outputs);
        var (normX, normY) = normalizer.Transform(inputs, outputs);

        var network = new NN.NeuralNetwork(2, request.HiddenSize);
        double finalMSE = network.Train(normX, normY, request.Epochs, request.LearningRate);

        var session = await _repo.SaveSessionAsync(network, normalizer,
            request.Epochs, request.LearningRate, finalMSE);

        return new TrainResponse
        {
            SessionId = session.Id,
            FinalMSE = finalMSE,
            Epochs = request.Epochs,
            LearningRate = request.LearningRate,
            HiddenSize = request.HiddenSize,
            TrainingRecords = records.Count,
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
