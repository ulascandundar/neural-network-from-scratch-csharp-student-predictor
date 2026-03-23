using Microsoft.AspNetCore.Mvc;
using StudentMarksPredictor.API.DTOs;
using StudentMarksPredictor.API.Services;

namespace StudentMarksPredictor.API.Controllers;

[ApiController]
[Route("api/training-data")]
public class TrainingDataController : ControllerBase
{
    private readonly TrainingDataService _service;
    private readonly TrainingDataQueryService _queryService;

    public TrainingDataController(TrainingDataService service, TrainingDataQueryService queryService)
    {
        _service = service;
        _queryService = queryService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<GetTrainingDataResponse>>> GetAll()
    {
        var result = await _queryService.GetAllAsync();
        return Ok(ApiResponse<GetTrainingDataResponse>.Ok(result, $"{result.TotalRecords} kayit listelendi"));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<AddTrainingDataResponse>>> AddRecords([FromBody] AddTrainingDataRequest request)
    {
        var result = await _service.AddRecordsAsync(request);
        return Ok(ApiResponse<AddTrainingDataResponse>.Ok(result, $"{result.AddedRecords} kayit eklendi"));
    }
}
