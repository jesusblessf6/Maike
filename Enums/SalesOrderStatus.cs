using System.ComponentModel;

namespace Enums
{
    public enum SalesOrderStatus
    {
        [Description("订单确定，客户下单成功后，直接变成该状态")]
        OrderConfirmed = 1,
        [Description("合同已完成")]
        ContractFinished = 2,
        [Description("付款已完成")]
        PaymentFinished = 3,
        [Description("已发货")]
        DeliveryFinished = 4,
        [Description("订单已完成")]
        OrderFinished = 5,
        [Description("订单已取消")]
        OrderCancelled = 6
    }
}
