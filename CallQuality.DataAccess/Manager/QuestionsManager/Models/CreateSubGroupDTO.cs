using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallQuality.Core.Manager.QuestionsManager.Models;


/// <summary>
/// ///////////////////////////////////////////////////////////////////////////////
/// Parent
/// ///////////////////////////////////////////////////////////////////////////////
/// </summary>

public class CreateSubGroupDTO
{
    public string SubGroupValue { get; set; } = string.Empty;
    public int AssessmentTypeId { get; set; }
    public List<SubGroupQuestionLink> LinkedQuestions { get; set; } = new List<SubGroupQuestionLink>();
}

/// <summary>
/// ///////////////////////////////////////////////////////////////////////////////
/// Children
/// ///////////////////////////////////////////////////////////////////////////////
/// </summary>
public class SubGroupQuestionLink
{
    public int QuestionId { get; set; }
    public bool Active { get; set; }
    public int Score { get; set; }
}