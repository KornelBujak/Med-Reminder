using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Med_Reminder
{
    public interface IUserProfileRepository
    {
        Task<DaneOsobowe> GetDaneOsoboweAsync(int userId);
        void UpdateDaneOsobowe(DaneOsobowe daneOsobowe);
        Task SaveChanges();

    }
}
