using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ServiceStack;
using ServiceStack.NativeTypes.CSharp;
using ServiceStack.NativeTypes.Dart;
using ServiceStack.NativeTypes.FSharp;
using ServiceStack.NativeTypes.Java;
using ServiceStack.NativeTypes.Kotlin;
using ServiceStack.NativeTypes.Swift;
using ServiceStack.NativeTypes.TypeScript;
using ServiceStack.NativeTypes.VbNet;
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

        private CSharpGenerator Gen => new(new MetadataTypesConfig());
        public override string GetTypeName(string typeName, string[] genericArgs) => Gen.Type(typeName, genericArgs);
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
        private TypeScriptGenerator Gen => new(new MetadataTypesConfig());
        public override string GetTypeName(string typeName, string[] genericArgs) => Gen.Type(typeName, genericArgs);

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
        
        private DartGenerator Gen => new(new MetadataTypesConfig());
        public override string GetTypeName(string typeName, string[] genericArgs) => Gen.Type(typeName, genericArgs);

        public override string GetPropertyAssignment(MetadataPropertyType prop, string propValue) =>
            $"    ..{prop.Name.ToCamelCase()} = {propValue}";

        public override string GetLiteralCollection(bool isArray, string collectionBody, string collectionType) => 
            "[" + collectionBody + "]";
    }

    public class JavaLangInfo : LangInfo
    {
        public JavaLangInfo()
        {
            Code = "java";
            Name = "Java";
            Ext = "java";
            DtosPathPrefix = "src\\main\\java\\myapp\\";
            Files = new Dictionary<string, string> {
                ["src\\main\\java\\myapp\\App.java"] = @"package myapp;

import net.servicestack.client.JsonServiceClient;
import net.servicestack.gistcafe.Inspect;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collections;

import static myapp.dtos.*;

public class App {

    public static void main(String[] args) {

        JsonServiceClient client = new JsonServiceClient(
            ""{BASE_URL}"");

        {API_COMMENT}{RESPONSE} response = client.send(({REQUEST}) new {REQUEST}(){REQUEST_BODY});

        {API_COMMENT}Inspect.printDump(response);
        {INSPECT_VARS}
    }
}
",
                ["gradle\\wrapper\\gradle-wrapper.properties"] = @"distributionBase=GRADLE_USER_HOME
distributionPath=wrapper/dists
distributionUrl=https\://services.gradle.org/distributions/gradle-6.7-bin.zip
zipStoreBase=GRADLE_USER_HOME
zipStorePath=wrapper/dists",
                ["build.gradle"] = @"plugins {
    id 'application'
}

group 'myapp'
version '1.0-SNAPSHOT'

repositories {
    jcenter()
}

dependencies {
    implementation 'com.google.code.gson:gson:2.8.6'
    implementation 'net.servicestack:client:1.0.43'
    implementation 'net.servicestack:gistcafe:0.0.7'
    testImplementation 'org.junit.jupiter:junit-jupiter-api:5.6.0'
    testRuntimeOnly 'org.junit.jupiter:junit-jupiter-engine'
}

test {
    useJUnitPlatform()
}

application {
    mainClass = 'myapp.App'
}
",
            };
            InspectVarsResponse = "Inspect.vars(Collections.singletonMap(\"response\", response));";
        }
        private JavaGenerator Gen => new(new MetadataTypesConfig());
        public override string GetTypeName(string typeName, string[] genericArgs) => Gen.Type(typeName, genericArgs);

        public override string GetPropertyAssignment(MetadataPropertyType prop, string propValue) =>
            $"            .set{prop.Name.ToPascalCase()}({propValue})";

        public override string GetLiteralCollection(bool isArray, string collectionBody, string collectionType) => 
            "new ArrayList(Arrays.asList(" + collectionBody + "))";
    }

    public class KotlinLangInfo : LangInfo
    {
        public KotlinLangInfo()
        {
            Code = "kotlin";
            Name = "Kotlin";
            Ext = "kt";
            DtosPathPrefix = "src\\main\\kotlin\\myapp\\";
            Files = new Dictionary<string, string> {
                ["src\\main\\kotlin\\myapp\\App.kt"] = @"package myapp
import net.servicestack.client.JsonServiceClient
import net.servicestack.gistcafe.Inspect

fun main(args: Array<String>) {
    val client = JsonServiceClient(""{BASE_URL}"")

    {API_COMMENT}val response = client.send({REQUEST}().apply {{REQUEST_BODY}
    {API_COMMENT}});

    {API_COMMENT}Inspect.printDump(response)
    {INSPECT_VARS}
}
",
                ["gradle\\wrapper\\gradle-wrapper.properties"] = @"distributionBase=GRADLE_USER_HOME
distributionPath=wrapper/dists
distributionUrl=https\://services.gradle.org/distributions/gradle-6.7-bin.zip
zipStoreBase=GRADLE_USER_HOME
zipStorePath=wrapper/dists",
                ["build.gradle.kts"] = @"plugins {
    java
    maven
    application
    kotlin(""jvm"") version ""1.4.21""
}

group = ""myapp""
version = ""1.0-SNAPSHOT""

application {
    mainClass.set(""myapp.AppKt"")
}

repositories {
    mavenCentral()
}

tasks.test {
    useJUnitPlatform()
}

dependencies {
    implementation(kotlin(""stdlib""))
    implementation(""com.google.code.gson:gson:2.8.6"")
    implementation(""net.servicestack:client:1.0.43"")
    implementation(""net.servicestack:gistcafe:0.0.7"")
    testImplementation(platform(""org.junit:junit-bom:5.7.0""))
    testImplementation(""org.junit.jupiter:junit-jupiter"")
}
",
            };
            InspectVarsResponse = "Inspect.vars(mapOf(\"response\" to response))";
        }
        private KotlinGenerator Gen => new(new MetadataTypesConfig());
        public override string GetTypeName(string typeName, string[] genericArgs) => Gen.Type(typeName, genericArgs);

        public override string GetPropertyAssignment(MetadataPropertyType prop, string propValue) =>
            $"        {prop.Name.ToCamelCase()} = {propValue}";

        public override string GetLiteralCollection(bool isArray, string collectionBody, string collectionType) => 
            "arrayListOf(" + collectionBody + ")";
    }

    public class SwiftLangInfo : LangInfo
    {
        public SwiftLangInfo()
        {
            Code = "swift";
            Name = "Swift";
            Ext = "swift";
            DtosPathPrefix = "Sources\\MyApp\\";
            Files = new Dictionary<string, string> {
                ["Package.swift"] = @"import PackageDescription

let package = Package(
    name: ""MyApp"",
    dependencies: [
        .package(name: ""ServiceStack"", url: ""https://github.com/ServiceStack/ServiceStack.Swift.git"", 
            Version(5,0,0)..<Version(6,0,0))
        ],
    targets: [
        .target(
            name: ""MyApp"",
            dependencies: [""ServiceStack""]),
    ]
)
",
                ["Sources\\MyApp\\main.swift"] = @"import Foundation
import ServiceStack

let client = JsonServiceClient(baseUrl: ""{BASE_URL}"")

{API_COMMENT}let request = {REQUEST}(){REQUEST_BODY}
{API_COMMENT}client.sendAsync(request)
    {API_COMMENT}.done { response in 
        {API_COMMENT}Inspect.printDump(response)
        {INSPECT_VARS}
    {API_COMMENT}}
",
            };
            InspectVarsResponse = @"Inspect.vars([""response"":response])";
        }
        private SwiftGenerator Gen => new(new MetadataTypesConfig());
        public override string GetTypeName(string typeName, string[] genericArgs) => Gen.Type(typeName, genericArgs);

        public override string GetPropertyAssignment(MetadataPropertyType prop, string propValue) =>
            $"request.{prop.Name.ToCamelCase()} = {propValue}";

        public override string GetLiteralCollection(bool isArray, string collectionBody, string collectionType) => 
            "[" + collectionBody + "]";
    }

    public class VbNetLangInfo : LangInfo
    {
        public VbNetLangInfo()
        {
            Code = "vbnet";
            Name = "VB.NET";
            Ext = "vb";
            LineComment = "'";
            Files = new Dictionary<string, string> {
                ["MyApp.vbproj"] = @"<Project Sdk=""Microsoft.NET.Sdk"">

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
                ["Program.vb"] = @"Imports ServiceStack
Imports ServiceStack.Text
Imports MyApp

Module Program

    Sub Main(args As String())

        Dim client = New JsonServiceClient(""{BASE_URL}"")

        {API_COMMENT}Dim response = client.Send(New {REQUEST}(){REQUEST_BODY})
            
        {API_COMMENT}response.PrintDump()
        {INSPECT_VARS}

    End Sub

End Module
"
            };
            InspectVarsResponse = "Inspect.vars(New With { response })";
        }
        private VbNetGenerator Gen => new(new MetadataTypesConfig());
        public override string GetTypeName(string typeName, string[] genericArgs) => Gen.Type(typeName, genericArgs);

        public override string GetPropertyAssignment(MetadataPropertyType prop, string propValue) =>
            $"            .{Gen.EscapeKeyword(prop.Name)} = {propValue},";

        public override string GetLiteralCollection(bool isArray, string collectionBody, string collectionType) => isArray 
            ? $"New {GetTypeName(collectionType, new string[0])}() {{" + collectionBody + "}"
            : $"New {collectionType}() From {{" + collectionBody + "}";

        public override string RequestBodyFilter(string assignments)
        {
            var to = assignments.TrimEnd();
            if (to.EndsWith(","))
                to = to.Substring(0, to.Length - 1);

            return " With {" + to + "\n        }";
        }
    }

    public class FSharpLangInfo : LangInfo
    {
        public FSharpLangInfo()
        {
            Code = "fsharp";
            Name = "F#";
            Ext = "fs";
            ItemsSep = ';';
            Files = new Dictionary<string, string> {
                ["MyApp.fsproj"] = @"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net5.0</TargetFramework>
    <NoWarn>1591,FS0058</NoWarn>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include=""ServiceStack.Client"" Version=""5.*"" />
    <PackageReference Include=""ServiceStack.Common"" Version=""5.*"" />
  </ItemGroup>

  <ItemGroup>
    <Compile Include=""dtos.fs"" />
    <Compile Include=""Program.fs"" />
  </ItemGroup>

</Project>",
                ["Program.fs"] = @"open System
open System.Linq
open ServiceStack
open ServiceStack.Text
open MyApp

module Program =

    [<EntryPoint>]
    let main args =

        let client = new JsonServiceClient(""{BASE_URL}"")

        let response = client.Send(new {REQUEST}({REQUEST_BODY}))

        {API_COMMENT}response.PrintDump()
        {INSPECT_VARS}

        0 //exitCode
"
            };
            InspectVarsResponse = "Inspect.vars({| response = response |})";
        }
        private FSharpGenerator Gen => new(new MetadataTypesConfig());
        public override string GetTypeName(string typeName, string[] genericArgs) => Gen.Type(typeName, genericArgs);

        public override string GetPropertyAssignment(MetadataPropertyType prop, string propValue) =>
            $"            {prop.Name} = {propValue},";

        public override string GetLiteralCollection(bool isArray, string collectionBody, string collectionType) => isArray 
            ? $"[| " + collectionBody + " |]"
            : $"new {collectionType}([ " + collectionBody + "]";

        public override string RequestBodyFilter(string assignments)
        {
            var to = assignments.TrimEnd();
            return to.EndsWith(",") 
                ? to.Substring(0, to.Length - 1) 
                : to;
        }
    }

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
                ? $"{prop.Type.TrimPrefixes("I").LeftPart('`')}<{string.Join(',', prop.GenericArgs)}>"
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