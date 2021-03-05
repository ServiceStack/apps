using System;
using System.Collections.Generic;
using ServiceStack;
using ServiceStack.NativeTypes.FSharp;
using ServiceStack.Text;

namespace Apps.ServiceInterface.Langs
{
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
open System.Collections.Generic
open System.Linq
open ServiceStack
open ServiceStack.Text
open MyApp

module Program =

    [<EntryPoint>]
    let main args =

        let client = new JsonServiceClient(""{BASE_URL}"")

        let response = client.Send(new {REQUEST}({REQUEST_BODY})){REQUIRES_AUTH}

        {API_COMMENT}response.PrintDump()
        {INSPECT_VARS}

        0 //exitCode
"
            };
            InspectVarsResponse = "Inspect.vars({| response = response |})";
            RequiresAuthTemplate = @"
        // Authentication is required
        // client.Post(new Authenticate(
        //     provider = ""credentials"",
        //     UserName = ""..."",
        //     Password = ""...""))";
        }
        private FSharpGenerator Gen => new(new MetadataTypesConfig());
        public override string GetTypeName(string typeName, string[] genericArgs) => Gen.Type(typeName, genericArgs);

        public override string GetPropertyAssignment(MetadataPropertyType prop, string propValue) =>
            $"            {prop.Name} = {propValue},";

        public override string GetLiteralCollection(bool isArray, string collectionBody, string collectionType) =>
            isArray
                ? $"[| " + collectionBody + " |]"
                : collectionType.StartsWith("ResizeArray")
                    ? $"ResizeArray([{ConvertCollectionBody(collectionBody,collectionType)}])"
                    : $"new {collectionType}([{ConvertCollectionBody(collectionBody,collectionType)}])";

        private static string ConvertCollectionBody(string collectionBody, string collectionType)
        {
            var elementType = collectionType.IndexOf('<') >= 0
                ? collectionType.RightPart('<').LeftPart('>')
                : null;
            if (elementType == null || !elementType.IsNumericType())
                return collectionBody;

            var sb = StringBuilderCache.Allocate();
            var nums = collectionBody.Split(';');
            foreach (var num in nums)
            {
                if (sb.Length > 0)
                    sb.Append("; ");
                sb.Append(num.Trim());
                sb.Append(NumericTypeLiteralSuffix(elementType));
            }
            var ret = StringBuilderCache.ReturnAndFree(sb);
            return ret;
        }

        public static string NumericTypeLiteralSuffix(string numericType) =>
            numericType switch {
                nameof(Byte) => "uy",
                nameof(SByte) => "y",
                nameof(Int16) => "s",
                nameof(Int32) => "",
                nameof(Int64) => "L",
                nameof(UInt16) => "us",
                nameof(UInt32) => "ul",
                nameof(UInt64) => "UL",
                nameof(Single) => "f",
                nameof(Double) => "",
                nameof(Decimal) => "m",
                _ => throw new ArgumentException("Unknown Numeric Type: " + numericType)
            };

        public override string RequestBodyFilter(string assignments)
        {
            var to = assignments.TrimEnd();
            return to.EndsWith(",") 
                ? to.Substring(0, to.Length - 1) 
                : to;
        }
    }
}