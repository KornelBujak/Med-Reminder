using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Med_Reminder
{
    public class MyAppDbContext:DbContext
    {
        public DbSet<DaneOsobowe> _dane_osobowe { get; set; }
        public DbSet<Lek> _leki { get; set; }
        public DbSet<Przypomnienie> _przypomnienia { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=KORNEL\\SQLEXPRESS01;Database=Med;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Przypomnienie>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Przypomnienie>()
                .HasOne(p => p.DaneOsobowe)
                .WithMany(d => d.Przypomnienia)
                .HasForeignKey(p => p.dane_osobowe_id);

            modelBuilder.Entity<Przypomnienie>()
                .HasOne(p => p.Lek)
                .WithMany(l => l.Przypomnienia)
                .HasForeignKey(p => p.id_leku);

            modelBuilder.Entity<Lek>()
                .HasKey(l => l.id_leku);

            modelBuilder.Entity<Lek>()
                .HasOne(l => l.DaneOsobowe)
                .WithMany(d => d.Leki)
                .HasForeignKey(l => l.DaneOsoboweId);

            modelBuilder.Entity<DaneOsobowe>()
                .HasKey(d => d.Id);
        }

        public string GetUserPhoneNumber(int userId)
        {
            var user = _dane_osobowe.FirstOrDefault(u => u.Id == userId);
            return user != null ? user._numer_telefonu_ : null;
        }

        public async Task<List<Przypomnienie>> GetRemindersForTodayAndFuture(int userId)
        {
            var currentDateTime = DateTime.Now;
            var reminders = await _przypomnienia
              .Where(r => r.dane_osobowe_id == userId &&
                          r.data_początkowa >= currentDateTime.Date &&
                          (r.data_końcowa == null || r.data_końcowa >= currentDateTime.Date))
              .ToListAsync();

            return reminders;
        }

    }
}

