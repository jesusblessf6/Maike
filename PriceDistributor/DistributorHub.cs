using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Entities;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Enums;
using Util;

namespace PriceDistributor
{
    [HubName("distributorHub")]
    public class DistributorHub : Hub
    {
        #region 系统事件
        public override Task OnConnected()
        {
            RequireAllPrices();
            return base.OnConnected();
        }

        public override Task OnReconnected()
        {
            RequireAllPrices();
            return base.OnReconnected();
        }

        #endregion

        private readonly Distributor _distributor;

        public DistributorHub() : this(Distributor.Instance) { }

        public DistributorHub(Distributor distributor)
        {
            _distributor = distributor;
        }

        public void RequireAllPrices()
        {
            _distributor.RequireAllPrices();
        }

        public void RefreshSHFECodes(string shfeCodeId)
        {
            _distributor.UpdateSHFECodes();
            int id = Convert.ToInt32(shfeCodeId);
            using (MaikeEntities ctx = new MaikeEntities())
            {
                SHFECode code = ctx.SHFECodes.Include("Commodity").Where(c => c.Id == id && c.IsDeleted == false).FirstOrDefault();
                if (code != null)
                {
                    Clients.All.updateSHFECodes(code.Commodity.Code, code.Code, code.Name);
                }
            }
        }


        /// <summary>
        /// 后台修改库存的升贴水
        /// </summary>
        public void Backend_UpdatePremium(int id, decimal newPremium, decimal oldPremium)
        {
            Clients.All.updatePremium(id, newPremium);
        }

        /// <summary>
        /// 后台修改库存，delta是增量
        /// </summary>
        /// <param name="id"></param>
        /// <param name="delta"></param>
        public void Backend_UpdateStock(int id, decimal delta)
        {
            Clients.All.updateBackendStock(id, delta);
        }

        /// <summary>
        /// 后台库存清0
        /// </summary>
        /// <param name="id"></param>
        public void Backend_ClearStock(int id)
        {
            Clients.All.clearStock(id);
        }

        /// <summary>
        /// 后台修改固定价
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newPrice"></param>
        public void Backend_UpdatePrice(int id, decimal newPrice, decimal oldPrice)
        {
            Clients.All.updateStockPrice(id, newPrice);
        }

        /// <summary>
        /// 前台下单，造成库存的修改
        /// </summary>
        /// <param name="id"></param>
        /// <param name="delta"></param>
        public void Frontend_UpdateStock(int id, decimal delta)
        {
            Clients.All.updateFrontendStock(id, delta);
        }

        /// <summary>
        /// 增加订单的提醒
        /// </summary>
        public void Backend_PlusSalesOrder(string purchase, string sales, string quantity, string commType, string brand, string warehouse, string price)
        {
            string alert = purchase + "从" + sales + "采购" + quantity + "吨" + brand + commType + "；价格" + price + "；仓库" + warehouse;
            Clients.All.backendAddAlert(alert); //仅后台实现该函数
        }

        /// <summary>
        /// 后台增加库存
        /// </summary>
        public void Backend_AddStock()
        {
            Clients.All.backendAddStock();
        }

        /// <summary>
        /// 广播提醒
        /// </summary>
        /// <param name="alert"></param>
        public void All_AddAlert(string alert)
        {
            Clients.All.addAlert(alert);
        }

        public void Backend_SalesOrderStatusChanged(int id, int after)
        {
            string des = EnumHelper.GetDescription<SalesOrderStatus>((SalesOrderStatus)after);
            string[] desTmp = des.Split(new char[] { ',' });
            string status = string.Empty;
            string btn = string.Empty;
            if (desTmp.Length <= 1)
            {
                status = desTmp[0];
            }
            else
            {
                status = desTmp[0];
                btn = desTmp[1];
            }

            Clients.All.onSalesOrderStatusChanged(id, status, btn);
        }

        public void Backend_CancelSalesOrder(int id)
        {
            string des = EnumHelper.GetDescription<SalesOrderStatus>(SalesOrderStatus.OrderCancelled);
            Clients.All.cancelSalesOrder(id, des);
        }
    }
}