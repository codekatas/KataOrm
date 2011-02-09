using System;

namespace KataOrm.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class PrimaryKeyAttribute : Attribute
    {
        public PrimaryKeyAttribute(string primaryKeyName)
        {
            PrimaryKeyName = primaryKeyName;
        }

        public string PrimaryKeyName { get; private set; }
    }
}