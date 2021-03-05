using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack;
using ServiceStack.Text;

namespace Apps.ServiceInterface.Langs
{
    public abstract class LangInfo
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Ext { get; set; }
        public string DtosPathPrefix { get; set; } = "";
        public string LineComment { get; set; } = "//";
        public string InspectVarsResponse { get; set; }
        public char ItemsSep { get; set; } = ',';
        public Dictionary<string, string> Files { get; set; } = new();

        public virtual string RequestBody(string requestDto, Dictionary<string, string> args, MetadataTypes types)
        {
            var requestType = types.Operations.FirstOrDefault(x => x.Request.Name == requestDto)?.Request;
            if (requestType != null)
            {
                var sb = StringBuilderCache.Allocate();
                sb.AppendLine();
                var requestProps = requestType.GetFlattenedProperties(types);
                foreach (var entry in args)
                {
                    var prop = requestProps.FirstOrDefault(x => x.Name == entry.Key)
                               ?? requestProps.FirstOrDefault(x =>
                                   string.Equals(x.Name, entry.Key, StringComparison.OrdinalIgnoreCase));
                    if (prop != null)
                    {
                        var propValue = GetLiteralValue(entry.Value, prop, types);
                        if (propValue == null)
                            continue;
                        var propAssign = GetPropertyAssignment(prop, propValue);
                        sb.AppendLine(propAssign);
                    }
                }

                var props = StringBuilderCache.ReturnAndFree(sb);
                return RequestBodyFilter(props);
            }

            return "";
        }

        public virtual string RequestBodyFilter(string assignments) => assignments.TrimEnd();

        public virtual string GetPropertyAssignment(MetadataPropertyType prop, string propValue) =>
            $"    {prop.Name} = {propValue},";

        public virtual string GetLiteralValue(string value, MetadataPropertyType prop, MetadataTypes types)
        {
            var useType = prop.Type == "Nullable`1"
                ? prop.GenericArgs[0]
                : prop.Type;
            var isArray = useType.EndsWith("[]");
            var elementType = isArray
                ? useType.LeftPart("[")
                : null;
            var enumType = prop.IsEnum == true
                ? types.FindType(prop.Type, prop.TypeNamespace)
                : null;
            var collectionType = prop.TypeNamespace == "System.Collections.Generic"
                ? GetTypeName(prop.Type, prop.GenericArgs) 
                : null;
            if (collectionType != null)
            {
                elementType = prop.GenericArgs.Length == 1 
                    ? prop.GenericArgs[0]
                    : prop.GenericArgs.Length == 2
                        ? $"KeyValuePair<{string.Join(',',prop.GenericArgs)}>"
                        : null;
                if (collectionType.IndexOf("Dictionary", StringComparison.Ordinal) >= 0)
                    return null; //not supported
            }
            if (isArray || collectionType != null)
            {
                var items = value.FromJsv<List<string>>();
                var sb = StringBuilderCacheAlt.Allocate();
                foreach (var item in items)
                {
                    var itemProp = new MetadataPropertyType {
                        Type = elementType,
                        TypeNamespace = "System",
                    };
                    var literalValue = GetLiteralValue(item, itemProp, types);
                    if (sb.Length > 0)
                        sb.Append($"{ItemsSep} ");
                    sb.Append(literalValue);
                }
                var collectionBody = StringBuilderCacheAlt.ReturnAndFree(sb);
                return GetLiteralCollection(isArray, collectionBody, collectionType ?? useType);
            }
            if (enumType != null)
                return GetEnumLiteral(value, enumType);
            if (useType == nameof(String))
                return GetStringLiteral(value);
            if (useType.IsNumericType())
                return GetNumericTypeLiteral(value);
            if (useType == nameof(DateTime))
                return GetDateTimeLiteral(value);
            if (useType == nameof(TimeSpan))
                return GetTimeSpanLiteral(value);
            if (useType == nameof(Boolean))
                return GetBoolLiteral(value);

            return null;
        }

        public virtual string GetEnumLiteral(string value, MetadataType enumType)
        {
            if (enumType.EnumNames == null)
                return null;
            var enumName = enumType.EnumNames.FirstOrDefault(x => x == value) ??
                           enumType.EnumNames.FirstOrDefault(x =>
                               string.Equals(x, value, StringComparison.OrdinalIgnoreCase));
            if (enumName == null)
                return null;
            return $"{enumType.Name}.{enumName}";
        }

        public virtual string GetStringLiteral(string value) => value.ToJson();

        public virtual string GetNumericTypeLiteral(string value) => value;

        public virtual string GetTimeSpanLiteral(string value)
        {
            var timeSpanValue = value.ConvertTo<TimeSpan>();
            return $"new TimeSpan({timeSpanValue.Ticks})";
        }

        public virtual string GetBoolLiteral(string value)
        {
            var boolValue = value.ConvertTo<bool>();
            return boolValue ? "true" : "false";
        }

        public virtual string GetDateTimeLiteral(string value)
        {
            var dateValue = value.ConvertTo<DateTime>();
            if (dateValue.Hour + dateValue.Minute + dateValue.Second + dateValue.Millisecond == 0)
                return $"new DateTime({dateValue.Year},{dateValue.Month},{dateValue.Day})";
            if (dateValue.Millisecond == 0)
                return
                    $"new DateTime({dateValue.Year},{dateValue.Month},{dateValue.Day},{dateValue.Hour},{dateValue.Minute},{dateValue.Second})";
            return
                $"new DateTime({dateValue.Year},{dateValue.Month},{dateValue.Day},{dateValue.Hour},{dateValue.Minute},{dateValue.Second},{dateValue.Millisecond})";
        }

        public virtual string GetLiteralCollection(bool isArray, string collectionBody, string collectionType)
        {
            return isArray
                ? "new[] { " + collectionBody + " }"
                : "new " + collectionType + " { " + collectionBody + " }";
        }

        public abstract string GetTypeName(string typeName, string[] genericArgs);

        public virtual string GetResponse(MetadataOperationType op)
        {
            if (op?.Response != null)
            {
                var genericArgs = op.Response.Name.IndexOf('`') >= 0 && op.Response.GenericArgs[0] == "'T" && op.DataModel != null
                    ? new[] { op.DataModel.Name }
                    : op.Response.GenericArgs;
                var typeName = GetTypeName(op.Response.Name, genericArgs);
                return typeName;
            }
            return "var";
        }
    }
}