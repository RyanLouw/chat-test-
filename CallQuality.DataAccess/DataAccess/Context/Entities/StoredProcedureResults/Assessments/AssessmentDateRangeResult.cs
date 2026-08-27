using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CallQuality.Core.DataAccess.Context.Entities.StoredProcedureResults.Assessments;

public sealed class AssessmentDateRangeResult
{
    [Column("AssessmentID")]
    public int AssessmentId { get; set; }

    [Column("AssessmentTypeId")]
    public int? AssessmentTypeId { get; set; }

    [Column("FamilyIdentifier")]
    public string? FamilyIdentifier { get; set; }

    [Column("PatientID")]
    public long? PatientId { get; set; }

    [Column("ScriptID")]
    public long? ScriptId { get; set; }

    [Column("SystemName")]
    public string? SystemName { get; set; }

    [Column("InteractionID")]
    public long? InteractionId { get; set; }

    [Column("ProfileNumber")]
    public string? ProfileNumber { get; set; }

    [Column("PharmacyGroup")]
    public string? PharmacyGroup { get; set; }

    [Column("PharmacyName")]
    public string? PharmacyName { get; set; }

    [Column("Assessment_Score")]
    public int? AssessmentScore { get; set; }

    [Column("AssessedBy")]
    public string? AssessedBy { get; set; }

    [Column("AssessedOn")]
    public DateTime? AssessedOn { get; set; }

    [Column("IsReassessed")]
    public bool? IsReassessed { get; set; }

    [Column("Reassessment_Score")]
    public int? ReassessmentScore { get; set; }

    [Column("ReassessedBy")]
    public string? ReassessedBy { get; set; }

    [Column("ReassessedOn")]
    public DateTime? ReassessedOn { get; set; }

    [Column("Extension")]
    public string? Extension { get; set; }

    [Column("agent")]
    public string? OperatorId { get; set; }

    [Column("AssessmentTypeName")]
    public string? AssessmentTypeName { get; set; }

    [Column("ShowInFrontend")]
    public bool? ShowInFrontend { get; set; }

    [Column("RowKey")]
    public int? RowKey { get; set; }

    [Column("QuestionID")]
    public int? QuestionId { get; set; }

    [Column("AssessorAnswer")]
    public bool? AssessorAnswer { get; set; }

    [Column("ReassessorAnswer")]
    public bool? ReassessorAnswer { get; set; }

    [Column("Score")]
    public int? Score { get; set; }

    [Column("ReassessorNote")]
    public string? ReassessorNote { get; set; }

    [Column("IsNa")]
    public bool? IsNa { get; set; }

    [Column("QuestionValue")]
    public string? QuestionValue { get; set; }

    [Column("DefaultFeedback")]
    public string? DefaultFeedback { get; set; }
}
