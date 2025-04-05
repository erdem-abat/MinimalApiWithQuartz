using Carter;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Data.Db;
using MinimalApi.Data.Dtos.Log;
using MinimalApi.Data.Entities;

namespace MinimalApi.Controllers
{
    public class LogEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/logs").WithTags("Logs");
            group.MapPost("/", CreateNewLogAsync).WithName("New Log");
            group.MapGet("/", GetLogsAsync).WithName(nameof(GetLogsAsync)); ;
        }

        public static async Task<IResult> CreateNewLogAsync(ApplicationDbContext _context, string SenderName, string Description)
        {
            var newLog = new Log()
            {
                UserName = SenderName,
                Description = Description
            };
            await _context.Logs.AddAsync(newLog);
            await _context.SaveChangesAsync();
            return Results.Created($"/api/logs/{newLog.Id}", new GetLogDto
            {
                CreatedAt = newLog.CreatedAt,
                UserName = newLog.UserName,
                Description = newLog.Description
            });
        }
        public static async Task<Results<Ok<List<GetLogDto>>, NotFound<string>>> GetLogsAsync(ApplicationDbContext _context)
        {
            var logs = await _context.Logs
                .Select(q => new GetLogDto
                {
                    CreatedAt = q.CreatedAt,
                    UserName = q.UserName,
                    Description = q.Description
                })
                .AsNoTracking()
                .ToListAsync();

            if (!logs.Any())
            {
                return TypedResults.NotFound("There is no log");
            }

            return TypedResults.Ok(logs);
        }
    }
}