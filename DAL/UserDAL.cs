using System.Linq;
using DAL.Base;
using Entities;

namespace DAL
{
	public class UserDAL : BaseDAL<User>
	{
		public User GetUserByLoginName(string userName,int userType)
		{
			return Query(o => o.LoginName == userName && o.Type == userType).FirstOrDefault();
		}
	}
}
