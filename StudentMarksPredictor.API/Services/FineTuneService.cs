using StudentMarksPredictor.API.DTOs;
using StudentMarksPredictor.API.Validators;
using StudentMarksPredictor.Shared.Exceptions;
using StudentMarksPredictor.Data;
using NN = StudentMarksPredictor.NeuralNetwork;

namespace StudentMarksPredictor.API.Services;

public class FineTuneService
{
    private readonly Repository _repo;

    public FineTuneService(Repository repo)
    {
        _repo = repo;
    }

    public async Task<TrainResponse> FineTuneAsync(TrainRequest request)
    {
        TrainRequestValidator.Validate(request);

        var existingSession = await _repo.GetLatestSessionAsync();
        if (existingSession == null)
            throw new ModelNotFoundException();

        var records = await _repo.GetAllTrainingRecordsAsync();
        if (records.Count == 0)
            throw new TrainingDataNotFoundException();

        var inputs = records.Select(r => new double[] { r.NumberCourses, r.TimeStudy }).ToArray();
        var outputs = records.Select(r => r.Marks).ToArray();

        var normalizer = new NN.Normalizer();
        normalizer.Fit(inputs, outputs);
        var (normX, normY) = normalizer.Transform(inputs, outputs);

        // load existing weights instead of random init
        var (network, _) = _repo.LoadModel(existingSession);

        double finalMSE = network.Train(normX, normY, request.Epochs, request.LearningRate);

        var session = await _repo.SaveSessionAsync(network, normalizer,
            request.Epochs, request.LearningRate, finalMSE);

        return new TrainResponse
        {
            SessionId = session.Id,
            FinalMSE = finalMSE,
            Epochs = request.Epochs,
            LearningRate = request.LearningRate,
            HiddenSize = existingSession.HiddenSize,
            TrainingRecords = records.Count,
            CreatedAt = session.CreatedAt
        };
    }
}
