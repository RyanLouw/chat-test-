
namespace CallQuality.Core.DataAccess.Context.Entities.StoredProcedureResults.Questions;

public class QuestionWrongStat
{
    public string? TypeName { get; set; }
    public int QuestionID { get; set; }
    public string? QuestionValue { get; set; }

    public int TotalAnswered { get; set; }
    public int TotalCorrect { get; set; }
    public int TotalWrong { get; set; }

    public decimal WrongPct { get; set; }
    public decimal CorrectPct { get; set; }
}
