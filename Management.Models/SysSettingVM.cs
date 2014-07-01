using System.ComponentModel.DataAnnotations;

namespace Management.Models
{
    public class SysSettingVM
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "购买最小单位不能为空！")]
        [RegularExpression(@"^[0-9]*[1-9][0-9]*$", ErrorMessage = "最小单位只能为正整数")]
        public int BuyUnit { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "客户下单时限不能为空！")]
        [RegularExpression(@"^[0-9]*[1-9][0-9]*$", ErrorMessage = "下单时限只能为正整数")]
        public int CountDown { get; set; }
    }
}
