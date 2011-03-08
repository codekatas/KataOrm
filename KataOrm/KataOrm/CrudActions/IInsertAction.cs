namespace KataOrm.CrudActions
{
    public interface IInsertAction
    {
        TEntity Insert<TEntity>(TEntity entity);
    }
}