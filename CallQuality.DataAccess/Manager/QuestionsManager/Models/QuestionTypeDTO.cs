
using CallQuality.Core.DataAccess.Context.Entities;
using CallQuality.Core.Manager.Models.CallQualityDTOs;
using System.ComponentModel.DataAnnotations;


namespace CallQuality.Core.Manager.QuestionsManager.Models;

    /// <summary>
    /// ///////////////////////////////////////////////////////////////////////////////
    /// Parent
    /// ///////////////////////////////////////////////////////////////////////////////
    /// </summary>

    public class QuestionTypeDTO
    {
        public List<QuestionWithTypesDTO> QuestionsByType { get; set; } = new();
        public List<SubGroupTypeWithQuestionsDTO> SubGroupTypesWithQuestions { get; set; } = new();
        public List<QuestionsDTO> Questions { get; set; } = new();
        public List<AssessmentTypeDTO> AssessmentTypes { get; set; } = new();
    }




/// <summary>
/// ///////////////////////////////////////////////////////////////////////////////
/// Children
/// ///////////////////////////////////////////////////////////////////////////////
/// </summary>
public sealed class SubGroupTypeWithQuestionsDTO
{
    public SubGroupTypeWithQuestionsDTO()
    {
    }

    public SubGroupTypeWithQuestionsDTO(SubGroupType entity)
    {
        SubGroupTypeId = entity.SubGroupTypeId;
        SubGroupValue = entity.SubGroupValue ?? string.Empty;
        AssessmentType =
            entity.AssessmentType?.TypeName ?? string.Empty;

        Questions = entity.QuestionInType
            .Where(qit =>
                qit.Active == true &&
                qit.Question != null)
            .OrderBy(qit => qit.OrderNumber)
            .Select(qit => new QuestionDTO
            {
                QuestionId = qit.Question!.QuestionId,
                QuestionValue =
                    qit.Question.QuestionValue ?? string.Empty,

                DefaultFeedback =
                    qit.Question.DefaultFeedback ?? string.Empty,

                OrderNumber = qit.OrderNumber,

                QuestionInTypes =
                [
                    new QuestionInTypeDTO
                    {
                        QuestionId = qit.QuestionId,
                        SubGroupTypeId = qit.SubGroupTypeId,
                        Score = qit.Score,
                        OrderNumber = qit.OrderNumber,
                        Active = qit.Active
                    }
                ]
            })
            .ToList();
    }

    public int SubGroupTypeId { get; set; }

    public string SubGroupValue { get; set; } = string.Empty;

    public string AssessmentType { get; set; } = string.Empty;

    public List<QuestionDTO> Questions { get; set; } = [];
}

public class QuestionWithTypesDTO
    {
        public QuestionWithTypesDTO()
        {
        }

        public QuestionWithTypesDTO(Questions entity)
        {
            Question = new QuestionDTO
            {
                QuestionId = entity.QuestionId,
                QuestionValue = entity.QuestionValue ?? string.Empty,
                DefaultFeedback = entity.DefaultFeedback ?? string.Empty
            };
            QuestionInTypes = entity.QuestionInType
                .Select(qt => new QuestionInTypeDTO
                {
                    RowKey = qt.RowKey,
                    QuestionId = qt.QuestionId,
                    SubGroupTypeId = qt.SubGroupTypeId,
                    Active = qt.Active,
                    Score = qt.Score,
                    OrderNumber = qt.OrderNumber,

                    SubGroupType = qt.SubGroupType == null
                        ? null
                        : new SubGroupTypeDTO
                        {
                            SubGroupTypeId = qt.SubGroupType.SubGroupTypeId,
                            SubGroupValue = qt.SubGroupType.SubGroupValue ?? string.Empty,

                            AssessmentType = qt.SubGroupType.AssessmentType == null
                                ? null
                                : new AssessmentTypeDTO
                                {
                                    AssessmentTypeId =
                                        qt.SubGroupType.AssessmentType.AssessmentTypeId,

                                    TypeName =
                                        qt.SubGroupType.AssessmentType.TypeName
                                        ?? string.Empty
                                }
                        }
                })
                .ToList();
    }

        [Required(ErrorMessage = "Question data is required.")]
        public QuestionDTO Question { get; set; } = new();

        [Required(ErrorMessage = "Question types are required.")]
        [MinLength(1, ErrorMessage = "At least one subgroup must be supplied.")]
        public List<QuestionInTypeDTO> QuestionInTypes { get; set; } = new();
    }






    public class QuestionAnswerDTO
{
    public QuestionAnswerDTO()
    {
    }

    public QuestionAnswerDTO(AssessmentDetail detail)
    {
        QuestionID = detail.QuestionId ?? 0;
        QuestionValue = detail.Question?.QuestionValue ?? string.Empty;
        Score = detail.Score ?? 0;

        AssessorAnswer = detail.IsNa
            ? "N/A"
            : detail.AssessorAnswer == true
                ? "Yes"
                : "No";

        ReassessorAnswer = detail.ReassessorAnswer == null
            ? string.Empty
            : detail.ReassessorAnswer == true
                ? "Yes"
                : "No";

        ReassessorNote = detail.ReassessorNote;
        IsNA = detail.IsNa;
    }

    public int QuestionID { get; set; }
    public string QuestionValue { get; set; } = string.Empty;
    public int Score { get; set; }
    public string AssessorAnswer { get; set; } = string.Empty;
    public string ReassessorAnswer { get; set; } = string.Empty;
    public string? ReassessorNote { get; set; }
    public bool IsNA { get; set; }
}