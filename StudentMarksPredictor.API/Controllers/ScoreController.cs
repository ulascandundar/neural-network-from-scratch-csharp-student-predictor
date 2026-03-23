using Microsoft.AspNetCore.Mvc;
using StudentMarksPredictor.API.DTOs;
using StudentMarksPredictor.API.Services;

namespace StudentMarksPredictor.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScoreController : ControllerBase
{
    private readonly ScoreService _service;

    public ScoreController(ScoreService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<ScoreResponse>>> Evaluate()
    {
        var result = await _service.EvaluateAsync();
        return Ok(ApiResponse<ScoreResponse>.Ok(result, "Model basari metrikleri hesaplandi"));
    }
}
