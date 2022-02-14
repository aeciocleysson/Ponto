using MegaPonto.Model;
using Microsoft.EntityFrameworkCore;

namespace MegaPonto.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DbContext> options) : base(options) { }

        public DataContext() { }
        public DbSet<Funcionario> Funcionario { get; set; }
        public DbSet<Ponto> Ponto { get; set; }
        public DbSet<LogPonto> Log { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseMySql("Host=192.168.1.200;Database=dbmega;Username=mega;Password=mega@3212");
            optionsBuilder.UseMySql("Host=localhost;Database=DbErpMega;Username=root;Password=3103");
        }
    }
}
