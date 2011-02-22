namespace KataOrm.Infrastructure
{
    public class Log
    {
        //Will eventually need a container here if implementing more than one Logger type
        public static ILogger BoundTo(object itemInNeedOfLogging)
        {
            return new TextLoggerFactory().CreateLogFor(itemInNeedOfLogging.GetType());
        }
    }
}