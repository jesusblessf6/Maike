using System;

namespace Management.Models
{
    public class StockUpdatePriceVM
    {
        public int Id { get; set; }
        private decimal price;

        public decimal Price {
            get {
                return Math.Round(price,4);
            }
            set {
                price = value;
            } 
        }
    }
}
