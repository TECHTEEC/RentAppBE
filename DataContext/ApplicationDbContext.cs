using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RentAppBE.Models;
using System.Reflection.PortableExecutable;

namespace RentAppBE.DataContext
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
	   : base(options)
		{
		}

		//Add DbSet--------------------------------------

		public DbSet<UserProfile> UserProfiles { get; set; }
		public DbSet<OtpRecord> OtpRecords { get; set; }
		public DbSet<UserMessage> UserMessages { get; set; }
		public DbSet<RefreshToken> RefreshTokens { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Configure entities here if needed
			modelBuilder.Entity<UserProfile>().HasQueryFilter(h => !h.ApplicationUser!.IsDeleted);
			modelBuilder.Entity<ApplicationUser>().HasQueryFilter(h => !h.IsDeleted);

			modelBuilder.Entity<UserProfile>()
				.HasIndex(u => u.WhatsAppNumber)
				.IsUnique();
		}
	}
}
