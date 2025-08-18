using System;
using System.Collections.Generic;
using AirWaterStore.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace AirWaterStore.Data;

public partial class AirWaterStoreContext : DbContext
{
    public AirWaterStoreContext()
    {
    }

    public AirWaterStoreContext(DbContextOptions<AirWaterStoreContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChatRoom> ChatRooms { get; set; }

    public virtual DbSet<CommissionRequest> CommissionRequests { get; set; }

    public virtual DbSet<CommissionRequestUpvote> CommissionRequestUpvotes { get; set; }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Wishlist> Wishlists { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChatRoom>(entity =>
        {
            entity.HasKey(e => e.ChatRoomId).HasName("PK__ChatRoom__69733CF733AE927A");

            entity.HasOne(d => d.Customer).WithMany(p => p.ChatRoomCustomers)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Conversations_Customer");

            entity.HasOne(d => d.Staff).WithMany(p => p.ChatRoomStaffs)
                .HasForeignKey(d => d.StaffId)
                .HasConstraintName("FK_Conversations_Staff");
        });

        modelBuilder.Entity<CommissionRequest>(entity =>
        {
            entity.HasKey(e => e.CommissionRequestId).HasName("PK__Commissi__FFCE520ACA235E85");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ExpectedPrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.GameTitle).HasMaxLength(100);
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.Upvotes).HasDefaultValue(0);

            entity.HasOne(d => d.User).WithMany(p => p.CommissionRequests)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CommissionRequests_User");
        });

        modelBuilder.Entity<CommissionRequestUpvote>(entity =>
        {
            entity.HasKey(e => e.UpvoteId).HasName("PK__Commissi__6432A92E5AE94466");

            entity.HasIndex(e => new { e.CommissionRequestId, e.UserId }, "UQ_Upvote").IsUnique();

            entity.Property(e => e.UpvotedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.CommissionRequest).WithMany(p => p.CommissionRequestUpvotes)
                .HasForeignKey(d => d.CommissionRequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Upvote_Request");

            entity.HasOne(d => d.User).WithMany(p => p.CommissionRequestUpvotes)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Upvote_User");
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.GameId).HasName("PK__Games__2AB897FD1DAB5CBF");

            entity.Property(e => e.Developer).HasMaxLength(100);
            entity.Property(e => e.Genre).HasMaxLength(200);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Publisher).HasMaxLength(100);
            entity.Property(e => e.ThumbnailUrl).HasMaxLength(255);
            entity.Property(e => e.Title).HasMaxLength(100);
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__Messages__C87C0C9C77D2FB88");

            entity.Property(e => e.SentAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.ChatRoom).WithMany(p => p.Messages)
                .HasForeignKey(d => d.ChatRoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Messages_Conversation");

            entity.HasOne(d => d.User).WithMany(p => p.Messages)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Messages_Sender");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BCF4A58A091");

            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_User");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK__OrderDet__D3B9D36CCE0FA4C9");

            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Game).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.GameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetails_Game");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetails_Order");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__Reviews__74BC79CE02FFD19E");

            entity.Property(e => e.ReviewDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Game).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.GameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reviews_Game");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reviews_User");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C9B49C2DF");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E4F1752391").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534E393C222").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.IsBan).HasDefaultValue(false);
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        modelBuilder.Entity<Wishlist>(entity =>
        {
            entity.HasKey(e => e.WishlistId).HasName("PK__Wishlist__233189EB11313F6D");

            entity.ToTable("Wishlist");

            entity.HasIndex(e => new { e.UserId, e.GameId }, "UQ_Wishlist").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Game).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.GameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wishlist_Game");

            entity.HasOne(d => d.User).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wishlist_User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
