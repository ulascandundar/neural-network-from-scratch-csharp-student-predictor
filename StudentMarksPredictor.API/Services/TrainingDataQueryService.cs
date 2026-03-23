using StudentMarksPredictor.API.DTOs;
using StudentMarksPredictor.Data;

namespace StudentMarksPredictor.API.Services;

public class TrainingDataQueryService
{
    private readonly Repository _repo;

    public TrainingDataQueryService(Repository repo)
    {
        _repo = repo;
    }

    public async Task<GetTrainingDataResponse> GetAllAsync()
    {
        var records = await _repo.GetAllTrainingRecordsAsync();

        return new GetTrainingDataResponse
        {
            TotalRecords = records.Count,
            Records = records.Select(r => new TrainingDataItem
            {
                NumberCourses = r.NumberCourses,
                TimeStudy = r.TimeStudy,
                Marks = r.Marks
            }).ToList()
        };
    }
}
