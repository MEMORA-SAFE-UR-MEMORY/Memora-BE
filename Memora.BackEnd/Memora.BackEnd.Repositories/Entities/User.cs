namespace Memora.BackEnd.Repositories.Entities
{
	public record User (Guid? Id, string UserName, string PasswordHash, int RoleId);
}
