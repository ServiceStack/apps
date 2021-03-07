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

import net.servicestack.client.JsonServiceClient;
import net.servicestack.gistcafe.Inspect;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collections;

import static myapp.dtos.*;

public class App {

    public static void main(String[] args) {

        JsonServiceClient client = new JsonServiceClient(
            ""{BASE_URL}"");{REQUIRES_AUTH}

        {API_COMMENT}{RESPONSE} response = client.send(({REQUEST})
        {API_COMMENT}    new {REQUEST}(){REQUEST_BODY});

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
    implementation 'net.servicestack:client:1.0.44'
    implementation 'net.servicestack:gistcafe:0.0.7'
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
            $"            .set{prop.Name.ToPascalCase()}({propValue})";

        public override string GetLiteralCollection(bool isArray, string collectionBody, string collectionType) => 
            "new ArrayList(Arrays.asList(" + collectionBody + "))";
    }
}