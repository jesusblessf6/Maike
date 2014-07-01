using System;
using System.Collections.Generic;

namespace Util.Query
{
	public class Component
	{
		public ComponentType ComponentType { get; set; }
		public string PropertyName { get; set; } //Property Name
		public string MethodName { get; set; }
		public List<object> Params { get; set; }
		public Type ElementType { get; set; }
		public object Value { get; set; }
	}
}
