import { customElement, html, state } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalBaseElement, UmbModalRejectReason } from "@umbraco-cms/backoffice/modal";
import type { UmbInputDocumentElement } from "@umbraco-cms/backoffice/document";
import { UmbDocumentItemRepository } from "@umbraco-cms/backoffice/document";

import { PageNotFoundModalData, PageNotFoundModalValue } from "./pagenotfound.modal.token.ts";
import { PageNotFoundManagerService } from "../api/services.gen.ts";

@customElement('page-not-found-modal')
export class PageNotFoundModalElement extends UmbModalBaseElement<PageNotFoundModalData, PageNotFoundModalValue>
{
    @state()
	private _selection: string | undefined | null;

    private _inherited404?: string;
    private _inheritsFrom?: string;

    @state()
	private _documentName = '';

    constructor() {
        super();
    }

    async connectedCallback() {
        super.connectedCallback();

        if(!this.data?.entityKey){
            // This is a problem
            console.error("PNFM: connectedCallback. There is NO EntityKey passed into the modal");
        }

        if(this.data?.target){
            this._selection = this.data?.target?.explicit404;
            if(this.data?.target?.inherited404?.explicit404)
            {
                this._inherited404 = await this.#getDocumentName(this.data?.target?.inherited404?.explicit404);
                this._inheritsFrom = await this.#getDocumentName(this.data?.target?.inherited404?.pageId);

                console.log(`inherits '${this._inherited404}' From:'${this._inheritsFrom}'`);
            }
        }

        this._documentName = (await this.#getDocumentName(this.data?.entityKey)) ?? '';
    }

    private handleClose() {
        this.modalContext?.reject({ type: "close" } as UmbModalRejectReason);
    }

    private async handleSave() {
        var res = await PageNotFoundManagerService.postApiV1HcsSetNotFound(
            {
                requestBody: {
                    parentId: this.data?.entityKey ?? "",
                    notFoundPageId: this._selection
                }
            }
        );

        console.log(res);

        this.value = {
            currentNodeName: this._documentName,
            selectedNodeName: await this.#getDocumentName(this._selection)
        }
        
        this.modalContext?.submit();
    }

    private _getIntroductionMessage(){
        if(this._selection)
            return html`The current 404 page for <strong>${this._documentName}</strong> and child pages is show below.`;

        return html`There is currently no 404 page explicitly set for <strong>${this._documentName}</strong>.`;
    }

    private _getInstructionMessage()
    {
        if(this._selection)
            return html `To change the 404 page, click on the content selector below and pick the new 404 page.`;
        return html`To <strong>set</strong> the 404 page, click on the content selector below and pick the new 404 page.`;
    }

    private _getInheritanceMessage()
    {
        if(!this._selection && this._inherited404)
        {
            return html`<p>This page currently inherits a 404 from an ancestor:<br><strong>${this._inherited404}</strong> inherited from ${this._inheritsFrom} </p>`;
        }
        return ``;
    }

    #selectionChanged(e: CustomEvent) {
		this._selection = (e.target as UmbInputDocumentElement).selection[0];
	}

    async #getDocumentName(entityKey: string | undefined | null) {
		if (!entityKey) return;
		// Should this be done here or in the action file?
		const { data } = await new UmbDocumentItemRepository(this).requestItems([entityKey]);
		if (!data) return;
		const item = data[0];
		//TODO How do we ensure we get the correct variant?
		return item.variants[0]?.name;

	}

    render() {
        console.log(this.data);
        return html`
            <umb-body-layout headline="Page Not Found #BETA#">
                <uui-box headline="Configure the 404 page for: '${this._documentName}'">
                    <p>${this._getIntroductionMessage()}</p>
                    ${this._getInheritanceMessage()}

                    <div>
                        <p>${this._getInstructionMessage()}</p>
                        <label>Selected 404 Page:</label>
                        <umb-input-document min=1 max=1
                            .value=${this._selection}
                            @change=${this.#selectionChanged}>
                        </umb-input-document>
                    </div>
                </uui-box>
                                
                <div slot="actions">
                    <uui-button look="primary" color="positive" id="saveAndClose" label="Close" @click="${this.handleSave}">Save & Close</uui-button>
                    <uui-button look="primary" color="danger" id="close" label="Close" @click="${this.handleClose}">Close</uui-button>
                </div>
            </umb-body-layout>
        `;
    }
}

export default PageNotFoundModalElement;