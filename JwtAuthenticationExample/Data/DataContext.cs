using JwtAuthenticationExample.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtAuthenticationExample.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> opt) : base(opt)
        {

        }

        public DbSet<Character> Characters { get; set; }

        // Con EF otteniamo l'accesso ai dati nel DB
        public DbSet<User> Users { get; set; }
    }
}
