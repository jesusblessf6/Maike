using System;
using DAL;
using Entities;
using Management.Models;
using Util;
using Enums;

namespace Management.Services
{
	public class SignService
	{
		#region Properties

		private UserDAL _userDal;
		public UserDAL UserDal
		{
			get { return _userDal ?? (_userDal = new UserDAL()); }
			set { _userDal = value; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Login
		/// </summary>
		/// <param name="loginName"></param>
		/// <param name="password"></param>
		/// <param name="userId"></param>
		/// <returns></returns>
		public ErrorCode Login(string loginName, string password, out int userId)
		{
			userId = 0;
			var user = UserDal.GetUserByLoginName(loginName,(int)CompanyUserType.Self);

			if (user == null)
			{
				return ErrorCode.UserNotExisted;
			}

			if (Encrypt.Encode(password) != user.Password)
			{
				return ErrorCode.PasswordNotCorrect;
			}

			userId = user.Id;
			return ErrorCode.NoError;
		}

		/// <summary>
		/// judge whether there is super user
		/// </summary>
		/// <returns></returns>
		public bool SuperUserExisted()
		{
			return UserDal.GetExisted(o => o.IsSuper);
		}

		/// <summary>
		/// Add super user
		/// </summary>
		/// <param name="super"></param>
		/// <returns></returns>
		public ErrorCode AddSuperUser(SignSuperVM super)
		{
			var user = new User
				           {
					           Description = super.Description,
					           IsSuper = true,
					           LoginName = super.LoginName,
					           Name = super.Name,
					           Password = Encrypt.Encode(super.Password)
				           };
			try
			{
				if (UserDal.GetExisted(o => o.LoginName == super.LoginName))
				{
					return ErrorCode.UserExisted;
				}

				UserDal.Create(user);
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
