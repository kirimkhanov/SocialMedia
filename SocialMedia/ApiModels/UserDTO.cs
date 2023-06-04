using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SocialMedia.ApiModels;

public class UserDTO
{
    [SwaggerExclude]
    public int Id { get; set; }
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string SecondName { get; set; }
    [Required]
    public DateTime Birthdate { get; set; }
    [Required]
    public string Gender { get; set; }
    public string Biography { get; set; }
    [Required]
    public string City { get; set; }
    [JsonIgnore]
    [Required]
    public string Password { get; set; }
}