using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using TodoApp.Enums;
using TodoApp.Models;

namespace TodoApp.Data
{
	public class AppDbContext : DbContext
	{
		public DbSet<User> Users { get; set; }
		public DbSet<Todo> Todos { get; set; }

		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// Configure the Priority enum to be stored as a string in the database
			modelBuilder.Entity<Todo>()
				.Property(t => t.Priority)
				.HasConversion(
					v => v.ToString(), // Convert enum to string when saving
					v => (PriorityLevel)Enum.Parse(typeof(PriorityLevel), v,true)); // Convert string back to enum when reading

			// Configure the Category enum to be stored as a string in the database
			modelBuilder.Entity<Todo>()
				.Property(t => t.Category)
				.HasConversion(
					v => v.ToString(), // Convert enum to string when saving
					v => (TodoCategory)Enum.Parse(typeof(TodoCategory), v,true)); // Convert string back to enum when reading

			base.OnModelCreating(modelBuilder);     
		}
	}
}
