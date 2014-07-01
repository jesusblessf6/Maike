using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Management.Models
{
    public class UserVM
    {
        public int UserId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "登录名不能为空！")]
        public string LoginName { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string Tel { get; set; }
        public string Fax { get; set; }
        public int Type { get; set; }
        public string Description { get; set; }
        public string SelectCompanyIds { get; set; }
        public string SelectCommodityIds { get; set; }
        public List<CompanyVM> Company { get; set; }
        public int? RoleId { get; set; }
    }
}
