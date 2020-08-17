using DataAccessLibary.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLibary.DataAccess
{
    public class AuthenticationContext : IdentityDbContext
    {
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Record> Records { get; set; }
        public DbSet<Invite> Invites { get; set; }
        public DbSet<Mail> Mails { get; set; }
        public AuthenticationContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Record>(RecordConfigure);
            modelBuilder.Entity<Invite>(InviteConfigure);
        }

        public void RecordConfigure(EntityTypeBuilder<Record> builder)
        {
            builder.HasKey(p => p.Id);
            builder.HasIndex(u => u.Id);
            builder.Property(b => b.Title).IsRequired().HasMaxLength(50);
            builder.Property(b => b.Text).IsRequired().HasMaxLength(500);
            builder
            .HasOne(p => p.User)
            .WithMany(t => t.records)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        }
        public void InviteConfigure(EntityTypeBuilder<Invite> builder)
        {
            builder.HasKey(p => p.Id);
            builder.HasIndex(u => u.Id);
            builder.Property(b => b.Code).IsRequired().HasMaxLength(8);
            builder.Property(b => b.Email).IsRequired();
            builder.Property(b => b.Status).IsRequired();
        }

    }
}