using System;
using System.Collections.Generic;
using System.Data;

namespace KataOrm.Converter
{
    public static class SqlDbTypeConverter
    {
        private static readonly Dictionary<Type, SqlDbType> typeToDbType = new Dictionary<Type, SqlDbType>
                                                                               {
                                                                                   {typeof (string), SqlDbType.VarChar},
                                                                                   {typeof (DateTime), SqlDbType.DateTime},
                                                                                   {typeof (DateTime?), SqlDbType.DateTime},
                                                                                   {typeof (int), SqlDbType.Int},
                                                                                   {typeof (int?), SqlDbType.Int},
                                                                                   {typeof (long), SqlDbType.BigInt},
                                                                                   {typeof (long?), SqlDbType.BigInt},
                                                                                   {typeof (bool), SqlDbType.Bit},
                                                                                   {typeof (bool?), SqlDbType.Bit},
                                                                                   {typeof (byte[]), SqlDbType.Binary},
                                                                                   {typeof (decimal), SqlDbType.Decimal},
                                                                                   {typeof (decimal?), SqlDbType.Decimal},
                                                                                   {typeof (double), SqlDbType.BigInt},
                                                                                   {typeof (double?), SqlDbType.BigInt},
                                                                                   {typeof (float), SqlDbType.Float},
                                                                                   {typeof (float?), SqlDbType.Float},
                                                                                   {typeof (Guid), SqlDbType.UniqueIdentifier},
                                                                                   {typeof (Guid?), SqlDbType.UniqueIdentifier}
                                                                               };

        public static SqlDbType ToDbType(Type type)
        {
            if (!typeToDbType.ContainsKey(type))
            {
                throw new InvalidOperationException(string.Format("Type {0} doesn't have a matching DbType configured",
                                                                  type.FullName));
            }

            return typeToDbType[type];
        }
    }
}