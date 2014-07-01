using DAL;
using Entities;
using Management.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Util;

namespace Management.Services
{
	public class WarehouseService : BaseService
	{
		#region Properties

		private WarehouseDAL _warehouseDal;
		public WarehouseDAL WarehouseDal
		{
			get { return _warehouseDal ?? (_warehouseDal = new WarehouseDAL()); }
			set { _warehouseDal = value; }
		}

		#endregion

		#region Method
		public ErrorCode Create(WarehouseEditVM warehouseEditVM)
		{
			var warehouse = new Warehouse
			{
				Name = warehouseEditVM.Name,
				Address = warehouseEditVM.Address,
				Contact = warehouseEditVM.Contact,
				Tel = warehouseEditVM.Tel,
				Fax = warehouseEditVM.Fax,
				Description = warehouseEditVM.Description
			};
			try
			{
				//if (WarehouseDal.GetExisted(o => o.Name == warehouseEditVM.Name))
				//{
				//    return ErrorCode.WarehouseExisted;
				//}
				//if (!string.IsNullOrEmpty(warehouse.Tel))
				//{
				//    if (!Regex.IsMatch(warehouse.Tel, @"^(\d{3,4}-)?\d{6,8}$") && !Regex.IsMatch(warehouse.Tel, @"^[1]+[3,5]+\d{9}"))
				//    {
				//        return ErrorCode.TelFormatIsIncorrect;
				//    }
				//}
				//if (!string.IsNullOrEmpty(warehouse.Fax) && !Regex.IsMatch(warehouse.Fax, @"^[+]{0,1}(\d){1,3}[ ]?([-]?((\d)|[ ]){1,12})+$"))
				//{
				//    return ErrorCode.FaxFormatIsIncorrect;
				//}
				WarehouseDal.Create(warehouse);
				return ErrorCode.NoError;
			}
			catch (Exception)
			{

				return ErrorCode.ServerError;
			}
		}

		public WarehouseEditVM GetById(int id)
		{
			var result = WarehouseDal.GetById(id, null);
			return new WarehouseEditVM
			{
				Id = result.Id,
				Name = result.Name,
				Address = result.Address,
				Contact = result.Contact,
				Tel = result.Tel,
				Fax = result.Fax,
				Description = result.Description
			};
		}

		public ErrorCode Update(WarehouseEditVM warehouseEditVM)
		{
			var warehouse = new Warehouse
			{
				Id = warehouseEditVM.Id,
				Name = warehouseEditVM.Name,
				Address = warehouseEditVM.Address,
				Contact = warehouseEditVM.Contact,
				Tel = warehouseEditVM.Tel,
				Fax = warehouseEditVM.Fax,
				Description = warehouseEditVM.Description
			};
			try
			{
				//if (WarehouseDal.GetExisted(c => c.Id != warehouse.Id && c.Name == warehouse.Name))
				//{
				//    return ErrorCode.WarehouseExisted;
				//}
				//if (!string.IsNullOrEmpty(warehouse.Tel))
				//{
				//    if (!Regex.IsMatch(warehouse.Tel, @"^(\d{3,4}-)?\d{6,8}$") && !Regex.IsMatch(warehouse.Tel, @"^[1]+[3,5]+\d{9}"))
				//    {
				//        return ErrorCode.TelFormatIsIncorrect;
				//    }
				//}
				//if (!string.IsNullOrEmpty(warehouse.Fax) && !Regex.IsMatch(warehouse.Fax, @"^[+]{0,1}(\d){1,3}[ ]?([-]?((\d)|[ ]){1,12})+$"))
				//{
				//    return ErrorCode.FaxFormatIsIncorrect;
				//}
				WarehouseDal.Update(warehouse);
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
				WarehouseDal.Delete(id);
				return ErrorCode.NoError;
			}
			catch (Exception)
			{
				return ErrorCode.ServerError;
			}
		}

		public List<WarehouseViewVM> GetWarehouseByRange(int from, int to, string warehouseName)
		{
			var func = GetQueryExp(warehouseName);
			var sorts = new List<SortCol> {new SortCol {ColName = "Id", IsDescending = false}};

			var result = WarehouseDal.Query(func, sorts, from, to);
			return result.Select(o => new WarehouseViewVM
			{
				Id = o.Id,
				Name = o.Name,
				Address = o.Address,
				Contact = o.Contact,
				Tel = o.Tel,
				Fax = o.Fax,
				Description = o.Description
			}).ToList();
		}

		public int GetCount(string warehouseName)
		{
			var func = GetQueryExp(warehouseName);
			return WarehouseDal.GetCount(func);
		}

		#region Private Methods

		private Expression<Func<Warehouse, bool>> GetQueryExp(string warehouseName)
		{
			Expression<Func<Warehouse, bool>> func = null;

			if (!string.IsNullOrEmpty(warehouseName))
			{
				func = o => o.Name.Contains(warehouseName);
			}

			return func ?? (o => true);
		}
		#endregion
		#region Validator
		public bool GetNameExisted(string name, int id)
		{
			return !WarehouseDal.GetExisted(c => c.Name == name && c.Id != id);
		}
		public bool ValidateTelFormat(string tel)
		{
			if (!string.IsNullOrEmpty(tel))
			{
				if (!Regex.IsMatch(tel, @"^(\d{3,4}-)?\d{6,8}$") && !Regex.IsMatch(tel, @"^[1]+[3,5]+\d{9}"))
				{
					return false;
				}
			}
			return true;
		}
		public bool ValidateFaxFormat(string fax)
		{
			if (!string.IsNullOrEmpty(fax) && !Regex.IsMatch(fax, @"^[+]{0,1}(\d){1,3}[ ]?([-]?((\d)|[ ]){1,12})+$"))
			{
				return false;
			}
			return true;
		}
		#endregion
		#endregion
	}
}
