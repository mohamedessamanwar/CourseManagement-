﻿

namespace BusinessAccessLayer.DTOS.TrainerDtos
{
    public class CreateTrainerDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

    }
}
