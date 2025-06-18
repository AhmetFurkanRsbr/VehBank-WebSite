using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using VEHABANK.WebApi.Entities;

namespace VEHABANK.WebApi.Context
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
       
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Branch> Branches{ get; set; }
        public DbSet<Login> Logins { get; set; }
        public DbSet<LoginLog> LoginLogs{ get; set; }
        public DbSet<TransactionHistory> TransactionHistories{ get; set; }
        public DbSet<Transfer> Transfers{ get; set; }
        public DbSet<User> Users{ get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Phone ve Email.. için benzersiz indeksler eklenebilmesini sağlar
            modelBuilder.Entity<Branch>()
                .HasIndex(b => b.Name)
                .IsUnique()
                .HasDatabaseName("IX_Branch_Name");

            modelBuilder.Entity<Branch>()
                .HasIndex(b => b.Phone)
                .IsUnique()
                .HasDatabaseName("IX_Branch_Phone"); 

            modelBuilder.Entity<Branch>()
                .HasIndex(b => b.Email)
                .IsUnique()
                .HasDatabaseName("IX_Branch_Email");

            modelBuilder.Entity<User>()
                .HasIndex(b => b.IdentityNumber)
                .IsUnique()
                .HasDatabaseName("IX_User_IdentityNumber");

            modelBuilder.Entity<User>()
                .HasIndex(b => b.Phone)
                .IsUnique()
                .HasDatabaseName("IX_User_Phone");

            modelBuilder.Entity<User>()
                .HasOne(u => u.Branch)
                .WithMany(b => b.Users)
                .HasForeignKey(u => u.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Login>()
                .HasIndex(b => b.CustomerNumber)
                .IsUnique()
                .HasDatabaseName("IX_Login_CustomerNumber");

            modelBuilder.Entity<TransactionHistory>()
               .HasOne(th => th.Account)
               .WithMany()
               .HasForeignKey(th => th.AccountId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TransactionHistory>()
                .HasOne(th => th.User)
                .WithMany()
                .HasForeignKey(th => th.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
