﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PDMF.Data.Contexts;

namespace PDMF.Data.Migrations
{
    [DbContext(typeof(PDMFDatabaseContext))]
    [Migration("20211122212248_AddTypeToDataset")]
    partial class AddTypeToDataset
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:Collation", "Cyrillic_General_CI_AS")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.12")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("PDMF.Data.Entities.Area", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime")
                        .HasColumnName("create_date");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .IsUnicode(false)
                        .HasColumnType("varchar(256)")
                        .HasColumnName("name");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(450)
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("area");
                });

            modelBuilder.Entity("PDMF.Data.Entities.AspNetRoleClaim", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "RoleId" }, "IX_AspNetRoleClaims_RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("PDMF.Data.Entities.AspNetUserClaim", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "UserId" }, "IX_AspNetUserClaims_UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("PDMF.Data.Entities.AspNetUserLogin", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex(new[] { "UserId" }, "IX_AspNetUserLogins_UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("PDMF.Data.Entities.AspNetUserRole", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("PDMF.Data.Entities.AspNetUserToken", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("PDMF.Data.Entities.Dataset", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("id");

                    b.Property<string>("AreaId")
                        .HasMaxLength(450)
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("area_id");

                    b.Property<string>("Columns")
                        .IsRequired()
                        .HasMaxLength(1200)
                        .HasColumnType("nvarchar(1200)")
                        .HasColumnName("columns");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime")
                        .HasColumnName("create_date");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)")
                        .HasColumnName("name");

                    b.Property<long>("Size")
                        .HasColumnType("bigint")
                        .HasColumnName("size");

                    b.Property<int>("Status")
                        .HasColumnType("int")
                        .HasColumnName("status");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(450)
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.HasIndex("AreaId");

                    b.HasIndex("UserId");

                    b.ToTable("dataset");
                });

            modelBuilder.Entity("PDMF.Data.Entities.ForecastResult", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime")
                        .HasColumnName("create_date");

                    b.Property<int>("Status")
                        .HasColumnType("int")
                        .HasColumnName("status");

                    b.Property<string>("TaskId")
                        .IsRequired()
                        .HasMaxLength(450)
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("task_id");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(450)
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.HasIndex("TaskId");

                    b.HasIndex("UserId");

                    b.ToTable("forecast_result");
                });

            modelBuilder.Entity("PDMF.Data.Entities.ForecastTask", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("id");

                    b.Property<DateTime?>("CompleteDate")
                        .HasColumnType("datetime")
                        .HasColumnName("complete_date");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime")
                        .HasColumnName("create_date");

                    b.Property<int>("Mode")
                        .HasColumnType("int")
                        .HasColumnName("mode");

                    b.Property<string>("ModelingResultId")
                        .IsRequired()
                        .HasMaxLength(450)
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("modeling_result_id");

                    b.Property<string>("ModelingTaskId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Status")
                        .HasColumnType("int")
                        .HasColumnName("status");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(450)
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.HasIndex("ModelingResultId");

                    b.HasIndex("ModelingTaskId");

                    b.HasIndex("UserId");

                    b.ToTable("forecast_task");
                });

            modelBuilder.Entity("PDMF.Data.Entities.ModelingResult", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime")
                        .HasColumnName("create_date");

                    b.Property<int>("ModelType")
                        .HasColumnType("int")
                        .HasColumnName("model_type");

                    b.Property<int>("Status")
                        .HasColumnType("int")
                        .HasColumnName("status");

                    b.Property<string>("TaskId")
                        .IsRequired()
                        .HasMaxLength(450)
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("task_id");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(450)
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.HasIndex("TaskId");

                    b.HasIndex("UserId");

                    b.ToTable("modeling_result");
                });

            modelBuilder.Entity("PDMF.Data.Entities.ModelingTask", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("id");

                    b.Property<DateTime?>("CompleteDate")
                        .HasColumnType("datetime")
                        .HasColumnName("complete_date");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime")
                        .HasColumnName("create_date");

                    b.Property<string>("DatasetId")
                        .IsRequired()
                        .HasMaxLength(450)
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("dataset_id");

                    b.Property<int>("DesiredColumn")
                        .HasColumnType("int")
                        .HasColumnName("desired_column");

                    b.Property<int>("ModelType")
                        .HasColumnType("int")
                        .HasColumnName("model_type");

                    b.Property<int>("Status")
                        .HasColumnType("int")
                        .HasColumnName("status");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(450)
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.HasIndex("DatasetId");

                    b.HasIndex("UserId");

                    b.ToTable("modeling_task");
                });

            modelBuilder.Entity("PDMF.Data.Entities.ParseTask", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("id");

                    b.Property<DateTime?>("CompleteDate")
                        .HasColumnType("datetime")
                        .HasColumnName("complete_date");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime")
                        .HasColumnName("create_date");

                    b.Property<string>("DatasetId")
                        .IsRequired()
                        .HasMaxLength(450)
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("dataset_id");

                    b.Property<string>("ResultDescription")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("result_description");

                    b.Property<int>("Status")
                        .HasColumnType("int")
                        .HasColumnName("status");

                    b.Property<int>("TaskType")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(450)
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.HasIndex("DatasetId");

                    b.HasIndex("UserId");

                    b.ToTable("parse_task");
                });

            modelBuilder.Entity("PDMF.Data.Entities.Role", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "NormalizedName" }, "RoleNameIndex")
                        .IsUnique()
                        .HasFilter("([NormalizedName] IS NOT NULL)");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("PDMF.Data.Entities.TaskAudit", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime")
                        .HasColumnName("create_date");

                    b.Property<string>("Description")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("description");

                    b.Property<int>("Status")
                        .HasColumnType("int")
                        .HasColumnName("status");

                    b.Property<string>("TaskId")
                        .HasMaxLength(450)
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("task_id");

                    b.HasKey("Id");

                    b.ToTable("task_audit");
                });

            modelBuilder.Entity("PDMF.Data.Entities.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SpareEmail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "NormalizedEmail" }, "EmailIndex");

                    b.HasIndex(new[] { "NormalizedUserName" }, "UserNameIndex")
                        .IsUnique()
                        .HasFilter("([NormalizedUserName] IS NOT NULL)");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("PDMF.Data.Entities.Area", b =>
                {
                    b.HasOne("PDMF.Data.Entities.User", "User")
                        .WithMany("Areas")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK__area__user_id__49C3F6B7")
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("PDMF.Data.Entities.AspNetRoleClaim", b =>
                {
                    b.HasOne("PDMF.Data.Entities.Role", "Role")
                        .WithMany("AspNetRoleClaims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("PDMF.Data.Entities.AspNetUserClaim", b =>
                {
                    b.HasOne("PDMF.Data.Entities.User", "User")
                        .WithMany("AspNetUserClaims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("PDMF.Data.Entities.AspNetUserLogin", b =>
                {
                    b.HasOne("PDMF.Data.Entities.User", "User")
                        .WithMany("AspNetUserLogins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("PDMF.Data.Entities.AspNetUserRole", b =>
                {
                    b.HasOne("PDMF.Data.Entities.Role", "Role")
                        .WithMany("AspNetUserRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PDMF.Data.Entities.User", "User")
                        .WithMany("AspNetUserRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("PDMF.Data.Entities.AspNetUserToken", b =>
                {
                    b.HasOne("PDMF.Data.Entities.User", "User")
                        .WithMany("AspNetUserTokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("PDMF.Data.Entities.Dataset", b =>
                {
                    b.HasOne("PDMF.Data.Entities.Area", "Area")
                        .WithMany("Datasets")
                        .HasForeignKey("AreaId")
                        .HasConstraintName("FK__dataset__area_id__4D94879B");

                    b.HasOne("PDMF.Data.Entities.User", "User")
                        .WithMany("Datasets")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK__dataset__user_id__4CA06362")
                        .IsRequired();

                    b.Navigation("Area");

                    b.Navigation("User");
                });

            modelBuilder.Entity("PDMF.Data.Entities.ForecastResult", b =>
                {
                    b.HasOne("PDMF.Data.Entities.ForecastTask", "Task")
                        .WithMany("ForecastResults")
                        .HasForeignKey("TaskId")
                        .HasConstraintName("FK__forecast___task___5BE2A6F2")
                        .IsRequired();

                    b.HasOne("PDMF.Data.Entities.User", "User")
                        .WithMany("ForecastResults")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK__forecast___user___628FA481")
                        .IsRequired();

                    b.Navigation("Task");

                    b.Navigation("User");
                });

            modelBuilder.Entity("PDMF.Data.Entities.ForecastTask", b =>
                {
                    b.HasOne("PDMF.Data.Entities.ModelingResult", "ModelingResult")
                        .WithMany("ForecastTasks")
                        .HasForeignKey("ModelingResultId")
                        .HasConstraintName("FK__forecast___model__59063A47")
                        .IsRequired();

                    b.HasOne("PDMF.Data.Entities.ModelingTask", "ModelingTask")
                        .WithMany()
                        .HasForeignKey("ModelingTaskId");

                    b.HasOne("PDMF.Data.Entities.User", "User")
                        .WithMany("ForecastTasks")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK__forecast___user___60A75C0F")
                        .IsRequired();

                    b.Navigation("ModelingResult");

                    b.Navigation("ModelingTask");

                    b.Navigation("User");
                });

            modelBuilder.Entity("PDMF.Data.Entities.ModelingResult", b =>
                {
                    b.HasOne("PDMF.Data.Entities.ModelingTask", "Task")
                        .WithMany("ModelingResults")
                        .HasForeignKey("TaskId")
                        .HasConstraintName("FK__modeling___task___5629CD9C")
                        .IsRequired();

                    b.HasOne("PDMF.Data.Entities.User", "User")
                        .WithMany("ModelingResults")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK__modeling___user___619B8048")
                        .IsRequired();

                    b.Navigation("Task");

                    b.Navigation("User");
                });

            modelBuilder.Entity("PDMF.Data.Entities.ModelingTask", b =>
                {
                    b.HasOne("PDMF.Data.Entities.Dataset", "Dataset")
                        .WithMany("ModelingTasks")
                        .HasForeignKey("DatasetId")
                        .HasConstraintName("FK__modeling___datas__534D60F1")
                        .IsRequired();

                    b.HasOne("PDMF.Data.Entities.User", "User")
                        .WithMany("ModelingTasks")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK__modeling___user___5FB337D6")
                        .IsRequired();

                    b.Navigation("Dataset");

                    b.Navigation("User");
                });

            modelBuilder.Entity("PDMF.Data.Entities.ParseTask", b =>
                {
                    b.HasOne("PDMF.Data.Entities.Dataset", "Dataset")
                        .WithMany("ParseTasks")
                        .HasForeignKey("DatasetId")
                        .HasConstraintName("FK__parse_tas__datas__5070F446")
                        .IsRequired();

                    b.HasOne("PDMF.Data.Entities.User", "User")
                        .WithMany("ParseTasks")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK__parse_tas__user___5EBF139D")
                        .IsRequired();

                    b.Navigation("Dataset");

                    b.Navigation("User");
                });

            modelBuilder.Entity("PDMF.Data.Entities.Area", b =>
                {
                    b.Navigation("Datasets");
                });

            modelBuilder.Entity("PDMF.Data.Entities.Dataset", b =>
                {
                    b.Navigation("ModelingTasks");

                    b.Navigation("ParseTasks");
                });

            modelBuilder.Entity("PDMF.Data.Entities.ForecastTask", b =>
                {
                    b.Navigation("ForecastResults");
                });

            modelBuilder.Entity("PDMF.Data.Entities.ModelingResult", b =>
                {
                    b.Navigation("ForecastTasks");
                });

            modelBuilder.Entity("PDMF.Data.Entities.ModelingTask", b =>
                {
                    b.Navigation("ModelingResults");
                });

            modelBuilder.Entity("PDMF.Data.Entities.Role", b =>
                {
                    b.Navigation("AspNetRoleClaims");

                    b.Navigation("AspNetUserRoles");
                });

            modelBuilder.Entity("PDMF.Data.Entities.User", b =>
                {
                    b.Navigation("Areas");

                    b.Navigation("AspNetUserClaims");

                    b.Navigation("AspNetUserLogins");

                    b.Navigation("AspNetUserRoles");

                    b.Navigation("AspNetUserTokens");

                    b.Navigation("Datasets");

                    b.Navigation("ForecastResults");

                    b.Navigation("ForecastTasks");

                    b.Navigation("ModelingResults");

                    b.Navigation("ModelingTasks");

                    b.Navigation("ParseTasks");
                });
#pragma warning restore 612, 618
        }
    }
}