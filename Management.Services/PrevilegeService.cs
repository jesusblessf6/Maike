using DAL;

namespace Management.Services
{
	public class PrevilegeService : BaseService
	{
		#region Properties

		private PrevilegeDAL _previlegeDal;
		public PrevilegeDAL PrevilegeDal
		{
			get { return _previlegeDal ?? (_previlegeDal = new PrevilegeDAL()); }
			set { _previlegeDal = value; }
		}

		#endregion


		#region Methods



		#endregion
	}
}
