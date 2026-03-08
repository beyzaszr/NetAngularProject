using Microsoft.EntityFrameworkCore;

namespace backend.Models
{
    public class MeetingContext : DbContext
    {
        public MeetingContext(DbContextOptions options) : base(options)
        {
        }

        protected MeetingContext()
        {
        }

        public DbSet<Meeting> Meetings { get; set; }
    }
}
