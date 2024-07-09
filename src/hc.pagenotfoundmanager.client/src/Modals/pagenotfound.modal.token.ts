import { UmbModalToken } from "@umbraco-cms/backoffice/modal";
export interface PageNotFoundModalData {
    entityKey: string | null;
    target: string | null;
}

export interface PageNotFoundModalValue {
}

export const PageNotFound_MODAL = new UmbModalToken<PageNotFoundModalData, PageNotFoundModalValue>('pagenotfound.modal', {
    modal: {
        type: "sidebar",
        size: "medium" // full, large, medium, small
    }
});