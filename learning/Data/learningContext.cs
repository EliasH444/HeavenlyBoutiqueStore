using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using learning.Models;

namespace learning.Data
{
    public class learningContext : DbContext
    {
        public learningContext (DbContextOptions<learningContext> options)
            : base(options)
        {
        }

        public DbSet<learning.Models.Product> Product { get; set; } = default!;
    }
}
