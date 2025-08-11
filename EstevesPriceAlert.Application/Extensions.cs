using EstevesPriceAlert.Application.Commands.AddUser;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace EstevesPriceAlert.Application
{
    public static class Extensions
    {
        public static IServiceCollection AddHandlers(this IServiceCollection services)
        {
            services.AddMediatR(typeof(AddUserCommand));

            return services;
        }
    }
}
