﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SimpleForum.Common.Server;

namespace SimpleForum.Common.Server.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("SimpleForum.Models.Comment", b =>
                {
                    b.Property<int>("CommentID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Content")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("DatePosted")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("DeleteReason")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<bool>("Deleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("DeletedBy")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("ThreadID")
                        .HasColumnType("int");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.HasKey("CommentID");

                    b.HasIndex("DatePosted");

                    b.HasIndex("ThreadID");

                    b.HasIndex("UserID");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("SimpleForum.Models.EmailCode", b =>
                {
                    b.Property<string>("Code")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Type")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.Property<bool>("Valid")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("ValidUntil")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Code");

                    b.HasIndex("UserID");

                    b.ToTable("EmailCodes");
                });

            modelBuilder.Entity("SimpleForum.Models.IncomingServerToken", b =>
                {
                    b.Property<int>("IncomingServerTokenID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Address")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("ApiAddress")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("CrossConnectionAddress")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Token")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("IncomingServerTokenID");

                    b.ToTable("IncomingServerTokens");
                });

            modelBuilder.Entity("SimpleForum.Models.Notification", b =>
                {
                    b.Property<int>("NotificationID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Content")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("Read")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Title")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.HasKey("NotificationID");

                    b.HasIndex("UserID");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("SimpleForum.Models.OutgoingServerToken", b =>
                {
                    b.Property<int>("OutgoingServerTokenID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Address")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("ApiAddress")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("CrossConnectionAddress")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Token")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("OutgoingServerTokenID");

                    b.ToTable("OutgoingServerTokens");
                });

            modelBuilder.Entity("SimpleForum.Models.RemoteAuthToken", b =>
                {
                    b.Property<string>("Token")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.Property<DateTime>("ValidUntil")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Token");

                    b.HasIndex("UserID");

                    b.ToTable("RemoteAuthTokens");
                });

            modelBuilder.Entity("SimpleForum.Models.TempApiToken", b =>
                {
                    b.Property<string>("Token")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.Property<DateTime>("ValidUntil")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Token");

                    b.HasIndex("UserID");

                    b.ToTable("TempApiTokens");
                });

            modelBuilder.Entity("SimpleForum.Models.Thread", b =>
                {
                    b.Property<int>("ThreadID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Content")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("DatePosted")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("DeleteReason")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<bool>("Deleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("DeletedBy")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<bool>("Locked")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("Pinned")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Title")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.HasKey("ThreadID");

                    b.HasIndex("DatePosted");

                    b.HasIndex("UserID");

                    b.ToTable("Threads");
                });

            modelBuilder.Entity("SimpleForum.Models.User", b =>
                {
                    b.Property<int>("UserID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<bool>("Activated")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("BanReason")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<bool>("Banned")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Bio")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<bool>("CommentsLocked")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("Deleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Email")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("MuteReason")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<bool>("Muted")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Password")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Role")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int?>("ServerID")
                        .HasColumnType("int");

                    b.Property<DateTime>("SignupDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Username")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("UserID");

                    b.HasIndex("ServerID");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("SimpleForum.Models.UserComment", b =>
                {
                    b.Property<int>("UserCommentID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Content")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("DatePosted")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("DeleteReason")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<bool>("Deleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("DeletedBy")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.Property<int>("UserPageID")
                        .HasColumnType("int");

                    b.HasKey("UserCommentID");

                    b.HasIndex("DatePosted");

                    b.HasIndex("UserID");

                    b.HasIndex("UserPageID");

                    b.ToTable("UserComments");
                });

            modelBuilder.Entity("SimpleForum.Models.Comment", b =>
                {
                    b.HasOne("SimpleForum.Models.Thread", "Thread")
                        .WithMany("Comments")
                        .HasForeignKey("ThreadID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SimpleForum.Models.User", "User")
                        .WithMany("Comments")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SimpleForum.Models.EmailCode", b =>
                {
                    b.HasOne("SimpleForum.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SimpleForum.Models.Notification", b =>
                {
                    b.HasOne("SimpleForum.Models.User", "User")
                        .WithMany("Notifications")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SimpleForum.Models.RemoteAuthToken", b =>
                {
                    b.HasOne("SimpleForum.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SimpleForum.Models.TempApiToken", b =>
                {
                    b.HasOne("SimpleForum.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SimpleForum.Models.Thread", b =>
                {
                    b.HasOne("SimpleForum.Models.User", "User")
                        .WithMany("Threads")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SimpleForum.Models.User", b =>
                {
                    b.HasOne("SimpleForum.Models.IncomingServerToken", "Server")
                        .WithMany("Users")
                        .HasForeignKey("ServerID");
                });

            modelBuilder.Entity("SimpleForum.Models.UserComment", b =>
                {
                    b.HasOne("SimpleForum.Models.User", "User")
                        .WithMany("UserComments")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SimpleForum.Models.User", "UserPage")
                        .WithMany("UserPageComments")
                        .HasForeignKey("UserPageID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
