using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Front.Models;
using Entities;
using Util;
using DAL;

namespace Front.Services
{
    public class UserService
    {
        private UserDAL _userDal;
        public UserDAL UserDAL
        {
            get { return _userDal ?? (_userDal = new UserDAL()); }
            set { _userDal = value; }
        }

        //用于重置密码的函数
        public ErrorCode ResetPassWord(ResetPasswordVM uvm)
        {
            try
            {
                if (uvm.ConfirmPsw != uvm.NewPsw)
                    return ErrorCode.PasswordNotMatch;
                int p_len = uvm.NewPsw.Length;
                if ((p_len < 6) || (p_len > 30))
                    return ErrorCode.PasswordFormatError;

                User user = UserDAL.GetById(uvm.UserId, null);
                user.Password = Encrypt.Encode(uvm.NewPsw);
                UserDAL.Update(user);
                return ErrorCode.NoError;
            }
            catch (Exception)
            {
                return ErrorCode.ServerError;
            }
        }

        public bool PasswordIsValid(string strpsw, int id)
        {
            //return !UserDAL.GetExisted(o => o.Id == id && o.Password == Encrypt.Encode(strpsw));
            User user = UserDAL.GetById(id, null);
            if (user.Password == Encrypt.Encode(strpsw))
                return true;
            else
                return false;
        }
    }
}
