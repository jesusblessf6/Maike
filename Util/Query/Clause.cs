using System;

namespace Util.Query
{
	public class Clause
	{
		public object Value { get; set; }
		public Operator Operator { get; set; }
		//public ComponentType ComponentType { get; set; }
		public string PropertyName { get; set; }
		public Type PropertyType { get; set; }

		public Clause()
		{
			Operator = Operator.Empty;
			PropertyType = null;
		}
	}
}
