using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Front.Models
{
    public class CommodityTypeVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CommodityId { get; set; }
        public string CommodityName { get; set; }
        public string Description { get; set; }
    }
}
