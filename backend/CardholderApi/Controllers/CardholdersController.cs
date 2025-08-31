using CardholderApi.DTOs;
using CardholderApi.Models;
using CardholderApi.Repositories.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CardholderApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CardholdersController(ICardholderRepository repository) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<PagedResult<CardholderDTO>>> GetAll(int page = 1, int pageSize = 10, string sortOrder = "desc")
        {
            var result = await repository.GetPagedAsync(page, pageSize, sortOrder);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CardholderDTO>> GetById(Guid id)
        {
            var cardholder = await repository.GetByIdAsync(id);
            return cardholder == null ? NotFound() : Ok(cardholder);
        }

        [HttpPost]
        public async Task<ActionResult<CardholderDTO>> Create([FromBody] CreateCardholderDTO dto)
        {
            try
            {
                var created = await repository.AddAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new
                {
                    Errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
                });
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCardholderDTO dto)
        {
            try
            {
                await repository.UpdateAsync(id, dto);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                return BadRequest(new
                {
                    Errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await repository.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
