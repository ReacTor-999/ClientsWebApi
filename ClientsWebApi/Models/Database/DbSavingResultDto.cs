namespace ClientsWebApi.Models.Dto
{
    public record DbSavingResultDto
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; } = string.Empty;
    }
}
