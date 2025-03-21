using BusinessAccessLayer.DTOS;
using BusinessAccessLayer.DTOS.TrainerCourseDtos;
using BusinessAccessLayer.Services.TrainerCourseService;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

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
        [EndpointName("AddCourseToTrainer")]
        public async Task<IActionResult> AddCourseToTrainer([FromBody] AddCourseToTrainerDto courseTrainerDto)
        {
            var result = await _trainerCourseService.AddCourseToTrainer(courseTrainerDto);
            if (!result)
                return BadRequest(new Response<string>(null, false, "Failed to assign course to trainer."));

            return Ok(new Response<string>("Course successfully assigned to trainer.", true));
        }


        [HttpGet]
        [Route("{id}")]
        [EndpointName("GetCoursesForTrainer")]
        public async Task<IActionResult> GetCourseToTrainer([FromRoute] int id)
        {
            IEnumerable<CourseTrainersViewDto>? result = await _trainerCourseService.GetCourseTrainers(id);
            return Ok(new Response<IEnumerable<CourseTrainersViewDto>>(result, true));
        }

        [HttpGet]
        [EndpointName("GetCourseTrainersWithPaymentsReport")]
        public async Task<IActionResult> GetCourseToTrainer()
        {
            IEnumerable<CourseTrainersWithPaymentsView>? result = await _trainerCourseService.GetCourseTrainersWithPayments();
            return Ok(new Response<IEnumerable<CourseTrainersWithPaymentsView>>(result, true));
        }

        [HttpGet]
        [Route("export-pdf")]
        [EndpointName("GetCourseTrainersWithPaymentsReportpdf")]
        public async Task<IActionResult> GetCourseTrainersWithPaymentsReportpdf()
        {
            IEnumerable<CourseTrainersWithPaymentsView>? result = await _trainerCourseService.GetCourseTrainersWithPayments();

            if (result == null || !result.Any())
                return NotFound(new Response<string>(null, false, "No course trainers with payments found."));

            using (MemoryStream stream = new MemoryStream())
            {
                Document document = new Document(PageSize.A4);
                PdfWriter writer = PdfWriter.GetInstance(document, stream);
                document.Open();

                Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                Paragraph title = new Paragraph("Course Trainers With Payments Report", titleFont)
                {
                    Alignment = Element.ALIGN_CENTER
                };
                document.Add(title);
                document.Add(new Paragraph("\n"));

                Font subTitleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 13);
                Font dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 11);

                foreach (var item in result)
                {
                    // Trainer Information
                    string trainerName = item.Trainer?.FullName ?? "Unknown Trainer";
                    string trainerEmail = item.Trainer?.Email ?? "N/A";
                    string trainerPhone = item.Trainer?.PhoneNumber ?? "N/A";

                    document.Add(new Paragraph($"Trainer: {trainerName} (ID: {item.TrainerId})", subTitleFont));
                    document.Add(new Paragraph($"Email: {trainerEmail}", dataFont));
                    document.Add(new Paragraph($"Phone: {trainerPhone}", dataFont));

                    // Course Information
                    string courseTitle = item.Course?.Title ?? "Unknown Course";
                    string courseDesc = item.Course?.Description ?? "No description";
                    string coursePrice = item.Course?.Price.ToString("C2", CultureInfo.CurrentCulture) ?? "$0.00";

                    document.Add(new Paragraph($"Course: {courseTitle} (ID: {item.CourseId})", subTitleFont));
                    document.Add(new Paragraph($"Description: {courseDesc}", dataFont));
                    document.Add(new Paragraph($"Price: {coursePrice}", dataFont));
                    document.Add(new Paragraph("\n"));

                    // Payments Table
                    PdfPTable table = new PdfPTable(4); // Corrected to match actual number of columns
                    table.WidthPercentage = 100;
                    table.SetWidths(new float[] { 15f, 20f, 20f, 25f }); // Adjusted to match 4 columns

                    // Table Headers
                    Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                    string[] headers = { "Payment ID", "Amount", "Remaining", "Date" };

                    foreach (string header in headers)
                    {
                        PdfPCell cell = new PdfPCell(new Phrase(header, headerFont))
                        {
                            BackgroundColor = new BaseColor(200, 200, 200),
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            Padding = 5
                        };
                        table.AddCell(cell);
                    }

                    // Payment Rows
                    if (item.Payments != null && item.Payments.Any())
                    {
                        foreach (var payment in item.Payments)
                        {
                            table.AddCell(new PdfPCell(new Phrase(payment?.Id.ToString() ?? "N/A", dataFont))
                            {
                                HorizontalAlignment = Element.ALIGN_CENTER
                            });
                            table.AddCell(new PdfPCell(new Phrase(payment?.Amount.ToString("C2", CultureInfo.CurrentCulture) ?? "$0.00", dataFont))
                            {
                                HorizontalAlignment = Element.ALIGN_RIGHT
                            });
                            table.AddCell(new PdfPCell(new Phrase(payment?.RemainingAmount.ToString("C2", CultureInfo.CurrentCulture) ?? "$0.00", dataFont))
                            {
                                HorizontalAlignment = Element.ALIGN_RIGHT
                            });
                            table.AddCell(new PdfPCell(new Phrase(payment?.CreatedAt.ToString("MM/dd/yyyy") ?? "N/A", dataFont))
                            {
                                HorizontalAlignment = Element.ALIGN_CENTER
                            });
                        }
                    }
                    else
                    {
                        PdfPCell noDataCell = new PdfPCell(new Phrase("No payment records found", dataFont))
                        {
                            Colspan = 4, // Corrected to match actual column count
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            Padding = 5
                        };
                        table.AddCell(noDataCell);
                    }

                    document.Add(table);
                    document.Add(new Paragraph("\n\n"));
                }

                document.Close();
                writer.Close();

                byte[] bytes = stream.ToArray();
                return File(bytes, "application/pdf", "CourseTrainersWithPaymentsReport.pdf");
            }
        }
    }
}


