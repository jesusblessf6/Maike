using System;
using System.Collections.Generic;
using System.Linq;
using DAL;
using Entities;
using Management.Models;
using Util;

namespace Management.Services
{
    public class MarketManagementService : BaseService
    {
        #region Properties
        private MarketManagementDAL _marketManagementDal;
        public MarketManagementDAL MarketManagementDal
        {
            get { return _marketManagementDal ?? (_marketManagementDal = new MarketManagementDAL()); }
            set { _marketManagementDal = value; }
        }
        #endregion

        #region Method
        public List<MarketManagementVM> GetAllSHFECodeGroup()
        {
            var mmClassList = new List<MarketManagementVM>();
            List<SHFECode> markets = MarketManagementDal.GetAll(new List<string> { "Commodity"});
            IEnumerable<IGrouping<int?, SHFECode>> groupList = markets.GroupBy(c => c.CommodityId).ToList();
            foreach(IGrouping<int?, SHFECode> group in groupList)
            {
                List<SHFECode> shfeCodeList = group.ToList();
                var mmClass = new MarketManagementVM {CommodityName = shfeCodeList[0].Commodity.Name};
	            foreach(SHFECode code in shfeCodeList)
               {
                   var sce = new ShfeCodeEntity
	                             {
		                             Id = code.Id,
		                             IsInUse = code.IsInUse,
		                             Name = code.Name,
		                             Code = code.Code
	                             };
	               mmClass.SHFECodeList.Add(sce);
               }
               mmClassList.Add(mmClass);
            }

            return mmClassList;
        }

	    /// <summary>
	    /// 更新
	    /// </summary>
	    /// <param name="shfeCodeId"></param>
	    /// <returns></returns>
	    public ErrorCode UpdateMarketManagement(int shfeCodeId)
        {
            try
            {
                SHFECode shfeCode = MarketManagementDal.GetById(shfeCodeId, null);
                SHFECode oldIsInUsedcode = MarketManagementDal.GetAll().FirstOrDefault(c => c.CommodityId == shfeCode.CommodityId && c.IsInUse.HasValue && c.IsInUse.Value);
                if(oldIsInUsedcode != null)
                {
                    oldIsInUsedcode.IsInUse = false;
                    MarketManagementDal.Update(oldIsInUsedcode);
                }
                shfeCode.IsInUse = true;
                MarketManagementDal.Update(shfeCode);
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
