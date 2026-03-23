using StudentMarksPredictor.API.DTOs;
using StudentMarksPredictor.Shared.Exceptions;

namespace StudentMarksPredictor.API.Validators;

public static class TrainRequestValidator
{
    public static void Validate(TrainRequest request)
    {
        if (request.Epochs <= 0)
            throw new ValidationException("Epochs 0 dan buyuk olmali");

        if (request.LearningRate <= 0)
            throw new ValidationException("LearningRate 0 dan buyuk olmali");

        if (request.HiddenSize <= 0)
            throw new ValidationException("HiddenSize 0 dan buyuk olmali");

        if (request.TestSplitRatio < 0 || request.TestSplitRatio >= 1)
            throw new ValidationException("TestSplitRatio 0 ile 1 arasinda olmali");
    }
}
