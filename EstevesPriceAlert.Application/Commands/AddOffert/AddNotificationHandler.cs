using EstevesPriceAlert.Core.Entities;
using EstevesPriceAlert.Core.Repositories;
using MediatR;

namespace EstevesPriceAlert.Application.Commands.AddOffert
{
    public class AddNotificationHandler : IRequestHandler<AddNotificationCommand, User>
    {
        private readonly IUserRepository _repository;

        public AddNotificationHandler(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<User> Handle(AddNotificationCommand request, CancellationToken cancellationToken)
        {
            var user = await _repository.GetByIdAsync(request.UserId);

            if (user == null)
                return null;

            user.Notifications.Add(request.NotificationTotEntity());
            await _repository.UpdateAsync(user);

            return user;
        }
    }
}
