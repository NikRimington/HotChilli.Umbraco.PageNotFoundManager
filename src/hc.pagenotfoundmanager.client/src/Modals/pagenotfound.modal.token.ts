import { UmbModalToken } from "@umbraco-cms/backoffice/modal";
export interface PageNotFoundModalData {
    entityKey: string | null;
    target: string | undefined | null;
}

export interface PageNotFoundModalValue {
    
}

export const PageNotFound_MODAL = new UmbModalToken<PageNotFoundModalData, PageNotFoundModalValue>('hcs.pagenotfound.modal', {
    modal: {
        type: "sidebar",
        size: "medium" // full, large, medium, small
    }
});