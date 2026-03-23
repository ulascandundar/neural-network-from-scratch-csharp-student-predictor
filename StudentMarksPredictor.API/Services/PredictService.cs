using StudentMarksPredictor.API.DTOs;
using StudentMarksPredictor.API.Validators;
using StudentMarksPredictor.Shared.Exceptions;
using StudentMarksPredictor.Data;

namespace StudentMarksPredictor.API.Services;

public class PredictService
{
    private readonly Repository _repo;

    public PredictService(Repository repo)
    {
        _repo = repo;
    }

    public async Task<PredictResponse> PredictMarksAsync(PredictRequest request)
    {
        PredictRequestValidator.Validate(request);

        var session = await _repo.GetLatestSessionAsync();
        if (session == null)
            throw new ModelNotFoundException();

        var (network, normalizer) = _repo.LoadModel(session);

        var normalizedInput = normalizer.NormalizeInput(new[] { (double)request.NumberCourses, request.TimeStudy });
        var normalizedOutput = network.Predict(normalizedInput);
        var predictedMarks = normalizer.DenormalizeOutput(normalizedOutput);

        return new PredictResponse
        {
            NumberCourses = request.NumberCourses,
            TimeStudy = request.TimeStudy,
            PredictedMarks = Math.Round(Math.Max(0, predictedMarks), 2),
            SessionId = session.Id
        };
    }
}
