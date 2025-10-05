using CampusHub.Domain.Entities;
using FluentResults;

namespace CampusHub.Application.Interfaces
{
    public interface IEventRepository
    {
        Task<Result<List<Event>>> GetAllAsync();
        Task<Result<Event>> GetByIdAsync(int id);
        Task<Result<List<Event>>> GetByCollegeAsync(string college);
        Task<Result<Event>> CreateAsync(Event evt);
        Task<Result<Event>> UpdateAsync(Event evt);
        Task<Result> DeleteAsync(int id);
    }
}