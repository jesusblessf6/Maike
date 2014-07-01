using System;
using System.Collections.Generic;
using DAL;
using Management.Models;
using System.Linq;
using Util;
using Entities;

namespace Management.Services
{
	public class CommodityService : BaseService
	{
		#region Properties

		private CommodityDAL _commodityDal;
		public CommodityDAL CommodityDal
		{
			get { return _commodityDal ?? (_commodityDal = new CommodityDAL()); }
			set { _commodityDal = value; }
		}


		private UserDAL _userDal;
		public UserDAL UserDAL
		{
			get { return _userDal ?? (_userDal = new UserDAL()); }
			set { _userDal = value; }
		}
		#endregion

		#region Methods

		public List<Commodity> GetCommodityByUser(int userId)
		{ 
			User user = UserDAL.GetById(userId, new List<string> { "RelUserCommodities.Commodity" });
			return (from relCommodity in user.RelUserCommodities where !relCommodity.IsDeleted select relCommodity.Commodity).ToList();
		}

		public List<CommodityViewVM> GetAllCommodities()
		{
			var comms = CommodityDal.GetAll();
			return comms.Select(o => new CommodityViewVM
										 {
											 Id = o.Id,
											 Name = o.Name,
											 Description = o.Description,
											 Code = o.Code,
											 IsOpen = o.IsOpen
										 }).ToList();
		}

		public List<CommodityViewVM> GetAllCommoditiesByUser(int userId)
		{
			List<Commodity> commodities = GetCommodityByUser(userId);
			List<int> list = commodities.Select(o => o.Id).ToList();
			if (list.Count > 0)
			{
				var comms = CommodityDal.Query(o => list.Contains(o.Id));
				return comms.Select(o => new CommodityViewVM
				{
					Id = o.Id,
					Name = o.Name,
					Description = o.Description,
					Code = o.Code,
					IsOpen = o.IsOpen
				}).ToList();
			}
			
			return new List<CommodityViewVM>();
		}

		public List<CommodityViewVM> SearchCommoditiesByUser(string nameTerm,int userId)
		{
			List<Commodity> commodities = GetCommodityByUser(userId);
			List<int> list = commodities.Select(o => o.Id).ToList();
			if (list.Count > 0)
			{
				var comms = CommodityDal.Query(o => o.Name.Contains(nameTerm) && list.Contains(o.Id));
				return comms.Select(o => new CommodityViewVM
				{
					Id = o.Id,
					Name = o.Name,
					Description = o.Description,
					Code = o.Code,
					IsOpen = o.IsOpen
				}).ToList();
			}
			
			return new List<CommodityViewVM>();
		}

		public List<CommodityViewVM> SearchCommodities(string nameTerm)
		{
			var comms = CommodityDal.Query(o => o.Name.Contains(nameTerm));
			return comms.Select(o => new CommodityViewVM
										 {
											 Id = o.Id,
											 Name = o.Name,
											 Description = o.Description,
											 Code = o.Code,
											 IsOpen = o.IsOpen
										 }).ToList();
		}

		public ErrorCode UpdateCommodityIsOpen(int id, bool state)
		{
			try
			{
				Commodity commodity = CommodityDal.GetById(id, null);
				commodity.IsOpen = state;
				CommodityDal.Update(commodity);
				return ErrorCode.NoError;
			}
			catch (Exception)
			{
				return ErrorCode.ServerError;
			}
		}

		#endregion
		
	}
}
