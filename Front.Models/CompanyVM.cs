using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Front.Models
{
    public class CompanyVM
    {
        public int Id { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "公司简称不能为空！")]
        public string Name { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "公司全称不能为空！")]
        public string FullName { get; set; }
        public string Address { get; set; }
        public string Zip { get; set; }
        public string Comment { get; set; }
        public int Type { get; set; }
    }
}
