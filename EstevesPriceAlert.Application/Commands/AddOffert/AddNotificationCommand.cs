using EstevesPriceAlert.Application.Dtos;
using EstevesPriceAlert.Core.Entities;
using EstevesPriceAlert.Core.ValueObjects;
using MediatR;

namespace EstevesPriceAlert.Application.Commands.AddOffert
{
    public class AddNotificationCommand : IRequest<User>
    {
        public UserOffertInput UserOffertInput { get; set; } = default;
        public Guid UserId { get; set; }


        public Notifications NotificationTotEntity()
        {
            var notification = new Notifications()
            {
                IsActive = true,
                LastNotifiedAtUtc = DateTime.UtcNow,
                MinHoursBetweenAlerts = UserOffertInput.Notifications.MinHoursBetweenAlerts,
                NotifyCount = UserOffertInput.Notifications.NotifyCount,
                ProductUrls = UserOffertInput.Notifications.ProductUrls,
                TargetPrice = UserOffertInput.Notifications.TargetPrice,
                ProductName = UserOffertInput.Notifications.ProductName,
            };

            return notification;
        }
    }
}
