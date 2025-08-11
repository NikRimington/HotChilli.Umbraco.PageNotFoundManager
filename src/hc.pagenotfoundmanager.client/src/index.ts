import { UmbEntryPointOnInit } from '@umbraco-cms/backoffice/extension-api';
import { manifests as entityActionManifests } from './Actions/Entity/manifest.ts';
import { manifests as modalManifests } from './Modals/manifest.ts';
import { UMB_AUTH_CONTEXT } from '@umbraco-cms/backoffice/auth';
import { client } from "./api/client.gen.ts";
export const onInit: UmbEntryPointOnInit = (_host, _extensionRegistry) => {

    // We can register many manifests at once via code 
    // as opposed to a long umbraco-package.json file
    _extensionRegistry.registerMany([
        ...entityActionManifests,
        ...modalManifests,
    ]);

    _host.consumeContext(UMB_AUTH_CONTEXT, async (authContext) => {
    // Get the token info from Umbraco
    const config = authContext?.getOpenApiConfiguration();

    client.setConfig({
      auth: config?.token ?? undefined,
      baseUrl: config?.base ?? "",
      credentials: config?.credentials ?? "same-origin",
    });
  });
    // We can register many manifests at once via code 
    // as opposed to a long umbraco-package.json file
    // _extensionRegistry.registerMany([
    //     ...entityActionManifests,
    //     ...modalManifests
    // ]);
};
