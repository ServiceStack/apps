/* Options:
Date: 2021-02-25 19:01:55
Version: 5.105
Tip: To override a DTO option, remove "//" prefix before updating
BaseUrl: https://localhost:5002

//GlobalNamespace:
//MakePropertiesOptional: False
//AddServiceStackTypes: True
//AddResponseStatus: False
//AddImplicitVersion:
//AddDescriptionAsComments: True
//IncludeTypes:
//ExcludeTypes:
//DefaultImports:
*/
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports"], factory);
    }
})(function (require, exports) {
    "use strict";
    exports.__esModule = true;
    exports.SiteMeta = exports.GistRefFile = exports.GistRef = exports.SiteProxy = exports.SiteInvoke = exports.GetSiteInfoLanguages = exports.SiteMetaResponse = exports.GetSiteInfoLanguagesResponse = exports.MetadataTypes = exports.MetadataOperationType = exports.MetadataRoute = exports.MetadataType = exports.MetadataPropertyType = exports.MetadataAttribute = exports.MetadataDataMember = exports.MetadataDataContract = exports.MetadataTypeName = exports.MetadataTypesConfig = exports.ResponseStatus = exports.ResponseError = void 0;
    // @DataContract
    var ResponseError = /** @class */ (function () {
        function ResponseError(init) {
            Object.assign(this, init);
        }
        return ResponseError;
    }());
    exports.ResponseError = ResponseError;
    // @DataContract
    var ResponseStatus = /** @class */ (function () {
        function ResponseStatus(init) {
            Object.assign(this, init);
        }
        return ResponseStatus;
    }());
    exports.ResponseStatus = ResponseStatus;
    var MetadataTypesConfig = /** @class */ (function () {
        function MetadataTypesConfig(init) {
            Object.assign(this, init);
        }
        return MetadataTypesConfig;
    }());
    exports.MetadataTypesConfig = MetadataTypesConfig;
    var MetadataTypeName = /** @class */ (function () {
        function MetadataTypeName(init) {
            Object.assign(this, init);
        }
        return MetadataTypeName;
    }());
    exports.MetadataTypeName = MetadataTypeName;
    var MetadataDataContract = /** @class */ (function () {
        function MetadataDataContract(init) {
            Object.assign(this, init);
        }
        return MetadataDataContract;
    }());
    exports.MetadataDataContract = MetadataDataContract;
    var MetadataDataMember = /** @class */ (function () {
        function MetadataDataMember(init) {
            Object.assign(this, init);
        }
        return MetadataDataMember;
    }());
    exports.MetadataDataMember = MetadataDataMember;
    var MetadataAttribute = /** @class */ (function () {
        function MetadataAttribute(init) {
            Object.assign(this, init);
        }
        return MetadataAttribute;
    }());
    exports.MetadataAttribute = MetadataAttribute;
    var MetadataPropertyType = /** @class */ (function () {
        function MetadataPropertyType(init) {
            Object.assign(this, init);
        }
        return MetadataPropertyType;
    }());
    exports.MetadataPropertyType = MetadataPropertyType;
    var MetadataType = /** @class */ (function () {
        function MetadataType(init) {
            Object.assign(this, init);
        }
        return MetadataType;
    }());
    exports.MetadataType = MetadataType;
    var MetadataRoute = /** @class */ (function () {
        function MetadataRoute(init) {
            Object.assign(this, init);
        }
        return MetadataRoute;
    }());
    exports.MetadataRoute = MetadataRoute;
    var MetadataOperationType = /** @class */ (function () {
        function MetadataOperationType(init) {
            Object.assign(this, init);
        }
        return MetadataOperationType;
    }());
    exports.MetadataOperationType = MetadataOperationType;
    var MetadataTypes = /** @class */ (function () {
        function MetadataTypes(init) {
            Object.assign(this, init);
        }
        return MetadataTypes;
    }());
    exports.MetadataTypes = MetadataTypes;
    var GetSiteInfoLanguagesResponse = /** @class */ (function () {
        function GetSiteInfoLanguagesResponse(init) {
            Object.assign(this, init);
        }
        return GetSiteInfoLanguagesResponse;
    }());
    exports.GetSiteInfoLanguagesResponse = GetSiteInfoLanguagesResponse;
    var SiteMetaResponse = /** @class */ (function () {
        function SiteMetaResponse(init) {
            Object.assign(this, init);
        }
        return SiteMetaResponse;
    }());
    exports.SiteMetaResponse = SiteMetaResponse;
    // @Route("/sites")
    // @Route("/sites/{Slug}/languages")
    var GetSiteInfoLanguages = /** @class */ (function () {
        function GetSiteInfoLanguages(init) {
            Object.assign(this, init);
        }
        GetSiteInfoLanguages.prototype.createResponse = function () { return new GetSiteInfoLanguagesResponse(); };
        GetSiteInfoLanguages.prototype.getTypeName = function () { return 'GetSiteInfoLanguages'; };
        return GetSiteInfoLanguages;
    }());
    exports.GetSiteInfoLanguages = GetSiteInfoLanguages;
    // @DataContract
    var SiteInvoke = /** @class */ (function () {
        function SiteInvoke(init) {
            Object.assign(this, init);
        }
        SiteInvoke.prototype.createResponse = function () { return ''; };
        SiteInvoke.prototype.getTypeName = function () { return 'SiteInvoke'; };
        return SiteInvoke;
    }());
    exports.SiteInvoke = SiteInvoke;
    // @DataContract
    var SiteProxy = /** @class */ (function () {
        function SiteProxy(init) {
            Object.assign(this, init);
        }
        SiteProxy.prototype.createResponse = function () { return new Uint8Array(0); };
        SiteProxy.prototype.getTypeName = function () { return 'SiteProxy'; };
        return SiteProxy;
    }());
    exports.SiteProxy = SiteProxy;
    // @Route("/gists")
    // @Route("/gists/{Slug}/{Lang}")
    // @Route("/gists/{Slug}/{Lang}/{IncludeTypes}")
    var GistRef = /** @class */ (function () {
        function GistRef(init) {
            Object.assign(this, init);
        }
        return GistRef;
    }());
    exports.GistRef = GistRef;
    // @Route("/gists/files/{Slug}/{Lang}/{File}")
    var GistRefFile = /** @class */ (function () {
        function GistRefFile(init) {
            Object.assign(this, init);
        }
        return GistRefFile;
    }());
    exports.GistRefFile = GistRefFile;
    // @Route("/sites/{Slug}/meta")
    var SiteMeta = /** @class */ (function () {
        function SiteMeta(init) {
            Object.assign(this, init);
        }
        SiteMeta.prototype.createResponse = function () { return new SiteMetaResponse(); };
        SiteMeta.prototype.getTypeName = function () { return 'SiteMeta'; };
        return SiteMeta;
    }());
    exports.SiteMeta = SiteMeta;
});
