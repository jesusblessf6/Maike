using DAL;
using Entities;
using Management.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Util;

namespace Management.Services
{
	public class UserService : BaseService
	{
		#region Properties

		private UserDAL _userDal;
		public UserDAL UserDAL
		{
			get { return _userDal ?? (_userDal = new UserDAL()); }
			set { _userDal = value; }
		}

		private CompanyDAL _companyDal;
		public CompanyDAL CompanyDAL
		{
			get { return _companyDal ?? (_companyDal = new CompanyDAL()); }
			set { _companyDal = value; }
		}

		private CommodityDAL _commodityDal;
		public CommodityDAL CommodityDAL
		{
			get { return _commodityDal ?? (_commodityDal = new CommodityDAL()); }
			set { _commodityDal = value; }
		}

		private RelUserCommodityDAL _relUserCommodityDal;
		public RelUserCommodityDAL RelUserCommodityDAL
		{
			get { return _relUserCommodityDal ?? (_relUserCommodityDal = new RelUserCommodityDAL()); }
			set { _relUserCommodityDal = value; }
		}

		private RelUserCompanyDAL _relUserCompanyDal;
		public RelUserCompanyDAL RelUserCompanyDAL
		{
			get { return _relUserCompanyDal ?? (_relUserCompanyDal = new RelUserCompanyDAL()); }
			set { _relUserCompanyDal = value; }
		}

		//todo::外键过滤isdelete
		public List<UserVM> GetUsersByRange(int from, int to, string userName, int companyId, int customerType)
		{
			var sorts = new List<SortCol> {new SortCol {ColName = "Id", IsDescending = false}};
			List<User> res;
			var users = new List<UserVM>();

			if (string.IsNullOrWhiteSpace(userName) && companyId == 0)
			{
				res = UserDAL.Query(o => o.Type == customerType, sorts, from, to, new List<string> { "RelUserCompanies", "RelUserCompanies.Company" });
			}
			else if (string.IsNullOrWhiteSpace(userName) && companyId != 0)
			{
				res = UserDAL.Query(o => o.Type == customerType && o.RelUserCompanies.Where(r=>!r.IsDeleted).Select(u => u.CompanyId).Contains(companyId), sorts, from, to,
					new List<string> { "RelUserCompanies", "RelUserCompanies.Company" });
			}
			else if (!string.IsNullOrWhiteSpace(userName) && companyId == 0)
			{
				res = UserDAL.Query(o => o.Type == customerType && o.Name.Contains(userName), sorts, from, to, new List<string> { "RelUserCompanies", "RelUserCompanies.Company" });
			}
			else
			{
				res = UserDAL.Query(o => o.Type == customerType && o.Name.Contains(userName) && o.RelUserCompanies.Where(r => !r.IsDeleted).Select(u => u.CompanyId).Contains(companyId),
					sorts, from, to, new List<string> { "RelUserCompanies", "RelUserCompanies.Company" });
			}

			for (int i = 0; i < res.Count; i++)
			{
				var user = new UserVM
					           {
						           UserId = res[i].Id,
						           Name = res[i].Name,
						           Type = res[i].Type,
						           LoginName = res[i].LoginName,
						           Mobile = res[i].Mobile,
						           Tel = res[i].Tel,
						           Fax = res[i].Fax,
						           Description = res[i].Description
					           };

				List<RelUserCompany> relUserCompany = res[i].RelUserCompanies.Where(o => o.IsDeleted == false).ToList();
				var company = new List<CompanyVM>();
				for (int j = 0; j < relUserCompany.Count; j++)
				{
					if (!relUserCompany[j].Company.IsDeleted)
					{
						var c = new CompanyVM {Id = relUserCompany[j].Company.Id, Name = relUserCompany[j].Company.Name};
						if (!company.Contains(c))
						{
							company.Add(c);
						}
					}
				}
				user.Company = company;
				users.Add(user);
			}

			return users;
		}

		//todo::增加查询条件 
		public int GetAllCount(int customerType)
		{
			return UserDAL.GetCount(o => o.Type == customerType);
		}

		public int GetUserCount(string userName, int companyId, int customerType)
		{
			int count;
			if (string.IsNullOrWhiteSpace(userName) && companyId == 0)
			{
				count = UserDAL.GetCount(o => o.Type == customerType);
			}
			else if (string.IsNullOrWhiteSpace(userName) && companyId != 0)
			{
				count = UserDAL.GetCount(o => o.Type == customerType && o.RelUserCompanies.Where(r => !r.IsDeleted).Select(u => u.CompanyId).Contains(companyId), new List<string> { "RelUserCompanies", "RelUserCompanies.Company" });
			}
			else if (!string.IsNullOrWhiteSpace(userName) && companyId == 0)
			{
				count = UserDAL.GetCount(o => o.Type == customerType && o.Name.Contains(userName));
			}
			else
			{
				count = UserDAL.GetCount(o => o.Type == customerType && o.Name.Contains(userName) && o.RelUserCompanies.Where(r => !r.IsDeleted).Select(u => u.CompanyId).Contains(companyId),
					new List<string> { "RelUserCompanies", "RelUserCompanies.Company" });
			}
			return count;
		}


		public UserVM GetById(int id)
		{
			var user = UserDAL.GetById(id, new List<string> { "RelUserCompanies", "RelUserCompanies.Company", "RelUserCommodities", "RelUserCommodities.Commodity" });
			var u = new UserVM
				        {
					        UserId = user.Id,
					        LoginName = user.LoginName,
					        Name = user.Name,
					        Mobile = user.Mobile,
					        Tel = user.Tel,
					        Description = user.Description,
					        Fax = user.Fax,
					        Type = user.Type,
					        RoleId = user.RoleId
				        };

			List<RelUserCompany> relUserCompany = user.RelUserCompanies.Where(o => o.IsDeleted == false).ToList();
			var company = new List<CompanyVM>();
			string strCompanyId = "";
			for (int j = 0; j < relUserCompany.Count; j++)
			{
				if (!relUserCompany[j].Company.IsDeleted)
				{
					var c = new CompanyVM {Id = relUserCompany[j].Company.Id, Name = relUserCompany[j].Company.Name};
					if (!company.Contains(c))
					{
						company.Add(c);
						strCompanyId += c.Id + ",";
					}
				}
			}

			if (strCompanyId.Length > 0)
			{
				strCompanyId = strCompanyId.Remove(strCompanyId.Length - 1);
			}

			List<RelUserCommodity> relUserCommodity = user.RelUserCommodities.Where(o => o.IsDeleted == false).ToList();
			var commodity = new List<Commodity>();
			for (int i = 0; i < relUserCommodity.Count; i++)
			{
				if (!relUserCommodity[i].Commodity.IsDeleted)
				{
					commodity.Add(relUserCommodity[i].Commodity);
				}
			}

			string selectCommodity = String.Join(",", commodity.Select(o => o.Id));

			u.Company = company;
			u.SelectCompanyIds = strCompanyId;
			u.SelectCommodityIds = selectCommodity;

			return u;
		}

		public ErrorCode ResetPwd(int userId)
		{
			try
			{
				User user = UserDAL.GetById(userId, null);
				user.Password = Encrypt.Encode("888888");
				UserDAL.Update(user);
				return ErrorCode.NoError;
			}
			catch (Exception)
			{
				return ErrorCode.ServerError;
			}
		}

		public ErrorCode Create(UserVM vm)
		{
			try
			{
				var user = new User
					           {
						           LoginName = vm.LoginName,
						           Name = vm.Name,
						           Mobile = vm.Mobile,
						           Tel = vm.Tel,
						           Fax = vm.Fax,
						           Description = vm.Description,
						           Password = Encrypt.Encode("888888"),
						           Type = vm.Type
					           };
				if (vm.RoleId.HasValue)
				{
					user.RoleId = vm.RoleId.Value;
				}
				string[] company = vm.SelectCompanyIds.Split(new[] { ',' });
				string[] commodity = vm.SelectCommodityIds.Split(new[] { ',' });
				var listCompany = new List<RelUserCompany>();
				var listCommodity = new List<RelUserCommodity>();
				for (int i = 0; i < company.Length; i++)
				{
					var relCompany = new RelUserCompany {CompanyId = int.Parse(company[i])};
					listCompany.Add(relCompany);
				}
				for (int i = 0; i < commodity.Length; i++)
				{
					var relCommodity = new RelUserCommodity {CommodityId = int.Parse(commodity[i])};
					listCommodity.Add(relCommodity);
				}

				user.RelUserCommodities = listCommodity;
				user.RelUserCompanies = listCompany;

				UserDAL.Create(user);
				return ErrorCode.NoError;
			}
			catch (Exception)
			{
				return ErrorCode.ServerError;
			}
		}

		public ErrorCode Update(UserVM vm)
		{
			try
			{
				User user = UserDAL.GetById(vm.UserId, null);
				user.LoginName = vm.LoginName;
				user.Name = vm.Name;
				user.Mobile = vm.Mobile;
				user.Tel = vm.Tel;
				user.Fax = vm.Fax;
				user.Description = vm.Description;
				user.Type = vm.Type;
				if (vm.RoleId.HasValue)
				{
					user.RoleId = vm.RoleId.Value;
				}
				else
				{
					user.RoleId = null;
				}

				user.RelUserCommodities = null;
				user.RelUserCompanies = null;

				UserDAL.Update(user);

				List<RelUserCompany> relCompanyList = RelUserCompanyDAL.Query(o => o.UserId == user.Id).ToList();
				List<RelUserCommodity> relCommodityList = RelUserCommodityDAL.Query(o => o.UserId == user.Id).ToList();
				foreach (var c in relCompanyList)
				{
					RelUserCompanyDAL.Delete(c.Id);
				}

				foreach (var com in relCommodityList)
				{
					RelUserCommodityDAL.Delete(com.Id);
				}

				string[] company = vm.SelectCompanyIds.Split(new[] { ',' });
				string[] commodity = vm.SelectCommodityIds.Split(new[] { ',' });

				for (int i = 0; i < company.Length; i++)
				{
					var relCompany = new RelUserCompany {CompanyId = int.Parse(company[i]), UserId = user.Id};
					RelUserCompanyDAL.Create(relCompany);
				}
				for (int i = 0; i < commodity.Length; i++)
				{
					var relCommodity = new RelUserCommodity {CommodityId = int.Parse(commodity[i]), UserId = user.Id};
					RelUserCommodityDAL.Create(relCommodity);
				}

				return ErrorCode.NoError;
			}
			catch (Exception)
			{
				return ErrorCode.ServerError;
			}
		}

		public bool LoginNameExisted(string name, int id)
		{
			return !UserDAL.GetExisted(o => o.Id != id && o.LoginName == name);
		}

		public ErrorCode Delete(int id)
		{
			try
			{
				List<RelUserCompany> relCompanyList = RelUserCompanyDAL.Query(o => o.UserId == id).ToList();
				List<RelUserCommodity> relCommodityList = RelUserCommodityDAL.Query(o => o.UserId == id).ToList();
				foreach (var c in relCompanyList)
				{
					RelUserCompanyDAL.Delete(c.Id);
				}

				foreach (var com in relCommodityList)
				{
					RelUserCommodityDAL.Delete(com.Id);
				}
				UserDAL.Delete(id);
				return ErrorCode.NoError;
			}
			catch (Exception)
			{
				return ErrorCode.ServerError;
			}
		}

		//用于重置密码的函数
		public ErrorCode ResetPassWord(ResetPasswordVM uvm)
		{
			try
			{
				if (uvm.ConfirmPsw != uvm.NewPsw)
					return ErrorCode.PasswordNotMatch;
				int pLen = uvm.NewPsw.Length;
				if ( (pLen < 6) || (pLen >30 ) )
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
			User user = UserDAL.GetById(id, null);
			if (user.Password == Encrypt.Encode(strpsw))
				return true;
			
			return false;
		}

		#endregion
	}
}
