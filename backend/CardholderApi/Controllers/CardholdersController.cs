using CardholderApi.Entities;
using CardholderApi.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CardholderApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CardholdersController(ICardholderRepository repository) : Controller
    {
        private readonly ICardholderRepository _repository = repository;

        [HttpGet]
        public async Task<ActionResult<List<Cardholder>>> GetAll()
        {
            var cardholders = await _repository.GetAllAsync();
            return Ok(cardholders);
        }

    }
}
