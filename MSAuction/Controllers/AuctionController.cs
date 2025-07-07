using MediatR;
using Microsoft.AspNetCore.Mvc;
using MSAuction.Commons.DTOs;
using MSAuction.Application.Commands;

namespace MSAuction.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuctionController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuctionController(IMediator mediator)
    {
        _mediator = mediator;
    }
    // Aca se crea la subasta y se activan los background jobs de activacion y finalizacion
    [HttpPost] 
    public async Task<IActionResult> CreateAuction([FromBody] AuctionDto dto, [FromHeader] int userId)
    {
        var command = new CreateAuctionCommand(dto, userId);
        var auctionId = await _mediator.Send(command);
        return Ok(auctionId);
    }
    // con este editas la subasta solo si esta en pendiente
    [HttpPut("{id}")]
    public async Task<IActionResult> EditAuction(int id, [FromBody] AuctionDto dto, [FromHeader] int userId)
    {
        var command = new UpdateAuctionCommand(id, dto, userId);
        var success = await _mediator.Send(command);

        if (success)
        {
            return Ok(new { message = "Subasta actualizada con éxito." });
        }
        else
        {
            return BadRequest(new
                { message = "No se pudo actualizar la subasta. Verifique los datos e intente nuevamente." });
        }
    }

    // con este eliminas la subasta igualmente solo si el estado es pendiente
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAuction(int id, [FromHeader] int userId)
    {
        var command = new DeleteAuctionCommand(id, userId);
        await _mediator.Send(command);
        return NoContent();
    }
}