using System;
using System.Collections.Generic;
using System.Text;

namespace Web.Api.Core.Domain;

public class SystemInformation
{
    public string OperatingSystem { get; set; } = string.Empty;
    public string Architecture { get; set; } = string.Empty;
    public string FrameworkDescription { get; set; } = string.Empty;
    public int ProcessorCount { get; set; }
    public long TotalMemoryMB { get; set; }
    public long TotalMemoryAllocatedMB { get; set; }
    public double UptimeSeconds { get; set; }
}
