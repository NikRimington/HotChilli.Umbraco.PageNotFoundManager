import { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { UmbEntityActionArgs, UmbEntityActionBase } from "@umbraco-cms/backoffice/entity-action";
import { UMB_MODAL_MANAGER_CONTEXT, UmbModalManagerContext } from "@umbraco-cms/backoffice/modal";
import { PageNotFound_MODAL } from "../../Modals/pagenotfound.modal.token.ts";

export class PageNotFoundEntityAction extends UmbEntityActionBase<never> {

    // Modal Manager Context - to open modals such as our custom one or a icon picker,
    // content picker etc
    #modalManagerContext?: UmbModalManagerContext;

    constructor(host: UmbControllerHost, args: UmbEntityActionArgs<never>) {
        super(host, args);

        // Fetch/consume the contexts & assign to the private fields
        this.consumeContext(UMB_MODAL_MANAGER_CONTEXT, (instance) => {
            this.#modalManagerContext = instance;
        });
    }

    async execute() {
        if (!this.args.unique) {
            throw new Error('The document unique identifier is missing');
        }

        //The modal does NOT return any data when closed (it does not submit)
        const modal = this.#modalManagerContext?.open(this, PageNotFound_MODAL, {
            data: {
                entityKey: this.args.unique,
                target: null
            }
        });

        await modal?.onSubmit().catch((_rejected) => {
            // User clicked close/cancel and no data was submitted
            console.log('rejected', _rejected)
            return;
        });
    }
}