using System.ComponentModel.DataAnnotations;

namespace Front.Models
{
	public class SignInVM
	{
		[Required(AllowEmptyStrings = false, ErrorMessage = "登录名不能为空！")]
		public string LoginName { get; set; }

		[Required(AllowEmptyStrings = false, ErrorMessage = "密码不能为空！")]
		public string Password { get; set; }
	}
}
