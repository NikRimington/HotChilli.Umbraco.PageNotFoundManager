import { html as _, state as u, customElement as v } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalBaseElement as y } from "@umbraco-cms/backoffice/modal";
import { UmbDocumentItemRepository as f } from "@umbraco-cms/backoffice/document";
import { P as b } from "./index-DwuL4jY1.js";
var g = Object.defineProperty, C = Object.getOwnPropertyDescriptor, m = (e) => {
  throw TypeError(e);
}, c = (e, t, o, a) => {
  for (var n = a > 1 ? void 0 : a ? C(t, o) : t, l = e.length - 1, r; l >= 0; l--)
    (r = e[l]) && (n = (a ? r(t, o, n) : r(n)) || n);
  return a && n && g(t, o, n), n;
}, N = (e, t, o) => t.has(e) || m("Cannot " + o), P = (e, t, o) => t.has(e) ? m("Cannot add the same private member more than once") : t instanceof WeakSet ? t.add(e) : t.set(e, o), d = (e, t, o) => (N(e, t, "access private method"), o), i, h, p;
let s = class extends y {
  constructor() {
    super(), P(this, i), this._documentName = "";
  }
  connectedCallback() {
    var e, t;
    super.connectedCallback(), (e = this.data) != null && e.entityKey ? console.log(`PNFM: ${(t = this.data) == null ? void 0 : t.entityKey}`) : console.error("PNFM: connectedCallback. There is NO EntityKey passed into the modal"), d(this, i, p).call(this);
  }
  handleClose() {
    var e;
    (e = this.modalContext) == null || e.reject({ type: "close" });
  }
  async handleSave() {
    var t, o;
    console.log("PNFM save triggered");
    var e = await b.postApiV1HcsSetNotFound(
      {
        requestBody: {
          parentId: ((t = this.data) == null ? void 0 : t.entityKey) ?? "",
          notFoundPageId: this._selection
        }
      }
    );
    console.log(e), (o = this.modalContext) == null || o.submit();
  }
  render() {
    return console.log(this.data), _`
            <umb-body-layout headline="Page Not Found #BETA#">
                <uui-box headline="Data">
                    Current node: "${this._documentName}"

                    <umb-input-document min=1 max=1
                        .value=${this._selection}
                        @change=${d(this, i, h)}>
                    </umb-input-document>
                </uui-box>
                                
                <div slot="actions">
                    <uui-button id="close" label="Close" @click="${this.handleSave}">Save & Close</uui-button>
                    <uui-button id="close" label="Close" @click="${this.handleClose}">Close</uui-button>
                </div>
            </umb-body-layout>
        `;
  }
};
i = /* @__PURE__ */ new WeakSet();
h = function(e) {
  this._selection = e.target.selection[0];
};
p = async function() {
  var o, a;
  if (!((o = this.data) != null && o.entityKey)) return;
  const { data: e } = await new f(this).requestItems([this.data.entityKey]);
  if (!e) return;
  const t = e[0];
  this._documentName = (a = t.variants[0]) == null ? void 0 : a.name;
};
c([
  u()
], s.prototype, "_selection", 2);
c([
  u()
], s.prototype, "_documentName", 2);
s = c([
  v("page-not-found-modal")
], s);
const x = s;
export {
  s as PageNotFoundModalElement,
  x as default
};
//# sourceMappingURL=pagenotfound.modal.element-Cxb9nyHz.js.map
