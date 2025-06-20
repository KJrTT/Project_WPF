using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp2.Models;
using Microsoft.EntityFrameworkCore;

namespace WpfApp2.Services
{
    public class DatabaseService : DbContext
    {
        public DbSet<Notes> Notes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=Calendar;Username=postgres;Password=;");
        }

        public async Task<List<Notes>> GetAllNotesAsync()
        {
            return await Notes.ToListAsync();
        }

        public async Task<Notes?> GetNoteByIdAsync(int id)
        {
            return await Notes.FindAsync(id);
        }

        public async Task AddNoteAsync(Notes note)
        {
            Notes.Add(note);
            await SaveChangesAsync();
        }

        public async Task UpdateNoteAsync(Notes note)
        {
            Notes.Update(note);
            await SaveChangesAsync();
        }

        public async Task DeleteNoteAsync(int id)
        {
            var note = await Notes.FindAsync(id);
            if (note != null)
            {
                Notes.Remove(note);
                await SaveChangesAsync();
            }
        }
    }
}
