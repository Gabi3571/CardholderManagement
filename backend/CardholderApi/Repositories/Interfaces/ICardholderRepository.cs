using CardholderApi.Entities;

namespace CardholderApi.Repositories.Interfaces
{
    public interface ICardholderRepository
    {
        Task<List<Cardholder>> GetAllAsync();
        Task<Cardholder?> GetByIdAsync(Guid id);
        Task AddAsync(Cardholder cardholder);
        Task UpdateAsync(Cardholder cardholder);
        Task DeleteAsync(Guid id);
    }
}
