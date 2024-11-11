namespace ClientsWebApi.Models.ContactInfo
{
    public record ContactInfoGetDto
    {
        public int Id { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string Description { get; set; } = string.Empty;

        public string? Other { get; set; }

        public ContactInfoGetDto(Entities.ContactInfo contactInfo)
        {
            Id = contactInfo.Id;
            Email = contactInfo.Email;
            PhoneNumber = contactInfo.PhoneNumber;
            Description = contactInfo.Description;
            Other = contactInfo.Other;
        }
    }
}
