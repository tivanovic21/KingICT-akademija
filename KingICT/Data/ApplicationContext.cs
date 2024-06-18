using System;
using KingICT.Models;
using Microsoft.EntityFrameworkCore;

namespace KingICT.Data
{
	public class ApplicationContext : DbContext
	{
		public ApplicationContext(DbContextOptions<ApplicationContext> dbContextOptions ) : base(dbContextOptions)
		{
		}

		public DbSet<Products> Products { get; set; }
    }
}

