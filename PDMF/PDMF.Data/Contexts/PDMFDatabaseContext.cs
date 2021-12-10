using Microsoft.EntityFrameworkCore;
using PDMF.Data.Entities;
using PDMF.Data.Utilities.Configuration;

#nullable disable

namespace PDMF.Data.Contexts
{
    public partial class PDMFDatabaseContext : DbContext
    {
        public PDMFDatabaseContext()
        {
        }

        public PDMFDatabaseContext(DbContextOptions<PDMFDatabaseContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Area> Areas { get; set; }
        public virtual DbSet<Role> AspNetRoles { get; set; }
        public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }
        public virtual DbSet<User> AspNetUsers { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUserRole> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }
        public virtual DbSet<Dataset> Datasets { get; set; }
        public virtual DbSet<ForecastResult> ForecastResults { get; set; }
        public virtual DbSet<ForecastTask> ForecastTasks { get; set; }
        public virtual DbSet<ModelingResult> ModelingResults { get; set; }
        public virtual DbSet<ModelingTask> ModelingTasks { get; set; }
        public virtual DbSet<ParseTask> ParseTasks { get; set; }
        public virtual DbSet<TaskAudit> TaskAudits { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Cyrillic_General_CI_AS");

            modelBuilder.Entity<Area>(entity =>
            {
                entity.ToTable("area");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasColumnName("name");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450)
                    .HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Areas)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__area__user_id__49C3F6B7");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedName] IS NOT NULL)");

                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetRoleClaim>(entity =>
            {
                entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

                entity.Property(e => e.RoleId).IsRequired();

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetRoleClaims)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedUserName] IS NOT NULL)");

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.State);

                entity.Property(e => e.CreateDate);
                
                entity.Property(e => e.UserName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetUserClaim>(entity =>
            {
                entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserClaims)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserLogin>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

                entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserLogins)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserRole>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasIndex(e => e.RoleId, "IX_AspNetUserRoles_RoleId");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.RoleId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserToken>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserTokens)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<Dataset>(entity =>
            {
                entity.ToTable("dataset");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AreaId)
                    .HasMaxLength(450)
                    .HasColumnName("area_id");

                entity.Property(e => e.Columns)
                    .IsRequired()
                    .HasMaxLength(1200)
                    .HasColumnName("columns");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasColumnName("name");

                entity.Property(e => e.Size).HasColumnName("size");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450)
                    .HasColumnName("user_id");

                entity.HasOne(d => d.Area)
                    .WithMany(p => p.Datasets)
                    .HasForeignKey(d => d.AreaId)
                    .HasConstraintName("FK__dataset__area_id__4D94879B");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Datasets)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__dataset__user_id__4CA06362");
            });

            modelBuilder.Entity<ForecastResult>(entity =>
            {
                entity.ToTable("forecast_result");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.TaskId)
                    .IsRequired()
                    .HasMaxLength(450)
                    .HasColumnName("task_id");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450)
                    .HasColumnName("user_id");

                entity.HasOne(d => d.Task)
                    .WithMany(p => p.ForecastResults)
                    .HasForeignKey(d => d.TaskId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__forecast___task___5BE2A6F2");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ForecastResults)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__forecast___user___628FA481");
            });

            modelBuilder.Entity<ForecastTask>(entity =>
            {
                entity.ToTable("forecast_task");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CompleteDate)
                    .HasColumnType("datetime")
                    .HasColumnName("complete_date");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date");

                entity.Property(e => e.Mode).HasColumnName("mode");

                entity.Property(e => e.ModelingResultId)
                    .IsRequired()
                    .HasMaxLength(450)
                    .HasColumnName("modeling_result_id");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450)
                    .HasColumnName("user_id");

                entity.HasOne(d => d.ModelingResult)
                    .WithMany(p => p.ForecastTasks)
                    .HasForeignKey(d => d.ModelingResultId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__forecast___model__59063A47");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ForecastTasks)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__forecast___user___60A75C0F");
            });

            modelBuilder.Entity<ModelingResult>(entity =>
            {
                entity.ToTable("modeling_result");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date");

                entity.Property(e => e.ModelType).HasColumnName("model_type");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.TaskId)
                    .IsRequired()
                    .HasMaxLength(450)
                    .HasColumnName("task_id");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450)
                    .HasColumnName("user_id");

                entity.HasOne(d => d.Task)
                    .WithMany(p => p.ModelingResults)
                    .HasForeignKey(d => d.TaskId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__modeling___task___5629CD9C");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ModelingResults)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__modeling___user___619B8048");
            });

            modelBuilder.Entity<ModelingTask>(entity =>
            {
                entity.ToTable("modeling_task");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CompleteDate)
                    .HasColumnType("datetime")
                    .HasColumnName("complete_date");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date");

                entity.Property(e => e.DatasetId)
                    .IsRequired()
                    .HasMaxLength(450)
                    .HasColumnName("dataset_id");

                entity.Property(e => e.DesiredColumn).HasColumnName("desired_column");

                entity.Property(e => e.ModelType).HasColumnName("model_type");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450)
                    .HasColumnName("user_id");

                entity.HasOne(d => d.Dataset)
                    .WithMany(p => p.ModelingTasks)
                    .HasForeignKey(d => d.DatasetId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__modeling___datas__534D60F1");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ModelingTasks)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__modeling___user___5FB337D6");
            });

            modelBuilder.Entity<ParseTask>(entity =>
            {
                entity.ToTable("parse_task");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CompleteDate)
                    .HasColumnType("datetime")
                    .HasColumnName("complete_date");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date");

                entity.Property(e => e.DatasetId)
                    .IsRequired()
                    .HasMaxLength(450)
                    .HasColumnName("dataset_id");

                entity.Property(e => e.ResultDescription)
                    .HasMaxLength(100)
                    .HasColumnName("result_description");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450)
                    .HasColumnName("user_id");

                entity.HasOne(d => d.Dataset)
                    .WithMany(p => p.ParseTasks)
                    .HasForeignKey(d => d.DatasetId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__parse_tas__datas__5070F446");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ParseTasks)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__parse_tas__user___5EBF139D");
            });

            modelBuilder.Entity<TaskAudit>(entity =>
            {
                entity.ToTable("task_audit");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date");

                entity.Property(e => e.Description)
                    .HasMaxLength(100)
                    .HasColumnName("description");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.TaskId)
                    .HasMaxLength(450)
                    .HasColumnName("task_id");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
