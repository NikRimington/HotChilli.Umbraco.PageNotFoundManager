import { UmbEntryPointOnInit } from '@umbraco-cms/backoffice/extension-api';
import { manifests as entityActionManifests } from './Actions/Entity/manifest.ts';
import { manifests as modalManifests } from './Modals/manifest.ts';
import { UMB_AUTH_CONTEXT } from '@umbraco-cms/backoffice/auth';
import { OpenAPI } from './api/index.ts';
export const onInit: UmbEntryPointOnInit = (_host, _extensionRegistry) => {

    // We can register many manifests at once via code 
    // as opposed to a long umbraco-package.json file
    _extensionRegistry.registerMany([
        ...entityActionManifests,
        ...modalManifests,
    ]);

    _host.consumeContext(UMB_AUTH_CONTEXT, (_auth) => {
        const umbOpenApi = _auth.getOpenApiConfiguration();
        OpenAPI.TOKEN = umbOpenApi.token;
        OpenAPI.BASE = umbOpenApi.base;
        OpenAPI.WITH_CREDENTIALS = umbOpenApi.withCredentials;
    });
    // We can register many manifests at once via code 
    // as opposed to a long umbraco-package.json file
    // _extensionRegistry.registerMany([
    //     ...entityActionManifests,
    //     ...modalManifests
    // ]);
};
