using DAL;
using Entities;
using Front.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Util.Query;

namespace Front.Services
{
    public class BrandService
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
        public List<BrandVM> GetBrandByCommodity(int commodityId, int commodityTypeId)
        {
            var func1 = GetQueryExp(commodityId, commodityTypeId);
            var result = BrandDal.Query(func1, new List<string> { });
            return result.Select(o => new BrandVM
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
        #endregion
    }
}
