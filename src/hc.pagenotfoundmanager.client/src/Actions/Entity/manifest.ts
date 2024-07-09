import { UMB_DOCUMENT_ENTITY_TYPE } from "@umbraco-cms/backoffice/document";
import { ManifestEntityAction } from "@umbraco-cms/backoffice/extension-registry";
import { PageNotFoundEntityAction } from "./pagenotfound.entityaction.ts";

const entityAction: ManifestEntityAction = {
    type: 'entityAction',
    kind: 'default',
    alias: 'hcs.pagenotfound.entity.action',
    name: 'HCS: Page Not Found Manager: Action',
    weight: 400,
    api: PageNotFoundEntityAction,
    meta: {
        icon: 'icon-sensor',
        label: '404 Manager',
    },
    forEntityTypes: [ UMB_DOCUMENT_ENTITY_TYPE ]
}
export const manifests = [entityAction];