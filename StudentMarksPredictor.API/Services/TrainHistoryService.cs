using StudentMarksPredictor.API.DTOs;
using StudentMarksPredictor.Data;

namespace StudentMarksPredictor.API.Services;

public class TrainHistoryService
{
    private readonly Repository _repo;

    public TrainHistoryService(Repository repo)
    {
        _repo = repo;
    }

    public async Task<TrainHistoryResponse> GetHistoryAsync()
    {
        var sessions = await _repo.GetAllSessionsAsync();

        return new TrainHistoryResponse
        {
            TotalSessions = sessions.Count,
            Sessions = sessions.Select(s => new TrainHistoryItem
            {
                SessionId = s.Id,
                Epochs = s.Epochs,
                LearningRate = s.LearningRate,
                TrainMSE = s.TrainMSE,
                TestMSE = s.TestMSE,
                HiddenSize = s.HiddenSize,
                CreatedAt = s.CreatedAt
            }).ToList()
        };
    }
}
