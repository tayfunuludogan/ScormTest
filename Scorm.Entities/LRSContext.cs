using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scorm.Entities
{
    //Learning Record Store(LRS) = A place to store learning records (Öprenme Kayıt Deposu)
    public class LRSContext : DbContext
    {
        public LRSContext(DbContextOptions<LRSContext> options)
        : base(options)
        {

        }

        // Core
        public DbSet<User> Users => Set<User>();
        public DbSet<ContentPackage> ContentPackages => Set<ContentPackage>();
        public DbSet<ContentAttempt> ContentAttempts => Set<ContentAttempt>();

        // Summaries
        public DbSet<ContentAttemptScormSummary> AttemptScormSummaries => Set<ContentAttemptScormSummary>();
        public DbSet<ContentAttemptXapiSummary> AttemptXapiSummaries => Set<ContentAttemptXapiSummary>();

        // SCORM detail tables
        public DbSet<ScormRuntimeData> ScormRuntimeData => Set<ScormRuntimeData>();
        public DbSet<ScormInteraction> ScormInteractions => Set<ScormInteraction>();
        public DbSet<ScormInteractionCorrectResponse> ScormInteractionCorrectResponses => Set<ScormInteractionCorrectResponse>();
        public DbSet<ScormInteractionObjective> ScormInteractionObjectives => Set<ScormInteractionObjective>();

        // xAPI detail tables
        public DbSet<XapiStatement> XapiStatements => Set<XapiStatement>();



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // =========================
            // User
            // =========================
            modelBuilder.Entity<User>(e =>
            {
                e.ToTable("Users");

                e.HasKey(x => x.Id);

                e.Property(x => x.Username)
                    .HasMaxLength(100)
                    .IsRequired();

                e.Property(x => x.Email)
                    .HasMaxLength(255)
                    .IsRequired();

                e.HasIndex(x => x.Username).IsUnique();

                e.HasIndex(x => x.Email).IsUnique();

                e.HasMany(x => x.Attempts)
                    .WithOne(x => x.User)
                    .HasForeignKey(x => x.UserId);
            });

            // =========================
            // ContentPackage
            // =========================
            modelBuilder.Entity<ContentPackage>(e =>
            {
                e.ToTable("ContentPackages");
                e.HasKey(x => x.Id);
            });

            // =========================
            // ContentAttempt
            // =========================
            modelBuilder.Entity<ContentAttempt>(e =>
            {
                e.ToTable("ContentAttempts");
                e.HasKey(x => x.Id);

                e.HasOne<ContentPackage>()
                 .WithMany()
                 .HasForeignKey(x => x.PackageId);

                e.HasOne(x => x.ScormSummary)
                 .WithOne(x => x.Attempt)
                 .HasForeignKey<ContentAttemptScormSummary>(x => x.AttemptId);

                e.HasOne(x => x.XapiSummary)
                 .WithOne(x => x.Attempt)
                 .HasForeignKey<ContentAttemptXapiSummary>(x => x.AttemptId);
            });

            // =========================
            // AttemptScormSummary
            // =========================
            modelBuilder.Entity<ContentAttemptScormSummary>(e =>
            {
                e.ToTable("AttemptScormSummary");
                e.HasKey(x => x.AttemptId);

                e.Property(x => x.RawLessonStatus).HasMaxLength(50);
                e.Property(x => x.RawCompletionStatus).HasMaxLength(50);
                e.Property(x => x.RawSuccessStatus).HasMaxLength(50);
                e.Property(x => x.RawExitMode).HasMaxLength(50);

                e.Property(x => x.LastLocation).HasMaxLength(255);
                e.Property(x => x.SuspendData).HasColumnType("nvarchar(max)");

                e.Property(x => x.ScoreRaw).HasPrecision(10, 4);
                e.Property(x => x.ScoreScaled).HasPrecision(10, 6);
                e.Property(x => x.ScoreMin).HasPrecision(10, 4);
                e.Property(x => x.ScoreMax).HasPrecision(10, 4);

                e.Property(x => x.SessionTime).HasMaxLength(30);
                e.Property(x => x.TotalTime).HasMaxLength(30);
            });

            // =========================
            // AttemptXapiSummary
            // =========================
            modelBuilder.Entity<ContentAttemptXapiSummary>(e =>
            {
                e.ToTable("AttemptXapiSummary");
                e.HasKey(x => x.AttemptId);

                e.Property(x => x.ActorMbox).HasMaxLength(255);
                e.Property(x => x.ActorAccountHomePage).HasMaxLength(500);
                e.Property(x => x.ActorAccountName).HasMaxLength(255);

                e.Property(x => x.CompletionVerbId).HasMaxLength(500);

                e.Property(x => x.ScoreScaled).HasPrecision(10, 6);
                e.Property(x => x.ScoreRaw).HasPrecision(10, 4);
                e.Property(x => x.ScoreMin).HasPrecision(10, 4);
                e.Property(x => x.ScoreMax).HasPrecision(10, 4);

                e.Property(x => x.ResumeStateId).HasMaxLength(100);

                //e.HasIndex(x => x.Registration);
            });

            // =========================
            // ScormRuntimeData
            // =========================
            modelBuilder.Entity<ScormRuntimeData>(e =>
            {
                e.ToTable("ScormRuntimeData");
                e.HasKey(x => x.Id);

                e.Property(x => x.Element).HasMaxLength(255).IsRequired();
                e.Property(x => x.Value).HasColumnType("nvarchar(max)");

                e.HasIndex(x => new { x.AttemptId, x.Element })
                 .IsUnique();

                e.HasOne<ContentAttempt>()
                 .WithMany()
                 .HasForeignKey(x => x.AttemptId);
            });

            // =========================
            // ScormInteraction
            // =========================
            modelBuilder.Entity<ScormInteraction>(e =>
            {
                e.ToTable("ScormInteraction");
                e.HasKey(x => x.Id);

                e.HasIndex(x => new { x.AttemptId, x.InteractionIndex })
                 .IsUnique();

                e.HasOne<ContentAttempt>()
                 .WithMany()
                 .HasForeignKey(x => x.AttemptId);
            });

            // =========================
            // ScormInteractionCorrectResponse
            // =========================
            modelBuilder.Entity<ScormInteractionCorrectResponse>(e =>
            {
                e.ToTable("ScormInteractionCorrectResponse");
                e.HasKey(x => x.Id);

                e.Property(x => x.Pattern).HasColumnType("nvarchar(max)");

                e.HasIndex(x => new { x.InteractionId, x.PatternIndex })
                 .IsUnique();

                e.HasOne<ScormInteraction>()
                 .WithMany()
                 .HasForeignKey(x => x.InteractionId);
            });

            // =========================
            // ScormInteractionObjective
            // =========================
            modelBuilder.Entity<ScormInteractionObjective>(e =>
            {
                e.ToTable("ScormInteractionObjective");
                e.HasKey(x => x.Id);

                e.Property(x => x.ObjectiveId).HasMaxLength(255).IsRequired();

                e.HasIndex(x => new { x.InteractionId, x.ObjectiveIndex })
                 .IsUnique();

                e.HasOne<ScormInteraction>()
                 .WithMany()
                 .HasForeignKey(x => x.InteractionId);
            });

            // =========================
            // XapiStatement
            // =========================
            modelBuilder.Entity<XapiStatement>(e =>
            {
                e.ToTable("XapiStatement");
                e.HasKey(x => x.Id);

                e.Property(x => x.VerbId).HasMaxLength(500).IsRequired();
                e.Property(x => x.ObjectId).HasMaxLength(500).IsRequired();

                e.Property(x => x.ActorJson).HasColumnType("nvarchar(max)");
                e.Property(x => x.ResultJson).HasColumnType("nvarchar(max)");
                e.Property(x => x.ContextJson).HasColumnType("nvarchar(max)");
                e.Property(x => x.RawJson).HasColumnType("nvarchar(max)");

                e.HasIndex(x => new { x.AttemptId, x.Timestamp });


                e.HasOne<ContentAttempt>()
                 .WithMany()
                 .HasForeignKey(x => x.AttemptId)
                 .IsRequired(false);
            });


        }
    }
}
