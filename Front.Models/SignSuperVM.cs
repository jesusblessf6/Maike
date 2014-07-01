using System.ComponentModel.DataAnnotations;

namespace Front.Models
{
	public class SignSuperVM
	{
		[Required(AllowEmptyStrings = false, ErrorMessage = "登录名不能为空")]
		public string LoginName { get; set; }

		[Required(AllowEmptyStrings = false, ErrorMessage = "姓名不能为空")]
		public string Name { get; set; }

		[Required(AllowEmptyStrings = false, ErrorMessage = "密码不能为空")]
		public string Password { get; set; }

		[Required(AllowEmptyStrings = false, ErrorMessage = "密码确认不能为空")]
		public string PasswordConfirm { get; set; }

		public string Description { get; set; }

		public bool IsSuper { get; set; }
	}
}
