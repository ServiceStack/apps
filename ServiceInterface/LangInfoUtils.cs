using System;
using System.Collections.Generic;
using Apps.ServiceInterface.Langs;

namespace Apps.ServiceInterface
{
    public class LangInfoUtils
    {
        public static LangInfo CSharp = new CSharpLangInfo();
        public static LangInfo TypeScript = new TypeScriptLangInfo();
        public static LangInfo Python = new PythonLangInfo();
        public static LangInfo Swift = new SwiftLangInfo();
        public static LangInfo Java = new JavaLangInfo();
        public static LangInfo Kotlin = new KotlinLangInfo();
        public static LangInfo Dart = new DartLangInfo();
        public static LangInfo FSharp = new FSharpLangInfo();
        public static LangInfo VbNet = new VbNetLangInfo();
        
        public static Dictionary<string, LangInfo> LangAliases { get; set; } = new() {
            ["csharp"] = CSharp,
            ["cs"] = CSharp,
            ["python"] = Python,
            ["py"] = Python,
            ["typescript"] = TypeScript,
            ["ts"] = TypeScript,
            ["swift"] = Swift,
            ["sw"] = Swift,
            ["java"] = Java,
            ["ja"] = Java,
            ["kotlin"] = Kotlin,
            ["kt"] = Kotlin,
            ["dart"] = Dart,
            ["da"] = Dart,
            ["fsharp"] = FSharp,
            ["fs"] = FSharp,
            ["vbnet"] = VbNet,
            ["vb"] = VbNet,
        };

        public static ArgumentException UnknownLanguageError(string arg) => 
            new("Unknown Language, choose from: csharp, typescript, swift, java, kotlin, dart, fsharp or vbnet", arg);

        public static LangInfo AssertLangInfo(string lang)
        {
            if (!LangAliases.TryGetValue(lang, out var langInfo))
                throw UnknownLanguageError(nameof(lang));
            return langInfo;
        }
    }
}