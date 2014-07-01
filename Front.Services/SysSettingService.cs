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
    public class SysSettingService
    {
        #region Properties

        private SysSettingDAL _sysSettingDal;
        public SysSettingDAL SysSettingDal
        {
            get { return _sysSettingDal ?? (_sysSettingDal = new SysSettingDAL()); }
            set { _sysSettingDal = value; }
        }

        #endregion

        #region Methods

        public SysSettingVM GetSysSetting()
        {
            SysSettingVM sysSettingVM = new SysSettingVM();
            SysSetting sysSetting = SysSettingDal.GetAll().FirstOrDefault();
            if (sysSetting != null)
            {
                sysSettingVM.BuyUnit = sysSetting.BuyUnit;
                sysSettingVM.CountDown = sysSetting.CountDown;
            }
            else
            {
                sysSettingVM.BuyUnit = 10;
                sysSettingVM.CountDown = 5;
            }
            return sysSettingVM;
        }
        #endregion
    }
}
