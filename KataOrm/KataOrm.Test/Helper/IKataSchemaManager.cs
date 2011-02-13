using System.Reflection;

namespace KataOrm.Test.Helper
{
    public interface IKataSchemaManager
    {
        void CreateSchema();
        void DeleteSchema();
    }
}