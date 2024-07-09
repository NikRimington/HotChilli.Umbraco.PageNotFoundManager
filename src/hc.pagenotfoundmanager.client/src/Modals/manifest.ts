import { ManifestModal } from "@umbraco-cms/backoffice/extension-registry";

const pageNotFoundModal: ManifestModal = {
    type: 'modal',
    alias: 'hcs.pagenotfound.modal',
    name: 'Page Not Found Modal',
    js: () => import('./pagenotfound.modal.element.ts')
}
export const manifests = [pageNotFoundModal];