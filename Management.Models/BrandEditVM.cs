using System.ComponentModel.DataAnnotations;

namespace Management.Models
{
	public class BrandEditVM
	{
		public int Id { get; set; }

		[Required]
		[Range(1, int.MaxValue, ErrorMessage = "请选择金属")]
		public int CommodityId { get; set; }

		[Required]
		[Range(1, int.MaxValue, ErrorMessage = "请选择金属类型")]
		public int CommodityTypeId { get; set; }

		[Required(AllowEmptyStrings = false, ErrorMessage = "名称不能为空")]
		public string Name { get; set; }

		public string Description { get; set; }
	}
}
