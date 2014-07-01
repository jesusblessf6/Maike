using System.ComponentModel.DataAnnotations;

namespace Management.Models
{
    public class SwitchMarketManagementEditVM
    {
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "开始时间不能为空")]
        public string StartTime { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "结束时间不能为空")]
        public string EndTime { get; set; }
    }
}
