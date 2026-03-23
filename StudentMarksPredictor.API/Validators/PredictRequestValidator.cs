using StudentMarksPredictor.API.DTOs;
using StudentMarksPredictor.Shared.Exceptions;

namespace StudentMarksPredictor.API.Validators;

public static class PredictRequestValidator
{
    public static void Validate(PredictRequest request)
    {
        if (request.NumberCourses <= 0)
            throw new ValidationException("NumberCourses 0 dan buyuk olmali");

        if (request.TimeStudy < 0)
            throw new ValidationException("TimeStudy negatif olamaz");
    }
}
