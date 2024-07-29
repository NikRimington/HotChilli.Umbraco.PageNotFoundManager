import { html as c, state as b, customElement as w } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalBaseElement as $ } from "@umbraco-cms/backoffice/modal";
import { UmbDocumentItemRepository as M } from "@umbraco-cms/backoffice/document";
import { P } from "./index-D9oc8jC4.js";
var F = Object.defineProperty, x = Object.getOwnPropertyDescriptor, N = (e) => {
  throw TypeError(e);
}, u = (e, t, i, o) => {
  for (var n = o > 1 ? void 0 : o ? x(t, i) : t, s = e.length - 1, r; s >= 0; s--)
    (r = e[s]) && (n = (o ? r(t, i, n) : r(n)) || n);
  return o && n && F(t, i, n), n;
}, I = (e, t, i) => t.has(e) || N("Cannot " + i), k = (e, t, i) => t.has(e) ? N("Cannot add the same private member more than once") : t instanceof WeakSet ? t.add(e) : t.set(e, i), l = (e, t, i) => (I(e, t, "access private method"), i), a, C, h;
let d = class extends $ {
  constructor() {
    super(), k(this, a), this._documentName = "";
  }
  async connectedCallback() {
    var e, t, i, o, n, s, r, m, p, g, _, f, v, y;
    super.connectedCallback(), (e = this.data) != null && e.entityKey || console.error("PNFM: connectedCallback. There is NO EntityKey passed into the modal"), (t = this.data) != null && t.target && (this._selection = (o = (i = this.data) == null ? void 0 : i.target) == null ? void 0 : o.explicit404, (r = (s = (n = this.data) == null ? void 0 : n.target) == null ? void 0 : s.inherited404) != null && r.explicit404 && (this._inherited404 = await l(this, a, h).call(this, (g = (p = (m = this.data) == null ? void 0 : m.target) == null ? void 0 : p.inherited404) == null ? void 0 : g.explicit404), this._inheritsFrom = await l(this, a, h).call(this, (v = (f = (_ = this.data) == null ? void 0 : _.target) == null ? void 0 : f.inherited404) == null ? void 0 : v.pageId), console.log(`inherits '${this._inherited404}' From:'${this._inheritsFrom}'`))), this._documentName = await l(this, a, h).call(this, (y = this.data) == null ? void 0 : y.entityKey) ?? "";
  }
  handleClose() {
    var e;
    (e = this.modalContext) == null || e.reject({ type: "close" });
  }
  async handleSave() {
    var t, i;
    var e = await P.postApiV1HcsSetNotFound(
      {
        requestBody: {
          parentId: ((t = this.data) == null ? void 0 : t.entityKey) ?? "",
          notFoundPageId: this._selection
        }
      }
    );
    console.log(e), this.value = {
      currentNodeName: this._documentName,
      selectedNodeName: await l(this, a, h).call(this, this._selection)
    }, (i = this.modalContext) == null || i.submit();
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
                            .value=${this._selection}
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
  var o;
  if (!e) return;
  const { data: t } = await new M(this).requestItems([e]);
  return t ? (o = t[0].variants[0]) == null ? void 0 : o.name : void 0;
};
u([
  b()
], d.prototype, "_selection", 2);
u([
  b()
], d.prototype, "_documentName", 2);
d = u([
  w("page-not-found-modal")
], d);
const A = d;
export {
  d as PageNotFoundModalElement,
  A as default
};
//# sourceMappingURL=pagenotfound.modal.element-DAvG6IRv.js.map
