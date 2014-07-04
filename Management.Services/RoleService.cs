using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using DAL;
using Entities;
using Enums;
using Management.Models;
using Util;

namespace Management.Services
{
	public class RoleService : BaseService
	{
		#region Properties

		private RoleDAL _roleDal;
		public RoleDAL RoleDal
		{
			get { return _roleDal ?? (_roleDal = new RoleDAL()); }
			set { _roleDal = value; }
		}

		private UserDAL _userDal;
		public UserDAL UserDal
		{
			get { return _userDal ?? (_userDal = new UserDAL()); }
			set { _userDal = value; }
		}

		private PrevilegeDAL _previlegeDal;
		public PrevilegeDAL PrevilegeDal
		{
			get { return _previlegeDal ?? (_previlegeDal = new PrevilegeDAL()); }
			set { _previlegeDal = value; }
		}

		#endregion


		#region Method

		public List<RoleVM> GetRolesByRange(int @from, int to)
		{
			var roles = RoleDal.Query(null, new List<SortCol> {new SortCol {ColName = "Id", IsDescending = false}}, from, to);
			return roles.Select(o => new RoleVM
										 {
										   Id = o.Id,
										   Name = o.Name,
										   Description = o.Description
										 }).ToList();
		}

		public int GetAllCount()
		{
			return RoleDal.GetAllCount();
		}

		public bool NameValidate(string name, int roleId)
		{
			return !RoleDal.GetExisted(o => o.Name == name && o.Id != roleId);
		}

		public ErrorCode Create(RoleVM vm)
		{
			try
			{
				if (RoleDal.GetExisted(o => o.Name == vm.Name))
				{
					return ErrorCode.RoleNameExisted;
				}

				var role = new Role
							   {
								   Name =  vm.Name,
								   Description = vm.Description
							   };
				RoleDal.Create(role);

				return ErrorCode.NoError;
			}
			catch (Exception)
			{
				return ErrorCode.ServerError;
			}
		}

		public RoleVM GetById(int id)
		{
			var role = RoleDal.GetById(id, null);
			return new RoleVM
					   {
						   Name = role.Name,
						   Description = role.Description,
						   Id = role.Id
					   };
		}

		public ErrorCode Update(RoleVM vm)
		{
			try
			{
				if (RoleDal.GetExisted(o => o.Name == vm.Name && o.Id != vm.Id))
				{
					return ErrorCode.RoleNameExisted;
				}

				var role = new Role
							   {
								   Name = vm.Name,
								   Id = vm.Id,
								   Description = vm.Description
							   };
				RoleDal.Update(role);

				return ErrorCode.NoError;
			}
			catch (Exception)
			{
				return ErrorCode.ServerError;
			}
		}

		public ErrorCode Delete(int id)
		{
			using (var ts = new TransactionScope())
			{
				try
				{
					if (UserDal.GetExisted(o => o.RoleId == id))
					{
						return ErrorCode.RoleHasAssigned;
					}

					var previleges = PrevilegeDal.Query(o => o.RoleId == id);
					foreach (var previlege in previleges)
					{
						PrevilegeDal.Delete(previlege.Id);
					}

					RoleDal.Delete(id);

					ts.Complete();
					return ErrorCode.NoError;
				}
				catch (Exception)
				{
					return ErrorCode.ServerError;
				}
				finally
				{
					ts.Dispose();
				}
			}
			
		}

		public List<Dictionary<string, object>> GetControllersWithPermInfo(int id)
		{
			var controllerDal = new ControllerDAL();
			var controllers = controllerDal.Query(o => !o.ForAll, new List<SortCol> {new SortCol {ColName = "Id", IsDescending = false}},
								new List<string> {"Actions"});

			var role = RoleDal.GetById(id, new List<string> {"Previleges"});
			var perms = role.Previleges.Where(o => !o.IsDeleted).ToList();

			var result = new List<Dictionary<string, object>>();
			int i = 1;
			foreach (var cdto in controllers)
			{
				var cdtoDic = new Dictionary<string, object>
								  {
									  {"internalId", cdto.Id},
									  {"id", i ++},
									  {"name", cdto.ChineseName},
									  {"internalName", cdto.Name},
									  {"description", cdto.Description},
									  {"isOpenForAll", cdto.ForAll ? "是" : "否"},
									  {"type", "Controller"},
									  {"ck", perms.Any(o => o.ControllerId == cdto.Id && o.PrevilegeLevel == (int)PrevilegeLevel.ControllerLevel)}
								  };

				if (cdto.Actions != null && cdto.Actions.Count > 0)
				{
					cdtoDic.Add("state", "closed");
					var children = cdto.Actions.Where(o => !o.ForAll).Select(act => new Dictionary<string, object>
																  {
																	  {"internalId", act.Id},
																	  {"id", i ++},
																	  {"name", act.ChineseName},
																	  {"internalName", act.Name},
																	  {"description", act.Description},
																	  {"isOpenForAll", act.ForAll ? "是" : "否"},
																	  {"type", "Action"},
																	  {"ck", perms.Any(o => o.ControllerId == cdto.Id && o.ActionId == act.Id && o.PrevilegeLevel == (int)PrevilegeLevel.ActionLevel)}
																  }).ToList();

					cdtoDic.Add("children", children);
				}

				result.Add(cdtoDic);
			}

			return result;
		}

		public ErrorCode AddPerm(PrevilegeVM perm)
		{
			var previlegeDal = new PrevilegeDAL();
			try
			{
				if (
					previlegeDal.GetExisted(
						o => o.ActionId == perm.ActionId && o.ControllerId == perm.ControllerId && o.RoleId == perm.RoleId))
				{
					return ErrorCode.NoError;
				}

				var previlege = new Previlege
									{
										RoleId = perm.RoleId,
										PrevilegeLevel = (int) perm.PrevilegeLevel,
										ControllerId = perm.ControllerId,
										ActionId = perm.ActionId
									};
				previlegeDal.Create(previlege);
				return ErrorCode.NoError;
			}
			catch (Exception)
			{
				return ErrorCode.ServerError;
			}
		}

		public ErrorCode RemovePerm(PrevilegeVM perm)
		{
			var previlegeDal = new PrevilegeDAL();

			try
			{
				if (perm.PrevilegeLevel == PrevilegeLevel.ActionLevel && (!previlegeDal.GetExisted(
					o =>
					o.ActionId == perm.ActionId && o.ControllerId == perm.ControllerId && o.RoleId == perm.RoleId &&
					o.PrevilegeLevel == (int)PrevilegeLevel.ActionLevel)))
				{
					return ErrorCode.NoError;
				}

				if (perm.PrevilegeLevel == PrevilegeLevel.ControllerLevel && (!previlegeDal.GetExisted(
					o => o.ControllerId == perm.ControllerId && o.RoleId == perm.RoleId &&
					o.PrevilegeLevel == (int)PrevilegeLevel.ControllerLevel)))
				{
					return ErrorCode.NoError;
				}

				var previlege = new Previlege
				{
					RoleId = perm.RoleId,
					PrevilegeLevel = (int)perm.PrevilegeLevel,
					ControllerId = perm.ControllerId,
					ActionId = perm.ActionId
				};

				var existed = new List<Previlege>();
				if (previlege.PrevilegeLevel == (int)PrevilegeLevel.ControllerLevel)
				{
					//existed = QueryForObjs(GetObjQuery<RolePrevilege>(ctx),
					//		 o => o.RoleId == perm.RoleId && o.ControllerId == perm.ControllerId && o.PrevilegeLevel == (int)PrevilegeLevel.ControllerLevel).ToList();
					existed =
						previlegeDal.Query(
							o =>
							o.RoleId == perm.RoleId && o.ControllerId == perm.ControllerId &&
							o.PrevilegeLevel == (int) PrevilegeLevel.ControllerLevel).ToList();
				}
				else if (previlege.PrevilegeLevel == (int)PrevilegeLevel.ActionLevel)
				{
					//existed = QueryForObjs(GetObjQuery<RolePrevilege>(ctx),
					//		 o => o.ActionId == perm.ActionId && o.RoleId == perm.RoleId && o.ControllerId == perm.ControllerId && o.PrevilegeLevel == (int)PrevilegeLevel.ActionLevel).ToList();
					existed =
						previlegeDal.Query(
							o =>
							o.ActionId == perm.ActionId && o.RoleId == perm.RoleId && o.ControllerId == perm.ControllerId &&
							o.PrevilegeLevel == (int) PrevilegeLevel.ActionLevel).ToList();
				}

				foreach (var previlege1 in existed)
				{
					previlegeDal.Delete(previlege1.Id);
				}

				return ErrorCode.NoError;
			}
			catch (Exception)
			{
				return ErrorCode.ServerError;
			}
		}

		public List<RoleVM> GetAllRoles()
		{
			var roles = RoleDal.GetAll();
			return roles.Select(o => new RoleVM
			{
				Id = o.Id,
				Name = o.Name
			}).ToList();
		}

		public List<RoleVM> SearchRoles(string nameTerm)
		{
			var roles = RoleDal.Query(o => o.Name.Contains(nameTerm));
			return roles.Select(o => new RoleVM
			{
				Id = o.Id,
				Name = o.Name
			}).ToList();
		}
		#endregion

		
	}
}
