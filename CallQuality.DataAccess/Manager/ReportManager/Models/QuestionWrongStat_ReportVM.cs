using DataAccessQuestionWrongStat =
    CallQuality.Core.DataAccess.Context.Entities.StoredProcedureResults.Questions.QuestionWrongStat;

namespace CallQuality.Core.Manager.ReportManager.Models;

public class QuestionWrongStat_ReportVM
{
    public QuestionWrongStat_ReportVM()
    {
    }

    public QuestionWrongStat_ReportVM(
        IEnumerable<QuestionWrongStatVM> thisMonth,
        IEnumerable<QuestionWrongStatVM> lastMonth,
        IEnumerable<QuestionWrongComparisonVM> comparison,
        IEnumerable<string> typeNames,
        string? selectedTypeName)
    {
        ThisMonth = thisMonth.ToList();
        LastMonth = lastMonth.ToList();
        Comparison = comparison.ToList();
        TypeNames = typeNames.ToList();
        SelectedTypeName = selectedTypeName;
    }

    public List<QuestionWrongStatVM> ThisMonth { get; set; } = new();

    public List<QuestionWrongStatVM> LastMonth { get; set; } = new();

    public List<QuestionWrongComparisonVM> Comparison { get; set; } = new();

    public List<string> TypeNames { get; set; } = new();

    public string? SelectedTypeName { get; set; }

    public bool HasData => ThisMonth.Any();
}

public class QuestionWrongStatVM
{
    public QuestionWrongStatVM()
    {
    }

    public QuestionWrongStatVM(DataAccessQuestionWrongStat entity)
    {
        TypeName = entity.TypeName;
        QuestionID = entity.QuestionID;
        QuestionValue = entity.QuestionValue ?? string.Empty;

        TotalAnswered = entity.TotalAnswered;
        TotalCorrect = entity.TotalCorrect;
        TotalWrong = entity.TotalWrong;

        WrongPct = entity.WrongPct;
        CorrectPct = entity.CorrectPct;
    }

    public string? TypeName { get; set; }

    public int QuestionID { get; set; }

    public string QuestionValue { get; set; } = string.Empty;

    public int TotalAnswered { get; set; }

    public int TotalCorrect { get; set; }

    public int TotalWrong { get; set; }

    public decimal WrongPct { get; set; }

    public decimal CorrectPct { get; set; }
}

public class QuestionWrongComparisonVM
{
    public QuestionWrongComparisonVM()
    {
    }

    public QuestionWrongComparisonVM(
        QuestionWrongStatVM? thisMonth,
        QuestionWrongStatVM? lastMonth)
    {
        var source = thisMonth ?? lastMonth;

        if (source == null)
            return;

        TypeName = source.TypeName;
        QuestionID = source.QuestionID;
        QuestionValue = source.QuestionValue;

        ThisWrongPct = thisMonth?.WrongPct ?? 0;
        LastWrongPct = lastMonth?.WrongPct ?? 0;

        ThisTotalWrong = thisMonth?.TotalWrong ?? 0;
        LastTotalWrong = lastMonth?.TotalWrong ?? 0;

        ThisTotalAnswered = thisMonth?.TotalAnswered ?? 0;
        LastTotalAnswered = lastMonth?.TotalAnswered ?? 0;
    }

    public string? TypeName { get; set; }

    public int QuestionID { get; set; }

    public string QuestionValue { get; set; } = string.Empty;

    public decimal ThisWrongPct { get; set; }

    public decimal LastWrongPct { get; set; }

    public int ThisTotalWrong { get; set; }

    public int LastTotalWrong { get; set; }

    public int ThisTotalAnswered { get; set; }

    public int LastTotalAnswered { get; set; }

    public decimal DeltaWrongPct => ThisWrongPct - LastWrongPct;

    public string Status
    {
        get
        {
            if (DeltaWrongPct > 0) return "Worse";
            if (DeltaWrongPct < 0) return "Better";
            return "Same";
        }
    }
}