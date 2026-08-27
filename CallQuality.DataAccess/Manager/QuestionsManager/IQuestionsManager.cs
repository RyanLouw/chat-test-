

using CallQuality.Core.Manager.QuestionsManager.Models;

namespace CallQuality.Core.Manager.QuestionsManager;

public interface IQuestionsManager
{
    Task<QuestionTypeDTO> GetMasterQuestionsAsync();

    Task<bool> UpdateQuestionAsync(QuestionWithTypesDTO masterQuestion);
    Task<bool> CreateQuestionAsync(QuestionWithTypesDTO dto);
    Task<bool> UpdateQuestionOrderAsync(SubGroupTypeWithQuestionsDTO dto);
    Task<bool> CreateSubGroupWithQuestionsAsync(CreateSubGroupDTO dto);

}
