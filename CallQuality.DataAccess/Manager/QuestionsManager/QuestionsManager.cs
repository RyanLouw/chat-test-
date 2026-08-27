
using CallQuality.Core.DataAccess.CallQualityDataAccess.CQ;
using CallQuality.Core.DataAccess.Context.Entities;
using CallQuality.Core.Manager.Models.CallQualityDTOs;
using CallQuality.Core.Manager.QuestionsManager.Models;
using DocumentFormat.OpenXml.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Office.CustomUI;

namespace CallQuality.Core.Manager.QuestionsManager;

public class QuestionsManager: IQuestionsManager
{
    private readonly ICallQualityDataAccess _db;
    public QuestionsManager(ICallQualityDataAccess db)
    {
        _db = db;

    }


    public async Task<QuestionTypeDTO> GetMasterQuestionsAsync()
    {
        List<QuestionWithTypesDTO> questionsByType =(await _db.GetQuestionWithTypesAsync()).Select(x => new QuestionWithTypesDTO(x)).ToList();
        List<SubGroupTypeWithQuestionsDTO> ubGroupTypesWithQuestions = (await _db.GetSubGroupTypesWithQuestionsAsync()).Select(x => new SubGroupTypeWithQuestionsDTO(x)).ToList();
        List<QuestionsDTO> Questions = (await _db.GetAllQuestionsAsync()).Select(x => new QuestionsDTO(x)).ToList();
        List<AssessmentTypeDTO> assessemntTypes = (await _db.GetAllAssessmentTypesAsync()).Select(x => new AssessmentTypeDTO(x)).ToList();

        var master = new QuestionTypeDTO
        {
            QuestionsByType = questionsByType,
            SubGroupTypesWithQuestions = ubGroupTypesWithQuestions,
            
            AssessmentTypes = assessemntTypes,
            Questions =Questions

        };
        return master;
    }




    public async Task<bool> UpdateQuestionAsync(QuestionWithTypesDTO masterQuestion)
    {
        if (masterQuestion == null || masterQuestion.Question == null)
            return false;

        var questionEntity = new Questions
        {
            QuestionId = masterQuestion.Question.QuestionId,
            QuestionValue = masterQuestion.Question.QuestionValue,
            DefaultFeedback = masterQuestion.Question.DefaultFeedback,

            QuestionInType = masterQuestion.QuestionInTypes
                .Select(x => new QuestionInType
                {
                    SubGroupTypeId = x.SubGroupTypeId,
                    Active = x.Active,
                    Score = x.Score
                })
                .ToList()
        };

        return await _db.UpdateQuestionAsync(questionEntity);
    }


    public async Task<bool> CreateQuestionAsync(QuestionWithTypesDTO dto)
    {
        if (dto?.Question == null)
            return false;

        var questionEntity = new Questions
        {
            QuestionValue = dto.Question.QuestionValue,
            DefaultFeedback = dto.Question.DefaultFeedback,

            QuestionInType = (dto.QuestionInTypes ?? new List<QuestionInTypeDTO>())
                .Where(x => x.SubGroupTypeId > 0 && x.Active == true)
                .Select(x => new QuestionInType
                {
                    SubGroupTypeId = x.SubGroupTypeId,
                    Active = true,
                    Score = x.Score
                })
                .ToList()
        };

        return await _db.CreateNewQuestionAsync(questionEntity);
    }

    public async Task<bool> CreateSubGroupWithQuestionsAsync(CreateSubGroupDTO dto)
    {
        if (dto == null)
            return false;

        if (string.IsNullOrWhiteSpace(dto.SubGroupValue))
            return false;

        if (dto.AssessmentTypeId <= 0)
            return false;

        var subGroupEntity = new SubGroupType
        {
            SubGroupValue = dto.SubGroupValue,
            AssessmentTypeId = dto.AssessmentTypeId,

            QuestionInType = dto.LinkedQuestions
                .Where(q => q.QuestionId > 0)
                .Select(q => new QuestionInType
                {
                    QuestionId = q.QuestionId,
                    Active = q.Active,
                    Score = q.Score
                })
                .ToList()
        };

        return await _db.CreateSubGroupWithQuestionsAsync(subGroupEntity);
    }


    public async Task<bool> UpdateQuestionOrderAsync(SubGroupTypeWithQuestionsDTO dto)
    {
        if (dto == null || dto.SubGroupTypeId <= 0)
            return false;

        var subGroupEntity = new SubGroupType
        {
            SubGroupTypeId = dto.SubGroupTypeId,

            QuestionInType = dto.Questions
                .Where(q => q.QuestionId > 0)
                .Select(q => new QuestionInType
                {
                    SubGroupTypeId = dto.SubGroupTypeId,
                    QuestionId = q.QuestionId,
                    OrderNumber = q.OrderNumber
                })
                .ToList()
        };

        return await _db.UpdateQuestionOrderAsync(subGroupEntity);
    }




}
