﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using OIDC.Core_Minimal.DAL;

#nullable disable

namespace OIDC.Core_Minimal.DAL.Migrations
{
    [DbContext(typeof(OIDCCoreMinimalDbContext))]
    partial class OIDCCoreMinimalDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("AccessTokenScope", b =>
                {
                    b.Property<Guid>("AccessTokensId")
                        .HasColumnType("uuid");

                    b.Property<string>("ScopesName")
                        .HasColumnType("text");

                    b.HasKey("AccessTokensId", "ScopesName");

                    b.HasIndex("ScopesName");

                    b.ToTable("AccessTokenScope");
                });

            modelBuilder.Entity("ApplicationUser", b =>
                {
                    b.Property<Guid>("AuthorisedApplicationsId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UsersId")
                        .HasColumnType("uuid");

                    b.HasKey("AuthorisedApplicationsId", "UsersId");

                    b.HasIndex("UsersId");

                    b.ToTable("ApplicationUser");
                });

            modelBuilder.Entity("OIDC.Core_Minimal.DAL.Entities.AccessToken", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("ApplicationId")
                        .HasColumnType("uuid");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("ExpiresAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationId");

                    b.HasIndex("UserId");

                    b.ToTable("AccessTokens");
                });

            modelBuilder.Entity("OIDC.Core_Minimal.DAL.Entities.Application", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("CallbackUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("CancelUrl")
                        .HasColumnType("text");

                    b.Property<string>("ClientId")
                        .IsRequired()
                        .HasMaxLength(24)
                        .HasColumnType("character varying(24)");

                    b.Property<string>("ClientSecret")
                        .IsRequired()
                        .HasMaxLength(88)
                        .HasColumnType("character varying(88)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("HomepageUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Applications");
                });

            modelBuilder.Entity("OIDC.Core_Minimal.DAL.Entities.RefreshToken", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("ExpiresAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<int>("Uses")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("RefreshTokens");
                });

            modelBuilder.Entity("OIDC.Core_Minimal.DAL.Entities.Scope", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Name");

                    b.ToTable("Scopes");

                    b.HasData(
                        new
                        {
                            Name = "profile.read"
                        },
                        new
                        {
                            Name = "profile.write"
                        },
                        new
                        {
                            Name = "applications.authorised"
                        },
                        new
                        {
                            Name = "applications.published"
                        });
                });

            modelBuilder.Entity("OIDC.Core_Minimal.DAL.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("AccessTokenScope", b =>
                {
                    b.HasOne("OIDC.Core_Minimal.DAL.Entities.AccessToken", null)
                        .WithMany()
                        .HasForeignKey("AccessTokensId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("OIDC.Core_Minimal.DAL.Entities.Scope", null)
                        .WithMany()
                        .HasForeignKey("ScopesName")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ApplicationUser", b =>
                {
                    b.HasOne("OIDC.Core_Minimal.DAL.Entities.Application", null)
                        .WithMany()
                        .HasForeignKey("AuthorisedApplicationsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("OIDC.Core_Minimal.DAL.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("OIDC.Core_Minimal.DAL.Entities.AccessToken", b =>
                {
                    b.HasOne("OIDC.Core_Minimal.DAL.Entities.Application", "Application")
                        .WithMany("AccessTokens")
                        .HasForeignKey("ApplicationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("OIDC.Core_Minimal.DAL.Entities.User", "User")
                        .WithMany("AccessTokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Application");

                    b.Navigation("User");
                });

            modelBuilder.Entity("OIDC.Core_Minimal.DAL.Entities.Application", b =>
                {
                    b.HasOne("OIDC.Core_Minimal.DAL.Entities.User", "User")
                        .WithMany("PublishedApplications")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("OIDC.Core_Minimal.DAL.Entities.RefreshToken", b =>
                {
                    b.HasOne("OIDC.Core_Minimal.DAL.Entities.User", "User")
                        .WithMany("RefreshTokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("OIDC.Core_Minimal.DAL.Entities.Application", b =>
                {
                    b.Navigation("AccessTokens");
                });

            modelBuilder.Entity("OIDC.Core_Minimal.DAL.Entities.User", b =>
                {
                    b.Navigation("AccessTokens");

                    b.Navigation("PublishedApplications");

                    b.Navigation("RefreshTokens");
                });
#pragma warning restore 612, 618
        }
    }
}
