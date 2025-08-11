using EstevesPriceAlert.Application.Dtos;
using MediatR;

namespace EstevesPriceAlert.Application.Queries.GetUser
{
    public class GetUserByIdCommand : IRequest<UserDto>
    {
        public Guid Id { get; private set; }

        public GetUserByIdCommand(Guid id)
        {
            Id = id;
        }
    }
}
