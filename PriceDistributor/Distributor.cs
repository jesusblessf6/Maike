using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Text;
using System.Collections.Concurrent;
using Entities;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Threading;


namespace PriceDistributor
{
    public class PriceMessage
    {
        public string CommodityCode { get; set; }
        public string SHFECode { get; set; }
        public double? NewPrice { get; set; }
        public string Name { get; set; }
        public double? ChangePercent { get; set; }
        public double? ChangeAmount { get; set; }
        public double? BuyPrice { get; set; }
        public double? SellPrice { get; set; }
        public double? SellVolumn { get; set; }
        public double? BuyVolumn { get; set; }
    }

    public class Distributor
    {
        // Singleton instance
        private readonly static Lazy<Distributor> _instance = new Lazy<Distributor>(() => new Distributor(GlobalHost.ConnectionManager.GetHubContext<DistributorHub>().Clients));
        private readonly object _updateStockPricesLock = new object();
        private ConcurrentStack<SHFECode> _inUseCodes = new ConcurrentStack<SHFECode>();
        private volatile bool _updatingStockPrices = false;

        private Distributor(IHubConnectionContext clients)
        {
            Clients = clients;
            FillInUseCodes();
            var t = new Task(GetPrice);
            t.Start();
        }

        public void UpdateSHFECodes()
        {
            lock (_updateStockPricesLock)
            {
                if (!_updatingStockPrices)
                {
                    _updatingStockPrices = true;
                    FillInUseCodes();
                    RequireAllPrices();
                    _updatingStockPrices = false;
                }
            }
        }

        public static Distributor Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        private IHubConnectionContext Clients
        {
            get;
            set;
        }

        public void RequireAllPrices()
        {
            const int buffSize = 113;
            var r = new TcpClient();
            bool isSuccess = false;
            while (isSuccess == false)
            {
                try
                {
                    while (!r.Connected)
                    {
                        r.Connect("172.20.70.172", 7101);
                    }
                    NetworkStream write = r.GetStream();
                    if (write.CanWrite)
                    {
                        var bytes = new byte[buffSize];
                        bytes[0] = Encoding.ASCII.GetBytes("C")[0];
                        write.Write(bytes, 0, buffSize);
                        write.Close();
                        r.Close();
                        isSuccess = true;
                    }
                }
                catch (Exception)
                {
                    r.Close();
                    r = new TcpClient(); 
                }
            }
            
        }

        public void GetPrice()
        {
            const int buffSize = 113;
            var c = new TcpClient();
            while (true)
            {
                try
                {
                    while (!c.Connected)
                    {
                        c.Connect("172.20.70.172", 7101);
                    }

                    NetworkStream netStream = c.GetStream();
                    if (netStream.CanRead)
                    {
                        var bytes = new byte[buffSize];
                        netStream.Read(bytes, 0, buffSize);
                        string returndata = Encoding.ASCII.GetString(bytes);
                        Console.WriteLine(returndata.Trim());
                        var dataParts = returndata.Split(new[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                        var product = dataParts[1];
                        SHFECode co = _inUseCodes.Where(o => o.Code.Trim() == product).FirstOrDefault();

                        if (co != null)
                        {
                            var result = new PriceMessage();
                            result.SHFECode = co.Code;
                            result.CommodityCode = co.Commodity.Code;
                            result.Name = co.Name;

                            for (int i = 2; i < dataParts.Length; i++)
                            {
                                var pair = dataParts[i].Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                                if (pair.Length == 2 && !string.IsNullOrWhiteSpace(pair[1]))
                                {
                                    double tmpdb;
                                    switch (pair[0])
                                    {
                                        case "1":
                                            if (double.TryParse(pair[1], out tmpdb))
                                            {
                                                result.NewPrice = tmpdb;
                                            }
                                            break;

                                        case "2":
                                            if (double.TryParse(pair[1], out tmpdb))
                                            {
                                                result.BuyPrice = tmpdb;
                                            }
                                            break;

                                        case "3":
                                            if (double.TryParse(pair[1], out tmpdb))
                                            {
                                                result.BuyVolumn = tmpdb;
                                            }
                                            break;

                                        case "4":
                                            if (double.TryParse(pair[1], out tmpdb))
                                            {
                                                result.SellPrice = tmpdb;
                                            }
                                            break;

                                        case "5":
                                            if (double.TryParse(pair[1], out tmpdb))
                                            {
                                                result.SellVolumn = tmpdb;
                                            }
                                            break;
                                        case "7":
                                            if (double.TryParse(pair[1], out tmpdb))
                                            {
                                                result.ChangePercent = tmpdb;
                                            }
                                            break;
                                        case "8":
                                            if (double.TryParse(pair[1], out tmpdb))
                                            {
                                                result.ChangeAmount = tmpdb;
                                            }
                                            break;
                                    }
                                }
                            }
                            Clients.All.updatePrice(result);
                        }
                    }
                }
                catch (Exception)
                {
                    c.Close();
                    c = new TcpClient();
                }
            }
        }

        private List<SHFECode> GetInUseCode()
        {
            using (MaikeEntities ctx = new MaikeEntities())
            {
                return ctx.SHFECodes.Include("Commodity").Where(s => s.IsInUse == true && s.IsDeleted == false).ToList();
            }
        }

        private void FillInUseCodes()
        {
            using (MaikeEntities ctx = new MaikeEntities())
            {
                List<SHFECode> codes = ctx.SHFECodes.Include("Commodity").Where(c => c.IsInUse == true && c.IsDeleted == false).ToList();
                foreach (SHFECode code in codes)
                {
                    _inUseCodes.Push(code);
                }
            }
        }
    }
}