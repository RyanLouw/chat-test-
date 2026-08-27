using CallQuality.Core.DataAccess.Context.Entities;
using Microsoft.EntityFrameworkCore;

namespace CallQuality.Core.DataAccess.Context;

public partial class CallQualityDbContext : DbContext
{
    public CallQualityDbContext()
    {
    }

    public CallQualityDbContext(DbContextOptions<CallQualityDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AssessmentDetail> AssessmentDetails { get; set; }

    public virtual DbSet<AssessmentType> AssessmentTypes { get; set; }

    public virtual DbSet<Assessment> Assessments { get; set; }

    public virtual DbSet<Assessor> Assessors { get; set; }

    public virtual DbSet<CallDetail> CallDetails { get; set; }

    public virtual DbSet<Feedback> Feedback { get; set; }

    public virtual DbSet<OperatorAssignmentDel> OperatorAssignmentDel { get; set; }

    public virtual DbSet<OperatorAssignment> OperatorAssignment { get; set; }

    public virtual DbSet<QuestionInType> QuestionInType { get; set; }

    public virtual DbSet<Questions> Questions { get; set; }
    public DbSet<OperatorAssignmentChangeLog> OperatorAssignmentChangeLog { get; set; }
    public virtual DbSet<SubGroupType> SubGroupType { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AssessmentDetail>(entity =>
        {
            entity.HasKey(e => e.RowKey);

            entity.Property(e => e.RowKey).HasColumnName("rowKey");
            entity.Property(e => e.AssessmentId).HasColumnName("AssessmentID");
            entity.Property(e => e.IsNa).HasColumnName("IsNA");
            entity.Property(e => e.QuestionId).HasColumnName("QuestionID");
            entity.Property(e => e.ReassessorNote).HasMaxLength(250);

            entity.HasOne(d => d.Assessment)
                .WithMany(p => p.AssessmentDetails)
                .HasForeignKey(d => d.AssessmentId)
                .HasConstraintName("FK_AssessmentDetails_Assessments");

            entity.HasOne(d => d.Question)
                .WithMany(p => p.AssessmentDetails)
                .HasForeignKey(d => d.QuestionId)
                .HasConstraintName("FK_AssessmentDetails_Questions");
        });
        modelBuilder.Entity<OperatorAssignmentChangeLog>(entity =>
        {
            entity.ToTable("OperatorAssignmentChangeLog");

            entity.HasKey(x => x.LogId);

            entity.Property(x => x.ActionType)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(x => x.OperatorId)
                .HasMaxLength(100);

            entity.Property(x => x.ChangedBy)
                .HasMaxLength(256)
                .IsRequired();

            entity.Property(x => x.ChangedOn)
                .HasDefaultValueSql("SYSUTCDATETIME()");
        });

        modelBuilder.Entity<AssessmentType>(entity =>
        {
            entity.HasKey(e => e.AssessmentTypeId);

            entity.Property(e => e.ShowInFrontend)
                .HasDefaultValue(true);

            entity.Property(e => e.TypeName)
                .HasMaxLength(150);
        });

        modelBuilder.Entity<Assessment>(entity =>
        {
            entity.HasKey(e => e.AssessmentId);

            entity.Property(e => e.AssessmentId).HasColumnName("AssessmentID");

            entity.Property(e => e.AeId)
                .HasMaxLength(150)
                .HasColumnName("AE_id");

            entity.Property(e => e.AeSystemType)
                .HasMaxLength(150)
                .HasColumnName("AE_systemType");

            entity.Property(e => e.AssessedBy)
                .HasMaxLength(250);

            entity.Property(e => e.AssessedOn)
                .HasColumnType("datetime");

            entity.Property(e => e.AssessmentScore)
                .HasColumnName("Assessment_Score");

            entity.Property(e => e.ContactId)
                .HasColumnName("ContactID");

            entity.Property(e => e.ContactPerson)
                .HasMaxLength(150);

            entity.Property(e => e.Extension)
                .HasMaxLength(10);

            entity.Property(e => e.FamilyIdentifier)
                .HasMaxLength(50);

            entity.Property(e => e.InteractionId)
                .HasColumnName("InteractionID");

            entity.Property(e => e.OperatorId)
                .HasMaxLength(100)
                .HasColumnName("Operator_ID");

            entity.Property(e => e.OrderMadeOn)
                .HasColumnType("datetime");

            entity.Property(e => e.OrderPharmacyName)
                .HasMaxLength(100);

            entity.Property(e => e.PatientId)
                .HasColumnName("PatientID");

            entity.Property(e => e.PharmacyGroup)
                .HasMaxLength(100);

            entity.Property(e => e.PharmacyName)
                .HasMaxLength(100);

            entity.Property(e => e.ProfileNumber)
                .HasMaxLength(150);

            entity.Property(e => e.ReassessedBy)
                .HasMaxLength(250);

            entity.Property(e => e.ReassessedOn)
                .HasColumnType("datetime");

            entity.Property(e => e.ReassessmentScore)
                .HasColumnName("Reassessment_Score");

            entity.Property(e => e.ScriptId)
                .HasColumnName("ScriptID");

            entity.Property(e => e.SystemName)
                .HasMaxLength(75);

            entity.HasOne(d => d.AssessedByNavigation)
                .WithMany(p => p.Assessments)
                .HasForeignKey(d => d.AssessedById)
                .HasConstraintName("FK_Assessments_Assessors");

            entity.HasOne(d => d.AssessmentType)
                .WithMany(p => p.Assessments)
                .HasForeignKey(d => d.AssessmentTypeId)
                .HasConstraintName("FK_Assessments_AssessmentTypes");
        });

        modelBuilder.Entity<Assessor>(entity =>
        {
            entity.HasKey(e => e.AssessorId);

            entity.Property(e => e.AssessorName)
                .HasMaxLength(150);
        });

        modelBuilder.Entity<CallDetail>(entity =>
        {
            entity.HasKey(e => e.RowKey);

            entity.Property(e => e.RowKey)
                .HasColumnName("rowKey");

            entity.Property(e => e.AssessmentId)
                .HasColumnName("AssessmentID");

            entity.Property(e => e.CallDate)
                .HasColumnType("datetime");

            entity.Property(e => e.NumberAssessedOn)
                .HasMaxLength(100);

            entity.Property(e => e.RecordingId)
                .HasColumnName("RecordingID");

            entity.Property(e => e.RecordingLength)
                .HasMaxLength(80);

            entity.Property(e => e.RecordingTime)
                .HasPrecision(0);

            entity.Property(e => e.RecordingUrl)
                .HasColumnName("RecordingURL");

            entity.HasOne(d => d.Assessment)
                .WithMany(p => p.CallDetails)
                .HasForeignKey(d => d.AssessmentId)
                .HasConstraintName("FK_CallDetails_Assessments");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.Property(e => e.FeedbackId)
                .HasColumnName("feedbackId");

            entity.Property(e => e.AssessmentsIncluded)
                .HasMaxLength(150);

            entity.Property(e => e.FeedbackSendBy)
                .HasMaxLength(150);

            entity.Property(e => e.FeedbackSendOn)
                .HasColumnType("datetime");

            entity.Property(e => e.FeedbackSendTo)
                .HasMaxLength(150);

            entity.HasMany(d => d.Assessment)
                .WithMany(p => p.Feedback)
                .UsingEntity<Dictionary<string, object>>(
                    "FeedbackAssessment",
                    r => r.HasOne<Assessment>()
                        .WithMany()
                        .HasForeignKey("AssessmentId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_FeedbackAssessment_Assessment"),
                    l => l.HasOne<Feedback>()
                        .WithMany()
                        .HasForeignKey("FeedbackId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_FeedbackAssessment_Feedback"),
                    j =>
                    {
                        j.HasKey("FeedbackId", "AssessmentId");
                    });
        });

        modelBuilder.Entity<OperatorAssignmentDel>(entity =>
        {
            entity.HasKey(e => e.RowKey);

            entity.Property(e => e.ActionDate)
                .HasColumnType("datetime");

            entity.Property(e => e.OperatorId)
                .HasMaxLength(150)
                .HasColumnName("Operator_ID");

            entity.Property(e => e.SecondaryEndDate)
                .HasColumnType("datetime")
                .HasColumnName("Secondary_endDate");

            entity.Property(e => e.SecondaryStartDate)
                .HasColumnType("datetime")
                .HasColumnName("Secondary_startDate");
        });

        modelBuilder.Entity<OperatorAssignment>(entity =>
        {
            entity.ToTable("OperatorAssignments");

            entity.HasKey(e => e.RowKey);

            entity.Property(e => e.RowKey)
                .HasColumnName("rowKey");

            entity.Property(e => e.AssessorId)
                .HasColumnName("assessorId");

            entity.Property(e => e.AssessorIdSecondary)
                .HasColumnName("assessorIdSecondary");

            entity.Property(e => e.OperatorId)
                .HasMaxLength(150)
                .HasColumnName("Operator_ID");

            entity.Property(e => e.SecondaryEndDate)
                .HasColumnType("datetime")
                .HasColumnName("Secondary_endDate");

            entity.Property(e => e.SecondaryStartDate)
                .HasColumnType("datetime")
                .HasColumnName("Secondary_startDate");

            entity.HasOne(d => d.Assessor)
                .WithMany(p => p.OperatorAssignmentsAssessor)
                .HasForeignKey(d => d.AssessorId)
                .HasConstraintName("FK_OperatorAssignments_Assessors");

            entity.HasOne(d => d.AssessorIdSecondaryNavigation)
                .WithMany(p => p.OperatorAssignmentsAssessorIdSecondaryNavigation)
                .HasForeignKey(d => d.AssessorIdSecondary)
                .HasConstraintName("FK_OperatorAssignments_Assessors1");
        });

        modelBuilder.Entity<QuestionInType>(entity =>
        {
            entity.ToTable("QuestionInType");

            entity.HasKey(e => e.RowKey);

            entity.Property(e => e.RowKey)
                .HasColumnName("rowKey");

            entity.Property(e => e.QuestionId)
                .HasColumnName("QuestionID");

            entity.Property(e => e.SubGroupTypeId)
                .HasColumnName("SubGroupTypeId");

            entity.Property(e => e.Active)
                .HasColumnName("Active");

            entity.Property(e => e.Score)
                .HasColumnName("Score");

            entity.Property(e => e.OrderNumber)
                .HasColumnName("orderNumber");

            entity.HasOne(e => e.Question)
                .WithMany(e => e.QuestionInType)
                .HasForeignKey(e => e.QuestionId)
                .HasPrincipalKey(e => e.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_QuestionInType_Questions");

            entity.HasOne(e => e.SubGroupType)
                .WithMany(e => e.QuestionInType)
                .HasForeignKey(e => e.SubGroupTypeId)
                .HasPrincipalKey(e => e.SubGroupTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_QuestionInType_SubGroupType");
        });

        modelBuilder.Entity<Questions>(entity =>
        {
            entity.ToTable("Questions");

            entity.HasKey(e => e.QuestionId);

            entity.Property(e => e.QuestionId)
                .HasColumnName("QuestionID");

            entity.Property(e => e.QuestionValue)
                .HasMaxLength(400);

            entity.Property(e => e.DefaultFeedback)
                .HasColumnName("DefaultFeedback");

            entity.HasMany(e => e.QuestionInType)
                .WithOne(e => e.Question)
                .HasForeignKey(e => e.QuestionId)
                .HasPrincipalKey(e => e.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_QuestionInType_Questions");
        });

        modelBuilder.Entity<SubGroupType>(entity =>
        {
            entity.Property(e => e.SubGroupValue)
                .HasMaxLength(100);

            entity.HasOne(d => d.AssessmentType)
                .WithMany(p => p.SubGroupTypes)
                .HasForeignKey(d => d.AssessmentTypeId)
                .HasConstraintName("FK_SubGroupType_AssessmentTypes");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}