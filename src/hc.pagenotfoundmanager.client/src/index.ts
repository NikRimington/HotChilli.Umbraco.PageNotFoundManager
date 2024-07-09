import { UmbEntryPointOnInit } from '@umbraco-cms/backoffice/extension-api';
import { manifests as entityActionManifests } from './Actions/Entity/manifest.ts';
import { manifests as modalManifests } from './Modals/manifest.ts';

export const onInit: UmbEntryPointOnInit = (_host, _extensionRegistry) => {

    // We can register many manifests at once via code 
    // as opposed to a long umbraco-package.json file
    _extensionRegistry.registerMany([
        ...entityActionManifests,
        ...modalManifests,
    ]);

    // We can register many manifests at once via code 
    // as opposed to a long umbraco-package.json file
    // _extensionRegistry.registerMany([
    //     ...entityActionManifests,
    //     ...modalManifests
    // ]);
};
