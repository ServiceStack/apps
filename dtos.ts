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


export interface IReturn<T>
{
    createResponse(): T;
}

export interface IReturnVoid
{
    createResponse(): void;
}

// @DataContract
export class ResponseError
{
    // @DataMember(Order=1)
    public errorCode: string;

    // @DataMember(Order=2)
    public fieldName: string;

    // @DataMember(Order=3)
    public message: string;

    // @DataMember(Order=4)
    public meta: { [index: string]: string; };

    public constructor(init?: Partial<ResponseError>) { (Object as any).assign(this, init); }
}

// @DataContract
export class ResponseStatus
{
    // @DataMember(Order=1)
    public errorCode: string;

    // @DataMember(Order=2)
    public message: string;

    // @DataMember(Order=3)
    public stackTrace: string;

    // @DataMember(Order=4)
    public errors: ResponseError[];

    // @DataMember(Order=5)
    public meta: { [index: string]: string; };

    public constructor(init?: Partial<ResponseStatus>) { (Object as any).assign(this, init); }
}

export class MetadataTypesConfig
{
    public baseUrl: string;
    public usePath: string;
    public makePartial: boolean;
    public makeVirtual: boolean;
    public makeInternal: boolean;
    public baseClass: string;
    public package: string;
    public addReturnMarker: boolean;
    public addDescriptionAsComments: boolean;
    public addDataContractAttributes: boolean;
    public addIndexesToDataMembers: boolean;
    public addGeneratedCodeAttributes: boolean;
    public addImplicitVersion?: number;
    public addResponseStatus: boolean;
    public addServiceStackTypes: boolean;
    public addModelExtensions: boolean;
    public addPropertyAccessors: boolean;
    public excludeGenericBaseTypes: boolean;
    public settersReturnThis: boolean;
    public makePropertiesOptional: boolean;
    public exportAsTypes: boolean;
    public excludeImplementedInterfaces: boolean;
    public addDefaultXmlNamespace: string;
    public makeDataContractsExtensible: boolean;
    public initializeCollections: boolean;
    public addNamespaces: string[];
    public defaultNamespaces: string[];
    public defaultImports: string[];
    public includeTypes: string[];
    public excludeTypes: string[];
    public treatTypesAsStrings: string[];
    public exportValueTypes: boolean;
    public globalNamespace: string;
    public excludeNamespace: boolean;
    public ignoreTypes: string[];
    public exportTypes: string[];
    public exportAttributes: string[];
    public ignoreTypesInNamespaces: string[];

    public constructor(init?: Partial<MetadataTypesConfig>) { (Object as any).assign(this, init); }
}

export class MetadataTypeName
{
    public name: string;
    public namespace: string;
    public genericArgs: string[];

    public constructor(init?: Partial<MetadataTypeName>) { (Object as any).assign(this, init); }
}

export class MetadataDataContract
{
    public name: string;
    public namespace: string;

    public constructor(init?: Partial<MetadataDataContract>) { (Object as any).assign(this, init); }
}

export class MetadataDataMember
{
    public name: string;
    public order?: number;
    public isRequired?: boolean;
    public emitDefaultValue?: boolean;

    public constructor(init?: Partial<MetadataDataMember>) { (Object as any).assign(this, init); }
}

export class MetadataAttribute
{
    public name: string;
    public constructorArgs: MetadataPropertyType[];
    public args: MetadataPropertyType[];

    public constructor(init?: Partial<MetadataAttribute>) { (Object as any).assign(this, init); }
}

export class MetadataPropertyType
{
    public name: string;
    public type: string;
    public isValueType?: boolean;
    public isSystemType?: boolean;
    public isEnum?: boolean;
    public isPrimaryKey?: boolean;
    public typeNamespace: string;
    public genericArgs: string[];
    public value: string;
    public description: string;
    public dataMember: MetadataDataMember;
    public readOnly?: boolean;
    public paramType: string;
    public displayType: string;
    public isRequired?: boolean;
    public allowableValues: string[];
    public allowableMin?: number;
    public allowableMax?: number;
    public attributes: MetadataAttribute[];

    public constructor(init?: Partial<MetadataPropertyType>) { (Object as any).assign(this, init); }
}

export class MetadataType
{
    public name: string;
    public namespace: string;
    public genericArgs: string[];
    public inherits: MetadataTypeName;
    public implements: MetadataTypeName[];
    public displayType: string;
    public description: string;
    public isNested?: boolean;
    public isEnum?: boolean;
    public isEnumInt?: boolean;
    public isInterface?: boolean;
    public isAbstract?: boolean;
    public dataContract: MetadataDataContract;
    public properties: MetadataPropertyType[];
    public attributes: MetadataAttribute[];
    public innerTypes: MetadataTypeName[];
    public enumNames: string[];
    public enumValues: string[];
    public enumMemberValues: string[];
    public enumDescriptions: string[];
    public meta: { [index: string]: string; };

    public constructor(init?: Partial<MetadataType>) { (Object as any).assign(this, init); }
}

export class MetadataRoute
{
    public path: string;
    public verbs: string;
    public notes: string;
    public summary: string;

    public constructor(init?: Partial<MetadataRoute>) { (Object as any).assign(this, init); }
}

export class MetadataOperationType
{
    public request: MetadataType;
    public response: MetadataType;
    public actions: string[];
    public returnsVoid: boolean;
    public returnType: MetadataTypeName;
    public routes: MetadataRoute[];
    public dataModel: MetadataTypeName;
    public viewModel: MetadataTypeName;
    public requiresAuth: boolean;
    public requiredRoles: string[];
    public requiresAnyRole: string[];
    public requiredPermissions: string[];
    public requiresAnyPermission: string[];
    public tags: string[];

    public constructor(init?: Partial<MetadataOperationType>) { (Object as any).assign(this, init); }
}

export class MetadataTypes
{
    public config: MetadataTypesConfig;
    public namespaces: string[];
    public types: MetadataType[];
    public operations: MetadataOperationType[];

    public constructor(init?: Partial<MetadataTypes>) { (Object as any).assign(this, init); }
}

export class GetSiteInfoLanguagesResponse
{
    public baseUrl: string;
    public slug: string;
    public languages: { [index: string]: string; };
    public responseStatus: ResponseStatus;

    public constructor(init?: Partial<GetSiteInfoLanguagesResponse>) { (Object as any).assign(this, init); }
}

export class SiteMetaResponse
{
    public api: MetadataTypes;
    public responseStatus: ResponseStatus;

    public constructor(init?: Partial<SiteMetaResponse>) { (Object as any).assign(this, init); }
}

// @Route("/sites")
// @Route("/sites/{Slug}/languages")
export class GetSiteInfoLanguages implements IReturn<GetSiteInfoLanguagesResponse>
{
    public baseUrl: string;
    public slug: string;

    public constructor(init?: Partial<GetSiteInfoLanguages>) { (Object as any).assign(this, init); }
    public createResponse() { return new GetSiteInfoLanguagesResponse(); }
    public getTypeName() { return 'GetSiteInfoLanguages'; }
}

// @DataContract
export class SiteInvoke implements IReturn<string>
{
    // @DataMember(Order=1)
    public slug: string;

    // @DataMember(Order=2)
    public request: string;

    // @DataMember(Order=3)
    public args: string[];

    public constructor(init?: Partial<SiteInvoke>) { (Object as any).assign(this, init); }
    public createResponse() { return ''; }
    public getTypeName() { return 'SiteInvoke'; }
}

// @DataContract
export class SiteProxy implements IReturn<Uint8Array>
{
    // @DataMember(Order=1)
    public slug: string;

    // @DataMember(Order=2)
    public request: string;

    // @DataMember(Order=3)
    public query: string[];

    public constructor(init?: Partial<SiteProxy>) { (Object as any).assign(this, init); }
    public createResponse() { return new Uint8Array(0); }
    public getTypeName() { return 'SiteProxy'; }
}

// @Route("/gists")
// @Route("/gists/{Slug}/{Lang}")
// @Route("/gists/{Slug}/{Lang}/{IncludeTypes}")
export class GistRef
{
    public slug: string;
    public lang: string;
    public includeTypes: string;
    public noCache?: boolean;

    public constructor(init?: Partial<GistRef>) { (Object as any).assign(this, init); }
}

// @Route("/gists/files/{Slug}/{Lang}/{File}")
export class GistRefFile
{
    public slug: string;
    public lang: string;
    public file: string;

    public constructor(init?: Partial<GistRefFile>) { (Object as any).assign(this, init); }
}

// @Route("/sites/{Slug}/meta")
export class SiteMeta implements IReturn<SiteMetaResponse>
{
    public slug: string;
    public noCache?: boolean;

    public constructor(init?: Partial<SiteMeta>) { (Object as any).assign(this, init); }
    public createResponse() { return new SiteMetaResponse(); }
    public getTypeName() { return 'SiteMeta'; }
}

