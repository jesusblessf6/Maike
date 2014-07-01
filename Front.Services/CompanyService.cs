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
    public class CompanyService
    {
        #region Properties

        private CompanyDAL _companyDal;
        public CompanyDAL CompanyDal
        {
            get { return _companyDal ?? (_companyDal = new CompanyDAL()); }
            set { _companyDal = value; }
        }

        #endregion

       

        public List<CompanyVM> SearchAllCompanyByName(string name, int companyType)
        {
            List<CompanyVM> vmList = GetAllCompany(companyType);
            if (vmList != null && vmList.Count > 0)
            {
                var companyList = vmList.Where(c => c.Name.Contains(name)).ToList();
                return companyList.ToList();
            }
            else
            {
                return new List<CompanyVM>();
            }
        }
        //获取跟当前登录用户关联的Company
        public List<CompanyVM> GetCompanyByRange(int from, int to, string key, int customerType)
        {
            List<SortCol> sorts = new List<SortCol>();
            sorts.Add(new SortCol { ColName = "Id", IsDescending = false });
            List<Company> res = null;
            if (string.IsNullOrWhiteSpace(key))
            {
                res = CompanyDal.Query(o => o.Type == customerType, sorts, from, to, null);
            }
            else
            {
                res = CompanyDal.Query(o => o.Type == customerType && o.FullName.Contains(key), sorts, from, to, null);
            }
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
            else
            {
                return CompanyDal.GetCount(o => o.Type == customerType && o.FullName.Contains(key));
            }
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

        //type =1 根据当前登录用户查询出关联的客户公司
        public List<CompanyVM> GetCompanyByRelUserId(int currentUserId)
        {
            var relUserCompanyDal = new RelUserCompanyDAL();
            var relUserCompanyList = relUserCompanyDal.Query(c => c.UserId == currentUserId, new List<string> { "Company" });
            List<Company> companyList = relUserCompanyList.Select(c => c.Company).Where(o => !o.IsDeleted && o.Type == 1).ToList();
            return companyList.Select(c => new CompanyVM { 
             Id = c.Id,
             Address = c.Address,
             Comment = c.Comment,
             FullName = c.FullName,
             Name = c.Name,
             Type= c.Type,
             Zip = c.Zip
            }).ToList();
        }

        public List<CompanyVM> SearchCompanyByName(int currentUserId, string name)
        {
            List<CompanyVM> vmList = GetCompanyByRelUserId(currentUserId);
            if (vmList != null && vmList.Count > 0)
            {
                var companyList = vmList.Where(c => c.Name.Contains(name)).ToList();
                return companyList.ToList();
            }
            else
            {
                return new List<CompanyVM>();
            }
        }

        public CompanyVM GetCompanyById(int id)
        {
            var company = CompanyDal.GetById(id,null);
            return new CompanyVM { 
             Id = company.Id,
             Address = company.Address,
             Comment = company.Comment,
             FullName = company.FullName,
             Name = company.Name,
             Type = company.Type,
             Zip = company.Zip
            };
        }
    }
}
