using Microsoft.EntityFrameworkCore;

namespace VO.Infrastructure.Persistence;

public class ApplicationReadDbContext(DbContextOptions options) : ApplicationDbContext(options);