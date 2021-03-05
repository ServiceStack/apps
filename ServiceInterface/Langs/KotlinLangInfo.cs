using System.Collections.Generic;
using ServiceStack;
using ServiceStack.NativeTypes.Kotlin;

namespace Apps.ServiceInterface.Langs
{
    public class KotlinLangInfo : LangInfo
    {
        public KotlinLangInfo()
        {
            Code = "kotlin";
            Name = "Kotlin";
            Ext = "kt";
            DtosPathPrefix = "src\\main\\kotlin\\myapp\\";
            Files = new Dictionary<string, string> {
                ["src\\main\\kotlin\\myapp\\App.kt"] = @"package myapp
import net.servicestack.client.JsonServiceClient
import net.servicestack.gistcafe.Inspect

fun main(args: Array<String>) {
    val client = JsonServiceClient(""{BASE_URL}""){REQUIRES_AUTH}

    {API_COMMENT}val response = client.send({REQUEST}().apply {{REQUEST_BODY}
    {API_COMMENT}});

    {API_COMMENT}Inspect.printDump(response)
    {INSPECT_VARS}
}
",
                ["gradle\\wrapper\\gradle-wrapper.properties"] = @"distributionBase=GRADLE_USER_HOME
distributionPath=wrapper/dists
distributionUrl=https\://services.gradle.org/distributions/gradle-6.7-bin.zip
zipStoreBase=GRADLE_USER_HOME
zipStorePath=wrapper/dists",
                ["build.gradle.kts"] = @"plugins {
    java
    maven
    application
    kotlin(""jvm"") version ""1.4.21""
}

group = ""myapp""
version = ""1.0-SNAPSHOT""

application {
    mainClass.set(""myapp.AppKt"")
}

repositories {
    mavenCentral()
}

tasks.test {
    useJUnitPlatform()
}

dependencies {
    implementation(kotlin(""stdlib""))
    implementation(""com.google.code.gson:gson:2.8.6"")
    implementation(""net.servicestack:client:1.0.44"")
    implementation(""net.servicestack:gistcafe:0.0.7"")
    testImplementation(platform(""org.junit:junit-bom:5.7.0""))
    testImplementation(""org.junit.jupiter:junit-jupiter"")
}
",
            };
            InspectVarsResponse = "Inspect.vars(mapOf(\"response\" to response))";
            RequiresAuthTemplate = @"
    // Authentication is required
    // client.post(new Authenticate().apply {
    //     provider = ""credentials""
    //     userName = ""...""
    //     password = ""..."" })";
        }
        private KotlinGenerator Gen => new(new MetadataTypesConfig());
        public override string GetTypeName(string typeName, string[] genericArgs) => Gen.Type(typeName, genericArgs);

        public override string GetPropertyAssignment(MetadataPropertyType prop, string propValue) =>
            $"        {prop.Name.ToCamelCase()} = {propValue}";

        public override string GetLiteralCollection(bool isArray, string collectionBody, string collectionType) => 
            "arrayListOf(" + collectionBody + ")";
    }
}