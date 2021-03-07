using System;
using System.Collections.Generic;
using System.Globalization;
using ServiceStack;
using ServiceStack.NativeTypes.Swift;
using ServiceStack.Text.Support;

namespace Apps.ServiceInterface.Langs
{
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

let client = JsonServiceClient(baseUrl: ""{BASE_URL}""){REQUIRES_AUTH}

{API_COMMENT}let request = {REQUEST}(){REQUEST_BODY}
{API_COMMENT}client.sendAsync(request)
    {API_COMMENT}.done { response in 
        {API_COMMENT}Inspect.printDump(response)
        {INSPECT_VARS}
    {API_COMMENT}}
",
            };
            InspectVarsResponse = @"Inspect.vars([""response"":response])";
            RequiresAuthTemplate = @"
// Authentication is required
// let auth = Authenticate()
// auth.provider = ""credentials""
// auth.userName = ""...""
// auth.password = ""...""
// client.post(auth)";
        }
        private SwiftGenerator Gen => new(new MetadataTypesConfig());
        public override string GetTypeName(string typeName, string[] genericArgs) => Gen.Type(typeName, genericArgs);

        public override string GetPropertyAssignment(MetadataPropertyType prop, string propValue) =>
            $"request.{prop.Name.ToCamelCase()} = {propValue}";

        public override string GetLiteralCollection(bool isArray, string collectionBody, string collectionType) => 
            "[" + collectionBody + "]";

        public override string New(string ctor) => ctor; //no new
        public override string GetDateTimeLiteral(string value) => $"DateTime.parse(\"{ISO8601(value)}\")";
        public override string GetGuidLiteral(string value) => $"\"{UUID(value)}\"";
        public override string GetTimeSpanLiteral(string value) => $"TimeSpan.parse(\"{XsdDuration(value)}\")";
        public override string GetCharLiteral(string value) => $"\"{value.ConvertTo<char>()}\"";
    }
}