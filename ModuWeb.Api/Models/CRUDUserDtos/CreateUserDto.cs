namespace ModuWeb.Api.Models.CRUDUserDtos
{
    public class CreateUserDto
    {
        //TODO: Decide Nullable or not for ALL THREE
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        // Add other properties needed for creation
    }
}
