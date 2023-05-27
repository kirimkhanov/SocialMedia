using SocialMedia.Core.Entities;

namespace SocialMedia.ApiModels;

public static class UserDTOConverter
{
    public static UserDTO ToDTO(this User user) =>
        new ()
        {
            Id = user.Id,
            FirstName = user.FirstName,
            SecondName = user.SecondName,
            Biography = user.Biography,
            Birthdate = user.Birthdate,
            City = user.City,
            Gender = user.Gender
        };
    public static User FromDTO(this UserDTO userDTO, string password) =>
                  new ()
                  {
                      Id = userDTO.Id,
                      FirstName = userDTO.FirstName,
                      SecondName = userDTO.SecondName,
                      Biography = userDTO.Biography,
                      Birthdate = userDTO.Birthdate,
                      City = userDTO.City,
                      Gender = userDTO.Gender,
                      Password = password
                  };
}