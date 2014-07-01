using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using Management.Models;
using Entities;
using Util;
using System.Linq.Expressions;
using Util.Query;

namespace Management.Services
{
	public class BrandService : BaseService
	{
		#region Properties

		private BrandDAL _brandDal;
		public BrandDAL BrandDal
		{
			get { return _brandDal ?? (_brandDal = new BrandDAL()); }
			set { _brandDal = value; }
		}

		#endregion

		#region Method
		//获取分页数据
		public List<BrandViewVM> GetBrandByRange(int from, int to, int? commodityId, int? commodityTypeId, string brandName)
		{
			if (!commodityId.HasValue && !commodityTypeId.HasValue && string.IsNullOrEmpty(brandName))
			{
				return GetBrandByRange(from, to);
			}
			var func1 = GetQueryExp(commodityId, commodityTypeId, brandName);

			var sorts = new List<SortCol> {new SortCol {ColName = "Id", IsDescending = false}};
			var result = BrandDal.Query(func1, sorts, from, to, new List<string> { "Commodity", "CommodityType" });
			return result.Select(o => new BrandViewVM
			{
				Id = o.Id,
				Name = o.Name,
				CommodityId = o.CommodityId,
				CommodityName = o.Commodity.Name,
				CommodityTypeId = o.CommodityTypeId,
				CommodityTypeName = o.CommodityType.Name,
				Description = o.Description
			}).ToList();

		}

		public List<BrandViewVM> GetBrandByRange(int from, int to)
		{
			var sorts = new List<SortCol> {new SortCol {ColName = "Id", IsDescending = false}};
			var brands = BrandDal.Query(null, sorts, from, to, new List<string> { "Commodity", "CommodityType" });
			return brands.Select(c => new BrandViewVM
			{
				Id = c.Id,
				Name = c.Name,
				CommodityId = c.CommodityId,
				CommodityName = c.Commodity.Name,
				CommodityTypeId = c.CommodityTypeId,
				CommodityTypeName = c.CommodityType.Name,
				Description = c.Description
			}).ToList();
		}

		public List<BrandViewVM> GetBrandByCommodity(int commodityId, int commodityTypeId)
		{
			var func1 = GetQueryExp(commodityId, commodityTypeId);
			var result = BrandDal.Query(func1, new List<string>());
			return result.Select(o => new BrandViewVM
			{
				Id = o.Id,
				Name = o.Name
			}).ToList();
		}

		private Expression<Func<Brand, bool>> GetQueryExp(int commodityId, int commodityTypeId)
		{
			var clauses = new List<Clause>();
			if (commodityId != 0)
			{
				clauses.Add(new Clause
				{
					PropertyName = "CommodityId",
					Operator = Operator.Eq,
					Value = commodityId
				});
			}

			if (commodityTypeId != 0)
			{
				clauses.Add(new Clause
				{
					Operator = Operator.Eq,
					PropertyName = "CommodityTypeId",
					Value = commodityTypeId
				});
			}

			var manager = new QueryManager<Brand>();
			return manager.Compile(clauses);
		}

		public int GetAllCount(int? commodityId, int? commodityTypeId, string brandName)
		{
			if (!commodityId.HasValue && !commodityTypeId.HasValue && string.IsNullOrEmpty(brandName))
			{
				return BrandDal.GetAllCount();
			}
			var func = GetQueryExp(commodityId, commodityTypeId, brandName);
			return BrandDal.GetCount(func);
		}

		public BrandEditVM GetById(int id)
		{
			var entity = BrandDal.GetById(id, null);
			return new BrandEditVM
			{
				Id = entity.Id,
				Name = entity.Name,
				CommodityId = entity.CommodityId,
				CommodityTypeId = entity.CommodityTypeId,
				Description = entity.Description
			};
		}

		/// <summary>
		/// 新增
		/// </summary>
		/// <param name="brandEditVM"></param>
		/// <returns></returns>
		public ErrorCode Create(BrandEditVM brandEditVM)
		{
			var brand = new Brand
			{
				Name = brandEditVM.Name,
				CommodityTypeId = brandEditVM.CommodityTypeId,
				Description = brandEditVM.Description,
				CommodityId = brandEditVM.CommodityId
			};

			try
			{
				if (BrandDal.GetExisted(o => o.CommodityTypeId == brand.CommodityTypeId && o.Name == brandEditVM.Name))
				{
					return ErrorCode.BrandExisted;
				}
				BrandDal.Create(brand);
				return ErrorCode.NoError;
			}
			catch (Exception)
			{
				return ErrorCode.ServerError;
			}
		}

		/// <summary>
		/// 编辑
		/// </summary>
		/// <param name="brandVM"></param>
		/// <returns></returns>
		public ErrorCode Update(BrandEditVM brandVM)
		{
			var brand = new Brand
			{
				Id = brandVM.Id,
				Name = brandVM.Name,
				CommodityId = brandVM.CommodityId,
				CommodityTypeId = brandVM.CommodityTypeId,
				Description = brandVM.Description
			};

			try
			{
				//if(BrandDal.GetExisted(c => c.CommodityTypeId == brand.CommodityTypeId && c.Id != brand.Id && c.Name == brand.Name))
				//{
				//    return ErrorCode.BrandExisted;
				//}
				BrandDal.Update(brand);
				return ErrorCode.NoError;
			}
			catch (Exception)
			{
				return ErrorCode.ServerError;
			}
		}

		/// <summary>
		/// 删除
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public ErrorCode Delete(int id)
		{
			try
			{
				BrandDal.Delete(id);
				return ErrorCode.NoError;
			}
			catch (Exception)
			{
				return ErrorCode.ServerError;
			}
		}

		#endregion

		#region Private Methods

		private Expression<Func<Brand, bool>> GetQueryExp(int? commodityId, int? commodityTypeId, string brandName)
		{
			Expression<Func<Brand, bool>> func1 = null;

			if (commodityId != null)
			{
				func1 = o => o.CommodityId == commodityId;
			}

			if (commodityTypeId != null)
			{
				if (commodityId != null)
				{
					func1 = o => o.CommodityTypeId == commodityTypeId && o.CommodityId == commodityId;
				}
				else
				{
					func1 = o => o.CommodityTypeId == commodityTypeId;
				}
			}

			if (!string.IsNullOrWhiteSpace(brandName))
			{
				func1 = o => o.Name.Contains(brandName);

				if (commodityId != null && commodityTypeId != null)
				{
					func1 = o => o.CommodityTypeId == commodityTypeId && o.CommodityId == commodityId && o.Name.Contains(brandName);
				}

				if (commodityId != null)
				{
					func1 = o => o.CommodityId == commodityId && o.Name.Contains(brandName);
				}

				if (commodityTypeId != null)
				{
					func1 = o => o.CommodityTypeId == commodityTypeId && o.Name.Contains(brandName);
				}
			}

			return func1;
		}

		#endregion

		#region Validator
		public bool GetNameExisted(string name, int id, int commodityTypeId)
		{
			return !BrandDal.GetExisted(o => o.Id != id && o.Name == name && o.CommodityTypeId == commodityTypeId);
		   
		}
		#endregion
	}
}
