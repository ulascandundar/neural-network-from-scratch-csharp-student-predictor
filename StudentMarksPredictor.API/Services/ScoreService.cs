using StudentMarksPredictor.API.DTOs;
using StudentMarksPredictor.Shared.Exceptions;
using StudentMarksPredictor.Data;
using NN = StudentMarksPredictor.NeuralNetwork;

namespace StudentMarksPredictor.API.Services;

public class ScoreService
{
    private readonly Repository _repo;

    public ScoreService(Repository repo)
    {
        _repo = repo;
    }

    public async Task<ScoreResponse> EvaluateAsync()
    {
        var session = await _repo.GetLatestSessionAsync();
        if (session == null)
            throw new ModelNotFoundException();

        var records = await _repo.GetAllTrainingRecordsAsync();
        if (records.Count == 0)
            throw new TrainingDataNotFoundException();

        var (network, normalizer) = _repo.LoadModel(session);

        var actuals = records.Select(r => r.Marks).ToArray();
        var predictions = records.Select(r =>
        {
            var normInput = normalizer.NormalizeInput(new[] { r.NumberCourses, r.TimeStudy });
            var normOutput = network.Predict(normInput);
            return normalizer.DenormalizeOutput(normOutput);
        }).ToArray();

        double mse = actuals.Zip(predictions, (a, p) => Math.Pow(a - p, 2)).Average();
        double mae = actuals.Zip(predictions, (a, p) => Math.Abs(a - p)).Average();

        double meanActual = actuals.Average();
        double ssTotal = actuals.Sum(a => Math.Pow(a - meanActual, 2));
        double ssResidual = actuals.Zip(predictions, (a, p) => Math.Pow(a - p, 2)).Sum();
        double r2 = 1 - (ssResidual / ssTotal);

        return new ScoreResponse
        {
            MSE = Math.Round(mse, 4),
            MAE = Math.Round(mae, 4),
            R2 = Math.Round(r2, 4),
            TotalRecords = records.Count,
            SessionId = session.Id
        };
    }
}
