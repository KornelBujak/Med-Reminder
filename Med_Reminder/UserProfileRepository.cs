using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Med_Reminder
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly MyAppDbContext _dbContext;

        public UserProfileRepository(MyAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<DaneOsobowe> GetDaneOsoboweAsync(int userId)
        {
            return await _dbContext._dane_osobowe.FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task SaveChanges()
        {
            await _dbContext.SaveChangesAsync();
        }

        public void UpdateDaneOsobowe(DaneOsobowe daneOsobowe)
        {
            _dbContext.Entry(daneOsobowe).State = EntityState.Modified;
        }
    }
}
