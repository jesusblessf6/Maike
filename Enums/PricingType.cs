using System.ComponentModel;

namespace Enums
{
    public enum PricingType
    {
        [Description("升贴水定价")]
        Premium = 1,
        [Description("固定价定价")]
        Fixed = 2
    }
}
