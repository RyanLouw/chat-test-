
using CallQuality.Core.Manager.AssessmentsManager.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CallQuality.Core.Manager.TrainingManager.Models;

public sealed class TrainingDetailsPageVM
{
    public NewTrainingRegisterVM NewTraining { get; set; } = new();
    public List<UserAddVM> Facilitators { get; set; } = new();
    public List<UserAddVM> Trainees { get; set; } = new();
    public List<UserAddVM> Leaders { get; set; } = new();
    public List<UserAddVM.Group> Groups { get; set; } = new();
    public List<DurationOption> TimeDurations { get; set; } = new();
}


public class NewTrainingRegisterVM
{
    [Required(ErrorMessage = "Training Topic is required")]
    public string? TrainingTopic { get; set; }

    [Required(ErrorMessage = "Training Description is required")]
    public string? TrainingDescription { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Time Duration is required")]
    public int TimeDuration { get; set; }

    [Required(ErrorMessage = "Due Date is required")]
    public DateTime? DueDate { get; set; }

    public UserAddVM TrainingFacilitator { get; set; } = new();

    [Required(ErrorMessage = "At least one trainee must be selected")]
    [MinLength(1, ErrorMessage = "At least one trainee must be selected")]
    public List<string> SelectedTraineeIds { get; set; } = new();

    public List<UserAddVM> Trainees { get; set; } = new();

    public string? LeaderId { get; set; }

    public string? GroupId { get; set; }

    public bool AddNewAssessment { get; set; }

    public bool IsCallQuality { get; set; }

    public string? SystemId { get; set; }

    public List<TrainingFileDTO> UploadedFiles { get; set; } = new();
}
public class TrainingFileDTO
{
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public byte[] FileContent { get; set; } = Array.Empty<byte>();
}

public class DurationOption
{
    public int DurationValue { get; set; }
    public string DurationText { get; set; }
}


