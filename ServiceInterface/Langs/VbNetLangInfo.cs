using System.Collections.Generic;
using ServiceStack;
using ServiceStack.NativeTypes.VbNet;

namespace Apps.ServiceInterface.Langs
{
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
                ["Program.vb"] = @"Imports System
Imports System.Collections.Generic
Imports ServiceStack
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
            RequiresAuthTemplate = @"
        ' Authentication is required
        ' client.Post(New Authenticate() With {
        '     .provider = ""credentials"",
        '     .UserName = ""..."",
        '     .Password = ""..."" })";
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
}