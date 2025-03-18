using BusinessAccessLayer.DTOS;
using BusinessAccessLayer.DTOS.TrainerDtos;
using BusinessAccessLayer.Services.TrainerService;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class TrainerController : ControllerBase
{
    private readonly ITrainerService _trainerService;

    public TrainerController(ITrainerService trainerService)
    {
        _trainerService = trainerService;
    }

    [HttpPost]
    public async Task<IActionResult> AddTrainer([FromBody] CreateTrainerDto createTrainerDto)
    {
        var result = await _trainerService.AddTrainer(createTrainerDto);
        if (!result)
            return BadRequest(new Response<string>("Failed to add trainer", false));

        return Ok(new Response<string>("Trainer added successfully", true));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTrainer(int id, [FromBody] CreateTrainerDto createTrainerDto)
    {
        var result = await _trainerService.UpdateTrainer(createTrainerDto, id);
        if (!result)
            return NotFound(new Response<string>("Update Have Problem", false));

        return Ok(new Response<string>("Trainer updated successfully", true));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTrainer(int id)
    {
        var result = await _trainerService.DeleteTrainer(id);
        if (!result)
            return NotFound(new Response<string>("Trainer not found", false));

        return Ok(new Response<string>("Trainer deleted successfully", true));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ViewTrainer(int id)
    {
        var trainer = await _trainerService.ViewTrainer(id);
        return Ok(new Response<TrainerViewDto>(trainer));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTrainers()
    {
        var trainers = await _trainerService.GetAllTrainers();
        return Ok(new Response<IEnumerable<TrainerViewDto>>(trainers));
    }
}
