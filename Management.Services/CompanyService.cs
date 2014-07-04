using DAL;
using Entities;
using Enums;
using Management.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Util;
using Util.Query;

namespace Management.Services
{
	public class CompanyService : BaseService
	{
		#region Properties

		private CompanyDAL _companyDal;
		public CompanyDAL CompanyDal
		{
			get { return _companyDal ?? (_companyDal = new CompanyDAL()); }
			set { _companyDal = value; }
		}

		private UserDAL _userDal;
		public UserDAL UserDAL
		{
			get { return _userDal ?? (_userDal = new UserDAL()); }
			set { _userDal = value; }
		}

		#endregion

		#region Methods

		public List<CompanyVM> GetCompanyByRange(int from, int to, string key, int customerType)
		{
			var sorts = new List<SortCol> {new SortCol {ColName = "Id", IsDescending = false}};
			List<Company> res = string.IsNullOrWhiteSpace(key)
				                    ? CompanyDal.Query(o => o.Type == customerType, sorts, @from, to)
				                    : CompanyDal.Query(o => o.Type == customerType && o.FullName.Contains(key), sorts, @from, to);
			return res.Select(o => new CompanyVM
			{
				Id = o.Id,
				Name = o.Name,
				FullName = o.FullName,
				Type = o.Type,
				Zip = o.Zip,
				Address = o.Address,
				Comment = o.Comment
			}).ToList();
		}

		public int GetAllCount(int customerType, string key)
		{
			if (string.IsNullOrWhiteSpace(key))
			{
				return CompanyDal.GetCount(o => o.Type == customerType);
			}
			
			return CompanyDal.GetCount(o => o.Type == customerType && o.FullName.Contains(key));
		}

		public List<Company> GetAllCompanyByUser(int customerType, int useId)
		{
			User user = UserDAL.GetById(useId, new List<string> { "RelUserCompanies.Company" });
			var companyList = new List<Company>();
			foreach (var relCompany in user.RelUserCompanies)
			{
				if (!relCompany.IsDeleted && relCompany.Company.Type == customerType)
				{
					companyList.Add(relCompany.Company);
				}
			}
			return companyList;
		}

		public List<CompanyVM> GetAllCompanyVMByUser(int customerType, int userId)
		{
			User user = UserDAL.GetById(userId, new List<string> { "RelUserCompanies.Company" });
			var companyList = new List<Company>();
			foreach (var relCompany in user.RelUserCompanies)
			{
				if (!relCompany.IsDeleted && relCompany.Company.Type == customerType)
				{
					companyList.Add(relCompany.Company);
				}
			}
			return companyList.Select(o => new CompanyVM
			{
				Id = o.Id,
				Name = o.Name
			}).ToList();
		}

		private Expression<Func<Company, bool>> GetQueryExp(string key, int customerType, int userId)
		{
			var clauses = new List<Clause>
				              {
					              new Clause
						              {
							              PropertyName = "Type",
							              Operator = Operator.Eq,
							              Value = customerType
						              }
				              };

			if (!string.IsNullOrWhiteSpace(key))
			{
				clauses.Add(new Clause
				{
					PropertyName = "FullName",
					Operator = Operator.Like,
					Value = key
				});
			}

			List<Company> companies = GetAllCompanyByUser(customerType, userId);
            var manager = new QueryManager<Company>();
            if (customerType == (int)CustomerType.Internal)
            {
                List<int> list = companies.Select(u => u.Id).ToList();
                return manager.Compile(clauses, o => list.Contains(o.Id));
            }
            else
            {
                return manager.Compile(clauses);
            }
		}

		public List<CompanyVM> GetCompanyByRangeByUser(int from, int to, string key, int customerType, int userId)
		{

			var func1 = GetQueryExp(key, customerType, userId);

            var sorts = new List<SortCol> { new SortCol { ColName = "Name", IsDescending = false } };
			List<Company> res = CompanyDal.Query(func1, sorts, @from, to);
			return res.Select(o => new CompanyVM
			{
				Id = o.Id,
				Name = o.Name,
				FullName = o.FullName,
				Type = o.Type,
				Zip = o.Zip,
				Address = o.Address,
				Comment = o.Comment
			}).ToList();
		}

		public int GetAllCountByUser(int customerType, string key, int userId)
		{
			var func1 = GetQueryExp(key, customerType, userId);
			return CompanyDal.GetCount(func1);
		}

		public List<CompanyVM> GetAllCompany(int type)
		{
			var list = CompanyDal.Query(o => o.Type == type).Select(o => new CompanyVM
			{
				Id = o.Id,
				Name = o.Name
			}).ToList();
			return list;
		}

		public CompanyVM GetById(int id)
		{
			Company company = CompanyDal.GetById(id, null);
			if (company != null)
			{
				var res = new CompanyVM
					          {
						          Id = company.Id,
						          Name = company.Name,
						          FullName = company.FullName,
						          Address = company.Address,
						          Zip = company.Zip,
						          Comment = company.Comment,
						          Type = company.Type
					          };
				return res;
			}
			
			return null;
		}

		public ErrorCode Create(CompanyVM vm)
		{
			var company = new Company
			{
				Name = vm.Name.Trim(),
                FullName = vm.FullName.Trim(),
                Address = vm.Address.Trim(),
				Type = vm.Type,
                Comment = vm.Comment.Trim(),
                Zip = vm.Zip.Trim()
			};

			try
			{
				if (CompanyDal.GetExisted(o => o.Name == vm.Name && o.FullName == vm.FullName))
				{
					return ErrorCode.CommodityTypeExisted;
				}
				CompanyDal.Create(company);
				return ErrorCode.NoError;
			}
			catch (Exception)
			{
				return ErrorCode.ServerError;
			}
		}

		public ErrorCode Update(CompanyVM vm)
		{
			var company = new Company
			{
				Id = vm.Id,
                Name = vm.Name.Trim(),
                FullName = vm.FullName.Trim(),
                Address = vm.Address.Trim(),
				Type = vm.Type,
                Comment = vm.Comment.Trim(),
                Zip = vm.Zip.Trim()
			};

			try
			{
				if (
					CompanyDal.GetExisted(
						o => o.Id != company.Id && o.Name == company.Name && o.FullName == company.FullName))
				{
					return ErrorCode.CommodityTypeExisted;
				}
				CompanyDal.Update(company);
				return ErrorCode.NoError;
			}
			catch (Exception)
			{
				return ErrorCode.ServerError;
			}
		}

		public ErrorCode Delete(int id)
		{
			try
			{
				var company = CompanyDal.GetById(id, new List<string> { "RelUserCompanies" });
				if (company.RelUserCompanies.Count(o => !o.IsDeleted) > 0)
				{
					return ErrorCode.UserConnectCompany;
				}
				CompanyDal.Delete(id);
				return ErrorCode.NoError;
			}
			catch (Exception)
			{
				return ErrorCode.ServerError;
			}
		}

		public bool NameExisted(string name, int type, int id)
		{
            //return !CompanyDal.GetExisted(o => o.Id != id && o.Name == name && o.Type == type);
            return !CompanyDal.GetExisted(o => o.Id != id && o.Name == name.Trim());
		}

		public bool FullNameExisted(string name, int type, int id)
		{
            //return !CompanyDal.GetExisted(o => o.Id != id && o.FullName == name && o.Type == type);
            return !CompanyDal.GetExisted(o => o.Id != id && o.FullName == name.Trim());
		}
		#endregion
	}
}
