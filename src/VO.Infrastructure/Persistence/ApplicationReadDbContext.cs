using Microsoft.EntityFrameworkCore;

namespace VO.Infrastructure.Persistence;

public class ApplicationReadDbContext : ApplicationDbContext
{
    public ApplicationReadDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
}