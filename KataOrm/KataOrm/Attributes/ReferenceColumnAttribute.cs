using System;

namespace KataOrm.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class ReferenceColumnAttribute : Attribute
    {
        public string ReferenceColumnName { get; private set; }

        public ReferenceColumnAttribute(string referenceColumnName)
        {
            ReferenceColumnName = referenceColumnName;
        }
    }
}