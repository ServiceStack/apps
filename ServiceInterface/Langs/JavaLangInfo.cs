using System;
using System.Collections.Generic;
using ServiceStack;
using ServiceStack.NativeTypes.Java;

namespace Apps.ServiceInterface.Langs
{
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

import net.servicestack.client.*;
import java.util.Collections;

import static myapp.dtos.*;

public class App {

    public static void main(String[] args) {

        JsonServiceClient client = new JsonServiceClient(
            ""{BASE_URL}"");{REQUIRES_AUTH}

        {API_COMMENT}{RESPONSE} response = client.send(({REQUEST})new {REQUEST}(){REQUEST_BODY});

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
    implementation 'net.servicestack:client:1.0.49'
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
            RequiresAuthTemplate = @"
        // Authentication is required
        // client.post(new Authenticate()
        //     .setProvider(""credentials"")
        //     .setUserName(""..."")
        //     .setPassword(""...""));";
        }
        private JavaGenerator Gen => new(new MetadataTypesConfig());
        public override string GetTypeName(string typeName, string[] genericArgs) => Gen.Type(typeName, genericArgs);

        public override string GetPropertyAssignment(MetadataPropertyType prop, string propValue) =>
            $"            .set{Gen.GetPropertyName(prop.Name).ToPascalCase()}({Value(prop.Type, propValue)})";

        /*
            .setNullableId(2)
            .setByte((short)3)
            .setShort((short)4)
            .setInt(5)
            .setLong((long)6)
            .setUShort(7)
            .setUInt((long)8)
            .setULong(java.math.BigInteger.valueOf(9))
            .setFloat((float)10.0)
            .setDouble(11.0)
            .setDecimal(java.math.BigDecimal.valueOf(12))
         */
        public override string Value(string typeName, string value) => typeName switch {
            nameof(Int32) => value,
            nameof(Byte) => $"(short){value}",
            nameof(SByte) => $"(short){value}",
            nameof(Int16) => $"(short){value}",
            nameof(UInt32) => $"(long){value}",
            nameof(Int64) => $"(long){value}",
            nameof(Double) => Float(value),
            nameof(Single) => $"(float){Float(value)}",
            nameof(UInt64) => $"java.math.BigInteger.valueOf({value})",
            nameof(Decimal) => $"java.math.BigDecimal.valueOf({value})",
            _ => value
        };

        public override string GetCollectionLiteral(string collectionBody, string collectionType, string elementType) =>
            "Utils.asList(" + collectionBody + ")";
        public override string GetByteArrayLiteral(byte[] bytes) => $"Utils.fromByteArray(\"{Convert.ToBase64String(bytes)}\")";
        public override string GetDateTimeLiteral(string value) => $"Utils.fromDateTime(\"{ISO8601(value)}\")";
        public override string GetTimeSpanLiteral(string value) => $"Utils.fromTimeSpan(\"{XsdDuration(value)}\")";
        public override string GetGuidLiteral(string value) => $"Utils.fromGuid(\"{value.ConvertTo<Guid>():D}\")";
        public override string GetCharLiteral(string value) => $"\"{value.ConvertTo<char>()}\"";
    }
}