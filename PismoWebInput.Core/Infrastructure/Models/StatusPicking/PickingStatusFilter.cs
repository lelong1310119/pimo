using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PismoWebInput.Core.Infrastructure.Models.StatusPicking
{
    public class PickingStatusFilter
    {
        public decimal? PickingStatusID { get; set; }
        public string? PickingInstructionNo { get; set; }
        public string? MFInstructionNo { get; set; }
    }
}
