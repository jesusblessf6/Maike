using System.Linq;
using System.Linq.Expressions;

namespace Entities
{
	public static class SortHelper
	{
		public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string ordering)
		{
			var type = typeof(T);
			var property = type.GetProperty(ordering);
			var parameter = Expression.Parameter(type, "p");
			var propertyAccess = Expression.MakeMemberAccess(parameter, property);
			var orderByExp = Expression.Lambda(propertyAccess, parameter);
			MethodCallExpression resultExp = Expression.Call(typeof(Queryable), "OrderBy", new[] { type, property.PropertyType }, source.Expression, Expression.Quote(orderByExp));
			return source.Provider.CreateQuery<T>(resultExp);
		}

		public static IQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string ordering)
		{
			var type = typeof(T);
			var property = type.GetProperty(ordering);
			var parameter = Expression.Parameter(type, "p");
			var propertyAccess = Expression.MakeMemberAccess(parameter, property);
			var orderByExp = Expression.Lambda(propertyAccess, parameter);
			MethodCallExpression resultExp = Expression.Call(typeof(Queryable), "OrderByDescending", new[] { type, property.PropertyType }, source.Expression, Expression.Quote(orderByExp));
			return source.Provider.CreateQuery<T>(resultExp);
		}

		public static IQueryable<T> ThenBy<T>(this IQueryable<T> source, string ordering)
		{
			var type = typeof(T);
			var property = type.GetProperty(ordering);
			var parameter = Expression.Parameter(type, "p");
			var propertyAccess = Expression.MakeMemberAccess(parameter, property);
			var orderByExp = Expression.Lambda(propertyAccess, parameter);
			MethodCallExpression resultExp = Expression.Call(typeof(Queryable), "ThenBy", new[] { type, property.PropertyType }, source.Expression, Expression.Quote(orderByExp));
			return source.Provider.CreateQuery<T>(resultExp);
		}

		public static IQueryable<T> ThenByDescending<T>(this IQueryable<T> source, string ordering)
		{
			var type = typeof(T);
			var property = type.GetProperty(ordering);
			var parameter = Expression.Parameter(type, "p");
			var propertyAccess = Expression.MakeMemberAccess(parameter, property);
			var orderByExp = Expression.Lambda(propertyAccess, parameter);
			MethodCallExpression resultExp = Expression.Call(typeof(Queryable), "ThenByDescending", new[] { type, property.PropertyType }, source.Expression, Expression.Quote(orderByExp));
			return source.Provider.CreateQuery<T>(resultExp);
		}
	}
}
