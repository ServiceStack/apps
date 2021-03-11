using System;
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
import net.servicestack.client.*

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
    implementation(""net.servicestack:client:1.0.49"")
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
        public override string New(string ctor) => ctor; //no new
        public override string GetPropertyAssignment(MetadataPropertyType prop, string propValue) =>
            $"        {Gen.GetPropertyName(prop.Name)} = {Value(prop.Type, propValue)}";
        /*
        uLong = 9.toBigInteger()
        Float = 10.toFloat()
        Double = 11.0
        decimal = 12.toBigDecimal()
         */
        public override string Value(string typeName, string value) => typeName switch {
            nameof(Int32) => value,
            nameof(Double) => Float(value),
            nameof(UInt64) => $"{value}.toBigInteger()",
            nameof(Single) => $"{Float(value)}.toFloat()",
            nameof(Decimal) => $"{value}.toBigDecimal()",
            _ => value
        };

        public override string GetCollectionLiteral(string collectionBody, string collectionType, string elementType) =>
            "arrayListOf(" + collectionBody + ")";
        public override string GetByteArrayLiteral(byte[] bytes) => $"Utils.fromByteArray(\"{Convert.ToBase64String(bytes)}\")";
        public override string GetDateTimeLiteral(string value) => $"Utils.fromDateTime(\"{ISO8601(value)}\")";
        public override string GetTimeSpanLiteral(string value) => $"Utils.fromTimeSpan(\"{XsdDuration(value)}\")";
        public override string GetGuidLiteral(string value) => $"Utils.fromGuid(\"{value.ConvertTo<Guid>():D}\")";
        public override string GetCharLiteral(string value) => $"\"{value.ConvertTo<char>()}\"";
    }
}