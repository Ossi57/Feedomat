using Feedomat.Shared.Model;
using Microsoft.EntityFrameworkCore;

namespace Feedomat.Server.Model
{
    public partial class FeedomatContext : DbContext
    {
        public FeedomatContext(DbContextOptions<FeedomatContext> options) : base(options)
        {

        }

        public DbSet<UserModel> Users { get; set; }
    }
}
