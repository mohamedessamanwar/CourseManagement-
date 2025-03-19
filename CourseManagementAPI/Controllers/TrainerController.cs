using BusinessAccessLayer.DTOS;
using BusinessAccessLayer.DTOS.TrainerDtos;
using BusinessAccessLayer.Services.TrainerService;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class TrainerController : ControllerBase
{
    private readonly ITrainerService _trainerService;
    private readonly ILogger<TrainerController> _logger;

    public TrainerController(ITrainerService trainerService, ILogger<TrainerController> logger)
    {
        _trainerService = trainerService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> AddTrainer([FromBody] CreateTrainerDto createTrainerDto)
    {
        _logger.LogInformation("Adding a new trainer.");
        var result = await _trainerService.AddTrainer(createTrainerDto);

        if (!result)
        {
            _logger.LogWarning("Failed to add trainer.");
            return BadRequest(new Response<string>("Failed to add trainer", false));
        }

        _logger.LogInformation("Trainer added successfully.");
        return Ok(new Response<string>("Trainer added successfully", true));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTrainer(int id, [FromBody] CreateTrainerDto createTrainerDto)
    {
        _logger.LogInformation("Updating trainer with ID: {TrainerId}", id);
        var result = await _trainerService.UpdateTrainer(createTrainerDto, id);

        if (!result)
        {
            _logger.LogWarning("Trainer update failed for ID: {TrainerId}", id);
            return NotFound(new Response<string>("Update has a problem", false));
        }

        _logger.LogInformation("Trainer updated successfully with ID: {TrainerId}", id);
        return Ok(new Response<string>("Trainer updated successfully", true));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTrainer(int id)
    {
        _logger.LogInformation("Deleting trainer with ID: {TrainerId}", id);
        var result = await _trainerService.DeleteTrainer(id);

        if (!result)
        {
            _logger.LogWarning("Trainer not found with ID: {TrainerId}", id);
            return NotFound(new Response<string>("Trainer not found", false));
        }

        _logger.LogInformation("Trainer deleted successfully with ID: {TrainerId}", id);
        return Ok(new Response<string>("Trainer deleted successfully", true));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ViewTrainer(int id)
    {
        _logger.LogInformation("Fetching trainer details for ID: {TrainerId}", id);
        var trainer = await _trainerService.ViewTrainer(id);

        if (trainer == null)
        {
            _logger.LogWarning("Trainer not found with ID: {TrainerId}", id);
            return NotFound(new Response<string>("Trainer not found", false));
        }

        _logger.LogInformation("Trainer details fetched successfully for ID: {TrainerId}", id);
        return Ok(new Response<TrainerViewDto>(trainer));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTrainers()
    {
        _logger.LogInformation("Fetching all trainers.");
        var trainers = await _trainerService.GetAllTrainers();

        _logger.LogInformation("Fetched {Count} trainers.", trainers.Count());
        return Ok(new Response<IEnumerable<TrainerViewDto>>(trainers));
    }
}
