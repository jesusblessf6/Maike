namespace Entities
{
	public interface IEntity
	{
		int Id { get; }
		bool IsDeleted { get; set; }
		byte[] Ver { get;}
	}
}
