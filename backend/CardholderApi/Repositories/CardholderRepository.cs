using AutoMapper;
using CardholderApi.DTOs;
using CardholderApi.Entities;
using CardholderApi.Models;
using CardholderApi.Persistence;
using CardholderApi.Repositories.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CardholderApi.Repositories
{
    public class CardholderRepository(
        CardholderDbContext context, 
        IValidator<Cardholder> validator, 
        IMapper mapper
    ) : ICardholderRepository
    {
        public async Task<PagedResult<CardholderDTO>> GetPagedAsync(int page, int pageSize, string sortOrder)
        {
            var query = context.Cardholders.AsNoTracking();

            query = sortOrder.ToLower() switch
            {
                "asc" => query.OrderBy(c => c.TransactionCount),
                _ => query.OrderByDescending(c => c.TransactionCount)
            };

            var totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            return new PagedResult<CardholderDTO>
            {
                Items = mapper.Map<List<CardholderDTO>>(items),
                TotalCount = totalCount
            };
        }

        public async Task<CardholderDTO?> GetByIdAsync(Guid id)
        {
            var entity = await context.Cardholders.FirstOrDefaultAsync(c => c.Id == id);
            return entity == null ? null : mapper.Map<CardholderDTO>(entity);
        }

        public async Task<CardholderDTO> AddAsync(CreateCardholderDTO dto)
        {
            var entity = mapper.Map<Cardholder>(dto);

            var validationResult = await validator.ValidateAsync(entity);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            await context.Cardholders.AddAsync(entity);
            await context.SaveChangesAsync();

            return mapper.Map<CardholderDTO>(entity);
        }

        public async Task UpdateAsync(Guid id, UpdateCardholderDTO dto)
        {
            var entity = await context.Cardholders.FirstOrDefaultAsync(c => c.Id == id) 
                ?? throw new KeyNotFoundException($"Cardholder with id {id} not found.");
            mapper.Map(dto, entity);

            var validationResult = await validator.ValidateAsync(entity);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await context.Cardholders.FindAsync(id) 
                ?? throw new KeyNotFoundException($"Cardholder with id {id} not found.");
            
            context.Cardholders.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
}
