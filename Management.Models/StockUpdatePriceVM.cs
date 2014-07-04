using System;

namespace Management.Models
{
    public class StockUpdatePriceVM
    {
        public int Id { get; set; }
        public int PricingType { get; set; }
        private decimal price;

        public decimal Price {
            get {
                return Math.Round(price,2);
            }
            set {
                price = value;
            } 
        }
    }
}
