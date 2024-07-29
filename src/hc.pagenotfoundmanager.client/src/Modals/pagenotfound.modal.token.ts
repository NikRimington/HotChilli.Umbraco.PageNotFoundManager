import { UmbModalToken } from "@umbraco-cms/backoffice/modal";
import { PageNotFoundDetails } from "../api/types.gen";
export interface PageNotFoundModalData {
    entityKey: string | null;
    target: PageNotFoundDetails | undefined | null;
}

export interface PageNotFoundModalValue {
    currentNodeName: string | undefined;
    selectedNodeName: string | undefined;
}

export const PageNotFound_MODAL = new UmbModalToken<PageNotFoundModalData, PageNotFoundModalValue>('hcs.pagenotfound.modal', {
    modal: {
        type: "sidebar",
        size: "medium" // full, large, medium, small
    }
});