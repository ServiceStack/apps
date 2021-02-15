using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack;
using ServiceStack.Text;

namespace Apps.ServiceInterface
{
    public class CSharpLangInfo : LangInfo
    {
        public CSharpLangInfo()
        {
            Code = "csharp";
            Name = "C#";
            Ext = "cs";
            Files = new Dictionary<string, string> {
                ["MyApp.csproj"] = @"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net5.0</TargetFramework>
    <NoWarn>1591</NoWarn>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include=""ServiceStack.Client"" Version=""5.*"" />
    <PackageReference Include=""ServiceStack.Common"" Version=""5.*"" />
  </ItemGroup>

</Project>",
                ["Program.cs"] = @"using MyApp;
using ServiceStack;
using ServiceStack.Text;

var client = new JsonServiceClient(""{BASE_URL}"");

{API_COMMENT}var response = client.Send(new {REQUEST} {{REQUEST_BODY}
{API_COMMENT}});
{API_COMMENT}response.PrintDump();
{INSPECT_VARS}"
            };
            InspectVarsResponse = "\nInspect.vars(new { response });";
        }
    }

    public class TypeScriptLangInfo : LangInfo
    {
        public TypeScriptLangInfo()
        {
            Code = "typescript";
            Name = "TypeScript";
            Ext = "ts";
            Files = new Dictionary<string, string> {
["package.json"] = @"{
  ""name"": ""my-app"",
  ""version"": ""1.0.0"",
  ""description"": ""{DESCRIPTION}"",
  ""scripts"": {
    ""postinstall"": ""tsc"",
    ""start"": ""ts-node index.ts"",
    ""test"": ""echo \""Error: no test specified\"" && exit 1""
  },
  ""author"": """",
  ""license"": ""ISC"",
  ""dependencies"": {
    ""@servicestack/client"": ""^1.0.39"",
    ""gistcafe"": ""^1.0.1""
  },
  ""devDependencies"": {
    ""ts-node"": ""^9.1.1"",
    ""typescript"": ""^4.1.5""
  }
}",
["tsconfig.json"] = @"{
    ""compilerOptions"": {
        ""target"": ""es5"",
        ""lib"": [""ES2015"",""DOM""]
    }
}",
["index.ts"] = @"import { JsonServiceClient } from '@servicestack/client';
import { Inspect } from 'gistcafe';
{API_COMMENT}import { {REQUEST} } from './dtos';

let client = new JsonServiceClient('{BASE_URL}');

(async () => {

{API_COMMENT}let response = await client.get(new {REQUEST}({{REQUEST_BODY}
{API_COMMENT}}));

{API_COMMENT}Inspect.printDump(response);
{INSPECT_VARS}

})();
",
            };
            InspectVarsResponse = "Inspect.vars({ response });";
        }

        public override string GetPropertyAssignment(MetadataPropertyType prop, string propValue) =>
            $"    {prop.Name.ToCamelCase()}: {propValue},";

        public override string GetLiteralCollection(bool isArray, string collectionBody, string collectionType) => 
            "[" + collectionBody + "]";
    }

    public class DartLangInfo : LangInfo
    {
        public DartLangInfo()
        {
            Code = "dart";
            Name = "Dart";
            Ext = "dart";
            DtosPathPrefix = "lib\\";
            Files = new Dictionary<string, string> {
                ["bin\\my_app.dart"] = @"import 'dart:io';
import 'package:servicestack/client.dart';
import 'package:gistcafe/gistcafe.dart';

import '../lib/dtos.dart';

void main(List<String> arguments) async {

  var client = new JsonServiceClient('{BASE_URL}');
  {API_COMMENT}var response = await client.send({REQUEST}(){REQUEST_BODY});

  {API_COMMENT}Inspect.printDump(response);
  {INSPECT_VARS}
  exit(0);
}",
                ["pubspec.yaml"] = @"name: my_app
description: {DESCRIPTION}

environment:
  sdk: '>=2.8.1 <3.0.0'

dependencies:
  http: ^0.12.2
  gistcafe: ^1.0.3
  servicestack: ^1.0.22

dev_dependencies:
#  pedantic: ^1.9.0",
            };
            InspectVarsResponse = "Inspect.vars({'response': response});";
        }

        public override string GetPropertyAssignment(MetadataPropertyType prop, string propValue) =>
            $"    ..{prop.Name.ToCamelCase()} = {propValue}";

        public override string GetLiteralCollection(bool isArray, string collectionBody, string collectionType) => 
            "[" + collectionBody + "]";
    }

    public class SwiftLangInfo : LangInfo
    {
        public SwiftLangInfo()
        {
            Code = "swift";
            Name = "Swift";
            Ext = "swift";
        }
    }

    public class JavaLangInfo : LangInfo
    {
        public JavaLangInfo()
        {
            Code = "java";
            Name = "Java";
            Ext = "java";
        }
    }

    public class KotlinLangInfo : LangInfo
    {
        public KotlinLangInfo()
        {
            Code = "kotlin";
            Name = "Kotlin";
            Ext = "kt";
        }
    }

    public class FSharpLangInfo : LangInfo
    {
        public FSharpLangInfo()
        {
            Code = "fsharp";
            Name = "F#";
            Ext = "fs";
        }
    }

    public class VbNetLangInfo : LangInfo
    {
        public VbNetLangInfo()
        {
            Code = "vbnet";
            Name = "VB.NET";
            Ext = "vb";
            LineComment = "'";
        }
    }

    public class LangInfo
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Ext { get; set; }
        public string DtosPathPrefix { get; set; } = "";
        public string LineComment { get; set; } = "//";
        public string InspectVarsResponse { get; set; }
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
                return props.TrimEnd();
            }

            return "";
        }

        public virtual string GetPropertyAssignment(MetadataPropertyType prop, string propValue) =>
            $"    {prop.Name} = {propValue},";

        public virtual string GetLiteralValue(string value, MetadataPropertyType prop, MetadataTypes types)
        {
            var useType = prop.Type == "Nullable`1"
                ? prop.GenericArgs[0]
                : prop.Type;
            var isArray = useType.EndsWith("[]");
            if (isArray)
                useType = useType.LeftPart("[");
            var enumType = prop.IsEnum == true
                ? types.FindType(prop.Type, prop.TypeNamespace)
                : null;
            var collectionType = prop.TypeNamespace == "System.Collections.Generic"
                ? $"{prop.Name.TrimPrefixes("I")}<{string.Join(',', prop.GenericArgs)}>"
                : null;

            if (isArray || collectionType != null)
            {
                if (collectionType?.IndexOf("Dictionary") >= 0)
                    return null; //not supported
                var items = value.FromJsv<List<string>>();
                var sb = StringBuilderCacheAlt.Allocate();
                foreach (var item in items)
                {
                    var itemProp = new MetadataPropertyType {
                        Type = isArray
                            ? useType
                            : prop.GenericArgs[0],
                        TypeNamespace = "System",
                    };
                    var literalValue = GetLiteralValue(item, itemProp, types);
                    if (sb.Length > 0)
                        sb.Append(", ");
                    sb.Append(literalValue);
                }

                var collectionBody = StringBuilderCacheAlt.ReturnAndFree(sb);
                return GetLiteralCollection(isArray, collectionBody, collectionType);
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
    }
}