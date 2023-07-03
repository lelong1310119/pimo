using PismoWebInput.Core.Infrastructure.Models.BlockMap;

namespace PismoWebInput.Core.Infrastructure.Models.Analysis
{
    public class HeatMapResult
    {
        public List<string> Labels { get; set; }
        public List<HeatMapItem> Data { get; set; }

        public HeatMapResult()
        {
            Data = new List<HeatMapItem>();
        }
    }

    public class HeatMapItem
    {
        public string label { get; set; }
        public List<int> data { get; set; }
    }
}
