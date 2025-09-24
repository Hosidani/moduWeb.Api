namespace ModuWeb.Api.Models.CRUDUserDtos
{
    public class ReadUserDto
    {
        public int Id { get; set; }
        //TODO: Decide Nullable or not for ALL TWO
        public required string UserName { get; set; }
        public string Email { get; set; }
        // Add other properties you want to expose
    }
}
