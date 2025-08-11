using EstevesPriceAlert.Application.Dtos;
using EstevesPriceAlert.Core.Repositories;
using MediatR;

namespace EstevesPriceAlert.Application.Queries.GetUser
{
    public class GetUserHandler : IRequestHandler<GetUserByIdCommand, UserDto>
    {
        private readonly IUserRepository _repository;

        public GetUserHandler(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<UserDto> Handle(GetUserByIdCommand request, CancellationToken cancellationToken)
        {
            var user = await _repository.GetByIdAsync(request.Id);
            var userDto = UserDto.FromEntity(user);
            return userDto;
        }
    }
}
