using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using My_Store.Domain.Entities;

namespace My_Store.Infrastructure.Persistence
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Banner> Banners { get; set; }
        public DbSet<Payment> Payments { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(p => p.Description)
                      .HasMaxLength(500);

                entity.Property(p => p.Price)
                      .HasPrecision(18, 2);

                entity.Property(p => p.Stock)
                      .IsRequired();
            });

            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.UserId)
                      .IsRequired();

                entity.HasMany(c => c.Items)
                      .WithOne(ci => ci.Cart)
                      .HasForeignKey(ci => ci.CartId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ✅ CartItem config
            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(ci => ci.Id);

                entity.Property(ci => ci.Quantity)
                      .IsRequired();

                entity.HasOne(ci => ci.Product)
                      .WithMany()
                      .HasForeignKey(ci => ci.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);
            });


            // User Config
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.HasAlternateKey(u => u.PublicId); // ⭐ REQUIRED

                entity.HasIndex(u => u.PublicId)
                      .IsUnique();

                entity.Property(u => u.FullName).IsRequired();
                entity.Property(u => u.Email).IsRequired();
                entity.Property(u => u.PasswordHash).IsRequired();
            });


            // Refresh Token

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(rt => rt.Id);

                entity.HasOne(rt => rt.User)
                      .WithMany(u => u.RefreshTokens)
                      .HasForeignKey(rt => rt.UserPublicId)
                      .HasPrincipalKey(u => u.PublicId); // ⭐ REQUIRED
            });


            // Order
                modelBuilder.Entity<Order>()
                    .HasOne<User>()
                    .WithMany()
                    .HasForeignKey(o => o.UserId)
                    .HasPrincipalKey(u => u.PublicId);


            // Payment

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Amount)
                      .HasPrecision(18, 2)
                      .IsRequired();

                entity.Property(p => p.Currency)
                      .HasMaxLength(10)
                      .IsRequired();

                entity.Property(p => p.RazorpayOrderId)
                      .HasMaxLength(100);

                entity.Property(p => p.RazorpayPaymentId)
                      .HasMaxLength(100);

                entity.Property(p => p.RazorpaySignature)
                      .HasMaxLength(255);

                entity.Property(p => p.CreatedAt)
                      .IsRequired();
                entity.Property(p => p.Status)
                        .HasMaxLength(50);

                entity.HasOne(p => p.Order)
                      .WithMany()
                      .HasForeignKey(p => p.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);
            });


        }

    }
}
