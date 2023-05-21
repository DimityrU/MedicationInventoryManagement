﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MedicationInventoryManagement.Models
{
    public partial class MedicationInventoryManagementContext : DbContext
    {
        public virtual DbSet<Medication> Medications { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<User> Users { get; set; }

        public MedicationInventoryManagementContext()
        {
        }

        public MedicationInventoryManagementContext(DbContextOptions<MedicationInventoryManagementContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Medication>(entity =>
            {
                entity.Property(e => e.MedicationId).ValueGeneratedNever();

                entity.Property(e => e.ExpirationDate).HasColumnType("date");

                entity.Property(e => e.MedicationName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.Property(e => e.NotificationId).ValueGeneratedNever();

                entity.Property(e => e.NotificationMessage).IsRequired();

                entity.Property(e => e.NotificationType)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.HasOne(d => d.Medication)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.MedicationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Notifications_Medications");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.OrderId).ValueGeneratedNever();

                entity.Property(e => e.OrderDate).HasColumnType("datetime");

                entity.HasOne(d => d.Medication)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.MedicationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Orders_Medications");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.UserId)
                    .ValueGeneratedNever()
                    .HasColumnName("userId");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(20);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
