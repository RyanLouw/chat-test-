using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallQuality.Core.DataAccess.ADUsersDataAccess.Models;

public class UsersInDepartment
{
    public long ADUserId { get; set; }
    public string GivenName { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string EmployeeId { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public bool AccountEnabled { get; set; }
}
