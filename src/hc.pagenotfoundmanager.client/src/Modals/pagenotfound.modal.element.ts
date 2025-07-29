import { tryExecute } from '@umbraco-cms/backoffice/resources';
import { customElement, html, state } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalBaseElement, UmbModalRejectReason } from "@umbraco-cms/backoffice/modal";
import type { UmbInputDocumentElement } from "@umbraco-cms/backoffice/document";
import { DocumentService } from '@umbraco-cms/backoffice/external/backend-api';
import { PageNotFoundModalData, PageNotFoundModalValue } from "./pagenotfound.modal.token.ts";
import { PageNotFoundManagerService } from "../api/sdk.gen.ts";
import type { PostApiV1HcsSetNotFoundData } from '../api/types.gen';

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
        const PostApiV1HcsSetNotFoundData: PostApiV1HcsSetNotFoundData = {
            body: {
                parentId: this.data?.entityKey ?? "",
                notFoundPageId: this._selection
            },
            url: '/api/v1/hcs/set-not-found'
        };

        var res = await PageNotFoundManagerService.postApiV1HcsSetNotFound(PostApiV1HcsSetNotFoundData);

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
        try {
            if (!entityKey) return;
            const { data, error } = await tryExecute(this, DocumentService.getDocumentById({ path: { id: entityKey } }));
            if (error) {
                console.error('Error fetching document:', error);
                return;
            }
            if (!data) return;
            return data.variants[0].name;
        } catch (error) {
            console.error('Error in getDocument:', error);
            return;
        }
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
                            .value=${this._selection ?? undefined}
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