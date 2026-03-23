using Microsoft.AspNetCore.Mvc;
using StudentMarksPredictor.API.DTOs;
using StudentMarksPredictor.API.Services;

namespace StudentMarksPredictor.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TrainController : ControllerBase
{
    private readonly TrainService _trainService;
    private readonly FineTuneService _fineTuneService;

    public TrainController(TrainService trainService, FineTuneService fineTuneService)
    {
        _trainService = trainService;
        _fineTuneService = fineTuneService;
    }

    [HttpPost("full")]
    public async Task<ActionResult<ApiResponse<TrainResponse>>> FullTrain([FromBody] TrainRequest request)
    {
        var result = await _trainService.TrainModelAsync(request);
        return Ok(ApiResponse<TrainResponse>.Ok(result, "Model sifirdan egitildi"));
    }

    [HttpPost("fine-tune")]
    public async Task<ActionResult<ApiResponse<TrainResponse>>> FineTune([FromBody] TrainRequest request)
    {
        var result = await _fineTuneService.FineTuneAsync(request);
        return Ok(ApiResponse<TrainResponse>.Ok(result, "Model mevcut agirliklar uzerinden fine-tune edildi"));
    }
}
