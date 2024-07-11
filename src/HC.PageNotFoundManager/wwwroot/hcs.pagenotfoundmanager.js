var s = (t) => {
  throw TypeError(t);
};
var r = (t, o, n) => o.has(t) || s("Cannot " + n);
var c = (t, o, n) => (r(t, o, "read from private field"), n ? n.call(t) : o.get(t)), d = (t, o, n) => o.has(t) ? s("Cannot add the same private member more than once") : o instanceof WeakSet ? o.add(t) : o.set(t, n), m = (t, o, n, e) => (r(t, o, "write to private field"), e ? e.call(t, n) : o.set(t, n), n);
import { UMB_DOCUMENT_ENTITY_TYPE as u } from "@umbraco-cms/backoffice/document";
import { UmbEntityActionBase as g } from "@umbraco-cms/backoffice/entity-action";
import { UmbModalToken as l, UMB_MODAL_MANAGER_CONTEXT as p } from "@umbraco-cms/backoffice/modal";
const h = new l("hcs.pagenotfound.modal", {
  modal: {
    type: "sidebar",
    size: "medium"
    // full, large, medium, small
  }
});
var i;
class y extends g {
  constructor(n, e) {
    super(n, e);
    // Modal Manager Context - to open modals such as our custom one or a icon picker,
    // content picker etc
    d(this, i);
    this.consumeContext(p, (a) => {
      m(this, i, a);
    });
  }
  async execute() {
    var e;
    if (!this.args.unique)
      throw new Error("The document unique identifier is missing");
    const n = (e = c(this, i)) == null ? void 0 : e.open(this, h, {
      data: {
        entityKey: this.args.unique,
        target: null
      }
    });
    await (n == null ? void 0 : n.onSubmit().catch((a) => {
      console.log("rejected", a);
    }));
  }
}
i = new WeakMap();
const M = {
  type: "entityAction",
  kind: "default",
  alias: "hcs.pagenotfound.entity.action",
  name: "HCS: Page Not Found Manager: Action",
  weight: 400,
  api: y,
  meta: {
    icon: "icon-sensor",
    label: "404 Manager"
  },
  forEntityTypes: [u]
}, f = [M], A = {
  type: "modal",
  alias: "hcs.pagenotfound.modal",
  name: "Page Not Found Modal",
  js: () => import("./pagenotfound.modal.element-BKqE-HqI.js")
}, E = [A], w = (t, o) => {
  o.registerMany([
    ...f,
    ...E
  ]);
};
export {
  w as onInit
};
//# sourceMappingURL=hcs.pagenotfoundmanager.js.map
