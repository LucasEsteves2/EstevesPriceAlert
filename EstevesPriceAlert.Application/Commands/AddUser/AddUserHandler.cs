using EstevesPriceAlert.Core.Repositories;
using MediatR;

namespace EstevesPriceAlert.Application.Commands.AddUser
{
    public class AddUserHandler : IRequestHandler<AddUserCommand, Guid>
    {
        private readonly IUserRepository _repository;

        public AddUserHandler(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            var user = request.ToEntity();
            await _repository.AddAsync(user);
            return user.Id;
        }
    }
}
