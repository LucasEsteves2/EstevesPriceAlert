using EstevesPriceAlert.Application.Commands.AddOffert;
using EstevesPriceAlert.Application.Commands.AddUser;
using EstevesPriceAlert.Application.Dtos;
using EstevesPriceAlert.Application.Queries.GetUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EstevesPriceAlert.Api.Controllers
{
    [ApiController]
    [Route("api/User")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var query = new GetUserByIdCommand(id);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound();

            return Ok(result);
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserInputDto input)
        {
            var command = new AddUserCommand() { UserInputDto = input };
            var id = await _mediator.Send(command);

            return CreatedAtAction(nameof(Get), new { id = id }, command);
        }


        [HttpPost("/api/users/notifications")]
        public async Task<IActionResult> AddNotificaiton([FromBody] UserOffertInput notification)
        {
            var user = Get(notification.UserId);

            if (user == null)
                return NotFound();

            var command = new AddNotificationCommand() { UserOffertInput = notification, UserId = notification.UserId };
            var result = await _mediator.Send(command);

            return Ok(result);
        }
    }
}
