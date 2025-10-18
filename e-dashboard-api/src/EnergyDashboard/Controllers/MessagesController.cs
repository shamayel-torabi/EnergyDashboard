using EnergyDashboard.Application.Messages;
using EnergyDashboard.Application.Messages.Commands;
using EnergyDashboard.Application.Messages.Queries;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace EnergyDashboard.Controllers;

[Route("[controller]")]
[ApiController]
public class MessagesController : ControllerBase
{
    private readonly ISender _mediator;

    public MessagesController(ISender mediator)
    {
        _mediator = mediator;
    }

    // GET: api/Messages
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessages(CancellationToken token)
    {
        var messages = await _mediator.Send(new GetMessagesQuery(), token);
        return Ok(messages);
    }

    // GET: api/Messages/{date}
    [HttpGet("{date}")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesByDate(DateTimeOffset date, CancellationToken token)
    {
        var messages = await _mediator.Send(new GetMessagesByDateQuery { Date = date}, token);
        return Ok(messages);
    }

    // DELETE: api/Messages/5
    [HttpDelete("{id}")]
    public async Task<ActionResult<Guid>> DeleteMessage(Guid id, CancellationToken token)
    {
        var messagesId = await _mediator.Send(new DeleteMessageCommand { MessaageId = id }, token);
        return Ok(messagesId);
    }

    // GET: api/Messages/DeleteAllAtDate/date
    [HttpDelete("DeleteAllAtDate/{date}")]
    public async Task<ActionResult> DeleteAllAtDate(DateTimeOffset date, CancellationToken token)
    {
        await _mediator.Send(new DeleteMessageByDateCommand { Date = date }, token);
        return Ok();
    }
}
