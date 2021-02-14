using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack;
using ServiceStack.Host;
using ServiceStack.NativeTypes;

namespace Apps.ServiceInterface
{
    public static class LangInfoExtensions
    {
        public static MetadataType FindType(this MetadataTypes types, string typeName, string typeNs) =>
            types.Types.FirstOrDefault(x => x.Name == typeName) ??
            types.Types.FirstOrDefault(x => string.Equals(x.Name, typeName, StringComparison.OrdinalIgnoreCase)) ??
            (typeName == "QueryBase" || typeName.StartsWith("QueryDb`") || typeName.StartsWith("QueryData`")
                ? QueryBaseType
                : typeName == "QueryResponse`1"
                    ? QueryResponseType
                    : null);

        private static readonly MetadataTypesGenerator MetaGen =
            new(new ServiceMetadata(new List<RestPath>()), new NativeTypesFeature().MetadataTypesConfig);
        
        private static MetadataType queryBaseType;
        private static MetadataType QueryBaseType => queryBaseType ??= MetaGen.ToType(typeof(QueryBase));  
        private static MetadataType QueryResponseType => queryBaseType ??= MetaGen.ToType(typeof(QueryResponse<>));  

        public static List<MetadataPropertyType> GetFlattenedProperties(this MetadataType type, MetadataTypes types)
        {
            var to = new List<MetadataPropertyType>();
            if (type == null) 
                return to;

            do
            {
                if (type.Properties != null)
                {
                    foreach (var metaProp in type.Properties)
                    {
                        to.Add(metaProp);
                    }
                }

                type = type.Inherits != null 
                    ? types.FindType(type.Inherits.Name, type.Inherits.Namespace) 
                    : null;
            } while (type != null);
            return to;
        }

        public static bool IsNumericType(this string typeName) => typeName switch {
            nameof(Byte) => true,
            nameof(SByte) => true,
            nameof(Int16) => true,
            nameof(Int32) => true,
            nameof(Int64) => true,
            nameof(UInt16) => true,
            nameof(UInt32) => true,
            nameof(UInt64) => true,
            nameof(Single) => true,
            nameof(Double) => true,
            nameof(Decimal) => true,
            _ => false,
        };
    }
}