// This file is auto-generated by @hey-api/openapi-ts

export enum EventMessageTypeModel {
    DEFAULT = 'Default',
    INFO = 'Info',
    ERROR = 'Error',
    SUCCESS = 'Success',
    WARNING = 'Warning'
}

export type NotificationHeaderModel = {
    message: string;
    category: string;
    type: EventMessageTypeModel;
};

export type PageNotFoundDetails = {
    pageId: string;
    explicit404?: string | null;
    inherited404?: PageNotFoundDetails | null;
};

export type PageNotFoundRequest = {
    notFoundPageId?: string | null;
    parentId: string;
};

export type GetApiV1HcsGetNotFoundData = {
    pageId?: string;
};

export type GetApiV1HcsGetNotFoundResponse = PageNotFoundDetails;

export type PostApiV1HcsSetNotFoundData = {
    requestBody?: PageNotFoundRequest;
};

export type PostApiV1HcsSetNotFoundResponse = PageNotFoundDetails;

export type $OpenApiTs = {
    '/api/v1/hcs/get-not-found': {
        get: {
            req: GetApiV1HcsGetNotFoundData;
            res: {
                /**
                 * OK
                 */
                200: PageNotFoundDetails;
                /**
                 * The resource is protected and requires an authentication token
                 */
                401: unknown;
            };
        };
    };
    '/api/v1/hcs/set-not-found': {
        post: {
            req: PostApiV1HcsSetNotFoundData;
            res: {
                /**
                 * OK
                 */
                200: PageNotFoundDetails;
                /**
                 * The resource is protected and requires an authentication token
                 */
                401: unknown;
            };
        };
    };
};