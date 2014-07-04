using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Entities;

namespace Util.Query
{
	public class QueryManager<TEntity> where TEntity : class ,IEntity,  new()
	{
		public Expression<Func<TEntity, bool>> Compile(List<Clause> clauses, Expression<Func<TEntity, bool>> extra = null)
		{
			if (clauses.Count <= 0)
			{
				return null;
			}
			ParameterExpression pe = Expression.Parameter(typeof(TEntity), "ent");
			Expression result = CompileOneClause(clauses[0], pe);
			for(int i= 1; i < clauses.Count; i ++)
			{
				result = Expression.AndAlso(result, CompileOneClause(clauses[i], pe));
			}

			var func = Expression.Lambda<Func<TEntity, bool>>(result, new[] { pe });

			if (extra != null)
			{
				func = func.And(extra);
			}

			return func;
		}

		public Expression<Func<TEntity, bool>> Compile(List<Clause> clauses, List<Expression<Func<TEntity, bool>>> extras)
		{
			if (clauses.Count <= 0)
			{
				return null;
			}
			ParameterExpression pe = Expression.Parameter(typeof(TEntity), "ent");
			Expression result = CompileOneClause(clauses[0], pe);
			for (int i = 1; i < clauses.Count; i++)
			{
				result = Expression.AndAlso(result, CompileOneClause(clauses[i], pe));
			}

			var func = Expression.Lambda<Func<TEntity, bool>>(result, new[] { pe });

			if (extras != null && extras.Count > 0)
			{
				func = extras.Aggregate(func, (current, expression) => current.And(expression));
			}

			return func;
		}

		public Expression CompileOneClause(Clause clause, ParameterExpression pe)
		{
			try
			{
				Expression left;
				Expression right;
				Expression result;

				if (clause.Value == null)
				{
					right = null;
				}
				else
				{
					if (clause.PropertyType != null)
					{
						right = Expression.Constant(clause.Value, clause.PropertyType);
					}
					else
					{
						right = Expression.Constant(clause.Value);
					}
					
				}

				left = Expression.Property(pe, typeof(TEntity).GetProperty(clause.PropertyName));

				switch (clause.Operator)
				{
					case Operator.Eq:
						result = Expression.Equal(left, right);
						break;

					case Operator.Ge:
						result = Expression.GreaterThanOrEqual(left, right);
						break;
					case Operator.Gt:
						result = Expression.GreaterThan(left, right);
						break;
					case Operator.Le:
						result = Expression.LessThanOrEqual(left, right);
						break;
					case Operator.Lt:
						result = Expression.LessThan(left, right);
						break;
					case Operator.Ne:
						result = Expression.NotEqual(left, right);
						break;

					case Operator.Like:
						result = Expression.Call(left, typeof(string).GetMethod("Contains", new[] { typeof(string) }), right);
						break;

					case Operator.Empty:
						result = Expression.IsTrue(left);
						break;

					default:
						result = null;
						break;
				}
				return result;
			}
			catch (Exception ex)
			{
				string m = ex.Message;
				throw;
			}
			
		} 

		//public Expression CompileLeft(Clause clause)
		//{
		//	ParameterExpression pe = Expression.Parameter(typeof(string), "ent");
		//	Expression result = null;

		//	//switch (clause.Left.ComponentType)
		//	//{
		//	//	case ComponentType.Property:
		//	//		result = Expression.Property(pe, typeof(TEntity).GetProperty(clause.Left.PropertyName));
		//	//		break;

		//	//	default:
		//	//		break;
		//	//}

		//	return result;
		//}

		bool IsNullableType(Type theType)
		{
			return (theType.IsGenericType && theType.GetGenericTypeDefinition() == typeof(Nullable<>));
		} 
	}
}
