import { tryExecute } from '@umbraco-cms/backoffice/resources';
import { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { UmbEntityActionArgs, UmbEntityActionBase } from "@umbraco-cms/backoffice/entity-action";
import { UMB_MODAL_MANAGER_CONTEXT, UmbModalManagerContext } from "@umbraco-cms/backoffice/modal";
import { PageNotFound_MODAL, PageNotFoundModalValue } from "../../Modals/pagenotfound.modal.token.ts";
import { PageNotFoundManagerService } from "../../api/sdk.gen.ts";
import type { GetApiV1HcsGetNotFoundData } from '../../api/types.gen';
import type {
	UmbNotificationColor,
	UmbNotificationOptions,
	UmbNotificationContext,
} from '@umbraco-cms/backoffice/notification';
import { UMB_NOTIFICATION_CONTEXT } from '@umbraco-cms/backoffice/notification';

export class PageNotFoundEntityAction extends UmbEntityActionBase<never> {

    // Modal Manager Context - to open modals such as our custom one or a icon picker,
    // content picker etc
    #modalManagerContext?: UmbModalManagerContext;
    private _notificationContext?: UmbNotificationContext;

    constructor(host: UmbControllerHost, args: UmbEntityActionArgs<never>) {
        super(host, args);

        // Fetch/consume the contexts & assign to the private fields
        this.consumeContext(UMB_MODAL_MANAGER_CONTEXT, (instance) => {
            this.#modalManagerContext = instance;
        });

        this.consumeContext(UMB_NOTIFICATION_CONTEXT, (instance) => {
			this._notificationContext = instance;
		});
    }

    private _handleNotification = (color: UmbNotificationColor, pnfmData: PageNotFoundModalValue ) => {
		const options: UmbNotificationOptions = {
			data: {
				headline: '404 Page Set',
				message: `The 404 Page '${pnfmData.selectedNodeName}' has been set against '${pnfmData.currentNodeName}'`,
			},
		};
		this._notificationContext?.peek(color, options);
	};

    async execute() {
        if (!this.args.unique) {
            throw new Error('The document unique identifier is missing');
        }

        const GetApiV1HcsGetNotFoundData: GetApiV1HcsGetNotFoundData = {
            query: {
                pageId: this.args.unique,
            },
            url: '/api/v1/hcs/get-not-found'
        };
        var currentNotFound = await tryExecute(this, PageNotFoundManagerService.getApiV1HcsGetNotFound(GetApiV1HcsGetNotFoundData ));

        //The modal does NOT return any data when closed (it does not submit)
        const modal = this.#modalManagerContext?.open(this, PageNotFound_MODAL, {
            data: {
                entityKey: this.args.unique,
                target: typeof currentNotFound.data === 'string' ? undefined : currentNotFound.data
            }
        });

        await modal?.onSubmit().catch((_rejected) => {
            // User clicked close/cancel and no data was submitted
            console.log('rejected', _rejected)
            return;
        }).then((_val) => {
            if(this.isReturnModel(_val))
                this._handleNotification('positive', _val);
        });
    }

    isReturnModel(value: void | PageNotFoundModalValue): value is PageNotFoundModalValue {
        return (value as PageNotFoundModalValue).currentNodeName !== undefined;
      }
}