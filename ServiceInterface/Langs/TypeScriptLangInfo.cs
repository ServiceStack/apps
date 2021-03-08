using System;
using System.Collections.Generic;
using System.Globalization;
using ServiceStack;
using ServiceStack.NativeTypes.TypeScript;
using ServiceStack.Text.Common;

namespace Apps.ServiceInterface.Langs
{
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
    {REQUIRES_AUTH}
    {API_COMMENT}let response = await client.get(new {REQUEST}({{REQUEST_BODY}
    {API_COMMENT}}));

    {API_COMMENT}Inspect.printDump(response);
    {INSPECT_VARS}

})();
",
            };
            InspectVarsResponse = "Inspect.vars({ response });";
            RequiresAuthTemplate = @"
    // Authentication is required
    // client.post(new Authenticate({ 
    //     provider: 'credentials',
    //     userName: '...',
    //     password: '...'}));";
        }
        private TypeScriptGenerator Gen => new(new MetadataTypesConfig());
        public override string GetTypeName(string typeName, string[] genericArgs) => Gen.Type(typeName, genericArgs);

        public override string GetPropertyAssignment(MetadataPropertyType prop, string propValue) =>
            $"        {Gen.GetPropertyName(prop.Name)}: {Value(prop.Type,propValue)},";

        public override string Value(string typeName, string value) => typeName switch {
            nameof(Double) => Float(value),
            nameof(Single) => Float(value),
            nameof(Decimal) => Float(value),
            _ => value
        };

        public override string GetCollectionLiteral(string collectionBody, string collectionType, string elementType) =>
            IsArray(collectionType) && elementType == nameof(Byte)
                ? "new Uint8Array([" + collectionBody + "])"
                : "[" + collectionBody + "]";

        // public override string GetDateTimeLiteral(string value) => New($"Date('{ISO8601(value)}')");
        public override string GetDateTimeLiteral(string value) => $"\"{ISO8601(value)}\"";
        public override string GetTimeSpanLiteral(string value) => $"\"{value.ConvertTo<TimeSpan>():c}\"";
        public override string GetGuidLiteral(string value) => $"\"{value.ConvertTo<Guid>():D}\"";
    }
}