using DAL;
using Entities;
using Management.Models;
using System;
using System.Linq;

namespace Management.Services
{
    public class SysSettingService : BaseService
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
            var sysSettingVM = new SysSettingVM();
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

        public bool SaveSysSetting(SysSettingVM sysSetting)
        {
            try
            {
                SysSetting setting = SysSettingDal.GetAll().FirstOrDefault();
                if (setting != null)
                {
                    setting.BuyUnit = sysSetting.BuyUnit;
                    setting.CountDown = sysSetting.CountDown;
                    SysSettingDal.Update(setting);
                }
                else
                {
                    setting = new SysSetting { BuyUnit = sysSetting.BuyUnit, CountDown = sysSetting.CountDown };
                    SysSettingDal.Create(setting);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion
    }
}
