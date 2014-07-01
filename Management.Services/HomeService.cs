using DAL;
using Entities;
using Management.Models;
using System.Collections.Generic;
using System.Linq;

namespace Management.Services
{
    public class HomeService
    {
        #region Properties

        public UserDAL UserDal
        {
            get { return _userDal ?? (_userDal = new UserDAL()); }
            set { _userDal = value; }
        }
        private UserDAL _userDal;

        #endregion

        #region Methods

        public CurrentUserVM GetCurrentUser(int id)
        {
            var user = UserDal.GetById(id, new List<string> { "RelUserCommodities.Commodity.SHFECodes" });

            var userVM = new CurrentUserVM
                       {
                           Id = user.Id,
                           Name = user.Name,
                           LoginName = user.LoginName,
                           Description = user.Description
                       };
            string shfeCodes = "";

            foreach (var relUserCommodity in user.RelUserCommodities)
            {
                if (!relUserCommodity.IsDeleted)
                {
                    Commodity commodity = relUserCommodity.Commodity;
                    string commodityCode = commodity.Code;
                    SHFECode shfeCode = relUserCommodity.Commodity.SHFECodes.FirstOrDefault(o => o.IsInUse ?? false);
                    if (shfeCode != null)
                    {
                        shfeCodes += commodityCode + "," + shfeCode.Code + "," +shfeCode.Name + "||";
                    }
                }
            }
            if (shfeCodes.Length > 0)
            {
                shfeCodes = shfeCodes.Remove(shfeCodes.Length - 2);
            }

            userVM.SHFECodes = shfeCodes;
            return userVM;
        }

        #endregion
    }
}
