using AutoMapper;
using PismoWebInput.Core.Infrastructure.Common.Mappings;
using PismoWebInput.Core.Infrastructure.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PismoWebInput.Core.Infrastructure.Models.StatusPicking
{
    public class PickingStatusModel : ICustomMappings
    {
        public int? Id { get; set; }
        public decimal PickingStatusID { get; set; }
        public string? PickingInstructionNo { get; set; }
        public string? MFInstructionNo { get; set; }
        public DateTimeOffset? StartPickingTime { get; set; }
        public DateTimeOffset? EndPickingTime { get; set; }
        public string? STCode { get; set; }
        public string? LineCode { get; set; }
        public string? ShutterCode { get; set; }
        public string? ParentItemCode { get; set; }
        public int? ParentItemQty { get; set; }
        public string? ChildItemCode { get; set; }
        public int? ChildItemQty { get; set; }
        public byte? PickingInstructionStatus { get; set; }
        public byte? MFInstructionStatus { get; set; }
        public string? User_code { get; set; }
        public byte? Status { get; set; }
        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<PickingStatus, PickingStatusModel>();
            configuration.CreateMap<PickingStatusModel, PickingStatus>();
        }
    }
}
