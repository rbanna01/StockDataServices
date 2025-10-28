namespace Persistence.Interfaces
{
    public interface IDatabaseSettings
    {
       string GetCollectionName();

       string GetConnectionString();

        string GetDatabaseName();
    }
}
