using System.ComponentModel;

namespace Util
{
	public enum ErrorCode
	{
		[Description("操作成功！")]
		NoError = 0,

		[Description("输入有误！")]
		InputError = 1,

		[Description("用户不存在！")]
		UserNotExisted = 2,

		[Description("密码不正确！")]
		PasswordNotCorrect = 3,

		[Description("服务器错误！")]
		ServerError = 4,

		[Description("该用户已存在！")]
		UserExisted = 5,

		[Description("该金属类型已存在！")]
		CommodityTypeExisted = 6,

		[Description("开始时间格式输入不正确")]
		StartTimeFormatIsIncorrect = 7,

		[Description("结束时间格式输入不正确")]
		EndTimeFormatIsIncorrect = 8,

		[Description("开市时间段重复")]
		OpenTimeExisted = 9,

		[Description("公司简称重复")]
		CompanyNameExisted = 10,

		[Description("公司全称重复")]
		CompanyFullNameExisted = 11,

		[Description("该金属品牌已存在！")]
		BrandExisted = 12,

		[Description("该仓库名称已存在！")]
		WarehouseExisted = 13,

		[Description("电话号码输入格式不正确！")]
		TelFormatIsIncorrect = 14,

		[Description("传真输入格式不正确！")]
		FaxFormatIsIncorrect = 15,

		[Description("控制器名称已存在")]
		ControllerNameExisted = 16,

		[Description("操作名称已存在")]
		ActionNameExisted = 17,

		[Description("两次输入的密码不匹配！")]
		PasswordNotMatch = 18,

		[Description("密码长度要大于5小于30！")]
		PasswordFormatError = 19,

		[Description("角色密码已存在")]
		RoleNameExisted = 20,

		[Description("公司有关联的用户，不能删除！")]
		UserConnectCompany = 21,

		[Description("库存不足！")]
		QtyNotEnough = 22,

		[Description("购买的数量不是最小购买单位的整数倍！")]
		NotInteger = 23,

		[Description("该金属不是开市状态！")]
		CommodityNotOpen = 24,

		[Description("该金属类型有关联的金属品牌，不能删除！")]
		CommodityTypeConnectedBrand = 25,

		[Description("库存数量不能小于0")]
		StockQuantityNotEnough = 26,

		[Description("控制器不存在")]
		ControllerNotExisted = 27,

		[Description("该控制器下存在操作！")]
		ControllerHasActions = 28,

		[Description("该控制器已分配权限！")]
		ControllerHasPrevileges = 29,

		[Description("该操作不存在！")]
		ActionNotExisted = 30,

		[Description("该操作已分配权限！")]
		ActionHasPrevilege = 31,

		[Description("该角色已经分配")]
		RoleHasAssigned = 32,
	}
}
