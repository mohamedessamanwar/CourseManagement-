using BusinessAccessLayer.DTOS.CourceDtos;

namespace BusinessAccessLayer.Services.CourseService
{
    public interface ICourseService
    {
        Task<bool> CreateCourse(CreateCourseDto createCourseDto);
        Task<bool> UpdateCourse(CreateCourseDto createCourseDto, int id);
        Task<bool> DeleteCourse(int id);
        Task<ViewCourseDto> GetCourseById(int id);
        Task<IEnumerable<ViewCourseDto>> GetAllCourses();
    }
}
