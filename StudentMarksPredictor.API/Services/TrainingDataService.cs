using StudentMarksPredictor.API.DTOs;
using StudentMarksPredictor.Shared.Exceptions;
using StudentMarksPredictor.Data;
using StudentMarksPredictor.Data.Models;

namespace StudentMarksPredictor.API.Services;

public class TrainingDataService
{
    private readonly Repository _repo;

    public TrainingDataService(Repository repo)
    {
        _repo = repo;
    }

    public async Task<AddTrainingDataResponse> AddRecordsAsync(AddTrainingDataRequest request)
    {
        if (request.Records.Count == 0)
            throw new ValidationException("En az bir kayit gonderilmeli");

        foreach (var item in request.Records)
        {
            if (item.NumberCourses <= 0)
                throw new ValidationException("NumberCourses 0 dan buyuk olmali");
            if (item.TimeStudy < 0)
                throw new ValidationException("TimeStudy negatif olamaz");
            if (item.Marks < 0)
                throw new ValidationException("Marks negatif olamaz");
        }

        var newRecords = request.Records.Select(r => new TrainingRecord
        {
            NumberCourses = r.NumberCourses,
            TimeStudy = r.TimeStudy,
            Marks = r.Marks
        }).ToList();

        await _repo.AddTrainingRecordsAsync(newRecords);

        var totalRecords = await _repo.GetTrainingRecordCountAsync();

        return new AddTrainingDataResponse
        {
            AddedRecords = newRecords.Count,
            TotalRecords = totalRecords
        };
    }
}
