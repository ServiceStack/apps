using System;
using System.Collections.Generic;
using ServiceStack;
using ServiceStack.NativeTypes.Dart;
using ServiceStack.Text;

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

        public override string New(string ctor) => ctor; //no new
        public override string GetDateTimeLiteral(string value)
        {
            var dateValue = value.ConvertTo<DateTime>().ToUniversalTime();
            if (dateValue.Hour + dateValue.Minute + dateValue.Second + dateValue.Millisecond == 0)
                return New($"DateTime({dateValue.Year},{dateValue.Month},{dateValue.Day})");
            if (dateValue.Millisecond == 0)
                return New($"DateTime({dateValue.Year},{dateValue.Month},{dateValue.Day},{dateValue.Hour},{dateValue.Minute},{dateValue.Second})");
            return New($"DateTime({dateValue.Year},{dateValue.Month},{dateValue.Day},{dateValue.Hour},{dateValue.Minute},{dateValue.Second},{dateValue.Millisecond})");
        }

        public override string GetTimeSpanLiteral(string value)
        {
            var from = value.ConvertTo<TimeSpan>();
            var sb = StringBuilderCache.Allocate();
            if (from.Days > 0)
                sb.Append($"days:{from.Days}");
            if (from.Hours > 0)
                sb.Append(sb.Length > 0 ? "," : "").Append($"hours:{from.Hours}");
            if (from.Minutes > 0)
                sb.Append(sb.Length > 0 ? "," : "").Append($"minutes:{from.Minutes}");
            if (from.Seconds > 0)
                sb.Append(sb.Length > 0 ? "," : "").Append($"seconds:{from.Seconds}");
            if (from.Milliseconds > 0)
                sb.Append(sb.Length > 0 ? "," : "").Append($"milliseconds:{from.Milliseconds}");
            if (sb.Length == 0)
                sb.Append($"seconds:{from.Seconds}");
            var to = $"Duration({StringBuilderCache.ReturnAndFree(sb)})";
            return to;
        }

        public override string GetGuidLiteral(string value) => $"\"{value.ConvertTo<Guid>():D}\"";
    }
}