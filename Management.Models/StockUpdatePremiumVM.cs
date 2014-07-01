using System;

namespace Management.Models
{
    public class StockUpdatePremiumVM
    {
        public int Id { get; set; }
        private decimal premium;
        public decimal Premium {
            get{
                return Math.Round(premium, 4);
            }

            set {
                premium = value; 
            }
        }
    }
}
