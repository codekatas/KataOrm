namespace KataOrm.MetaStore
{
    public abstract class MetaInfo
    {
        protected MetaInfoStore MetaInfoStore { get; set; }

        protected MetaInfo(MetaInfoStore metaInfoStore)
        {
            MetaInfoStore = metaInfoStore;
        }
    }
}