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
    public class CardholdersController(ICardholderRepository repository, ILogger<CardholdersController> logger) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<PagedResult<CardholderDTO>>> GetCardholders(int page = 1, int pageSize = 10, string sortOrder = "desc")
        {
            logger.LogInformation("GetCardholders called with page={Page}, pageSize={PageSize}, sortOrder={SortOrder}", page, pageSize, sortOrder);
            var result = await repository.GetPagedAsync(page, pageSize, sortOrder);

            logger.LogInformation("GetCardholders returned {Count} items", result.Items.Count);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CardholderDTO>> GetById(Guid id)
        {
            logger.LogInformation("GetById called with id={Id}", id);
            var cardholder = await repository.GetByIdAsync(id);

            if (cardholder == null)
            {
                logger.LogWarning("Cardholder with id={Id} not found", id);
                return NotFound();
            }

            logger.LogInformation("Cardholder with id={Id} retrieved", id);
            return Ok(cardholder);
        }

        [HttpPost]
        public async Task<ActionResult<CardholderDTO>> Create([FromBody] CreateCardholderDTO dto)
        {
            logger.LogInformation("Create called with DTO {@DTO}", dto);

            try
            {
                var created = await repository.AddAsync(dto);
                logger.LogInformation("Cardholder created successfully with id={Id}", created.Id);

                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ValidationException ex)
            {
                logger.LogWarning("Validation failed while creating Cardholder: {@Errors}", ex.Errors);

                return BadRequest(new
                {
                    Errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error occurred while creating Cardholder");
                
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCardholderDTO dto)
        {
            logger.LogInformation("Update called for id={Id} with DTO {@DTO}", id, dto);

            try
            {
                await repository.UpdateAsync(id, dto);
                logger.LogInformation("Cardholder with id={Id} updated successfully", id);

                return NoContent();
            }
            catch (ValidationException ex)
            {
                logger.LogWarning("Validation failed while updating Cardholder id={Id}: {@Errors}", id, ex.Errors);

                return BadRequest(new
                {
                    Errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
                });
            }
            catch (KeyNotFoundException ex)
            {
                logger.LogWarning("Cardholder with id={Id} not found while updating", id);

                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error occurred while updating Cardholder id={Id}", id);
                
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            logger.LogInformation("Delete called for id={Id}", id);

            try
            {
                await repository.DeleteAsync(id);
                logger.LogInformation("Cardholder with id={Id} deleted successfully", id);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                logger.LogWarning("Cardholder with id={Id} not found while deleting", id);

                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error occurred while deleting Cardholder id={Id}", id);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }
    }
}
