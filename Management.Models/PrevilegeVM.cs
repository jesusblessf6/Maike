using Enums;

namespace Management.Models
{
	public class PrevilegeVM
	{
		public int RoleId { get; set; }

		public int ControllerId { get; set; }

		public int? ActionId { get; set; }

		public PrevilegeLevel PrevilegeLevel { get; set; }

		public int Id { get; set; }
	}
}
