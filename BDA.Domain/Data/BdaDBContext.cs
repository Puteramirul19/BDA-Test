using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BDA.Entities;
using BDA.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BDA.Data
{
    public class BdaDBContext : IdentityDbContext<ApplicationUser, ApplicationRole, string, ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin, ApplicationRoleClaim, ApplicationUserToken>
    {
        public static string DefaultSchema => "dbo";
        public BdaDBContext(DbContextOptions<BdaDBContext> options)
            : base(options)
        {
        }

        public DbSet<AccountingTable> AccountingTable { get; set; }
        public DbSet<Attachment> Attachment { get; set; }
        public DbSet<BankDraft> BankDraft { get; set; }
        public DbSet<BankDraftAction> BankDraftAction { get; set; }
        public DbSet<Division> Division { get; set; }
        public DbSet<EmailTemplates> EmailTemplates { get; set; }
        public DbSet<EmailQueues> EmailQueues { get; set; }
        public DbSet<ERMSQueues> ERMSQueues { get; set; }
        public DbSet<Function> Function { get; set; }
        public DbSet<InstructionLetter> InstructionLetter { get; set; }
        public DbSet<Memo> Memo { get; set; }
        public DbSet<Unit> Unit { get; set; }
        public DbSet<WangCagaran> WangCagaran { get; set; }
        public DbSet<WangHangus> WangHangus { get; set; }
        public DbSet<Zone> Zone { get; set; }
        public DbSet<RunningNo> RunningNo { get; set; }
        public DbSet<State> State{ get; set; }
        public DbSet<Country> Country{ get; set; }
        public DbSet<VendorNo> VendorNo { get; set; }
        public DbSet<BankDetails> BankDetails { get; set; }
        public DbSet<BusinessArea> BusinessArea { get; set; }
        public DbSet<Cancellation> Cancellation { get; set; }
        public DbSet<Recovery> Recovery { get; set; }
        public DbSet<CancellationReason> CancellationReason { get; set; }
        public DbSet<Lost> Lost { get; set; }
        public DbSet<StateMonthlyAmount> StateMonthlyAmount { get; set; }
        public DbSet<JkrType> JkrType { get; set; }
        public DbSet<SendMethod> SendMethod { get; set; }
        public DbSet<MonthlyAmount> MonthlyAmount { get; set; }
        public DbSet<UserManual> UserManual { get; set; }
        public DbSet<WangCagaranHangus> WangCagaranHangus { get; set; }
        public DbSet<ErmsLog> ErmsLog { get; set; }
        public DbSet<PasswordHistory> PasswordHistory { get; set; }

        public override int SaveChanges()
        {
            var modifiedEntries = ChangeTracker.Entries().Where(x => x.State == EntityState.Modified);

            var now = DateTime.Now;
            foreach (var entry in modifiedEntries)
            {
                if (entry.Metadata.FindProperty("UpdatedOn") != null)
                {
                    entry.Property("UpdatedOn").CurrentValue = now;
                }
            }

            return base.SaveChanges();
        }

        public virtual void SetModified(object entity)
        {
            this.Entry(entity).State = EntityState.Modified;
        }

        public virtual void SetDeleted(object entity)
        {
            this.Entry(entity).State = EntityState.Deleted;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureForIdentity(modelBuilder);

        }

        protected void ConfigureForIdentity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Identity.ApplicationUser>(b =>
            {
                // Each User can have many UserClaims
                b.HasMany(e => e.Claims)
                    .WithOne(e => e.User)
                    .HasForeignKey(uc => uc.UserId)
                    .IsRequired();

                // Each User can have many UserLogins
                b.HasMany(e => e.Logins)
                    .WithOne(e => e.User)
                    .HasForeignKey(ul => ul.UserId)
                    .IsRequired();

                // Each User can have many UserTokens
                b.HasMany(e => e.Tokens)
                    .WithOne(e => e.User)
                    .HasForeignKey(ut => ut.UserId)
                    .IsRequired();

                // Each User can have many entries in the UserRole join table
                b.HasMany(e => e.Roles)
                    .WithOne(e => e.User)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });

            modelBuilder.Entity<ApplicationRole>(b =>
            {

                // Each Role can have many entries in the UserRole join table
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.Role)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();


                // Each Role can have many associated RoleClaims
                b.HasMany(e => e.RoleClaims)
                    .WithOne(e => e.Role)
                    .HasForeignKey(rc => rc.RoleId)
                    .IsRequired();
            });

            modelBuilder.Entity<ApplicationUserRole>(b =>
            {
                // Each Role can have many entries in the UserRole join table
                b.HasKey(e => e.Id);

            });
        }
    }

      

 }
