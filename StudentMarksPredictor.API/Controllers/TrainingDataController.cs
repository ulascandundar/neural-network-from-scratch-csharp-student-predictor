using Microsoft.AspNetCore.Mvc;
using StudentMarksPredictor.API.DTOs;
using StudentMarksPredictor.API.Services;

namespace StudentMarksPredictor.API.Controllers;

[ApiController]
[Route("api/training-data")]
public class TrainingDataController : ControllerBase
{
    private readonly TrainingDataService _service;

    public TrainingDataController(TrainingDataService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<AddTrainingDataResponse>>> AddRecords([FromBody] AddTrainingDataRequest request)
    {
        var result = await _service.AddRecordsAsync(request);
        return Ok(ApiResponse<AddTrainingDataResponse>.Ok(result, $"{result.AddedRecords} kayit eklendi"));
    }
}
