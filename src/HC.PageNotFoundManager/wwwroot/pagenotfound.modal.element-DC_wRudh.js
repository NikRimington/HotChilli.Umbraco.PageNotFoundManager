import { tryExecute as w } from "@umbraco-cms/backoffice/resources";
import { html as c, state as b, customElement as $ } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalBaseElement as P } from "@umbraco-cms/backoffice/modal";
import { DocumentService as F } from "@umbraco-cms/backoffice/external/backend-api";
import { P as M } from "./index-DjInfImN.js";
var x = Object.defineProperty, E = Object.getOwnPropertyDescriptor, N = (e) => {
  throw TypeError(e);
}, u = (e, t, o, n) => {
  for (var i = n > 1 ? void 0 : n ? E(t, o) : t, r = e.length - 1, s; r >= 0; r--)
    (s = e[r]) && (i = (n ? s(t, o, i) : s(i)) || i);
  return n && i && x(t, o, i), i;
}, I = (e, t, o) => t.has(e) || N("Cannot " + o), S = (e, t, o) => t.has(e) ? N("Cannot add the same private member more than once") : t instanceof WeakSet ? t.add(e) : t.set(e, o), l = (e, t, o) => (I(e, t, "access private method"), o), a, C, h;
let d = class extends P {
  constructor() {
    super(), S(this, a), this._documentName = "";
  }
  async connectedCallback() {
    var e, t, o, n, i, r, s, m, p, g, _, f, v, y;
    super.connectedCallback(), (e = this.data) != null && e.entityKey || console.error("PNFM: connectedCallback. There is NO EntityKey passed into the modal"), (t = this.data) != null && t.target && (this._selection = (n = (o = this.data) == null ? void 0 : o.target) == null ? void 0 : n.explicit404, (s = (r = (i = this.data) == null ? void 0 : i.target) == null ? void 0 : r.inherited404) != null && s.explicit404 && (this._inherited404 = await l(this, a, h).call(this, (g = (p = (m = this.data) == null ? void 0 : m.target) == null ? void 0 : p.inherited404) == null ? void 0 : g.explicit404), this._inheritsFrom = await l(this, a, h).call(this, (v = (f = (_ = this.data) == null ? void 0 : _.target) == null ? void 0 : f.inherited404) == null ? void 0 : v.pageId), console.log(`inherits '${this._inherited404}' From:'${this._inheritsFrom}'`))), this._documentName = await l(this, a, h).call(this, (y = this.data) == null ? void 0 : y.entityKey) ?? "";
  }
  handleClose() {
    var e;
    (e = this.modalContext) == null || e.reject({ type: "close" });
  }
  async handleSave() {
    var o, n;
    const e = {
      body: {
        parentId: ((o = this.data) == null ? void 0 : o.entityKey) ?? "",
        notFoundPageId: this._selection
      },
      url: "/api/v1/hcs/set-not-found"
    };
    var t = await M.postApiV1HcsSetNotFound(e);
    console.log(t), this.value = {
      currentNodeName: this._documentName,
      selectedNodeName: await l(this, a, h).call(this, this._selection)
    }, (n = this.modalContext) == null || n.submit();
  }
  _getIntroductionMessage() {
    return this._selection ? c`The current 404 page for <strong>${this._documentName}</strong> and child pages is show below.` : c`There is currently no 404 page explicitly set for <strong>${this._documentName}</strong>.`;
  }
  _getInstructionMessage() {
    return this._selection ? c`To change the 404 page, click on the content selector below and pick the new 404 page.` : c`To <strong>set</strong> the 404 page, click on the content selector below and pick the new 404 page.`;
  }
  _getInheritanceMessage() {
    return !this._selection && this._inherited404 ? c`<p>This page currently inherits a 404 from an ancestor:<br><strong>${this._inherited404}</strong> inherited from ${this._inheritsFrom} </p>` : "";
  }
  render() {
    return console.log(this.data), c`
            <umb-body-layout headline="Page Not Found #BETA#">
                <uui-box headline="Configure the 404 page for: '${this._documentName}'">
                    <p>${this._getIntroductionMessage()}</p>
                    ${this._getInheritanceMessage()}

                    <div>
                        <p>${this._getInstructionMessage()}</p>
                        <label>Selected 404 Page:</label>
                        <umb-input-document min=1 max=1
                            .value=${this._selection ?? void 0}
                            @change=${l(this, a, C)}>
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
};
a = /* @__PURE__ */ new WeakSet();
C = function(e) {
  this._selection = e.target.selection[0];
};
h = async function(e) {
  try {
    if (!e) return;
    const { data: t, error: o } = await w(this, F.getDocumentById({ path: { id: e } }));
    if (o) {
      console.error("Error fetching document:", o);
      return;
    }
    return t ? t.variants[0].name : void 0;
  } catch (t) {
    console.error("Error in getDocument:", t);
    return;
  }
};
u([
  b()
], d.prototype, "_selection", 2);
u([
  b()
], d.prototype, "_documentName", 2);
d = u([
  $("page-not-found-modal")
], d);
const B = d;
export {
  d as PageNotFoundModalElement,
  B as default
};
//# sourceMappingURL=pagenotfound.modal.element-DC_wRudh.js.map
