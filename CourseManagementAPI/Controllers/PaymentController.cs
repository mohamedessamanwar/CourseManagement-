using BusinessAccessLayer.DTOS.TrainerCourseDtos;
using BusinessAccessLayer.Services.PaymentServise;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddPayment([FromBody] AddCourseToTrainerDto courseTrainerDto)
        {
            var result = await _paymentService.AddPaymentToCourse(courseTrainerDto);

            if (!result)
                return BadRequest(new { message = "Failed to complete payment." });

            return Ok(new { message = "payment successfully added." });
        }
    }
}
