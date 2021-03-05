using System.Collections.Generic;
using ServiceStack;
using ServiceStack.NativeTypes.Dart;

namespace Apps.ServiceInterface.Langs
{
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

  var client = new JsonServiceClient('{BASE_URL}');{REQUIRES_AUTH}

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
  servicestack: ^1.0.25

dev_dependencies:
#  pedantic: ^1.9.0",
            };
            InspectVarsResponse = "Inspect.vars({'response': response});";
            RequiresAuthTemplate = @"
  // Authentication is required
  // client.post(new Authenticate()
  //   ..provider = 'credentials'
  //   ..userName = '...'
  //   ..password = '...');";
        }
        
        private DartGenerator Gen => new(new MetadataTypesConfig());
        public override string GetTypeName(string typeName, string[] genericArgs) => Gen.Type(typeName, genericArgs);

        public override string GetPropertyAssignment(MetadataPropertyType prop, string propValue) =>
            $"    ..{prop.Name.ToCamelCase()} = {propValue}";

        public override string GetLiteralCollection(bool isArray, string collectionBody, string collectionType) => 
            "[" + collectionBody + "]";
    }
}