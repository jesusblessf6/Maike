using System.ComponentModel.DataAnnotations;

namespace Management.Models
{
    public class WarehouseEditVM
    {
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "仓库名字不能为空")]
        public string Name { get; set; }

        public string Address { get; set; }

        public string Contact { get; set; }

        public string Tel { get; set; }

        public string Fax { get; set; }

        public string Description { get; set; }
    }
}
