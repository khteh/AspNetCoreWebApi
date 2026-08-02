using System;
using System.Collections.Generic;
using System.Text;

namespace Web.Api.Core.Domain;

public class DiskDriveInfo
{
    public string DriveName { get; set; } = string.Empty;
    public string DriveType { get; set; } = string.Empty;
    public long TotalSizeGB { get; set; }
    public long AvailableFreeSpaceGB { get; set; }
}
