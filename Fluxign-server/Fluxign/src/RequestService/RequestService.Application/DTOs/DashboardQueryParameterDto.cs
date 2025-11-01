using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestService.Application.DTOs
{
    public class DashboardQueryParameterDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 5;

        public string? StatusFilter { get; set; }
        public string? NameFilter { get; set; }

        public string? SortBy { get; set; }
        public bool SortDescending { get; set; } = false;
    }

}
