using Microsoft.EntityFrameworkCore;
using Npgsql;
using RequestService.Application.DTOs;
using RequestService.Application.Interfaces.Repositories;
using RequestService.Domain.Entities;
using RequestService.Persistence.Data;
using System.Text.Json;

namespace RequestService.Persistence.Repositories
{
    public class SigningRequestsRepository : ISigningRequestsRepository
    {
        private readonly RequestServiceDbContext _context;
        private readonly NpgsqlConnection _connection;

        public SigningRequestsRepository(RequestServiceDbContext context, NpgsqlConnection connection)
        {
            _context = context;
            _connection = connection;
        }

        public async Task AddAsync(SigningRequest request)
        {
            await _context.SigningRequests.AddAsync(request);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            await _context.SigningRequests
                .Where(r => r.Id == id)
                .ExecuteDeleteAsync();
        }

        public async Task<List<SigningRequest>> GetAllRequestsByUserIdAsync(Guid Id)
        {
            return await _context.SigningRequests.Where(x=>x.RequesterId == Id && x.IsActive == true).ToListAsync();
        }

        public async Task UpdateAsync(SigningRequest request)
        {
            _context.SigningRequests.Update(request);
            await _context.SaveChangesAsync();
        }

        public async Task<List<RequestDashboardDto>> GetDashboardByUserIdAsync(Guid userId, DashboardQueryParameterDto query)
        {
            await _connection.OpenAsync();

            using var cmd = new NpgsqlCommand("SELECT get_user_dashboard(@userId, @statusFilter, @nameFilter, @pageNumber, @pageSize)", _connection);
            cmd.Parameters.AddWithValue("userId", userId);
            cmd.Parameters.AddWithValue("statusFilter", query.StatusFilter ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("nameFilter", query.NameFilter ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("pageNumber", query.PageNumber);
            cmd.Parameters.AddWithValue("pageSize", query.PageSize);

            var jsonResult = await cmd.ExecuteScalarAsync() as string;

            await _connection.CloseAsync();

            if (string.IsNullOrEmpty(jsonResult)) return new List<RequestDashboardDto>();

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<List<RequestDashboardDto>>(jsonResult, options) ?? new();
        }

        public async Task<SigningRequest> GetRequestByIdAsync(Guid userId, Guid Id)
        {
            return await _context.SigningRequests
                .Where(x=>x.Id == Id && x.RequesterId == userId).FirstOrDefaultAsync();
        }
    }
}
