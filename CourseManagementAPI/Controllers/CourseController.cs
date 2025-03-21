using BusinessAccessLayer.DTOS;
using BusinessAccessLayer.DTOS.CourceDtos;
using BusinessAccessLayer.Services.CacheService;
using BusinessAccessLayer.Services.CourseService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace CourseManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly ILogger<CourseController> _logger;
        private readonly ICacheService cacheService;

        public CourseController(ICourseService courseService, ILogger<CourseController> logger, ICacheService cacheService)
        {
            _courseService = courseService;
            _logger = logger;
            this.cacheService = cacheService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDto createCourseDto)
        {
            _logger.LogInformation("Creating a new course.");
            var result = await _courseService.CreateCourse(createCourseDto);

            if (!result)
            {
                _logger.LogWarning("Failed to create course.");
                return BadRequest(new Response<string>("Failed to create course", false));
            }

            _logger.LogInformation("Course created successfully.");
            return Ok(new Response<string>("Course created successfully", true));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] CreateCourseDto createCourseDto)
        {
            _logger.LogInformation("Updating course with ID: {CourseId}", id);
            var result = await _courseService.UpdateCourse(createCourseDto, id);

            if (!result)
            {
                _logger.LogWarning("Failed to update course with ID: {CourseId}", id);
                return NotFound(new Response<string>("Course not found", false));
            }

            _logger.LogInformation("Course updated successfully with ID: {CourseId}", id);
            return Ok(new Response<string>("Course updated successfully", true));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            _logger.LogInformation("Deleting course with ID: {CourseId}", id);
            var result = await _courseService.DeleteCourse(id);

            if (!result)
            {
                _logger.LogWarning("Course not found with ID: {CourseId}", id);
                return NotFound(new Response<string>("Course not found", false));
            }

            _logger.LogInformation("Course deleted successfully with ID: {CourseId}", id);
            return Ok(new Response<string>("Course deleted successfully", true));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourseById(int id)
        {
            _logger.LogInformation("Fetching course details for ID: {CourseId}", id);

            // Check cache first
            var cacheKey = $"Course_{id}";
            var cachedCourse = cacheService.GetCache<ViewCourseDto>(cacheKey);
            if (cachedCourse != null)
            {
                _logger.LogInformation("Course fetched from cache for ID: {CourseId}", id);
                return Ok(new Response<ViewCourseDto>(cachedCourse));
            }

            // Fetch from database if not in cache
            var course = await _courseService.GetCourseById(id);
            if (course == null)
            {
                _logger.LogWarning("Course not found with ID: {CourseId}", id);
                return NotFound(new Response<string>("Course not found", false));
            }

            // Store in cache for future requests
            cacheService.SetCache(cacheKey, course, TimeSpan.FromMinutes(10));
            _logger.LogInformation("Course details fetched successfully for ID: {CourseId}", id);
            return Ok(new Response<ViewCourseDto>(course));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCourses()
        {
            _logger.LogInformation("Fetching all courses.");

            // Check cache first
            var cacheKey = "AllCourses";
            var cachedCourses = cacheService.GetCache<IEnumerable<ViewCourseDto>>(cacheKey);
            if (cachedCourses != null)
            {
                _logger.LogInformation("Courses fetched from cache.");
                return Ok(new Response<IEnumerable<ViewCourseDto>>(cachedCourses));
            }

            // Fetch from database if not in cache
            var courses = await _courseService.GetAllCourses();
            if (!courses.Any())
            {
                _logger.LogWarning("No courses found.");
                return NotFound(new Response<string>("No courses found", false));
            }

            // Store in cache for future requests
            cacheService.SetCache(cacheKey, courses, TimeSpan.FromMinutes(10));
            _logger.LogInformation("Fetched {Count} courses.", courses.Count());
            return Ok(new Response<IEnumerable<ViewCourseDto>>(courses));
        }
    }
}
