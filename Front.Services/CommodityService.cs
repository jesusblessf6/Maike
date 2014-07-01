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
    public class CommodityService
    {
        #region Properties

        private CommodityDAL _commodityDal;
        public CommodityDAL CommodityDal
        {
            get { return _commodityDal ?? (_commodityDal = new CommodityDAL()); }
            set { _commodityDal = value; }
        }

        #endregion
        //获取与当前登录用户相关联的金属
        public List<CommodityVM> GetCommodityByUserId(int currentUserId)
        {
            var relUserCommodityDal = new RelUserCommodityDAL();
            List<RelUserCommodity> relUserCommodityList = relUserCommodityDal.Query(c => c.UserId == currentUserId, new List<string> { "Commodity" });
            List<Commodity> commodityList = relUserCommodityList.Select(c => c.Commodity).Where(o => !o.IsDeleted).ToList();
            return commodityList.Select(c => new CommodityVM
            {
                Id = c.Id,
                Name = c.Name,
                Code = c.Code,
                Description = c.Description,
                IsOpen = c.IsOpen
            }).ToList();
        }

        public List<CommodityVM> SearchCommodities(int currentUserId, string nameTerm)
        {
            List<CommodityVM> vmList = GetCommodityByUserId(currentUserId);
            if (vmList != null && vmList.Count > 0)
            {
                var comms = vmList.Where(o => o.Name.Contains(nameTerm));
                return comms.ToList();
            }
            else
            {
                return new List<CommodityVM>();
            }
        }
    }
}
