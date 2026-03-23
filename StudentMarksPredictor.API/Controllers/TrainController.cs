using Microsoft.AspNetCore.Mvc;
using StudentMarksPredictor.API.DTOs;
using StudentMarksPredictor.API.Services;

namespace StudentMarksPredictor.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TrainController : ControllerBase
{
    private readonly TrainService _service;

    public TrainController(TrainService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<TrainResponse>> Train([FromBody] TrainRequest request)
    {
        var result = await _service.TrainModelAsync(request);
        return Ok(result);
    }
}
