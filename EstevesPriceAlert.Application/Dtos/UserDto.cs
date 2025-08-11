using EstevesPriceAlert.Core.Entities;
using System.Net.NetworkInformation;

namespace EstevesPriceAlert.Application.Dtos
{
    public class UserDto
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? WhatsApp { get; set; }

        public UserDto()
        {
        }

        public static UserDto FromEntity(User user)
        {
            return new UserDto()
            {
                Email = user.Email,
                Id  = user.Id.ToString(),
                Name = user.Name,
                WhatsApp = user.WhatsApp
            };
        }
    }
}
