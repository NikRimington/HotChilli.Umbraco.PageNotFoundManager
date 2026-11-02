import { UmbApiError } from '@umbraco-cms/backoffice/resources';

/**
 * Builds a readable message from an error returned by tryExecute.
 */
export function getErrorMessage(error: unknown): string {
    if (UmbApiError.isUmbApiError(error)) {
        const { status, title, detail } = error.problemDetails;
        const message = detail ?? title ?? error.message;
        return status ? `${message} (HTTP ${status})` : message;
    }

    return error instanceof Error ? error.message : 'An unknown error occurred.';
}
