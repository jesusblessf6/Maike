using DAL;
using Entities;
using Front.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Front.Services
{
    public class CommodityTypeService
    {
        #region Properties

        private CommodityTypeDAL _commodityTypeDal;
        public CommodityTypeDAL CommodityTypeDal
        {
            get { return _commodityTypeDal ?? (_commodityTypeDal = new CommodityTypeDAL()); }
            set { _commodityTypeDal = value; }
        }

        #endregion
        #region Method
        public List<CommodityTypeVM> GetCommodityTypeByCommodityId(int commodityId)
        {
            List<SortCol> sorts = new List<SortCol>();
            sorts.Add(new SortCol { ColName = "Id", IsDescending = false });
            var results = CommodityTypeDal.Query(o => o.CommodityId == commodityId, new List<string> { "Commodity" }).ToList();
            return results.Select(o => new CommodityTypeVM
            {
                Id = o.Id,
                Name = o.Name,
                CommodityId = o.CommodityId,
                CommodityName = o.Commodity.Name,
                Description = o.Description
            }).ToList();
        }
        #endregion
    }
}
