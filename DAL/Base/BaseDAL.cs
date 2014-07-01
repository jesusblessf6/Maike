using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq.Expressions;
using Entities;
using System.Linq;

namespace DAL.Base
{
	public abstract class BaseDAL<TEntity> 
		where TEntity: class, IEntity, new()
	{
		#region Basic & internal method
		
		protected IQueryable<TEntity> GetObjQuery(MaikeEntities ctx, List<string> includes = null)
		{
			var set = ctx.Set<TEntity>();
			

			if (includes != null && includes.Count > 0)
			{
				DbQuery<TEntity> dbQuery;
				if (includes.Count == 1)
				{
					dbQuery = set.Include(includes.First());
				}
				else
				{
					dbQuery = set.Include(includes[0]);
					for (int i = 1; i < includes.Count; i ++)
					{
						dbQuery = dbQuery.Include(includes[i]);
					}
				}
				return dbQuery.Where(o => !o.IsDeleted);
			}
			
			return set.Where(o => !o.IsDeleted);
		}

		protected IQueryable<TEntity> GetObjQueryWithDeleted(MaikeEntities ctx, List<string> includes = null)
		{
			var set = ctx.Set<TEntity>();


			if (includes != null && includes.Count > 0)
			{
				DbQuery<TEntity> dbQuery;
				if (includes.Count == 1)
				{
					dbQuery = set.Include(includes.First());
				}
				else
				{
					dbQuery = set.Include(includes[0]);
					for (int i = 1; i < includes.Count; i++)
					{
						dbQuery = dbQuery.Include(includes[i]);
					}
				}
				return dbQuery.AsQueryable();
			}

			return set.AsQueryable();
		}

		protected DbSet<TEntity> GetObjSet(MaikeEntities ctx)
		{
			return ctx.Set<TEntity>();
		}

		#endregion

		
		
		#region Query Method

		public List<TEntity> Query(Expression<Func<TEntity, bool>> func, List<string> includes = null)
		{
			using (var ctx = new MaikeEntities())
			{
				var objQuery = func == null ? GetObjQuery(ctx, includes) : GetObjQuery(ctx, includes).Where(func);
				return objQuery.ToList();
			}
		}

		public List<TEntity> Query(Expression<Func<TEntity, bool>> func, List<SortCol> sorts, List<string> includes = null)
		{
			using (var ctx = new MaikeEntities())
			{
				var objQuery = func == null ? GetObjQuery(ctx, includes) :  GetObjQuery(ctx, includes).Where(func);
				if (sorts != null && sorts.Count > 0)
				{
					
					objQuery = sorts[0].IsDescending
							        ? objQuery.OrderByDescending(sorts[0].ColName)
							        : objQuery.OrderBy(sorts[0].ColName);
					
					if (sorts.Count > 1)
					{
						for (int i = 1; i < sorts.Count; i ++)
						{
							objQuery = sorts[i].IsDescending
									? objQuery.ThenByDescending(sorts[0].ColName)
									: objQuery.ThenBy(sorts[0].ColName);
						}
					}
				}
				return objQuery.ToList();
			}
		}

		public List<TEntity> Query(Expression<Func<TEntity, bool>> func, List<SortCol> sorts, int from, int to, List<string> includes = null)
		{
			using (var ctx = new MaikeEntities())
			{
				var objQuery = func == null ? GetObjQuery(ctx, includes) : GetObjQuery(ctx, includes).Where(func);
				if (sorts != null && sorts.Count > 0)
				{

					objQuery = sorts[0].IsDescending
									? objQuery.OrderByDescending(sorts[0].ColName)
									: objQuery.OrderBy(sorts[0].ColName);

					if (sorts.Count > 1)
					{
						for (int i = 1; i < sorts.Count; i++)
						{
							objQuery = sorts[i].IsDescending
									? objQuery.ThenByDescending(sorts[0].ColName)
									: objQuery.ThenBy(sorts[0].ColName);
						}
					}
				}

				return objQuery.Skip(from).Take(to - from + 1).ToList();
				
			}
		}

		#endregion


		#region Frequently used query

		public TEntity GetById(int id, List<string> includes)
		{
			return Query(o => o.Id == id, includes).FirstOrDefault();
		}

		public bool GetExisted(Expression<Func<TEntity, bool>> func)
		{
			using (var ctx = new MaikeEntities())
			{
				return GetObjQuery(ctx).Any(func);
			}
		}

		public List<TEntity> GetAll(List<string> includes = null)
		{
			using (var ctx = new MaikeEntities())
			{
				return GetObjQuery(ctx, includes).ToList(); //use getobjquery to filter deleted
			}
		}

		#endregion


		#region Count

		public int GetCount(Expression<Func<TEntity, bool>> func, List<string> includes = null)
		{
			using (var ctx = new MaikeEntities())
			{
				return GetObjQuery(ctx, includes).Count(func);
			}
		}

		public int GetAllCount()
		{
			using (var ctx = new MaikeEntities())
			{
				return GetObjQuery(ctx).Count();
			}
		}

		#endregion

		
		
		#region Create

		public void Create(TEntity obj)
		{
			using (var ctx = new MaikeEntities())
			{
				GetObjSet(ctx).Add(obj);
				ctx.SaveChanges();
			}
		}

		#endregion

		
		
		#region Update

		public void Update(TEntity obj)
		{
			using (var ctx = new MaikeEntities())
			{
				ctx.Entry(obj).State = EntityState.Modified;
				ctx.SaveChanges();
			}
		}

		#endregion



		#region Delete
		/// <summary>
		/// Delete logically
		/// </summary>
		/// <param name="id"></param>
		public void Delete(int id)
		{
			using (var ctx = new MaikeEntities())
			{
				var deletedEntity = ctx.Set<TEntity>().Find(id);
				deletedEntity.IsDeleted = true;
				ctx.Entry(deletedEntity).State = EntityState.Modified;
				ctx.SaveChanges();
			}
		}

		/// <summary>
		/// delete physically
		/// </summary>
		public void Obliterate(int id)
		{
			using (var ctx = new MaikeEntities())
			{
				var deletedEntity = ctx.Set<TEntity>().Find(id);
				ctx.Entry(deletedEntity).State = EntityState.Deleted;
				ctx.SaveChanges();
			}
		}

		#endregion
	}
}
