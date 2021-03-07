using System.Collections.Generic;
using ServiceStack;
using ServiceStack.NativeTypes.TypeScript;

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
            $"        {prop.Name.ToCamelCase()}: {propValue},";

        public override string GetLiteralCollection(bool isArray, string collectionBody, string collectionType) => 
            "[" + collectionBody + "]";
    }
}