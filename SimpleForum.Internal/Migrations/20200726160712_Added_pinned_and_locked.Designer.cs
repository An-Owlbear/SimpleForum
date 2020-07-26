﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SimpleForum.Internal;

namespace SimpleForum.Internal.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20200726160712_Added_pinned_and_locked")]
    partial class Added_pinned_and_locked
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("SimpleForum.Models.AuthToken", b =>
                {
                    b.Property<int>("AuthTokenID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("AccessToken")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("Time")
                        .HasColumnType("int");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.HasKey("AuthTokenID");

                    b.HasIndex("UserID");

                    b.ToTable("AuthTokens");
                });

            modelBuilder.Entity("SimpleForum.Models.Comment", b =>
                {
                    b.Property<int>("CommentID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Content")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("DatePosted")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("ThreadID")
                        .HasColumnType("int");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.HasKey("CommentID");

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

                    b.Property<DateTime>("ValidUntil")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Code");

                    b.HasIndex("UserID");

                    b.ToTable("EmailCodes");
                });

            modelBuilder.Entity("SimpleForum.Models.Thread", b =>
                {
                    b.Property<int>("ThreadID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("DatePosted")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("Locked")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("Pinned")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Title")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.HasKey("ThreadID");

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

                    b.Property<bool>("Admin")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Email")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Password")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("SignupDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Username")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("UserID");

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

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.Property<int>("UserPageID")
                        .HasColumnType("int");

                    b.HasKey("UserCommentID");

                    b.HasIndex("UserID");

                    b.HasIndex("UserPageID");

                    b.ToTable("UserComments");
                });

            modelBuilder.Entity("SimpleForum.Models.AuthToken", b =>
                {
                    b.HasOne("SimpleForum.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
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

            modelBuilder.Entity("SimpleForum.Models.Thread", b =>
                {
                    b.HasOne("SimpleForum.Models.User", "User")
                        .WithMany("Threads")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
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
