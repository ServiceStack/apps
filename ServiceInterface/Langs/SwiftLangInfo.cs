using System;
using System.Collections.Generic;
using ServiceStack;
using ServiceStack.NativeTypes.Swift;

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
                ["Package.swift"] = @"// swift-tools-version:5.3
import PackageDescription

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
                ["Sources\\MyApp\\main.swift"] = @"#if canImport(FoundationNetworking)
    import FoundationNetworking
#endif
import Foundation
import ServiceStack

let client = JsonServiceClient(baseUrl: ""{BASE_URL}""){REQUIRES_AUTH}

{API_COMMENT}let request = {REQUEST}(){REQUEST_BODY}
{API_COMMENT}let response = try client.send(request)

{API_COMMENT}Inspect.printDump(response)
{INSPECT_VARS}
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
        public override string New(string ctor) => ctor; //no new

        public override string GetPropertyAssignment(MetadataPropertyType prop, string propValue) =>
            $"request.{Gen.GetPropertyName(prop.Name)} = {Value(prop.Type, propValue)}";

        public override string Value(string typeName, string value) => typeName switch {
            nameof(Double) => Float(value),
            nameof(Single) => Float(value),
            nameof(Decimal) => Float(value),
            _ => value
        };

        public override string GetCollectionLiteral(string collectionBody, string collectionType, string elementType) =>
            "[" + collectionBody + "]";
        public override string GetByteArrayLiteral(byte[] bytes) =>
            $"fromByteArray(\"{Convert.ToBase64String(bytes)}\")";

        public override string GetDateTimeLiteral(string value) => $"fromDateTime(\"{ISO8601(value)}\")";
        public override string GetGuidLiteral(string value) => $"\"{UUID(value)}\"";
        public override string GetTimeSpanLiteral(string value) => $"fromTimeSpan(\"{XsdDuration(value)}\")";
        public override string GetCharLiteral(string value) => $"\"{value.ConvertTo<char>()}\"";
    }
}