using CardholderApi.DTOs;
using CardholderApi.Models;

namespace CardholderApi.Repositories.Interfaces
{
    public interface ICardholderRepository
    {
        Task<PagedResult<CardholderDTO>> GetPagedAsync(int page, int pageSize, string sortOrder);
        Task<CardholderDTO?> GetByIdAsync(Guid id);
        Task<CardholderDTO> AddAsync(CreateCardholderDTO dto);
        Task UpdateAsync(Guid id, UpdateCardholderDTO dto);
        Task DeleteAsync(Guid id);
    }
}
