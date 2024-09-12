using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace api_layaway.Models;

public partial class LayawayDbContext : DbContext
{
    public LayawayDbContext()
    {
    }

    public LayawayDbContext(DbContextOptions<LayawayDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Article> Articles { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Layaway> Layaways { get; set; }

    public virtual DbSet<TransactionRecord> TransactionRecords { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("Pk_Account");

            entity.ToTable("Account");

            entity.HasIndex(e => e.CustomerId, "UQ__Account__A4AE64D9DBB5529C").IsUnique();

            entity.Property(e => e.PayableAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Status).HasDefaultValue(1);

            entity.HasOne(d => d.Customer).WithOne(p => p.Account)
                .HasForeignKey<Account>(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Account__Custome__46E78A0C");
        });

        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasKey(e => e.ArticleId).HasName("Pk_Article");

            entity.ToTable("Article");

            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.Subtotal).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Layaway).WithMany(p => p.Articles)
                .HasForeignKey(d => d.LayawayId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Article__Layaway__4222D4EF");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("Pk_Customer");

            entity.ToTable("Customer");

            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.Surname)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Layaway>(entity =>
        {
            entity.HasKey(e => e.LayawayId).HasName("Pk_Layaway");

            entity.ToTable("Layaway");

            entity.Property(e => e.Closing)
                .HasDefaultValueSql("(dateadd(month,(3),getdate()))")
                .HasColumnType("datetime");
            entity.Property(e => e.Opening)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.State)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("Active");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.Total).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Customer).WithMany(p => p.Layaways)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Layaway__Custome__3E52440B");
        });

        modelBuilder.Entity<TransactionRecord>(entity =>
        {
            entity.HasKey(e => e.TransactionRecordId).HasName("Pk_TransactionRecord");

            entity.ToTable("TransactionRecord");

            entity.Property(e => e.Balance).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.Payment).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Status).HasDefaultValue(1);

            entity.HasOne(d => d.Layaway).WithMany(p => p.TransactionRecords)
                .HasForeignKey(d => d.LayawayId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Transacti__Layaw__4AB81AF0");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
