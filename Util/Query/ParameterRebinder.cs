﻿using System.Collections.Generic;
using System.Linq.Expressions;

namespace Util.Query
{
	public class ParameterRebinder : ExpressionVisitor
	{
		#region Properties

		private readonly Dictionary<ParameterExpression, ParameterExpression> _map;
		public ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
		{
			_map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
		} 

		#endregion

		#region Method

		public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
		{
			return new ParameterRebinder(map).Visit(exp);
		}

		protected override Expression VisitParameter(ParameterExpression p)
		{
			ParameterExpression replacement;
			if (_map.TryGetValue(p, out replacement))
			{
				p = replacement;
			}
			return base.VisitParameter(p);
		} 

		#endregion
	}
}
