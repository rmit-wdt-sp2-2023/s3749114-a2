namespace WebApi.Models.Repository;

public interface IDataRepository<TEntity, TKey> where TEntity : class
{
    IEnumerable<TEntity> GetAll();
    TEntity Get(TKey id);
    TKey Update(TKey id, TEntity item);
}
