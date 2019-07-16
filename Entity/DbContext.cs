using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

public class AzureDBContext : DbContext
    {
        public DbSet<UserInfo> UserInfo { get; set; }
        public AzureDBContext(DbContextOptions<AzureDBContext> options)
            : base(options)
        { }
         
    }
 