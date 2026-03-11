using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class MeetingCleanupJob
    {
        private readonly MeetingContext _context;

        public MeetingCleanupJob(MeetingContext context)
        {
            _context = context;
        }

        public async Task DeleteCanceledMeetings()
        {
            var oldMeetings = await _context.Meetings
                .Where(x => x.IsCanceled &&
                            x.CanceledAt < DateTime.UtcNow.AddMinutes(-1))
                .ToListAsync();

            if (oldMeetings.Any())
            {
                _context.Meetings.RemoveRange(oldMeetings);
                await _context.SaveChangesAsync();
            }
        }
    }
}
