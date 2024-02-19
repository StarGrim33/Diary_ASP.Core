namespace FonTech.Domain.Interfaces.Databases
{
    public interface IStateSaveChangesAsync
    {
        Task<int> SaveChangesAsync();
    }
}
