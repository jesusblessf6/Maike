﻿namespace Management.Models
{
	public class CurrentUserVM
	{
		public int Id { get; set; }
		public string LoginName { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string SHFECodes { get; set; }
		public bool IsSuper { get; set; }
	}
}
