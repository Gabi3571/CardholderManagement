using CardholderApi.DTOs;

namespace CardholderApi.Repositories.Interfaces
{
    public interface ICardholderRepository
    {
        Task<List<CardholderDTO>> GetAllAsync();
        Task<CardholderDTO?> GetByIdAsync(Guid id);
        Task<CardholderDTO> AddAsync(CreateCardholderDTO dto);
        Task UpdateAsync(Guid id, UpdateCardholderDTO dto);
        Task DeleteAsync(Guid id);
    }
}
