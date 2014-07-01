using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DAL;
using Entities;
using Management.Models;
using Util;
using Action = Entities.Action;
using Controller = Entities.Controller;

namespace Management.Services
{
	public class ControllerActionService : BaseService
	{
		#region Properties

		private ControllerDAL _controllerDal;

		public ControllerDAL ControllerDal
		{
			get { return _controllerDal ?? (_controllerDal = new ControllerDAL()); }
			set { _controllerDal = value; }
		}

		private ActionDAL _actionDal;

		public ActionDAL ActionDal
		{
			get { return _actionDal ?? (_actionDal = new ActionDAL()); }
			set { _actionDal = value; }
		}

		#endregion

		#region Methods

		public List<Dictionary<string, object>> GetControllersByRange(int from, int to)
		{
			var controllers = ControllerDal.Query(null, new List<SortCol> {new SortCol {ColName = "Id", IsDescending = false}},
			                                      from, to,
			                                      new List<string> {"Actions"});

			int i = 1;
			var result = new List<Dictionary<string, object>>();
			foreach (var controller in controllers)
			{
				var cdtoDic = new Dictionary<string, object>
					              {
						              {"internalId", controller.Id},
						              {"id", i ++},
						              {"name", controller.ChineseName},
						              {"internalName", controller.Name},
						              {"description", controller.Description},
						              {"isOpenForAll", controller.ForAll ? "是" : "否"},
						              {"type", "Controller"}
					              };

				if (controller.Actions != null && controller.Actions.Any(o => !o.IsDeleted))
				{
					cdtoDic.Add("state", "closed");
					var children = controller.Actions.Where(o => !o.IsDeleted).Select(act => new Dictionary<string, object>
						                                                {
							                                                {"internalId", act.Id},
							                                                {"id", i ++},
							                                                {"name", act.ChineseName},
							                                                {"internalName", act.Name},
							                                                {"description", act.Description},
							                                                {"isOpenForAll", act.ForAll ? "是" : "否"},
							                                                {"type", "Action"}
						                                                }).ToList();

					cdtoDic.Add("children", children);
				}

				result.Add(cdtoDic);
			}

			return result;
		}

		public int GetControllerCount()
		{
			return ControllerDal.GetAllCount();
		}

		public ErrorCode CreateController(ControllerEditVM vm)
		{
			if (ControllerDal.GetExisted(o => o.Name == vm.Name))
			{
				return ErrorCode.ControllerNameExisted;
			}

			try
			{
				var c = new Controller
					        {
						        Name = vm.Name,
						        ChineseName = vm.ChineseName,
						        Description = vm.Description,
						        ForAll = vm.IsOpenForAll
					        };

				ControllerDal.Create(c);
			}
			catch (Exception)
			{
				return ErrorCode.ServerError;
			}

			return ErrorCode.NoError;
		}

		public SelectList GetAllControllerAsSelectList()
		{
			var cons = ControllerDal.GetAll().ToList();
			return new SelectList(cons, "Id", "ChineseName");
		}

		public ErrorCode CreateAction(ActionEditVM vm)
		{
			if (ActionDal.GetExisted(o => o.Name == vm.Name && o.ControllerId == vm.ControllerId))
			{
				return ErrorCode.ActionNameExisted;
			}

			try
			{
				var c = new Action
				{
					Name = vm.Name,
					ChineseName = vm.ChineseName,
					Description = vm.Description,
					ForAll = vm.IsOpenForAll,
					ControllerId = vm.ControllerId
				};

				ActionDal.Create(c);
			}
			catch (Exception)
			{
				return ErrorCode.ServerError;
			}

			return ErrorCode.NoError;
		}

		public ControllerEditVM GetControllerById(int id)
		{
			var controller = ControllerDal.GetById(id, null);
			return new ControllerEditVM
				       {
					       ChineseName = controller.ChineseName,
						   Description = controller.Description,
						   Id = controller.Id,
						   IsOpenForAll = controller.ForAll,
						   Name = controller.Name
				       };
		}

		public ErrorCode UpdateController(ControllerEditVM vm)
		{
			if (ControllerDal.GetExisted(o => o.Name == vm.Name && o.Id != vm.Id))
			{
				return ErrorCode.ControllerNameExisted;
			}

			try
			{
				var c = new Controller
				{
					Name = vm.Name,
					ChineseName = vm.ChineseName,
					Description = vm.Description,
					ForAll = vm.IsOpenForAll,
					Id = vm.Id
				};

				ControllerDal.Update(c);
			}
			catch (Exception)
			{
				return ErrorCode.ServerError;
			}

			return ErrorCode.NoError;
		}

		public ErrorCode DeleteController(int id)
		{
			try
			{
				ControllerDal.Delete(id);
			}
			catch (Exception)
			{
				return ErrorCode.ServerError;
			}

			return ErrorCode.NoError;
		}

		public ActionEditVM GetActionById(int id)
		{
			var action = ActionDal.GetById(id, null);
			return new ActionEditVM
				       {
					       Id = action.Id,
						   ChineseName = action.ChineseName,
						   ControllerId = action.ControllerId,
						   Description = action.Description,
						   IsOpenForAll = action.ForAll,
						   Name = action.Name
				       };
		}

		public ErrorCode UpdateAction(ActionEditVM vm)
		{
			if (ActionDal.GetExisted(o => o.Name == vm.Name && o.Id != vm.Id && o.ControllerId == vm.ControllerId))
			{
				return ErrorCode.ActionNameExisted;
			}

			try
			{
				var c = new Action
				{
					Name = vm.Name,
					ChineseName = vm.ChineseName,
					Description = vm.Description,
					ForAll = vm.IsOpenForAll,
					ControllerId = vm.ControllerId,
					Id = vm.Id
				};

				ActionDal.Update(c);
			}
			catch (Exception)
			{
				return ErrorCode.ServerError;
			}

			return ErrorCode.NoError;
		}

		public ErrorCode DeleteAction(int id)
		{
			try
			{
				ActionDal.Delete(id);
			}
			catch (Exception)
			{
				return ErrorCode.ServerError;
			}

			return ErrorCode.NoError;
			
		}

		#endregion

		
	}
}