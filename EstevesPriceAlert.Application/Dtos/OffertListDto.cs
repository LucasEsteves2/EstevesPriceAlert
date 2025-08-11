using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstevesPriceAlert.Application.Dtos
{
    public class OffertListDto
    {
        public decimal TargetPrice { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? LastNotifiedAtUtc { get; set; }
        public int NotifyCount { get; set; } = 0;
        public int MinHoursBetweenAlerts { get; set; } = 24;
        public List<string> ProductUrls { get; set; } = new();
        public string ProductName { get; internal set; }
    }
}
