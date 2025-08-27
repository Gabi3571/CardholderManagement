using CardholderApi.Entities;
using CardholderApi.Persistence;
using CardholderApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CardholderApi.Repositories
{
    public class CardholderRepository : ICardholderRepository
    {
        private readonly CardholderDbContext _context;

        public CardholderRepository(CardholderDbContext context)
        {
            _context = context;
        }

        public Task AddAsync(Cardholder cardholder)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Cardholder>> GetAllAsync()
        {
            return await _context.Cardholders.ToListAsync();
        }

        public Task<Cardholder?> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Cardholder cardholder)
        {
            throw new NotImplementedException();
        }
    }
}
