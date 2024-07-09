import { customElement, html, state } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalBaseElement, UmbModalRejectReason } from "@umbraco-cms/backoffice/modal";
import type { UmbInputDocumentElement } from "@umbraco-cms/backoffice/document";
import { UmbDocumentItemRepository } from "@umbraco-cms/backoffice/document";
//import {UmbPropertyEditorUIContentPickerElement} from "@umbraco-cms/backoffice/property-editor/"
//import { tryExecuteAndNotify } from '@umbraco-cms/backoffice/resources';

//import { TemplateResult, css } from "lit";

import { PageNotFoundModalData, PageNotFoundModalValue } from "./pagenotfound.modal.token.ts";
//import { ExaminePeekService, ISearchResult } from "../api/index.ts";
//import { UUIButtonElement } from "@umbraco-cms/backoffice/external/uui";

@customElement('page-not-found-modal')
export class PageNotFoundModalElement extends UmbModalBaseElement<PageNotFoundModalData, PageNotFoundModalValue>
{
    @state()
	private _selection?: string;

    @state()
	private _documentName = '';

    constructor() {
        super();
    }

    connectedCallback() {
        super.connectedCallback();

        if(this.data?.entityKey){
            // Use Swagger API client to get the record
            // this._getExamineRecord(this.data.entityKey).then((record) => {
            //     this.examineRecord = record;
            // });
            console.log(`PNFM: ${this.data?.entityKey}`);
        }
        else {
            // This is a problem
            console.error("PNFM: connectedCallback. There is NO EntityKey passed into the modal");
        }

        this.#getDocumentName();
    }

    private handleClose() {
        this.modalContext?.reject({ type: "close" } as UmbModalRejectReason);
    }

    #selectionChanged(e: CustomEvent) {
		this._selection = (e.target as UmbInputDocumentElement).selection[0];
	}

    async #getDocumentName() {
		if (!this.data?.entityKey) return;
		// Should this be done here or in the action file?
		const { data } = await new UmbDocumentItemRepository(this).requestItems([this.data.entityKey]);
		if (!data) return;
		const item = data[0];
		//TODO How do we ensure we get the correct variant?
		this._documentName = item.variants[0]?.name;

	}

    render() {
        console.log(this.data);
        return html`
            <umb-body-layout headline="Page Not Found #BETA#">
                <uui-box headline="Data">
                    Current node: "${this._documentName}"

                    <umb-input-document min=1 max=1
                        .value=${this._selection}
                        @change=${this.#selectionChanged}>
                    </umb-input-document>
                </uui-box>
                                
                <div slot="actions">
                    <uui-button id="close" label="Close" @click="${this.handleClose}">Close</uui-button>
                </div>
            </umb-body-layout>
        `;
    }
}

export default PageNotFoundModalElement;