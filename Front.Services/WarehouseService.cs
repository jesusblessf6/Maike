using DAL;
using Entities;
using Front.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Front.Services
{
    public class WarehouseService
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
        public List<WarehouseVM> GetWarehouseByRange(int from, int to, string warehouseName)
        {
            var func = GetQueryExp(warehouseName);
            List<SortCol> sorts = new List<SortCol>();
            sorts.Add(new SortCol { ColName = "Id", IsDescending = false });

            var result = WarehouseDal.Query(func, sorts, from, to, null);
            return result.Select(o => new WarehouseVM
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

        private Expression<Func<Warehouse, bool>> GetQueryExp(string warehouseName)
        {
            Expression<Func<Warehouse, bool>> func = null;

            if (!string.IsNullOrEmpty(warehouseName))
            {
                func = o => o.Name.Contains(warehouseName);
            }

            if (func == null)
            {
                func = o => 1 == 1;
            }

            return func;
        }
        #endregion
    }
}
