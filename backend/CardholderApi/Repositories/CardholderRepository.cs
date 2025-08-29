using AutoMapper;
using CardholderApi.DTOs;
using CardholderApi.DTOs;
using CardholderApi.Entities;
using CardholderApi.Persistence;
using CardholderApi.Repositories.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CardholderApi.Repositories
{
    public class CardholderRepository(CardholderDbContext context, IValidator<Cardholder> validator, IMapper mapper) : ICardholderRepository
    {
        public async Task<List<CardholderDTO>> GetAllAsync()
        {
            var entities = await context.Cardholders.ToListAsync();
            return mapper.Map<List<CardholderDTO>>(entities);
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
