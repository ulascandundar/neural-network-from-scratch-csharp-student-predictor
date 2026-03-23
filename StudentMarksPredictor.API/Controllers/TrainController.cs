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
    private readonly TrainHistoryService _historyService;

    public TrainController(TrainService trainService, FineTuneService fineTuneService, TrainHistoryService historyService)
    {
        _trainService = trainService;
        _fineTuneService = fineTuneService;
        _historyService = historyService;
    }

    [HttpGet("history")]
    public async Task<ActionResult<ApiResponse<TrainHistoryResponse>>> GetHistory()
    {
        var result = await _historyService.GetHistoryAsync();
        return Ok(ApiResponse<TrainHistoryResponse>.Ok(result, $"{result.TotalSessions} egitim listelendi"));
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
