using BusinessAccessLayer.DTOS.CourceDtos;
using BusinessAccessLayer.Exceptions;
using DataAccessLayer.Data.Models;
using DataAccessLayer.Repositories.CourseRepo;

namespace BusinessAccessLayer.Services.CourseService
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepo courseRepo;

        public CourseService(ICourseRepo courseRepo)
        {
            this.courseRepo = courseRepo;
        }

        public async Task<bool> CreateCourse(CreateCourseDto createCourseDto)
        {
            await courseRepo.AddAsync(new Course()
            {
                Title = createCourseDto.Title,
                Description = createCourseDto.Description,
                Price = createCourseDto.Price,
                CreatedAt = DateTime.UtcNow
            });
            var isSaved = (await courseRepo.Complete()) > 0;
            return isSaved;
        }

        public async Task<bool> UpdateCourse(CreateCourseDto createCourseDto, int id)
        {
            var course = await courseRepo.GetByIdAsync(id);
            if (course == null)
                throw new NotFoundException("Course not found");

            course.Title = createCourseDto.Title;
            course.Description = createCourseDto.Description;
            course.Price = createCourseDto.Price;
            course.UpdatedAt = DateTime.UtcNow;

            courseRepo.Update(course, nameof(course.Title), nameof(course.Description), nameof(course.Price));
            var isSaved = await courseRepo.Complete() > 0;
            return isSaved;
        }

        public async Task<bool> DeleteCourse(int id)
        {
            var course = await courseRepo.GetByIdAsync(id);
            if (course == null)
                throw new NotFoundException("Course not found");

            courseRepo.Delete(course);
            var isSaved = await courseRepo.Complete() > 0;
            return isSaved;
        }

        public async Task<ViewCourseDto> GetCourseById(int id)
        {
            var course = await courseRepo.GetByIdAsync(id);
            if (course == null)
                throw new NotFoundException("Course not found");

            return new ViewCourseDto()
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                Price = course.Price,
                CreatedAt = course.CreatedAt,
                UpdatedAt = course.UpdatedAt
            };
        }

        public async Task<IEnumerable<ViewCourseDto>> GetAllCourses()
        {
            var courses = await courseRepo.GetAll();
            return courses.Select(course => new ViewCourseDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                Price = course.Price,
                CreatedAt = course.CreatedAt,
                UpdatedAt = course.UpdatedAt
            }).ToList();
        }

    }
}

