using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DAL;
using Entities;
using Management.Models;
using Util;
using Util.Query;

namespace Management.Services
{
	public class CommodityTypeService : BaseService
	{
		#region Properties

		private CommodityTypeDAL _commodityTypeDal;
		public CommodityTypeDAL CommodityTypeDal
		{
			get { return _commodityTypeDal ?? (_commodityTypeDal = new CommodityTypeDAL()); }
			set { _commodityTypeDal = value; }
		}

		#endregion

		#region Methods

		public List<CommodityTypeViewVM> GetAllCommodityTypes()
		{
			var commTypes = CommodityTypeDal.GetAll(new List<string> {"Commodity"});
			return commTypes.Select(o => new CommodityTypeViewVM
											 {
												 Id = o.Id,
												 Name = o.Name,
												 CommodityId = o.CommodityId,
												 CommodityName = o.Commodity.Name,
												 Description = o.Description
											 }).ToList();
		}

		public List<CommodityTypeViewVM> GetCommodityTypeByRange(int from, int to)
		{
			var sorts = new List<SortCol> {new SortCol {ColName = "Id", IsDescending = false}};
			var res =  CommodityTypeDal.Query(null, sorts, from, to, new List<string>{"Commodity"});
			return res.Select(o => new CommodityTypeViewVM
									   {
										   Id = o.Id,
										   Name = o.Name,
										   CommodityId = o.CommodityId,
										   CommodityName = o.Commodity.Name,
										   Description = o.Description
									   }).ToList();
		}

		public List<CommodityTypeViewVM> GetCommodityTypeByRange(int from, int to, int? commodityId, string commodityTypeName)
		{
			var func1 = GetQueryExp(commodityId, commodityTypeName);
			var sorts = new List<SortCol> {new SortCol {ColName = "Id", IsDescending = false}};
			var result =CommodityTypeDal.Query(func1, sorts, from, to, new List<string> {"Commodity"});

			return result.Select(o => new CommodityTypeViewVM
										  {
											  Id = o.Id,
											  Name = o.Name,
											  CommodityId = o.CommodityId,
											  CommodityName = o.Commodity.Name,
											  Description = o.Description
										  }).ToList();
		}

		public List<CommodityTypeViewVM> GetCommodityTypeByCommodityId(int commodityId)
		{     
			var sorts = new List<SortCol> {new SortCol {ColName = "Id", IsDescending = false}};
			var results = CommodityTypeDal.Query(o => o.CommodityId == commodityId, sorts, new List<string> { "Commodity"}).ToList();
			return results.Select(o => new CommodityTypeViewVM { 
					Id = o.Id,
					Name = o.Name,
					CommodityId = o.CommodityId,
					CommodityName = o.Commodity.Name,
					Description = o.Description
			}).ToList();
		}

		public int GetAllCount()
		{
			return CommodityTypeDal.GetAllCount();
		}

		public int GetCount(int? commodityId, string commodityTypeName)
		{
			var func = GetQueryExp(commodityId, commodityTypeName);
			return CommodityTypeDal.GetCount(func);
		}

		public CommodityTypeEditVM GetById(int id)
		{
			var entity =  CommodityTypeDal.GetById(id, null);
			return new CommodityTypeEditVM
					   {
						   Id = entity.Id,
						   Name = entity.Name,
						   CommodityId = entity.CommodityId,
						   Description = entity.Description
					   };
		}

		public ErrorCode Create(CommodityTypeEditVM ct)
		{
			try
			{
				if (CommodityTypeDal.GetExisted(o => o.CommodityId == ct.CommodityId && o.Name == ct.Name))
				{
					return ErrorCode.CommodityTypeExisted;
				}

				var commType = new CommodityType
				{
					Name = ct.Name,
					Description = ct.Description,
					CommodityId = ct.CommodityId
				};

				CommodityTypeDal.Create(commType);
				return ErrorCode.NoError;
			}
			catch (Exception)
			{
				return ErrorCode.ServerError;
			}
		}

		public ErrorCode Update(CommodityTypeEditVM vm)
		{
			try
			{
				if (
					CommodityTypeDal.GetExisted(
						o => o.Id != vm.Id && o.CommodityId == vm.CommodityId && o.Name == vm.Name))
				{
					return ErrorCode.CommodityTypeExisted;
				}

				var commType = new CommodityType
				{
					Id = vm.Id,
					Name = vm.Name,
					Description = vm.Description,
					CommodityId = vm.CommodityId
				};

				CommodityTypeDal.Update(commType);
				return ErrorCode.NoError;
			}
			catch (Exception)
			{
				return ErrorCode.ServerError;
			}
		}

		public ErrorCode Delete(int id)
		{
			try
			{
				var commodityType = CommodityTypeDal.GetById(id, new List<string> { "Brands"});
				if (commodityType.Brands != null && commodityType.Brands.Count > 0)
				{
					if(commodityType.Brands.Any(c => !c.IsDeleted))
					{
						return ErrorCode.CommodityTypeConnectedBrand;
					}
				}

				CommodityTypeDal.Delete(id);
				return ErrorCode.NoError;
			}
			catch (Exception)
			{
				return ErrorCode.ServerError;
			}
		}

		public bool GetNameExisted(string name, int id, int commodityId)
		{
			return !CommodityTypeDal.GetExisted(o => o.Id != id && o.Name == name && o.CommodityId == commodityId);
		}

		#endregion

		#region Private Methods

		private Expression<Func<CommodityType, bool>> GetQueryExp(int? commodityId, string commodityTypeName)
		{
			var clauses = new List<Clause>();
			if (commodityId != null)
			{
				clauses.Add(new Clause
								{
									PropertyName = "CommodityId",
									Operator = Operator.Eq,
									Value = commodityId
								});
			}

			if (!string.IsNullOrWhiteSpace(commodityTypeName))
			{
				clauses.Add(new Clause
								{
									Operator = Operator.Like,
									PropertyName = "Name",
									Value = commodityTypeName
								});
			}

			var manager = new QueryManager<CommodityType>();
			return  manager.Compile(clauses);
		}

		#endregion
	}
}
