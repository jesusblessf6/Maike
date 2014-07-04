using System;
using System.Collections.Generic;
using System.Linq;
using DAL;
using Entities;
using Management.Models;
using Util;

namespace Management.Services
{
	public class SwitchMarketManagementService : BaseService
	{
		#region Properties
		private SwitchMarketManagementDAL _switchMarketManagementDal;
		public SwitchMarketManagementDAL SwitchManagementDal
		{
			get { return _switchMarketManagementDal ?? (_switchMarketManagementDal = new SwitchMarketManagementDAL()); }
			set { _switchMarketManagementDal = value; }
		}
		#endregion

		#region Method
		public List<SwitchMarketManagementVM> GetAllOpenTimes()
		{
			List<OpenTime> openTimeList = SwitchManagementDal.GetAll();
			return openTimeList.Select(c => new SwitchMarketManagementVM { 
									Id = c.Id,
									StartTime = c.StartTime.ToString(),
									EndTime = c.EndTime.ToString()
			}).ToList();
		}

		public List<SwitchMarketManagementVM> GetOpenTimeByRange(int from, int to)
		{
            var sorts = new List<SortCol> { new SortCol { ColName = "StartTime", IsDescending = false } };
			var res = SwitchManagementDal.Query(null, sorts, from, to);
			return res.Select(c => new SwitchMarketManagementVM { Id = c.Id, StartTime = c.StartTime.ToString(), EndTime = c.EndTime.ToString()}).ToList();
		}

		public int GetAllCount()
		{
			return SwitchManagementDal.GetAllCount();
		}

		public ErrorCode Create(SwitchMarketManagementEditVM smmEdit)
		{
			TimeSpan startTime;
			TimeSpan endTime;
			if (!TimeSpan.TryParse(smmEdit.StartTime, out startTime))
			{
				return ErrorCode.StartTimeFormatIsIncorrect;
			}
			if(!TimeSpan.TryParse(smmEdit.EndTime, out endTime))
			{
				return ErrorCode.EndTimeFormatIsIncorrect;
			}

			var openTime = new OpenTime
			{
				StartTime = startTime,
				EndTime = endTime
			};

			try
			{
			   //if(SwitchManagementDal.GetExisted(o => (o.EndTime <= endTime && o.StartTime >= startTime) || (o.EndTime >= startTime && o.EndTime <= endTime) || (o.StartTime >= startTime && o.StartTime <= endTime) || (o.StartTime <= startTime && o.EndTime >= endTime)))
			   //{
			   //    return ErrorCode.OpenTimeExisted;
			   //}
			   SwitchManagementDal.Create(openTime);
			   return ErrorCode.NoError;
			}
			catch (Exception)
			{
				return ErrorCode.ServerError;
			}
		}

		public ErrorCode Delete(int id)
		{
			try
			{
				SwitchManagementDal.Delete(id);
				return ErrorCode.NoError;
			}
			catch (Exception)
			{
				return ErrorCode.ServerError;
			}
		}
		#endregion

		#region Validator
		public bool GetOpenTimeExisted(string startTime, string endTime)
		{
			TimeSpan startTimeResult;
			TimeSpan endTimeResult;
			if (!TimeSpan.TryParse(startTime, out startTimeResult))
			{
				return false;
			}
			if(!TimeSpan.TryParse(endTime, out endTimeResult))
			{
				return false;
			}
			return !SwitchManagementDal.GetExisted(o => (o.EndTime <= endTimeResult && o.StartTime >= startTimeResult) || (o.EndTime >= startTimeResult && o.EndTime <= endTimeResult) || (o.StartTime >= startTimeResult && o.StartTime <= endTimeResult) || (o.StartTime <= startTimeResult && o.EndTime >= endTimeResult));
		}
		#endregion
	}
}
