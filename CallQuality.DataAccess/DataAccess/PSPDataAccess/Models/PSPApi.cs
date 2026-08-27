
using System.ComponentModel.DataAnnotations;

namespace CallQuality.Core.DataAccess.PSPDataAccess.Models;

public sealed class PSPApi
{
    [Required]
    public string BaseUrl { get; set; } = string.Empty;
    [Required]
    public bool IsDevelopment { get; set; }
}
