using CompanyEmployees.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace CompanyEmployees.Data
{
    public class _DbContext : DbContext
    {
        public _DbContext(DbContextOptions options) : base(options)
        {
        }
        
    }
}
