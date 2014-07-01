using System.Collections.Generic;

namespace Management.Models
{
	public class RoleVM
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public List<PrevilegeVM> Perms { get; set; }
	}
}
