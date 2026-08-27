
namespace CallQuality.Core.Manager.Models.CallQualityDTOs;

public sealed class QuestionInTypeDTO
{
    public int RowKey { get; set; }

    public int? QuestionId { get; set; }

    public bool? Active { get; set; }

    public int? SubGroupTypeId { get; set; }

    public int? Score { get; set; }

    public int OrderNumber { get; set; }

    public  QuestionsDTO? Question { get; set; }

    public  SubGroupTypeDTO? SubGroupType { get; set; }
}
