namespace ObligatorioProgramacion3.Models
{
    public interface IPermissionService
    {
        Task<bool> UserHasPermissionAsync(int userId, string permissionName);
    }

}
