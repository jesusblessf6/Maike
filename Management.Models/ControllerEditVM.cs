using System.ComponentModel.DataAnnotations;

namespace Management.Models
{
	public class ControllerEditVM
	{
		public int Id { get; set; }

		[Required(AllowEmptyStrings = false, ErrorMessage = "内部名称不能为空")]
		public string Name { get; set; }

		[Required(AllowEmptyStrings = false, ErrorMessage = "名称不能为空")]
		public string ChineseName { get; set; }

		public string Description { get; set; }


		public bool IsOpenForAll { get; set; }
	}
}
