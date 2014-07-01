using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Front.Models
{
    public class ResetPasswordVM
    {
        public int UserId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "原密码不能为空！")]
        public string OldPsw { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "新密码不能为空")]
        [StringLength(30, MinimumLength = 6, ErrorMessage = "密码长度要大于5小于30")]
        public string NewPsw { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "确认新密码不能为空")]
        [StringLength(30, MinimumLength = 6, ErrorMessage = "密码长度要大于5小于30")]
        [Compare("NewPsw", ErrorMessage = "您两次输入的新密码不一致，请确认")]
        public string ConfirmPsw { get; set; }
    }
}
