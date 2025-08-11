using EstevesPriceAlert.Application.Dtos;
using EstevesPriceAlert.Core.Entities;
using MediatR;

namespace EstevesPriceAlert.Application.Commands.AddUser
{
    public class AddUserCommand : IRequest<Guid>
    {
        public UserInputDto UserInputDto { get; set; } = default!;

        public User ToEntity()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = UserInputDto.Name?.Trim() ?? string.Empty,
                Email = UserInputDto.Email?.Trim() ?? string.Empty,
                CreatedAt = DateTime.UtcNow,
                Notifications = []
            };

            return user;
        }
    }
}
