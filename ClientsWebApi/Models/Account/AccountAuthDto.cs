namespace ClientsWebApi.Models.Dto
{
    public record AccountAuthDto
    {
        public string Token { get; set; } = string.Empty;
    }
}
