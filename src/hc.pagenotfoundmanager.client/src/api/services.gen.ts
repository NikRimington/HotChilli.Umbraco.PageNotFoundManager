// This file is auto-generated by @hey-api/openapi-ts

import type { CancelablePromise } from './core/CancelablePromise';
import { OpenAPI } from './core/OpenAPI';
import { request as __request } from './core/request';
import type { GetApiV1HcsGetNotFoundData, GetApiV1HcsGetNotFoundResponse, PostApiV1HcsSetNotFoundData, PostApiV1HcsSetNotFoundResponse } from './types.gen';

export class PageNotFoundManagerService {
    /**
     * @param data The data for the request.
     * @param data.pageId
     * @returns unknown OK
     * @throws ApiError
     */
    public static getApiV1HcsGetNotFound(data: GetApiV1HcsGetNotFoundData = {}): CancelablePromise<GetApiV1HcsGetNotFoundResponse> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/v1/hcs/get-not-found',
            query: {
                pageId: data.pageId
            },
            errors: {
                401: 'The resource is protected and requires an authentication token'
            }
        });
    }
    
    /**
     * @param data The data for the request.
     * @param data.requestBody
     * @returns unknown OK
     * @throws ApiError
     */
    public static postApiV1HcsSetNotFound(data: PostApiV1HcsSetNotFoundData = {}): CancelablePromise<PostApiV1HcsSetNotFoundResponse> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/v1/hcs/set-not-found',
            body: data.requestBody,
            mediaType: 'application/json',
            errors: {
                401: 'The resource is protected and requires an authentication token'
            }
        });
    }
    
}