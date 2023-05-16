namespace SocialMedia.ApiModels;

public class UserDTO
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string SecondName { get; set; }
    public DateTime Birthdate { get; set; }
    public string Gender { get; set; }
    public string Biography { get; set; }
    public string City { get; set; }
    public string Password { get; set; }
}