namespace ModuWeb.Api.Models.CRUDUserDtos
{
    public class UpdateUserDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Password { get; set; } // Optional

    }
}