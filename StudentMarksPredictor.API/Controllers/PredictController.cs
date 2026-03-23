using Microsoft.AspNetCore.Mvc;
using StudentMarksPredictor.API.DTOs;
using StudentMarksPredictor.API.Services;

namespace StudentMarksPredictor.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PredictController : ControllerBase
{
    private readonly PredictService _service;

    public PredictController(PredictService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<PredictResponse>> Predict([FromBody] PredictRequest request)
    {
        var result = await _service.PredictMarksAsync(request);
        return Ok(result);
    }
}
