let path1 = "techstacks.io"
let path2 = "http:techstacks.io"
let path3 = "techstacks.io/csharp"
let path4 = "techstacks.io/csharp/FindTechnologies"
let path5 = "techstacks.io/csharp/AutoQuery/FindTechnologies"
let path6 = "techstacks.io/csharp/FindTechnologies(VendorName:Google,Take:10,OrderByDesc:ViewCount,Fields:%22Id,%20Name,%20VendorName,%20ViewCount%22)"
let path7 = "techstacks.io/csharp/AutoQuery/FindTechnologies(VendorName:Google,Take:10,OrderByDesc:ViewCount,Fields:%22Id,%20Name,%20VendorName,%20ViewCount%22)"
//https://localhost:5002/apps/#test.servicestack.net/dart/AllTypes(Id:1,NullableId:2,Byte:3,Short:4,Int:5,Long:6,DateTime:%222021-12-30T23:59%22,Guid:bc525c18-ac21-4a60-a2be-6187c9af2716,Char:a,StringList:[AB,%20C,%20DEF],StringArray:[AB,%20C,%20DEF])

function leftPart(strVal, needle) {
    if (strVal == null) return null;
    let pos = strVal.indexOf(needle);
    return pos === -1
        ? strVal
        : strVal.substring(0, pos);
}
function rightPart(strVal, needle) {
    if (strVal == null) return null;
    let pos = strVal.indexOf(needle);
    return pos === -1
        ? strVal
        : strVal.substring(pos + needle.length);
}
function lastLeftPart(strVal, needle) {
    if (strVal == null) return null;
    let pos = strVal.lastIndexOf(needle);
    return pos === -1
        ? strVal
        : strVal.substring(0, pos);
}
function lastRightPart(strVal, needle) {
    if (strVal == null) return null;
    let pos = strVal.lastIndexOf(needle);
    return pos === -1
        ? strVal
        : strVal.substring(pos + needle.length);
}
function isInteger(str) {
    let n = Math.floor(Number(str));
    return n !== Infinity && String(n) === str && n >= 0;
}
function urlToSlug(url) {
    let slug = url;
    if (slug.startsWith("https://"))
        slug = slug.substring("https://".length);
    else if (slug.startsWith("http://"))
        slug = "http:" + slug.substring("http://".length);
    slug = slug.replace(/\//g, ':');
    return slug;
}
function urlFromSlug(slug) {
    let url = slug;
    let isUrl = url.startsWith("https://") || url.startsWith("http://");
    let scheme = !isUrl && (url.startsWith("http:") || url.startsWith("https:"))
        ? leftPart(url,':')
        : null;
    if (scheme != null)
        url = rightPart(url,':');

    let firstPos = url.indexOf(':');
    if (!isUrl && firstPos >= 0) {
        let atPort = rightPart(url,':');
        let delim1Pos = atPort.indexOf(':');
        let delim2Pos = atPort.indexOf('/');
        let endPos = delim1Pos >= 0 && delim2Pos >= 0
            ? Math.min(delim1Pos, delim2Pos)
            : Math.max(delim1Pos, delim2Pos);
        let testPort = endPos >= 0
            ? atPort.substring(0,endPos)
            : atPort.substring(0,atPort.length - 1);
        url = isInteger(testPort)
            ? leftPart(url,':') + ':' + unSlash(atPort)
            : leftPart(url,':') + '/' + unSlash(atPort);
    }
    url = decodeURIComponent(url);
    if (!isUrl) {
        url = scheme != null
            ? scheme + "://" + url
            : "https://" + url;
    }
    return url;
}
function unSlash(urlComponent) {
    // don't replace ':' after '('...)
    if (urlComponent.indexOf('(') >= 0) {
        let target = leftPart(urlComponent,'(');
        let suffix = rightPart(urlComponent,'(');
        return target.replace(/:/g, '/') + '(' + suffix;
    }
    return urlComponent.replace(/:/g, '/');
}
function parsePath(path) {
    let args = path.indexOf('(') >= 0 ? lastRightPart(path.substring(0,path.length-1),'(') : null;
    if (args) path = lastLeftPart(path,'(');
    let segments = path.split('/');
    let url = urlFromSlug(segments[0]);
    let parts = url.split('://');
    let hasScheme = parts.length > 1;
    let baseUrl = !hasScheme
        ? parts[0]
        : parts[1];
    let scheme = hasScheme ? parts[0] : this.scheme;
    if (scheme !== 'http' && scheme !== 'https') {
        return urlError({ message:'Invalid Scheme', field:'#scheme' });
    }
    let lang = segments.length > 1 ? segments[1] : null;
    let op = segments.length === 4
        ? segments[3]
        : segments.length === 3
            ? segments[2]
            : null;
    let tag = segments.length === 4 ? segments[2] : null;
    return  { scheme:scheme, baseUrl:baseUrl, lang:lang, tag:tag, op:op, args:args, segments:segments };
}

let tests = 0;
let E = []

function objectEq(o1,o2) {
    tests++;
    let keys1 = Object.keys(o1), values1 = Object.values(o2);
    let keys2 = Object.keys(o1), values2 = Object.values(o2);
    if (keys1.length !== values1.length) E.push(`FAIL: length: ${keys1.length} != ${keys2.length}`);
    keys1.forEach(k => {
        if (o1[k] == null || o2[k] == null) {
            if (o1[k] !== o2[k]) E.push(`FAIL: null: ${o1[k]} != ${o2[k]}`)
        }
        else if (o1[k].length) {
            if (o1[k].length !== o2[k].length)
                E.push(`FAIL: array.length: ${o1[k].length} != ${o2[k].length}`)
        } 
        else if (o1[k] !== o2[k]) {
            E.push(`FAIL: values: ${o1[k]} != ${o2[k]}`)
        }
    });
}

objectEq(parsePath(path1), JSON.parse(`{"scheme":"https","baseUrl":"techstacks.io","lang":null,"tag":null,"op":null,"args":null,"segments":["techstacks.io"]}`));
objectEq(parsePath(path2), JSON.parse(`{"scheme":"http","baseUrl":"techstacks.io","lang":null,"tag":null,"op":null,"args":null,"segments":["http:techstacks.io"]}`));
objectEq(parsePath(path3), JSON.parse(`{"scheme":"https","baseUrl":"techstacks.io","lang":"csharp","tag":null,"op":null,"args":null,"segments":["techstacks.io","csharp"]}`));
objectEq(parsePath(path4), JSON.parse(`{"scheme":"https","baseUrl":"techstacks.io","lang":"csharp","tag":null,"op":"FindTechnologies","args":null,"segments":["techstacks.io","csharp","FindTechnologies"]}`));
objectEq(parsePath(path5), JSON.parse(`{"scheme":"https","baseUrl":"techstacks.io","lang":"csharp","tag":"AutoQuery","op":"FindTechnologies","args":null,"segments":["techstacks.io","csharp","AutoQuery","FindTechnologies"]}`));
objectEq(parsePath(path6), JSON.parse(`{"scheme":"https","baseUrl":"techstacks.io","lang":"csharp","tag":null,"op":"FindTechnologies","args":"VendorName:Google,Take:10,OrderByDesc:ViewCount,Fields:%22Id,%20Name,%20VendorName,%20ViewCount%22","segments":["techstacks.io","csharp","FindTechnologies"]}`));
objectEq(parsePath(path7), JSON.parse(`{"scheme":"https","baseUrl":"techstacks.io","lang":"csharp","tag":"AutoQuery","op":"FindTechnologies","args":"VendorName:Google,Take:10,OrderByDesc:ViewCount,Fields:%22Id,%20Name,%20VendorName,%20ViewCount%22","segments":["techstacks.io","csharp","AutoQuery","FindTechnologies"]}`));

if (E.length === 0) console.log(`PASS: ${tests} tests.`);
else E.forEach(e => console.error(e));
