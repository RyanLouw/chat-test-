namespace CallQuality.Core.DataAccess.Context.Entities;

public class QuestionInType
{
    public int RowKey { get; set; }

    public int? QuestionId { get; set; }

    public bool? Active { get; set; }

    public int? SubGroupTypeId { get; set; }

    public int? Score { get; set; }

    public int OrderNumber { get; set; }

    public virtual Questions? Question { get; set; }

    public virtual SubGroupType? SubGroupType { get; set; }
}
