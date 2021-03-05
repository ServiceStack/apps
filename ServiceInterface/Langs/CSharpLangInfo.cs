using System.Collections.Generic;
using ServiceStack;
using ServiceStack.NativeTypes.CSharp;

namespace Apps.ServiceInterface.Langs
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
                ["Program.cs"] = @"using System;
using System.Collections.Generic;
using ServiceStack;
using ServiceStack.Text;
using MyApp;

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
}