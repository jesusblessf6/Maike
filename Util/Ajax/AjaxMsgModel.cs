namespace Util.Ajax
{
	public class AjaxMsgModel
	{
		public string Msg { get; set; }
		public AjaxStatusCode Status { get; set; }//success,error,noLogin,
		public string BackUrl { get; set; }
		public object Data { get; set; }//数据对象
	}
}