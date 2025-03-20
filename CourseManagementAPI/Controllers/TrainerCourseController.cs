using BusinessAccessLayer.DTOS.TrainerCourseDtos;
using BusinessAccessLayer.Services.TrainerCourseService;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainerCourseController : ControllerBase
    {
        private readonly ITrainerCourseService _trainerCourseService;

        public TrainerCourseController(ITrainerCourseService trainerCourseService)
        {
            _trainerCourseService = trainerCourseService;
        }

        [HttpPost]
        public async Task<IActionResult> AddCourseToTrainer([FromBody] AddCourseToTrainerDto courseTrainerDto)
        {
            var result = await _trainerCourseService.AddCourseToTrainer(courseTrainerDto);
            if (!result)
                return BadRequest(new { message = "Failed to assign course to trainer." });

            return Ok(new { message = "Course successfully assigned to trainer." });
        }
    }
}
