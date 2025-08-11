var R = Object.defineProperty;
var N = (t) => {
  throw TypeError(t);
};
var M = (t, e, r) => e in t ? R(t, e, { enumerable: !0, configurable: !0, writable: !0, value: r }) : t[e] = r;
var T = (t, e, r) => M(t, typeof e != "symbol" ? e + "" : e, r), j = (t, e, r) => e.has(t) || N("Cannot " + r);
var x = (t, e, r) => (j(t, e, "read from private field"), r ? r.call(t) : e.get(t)), _ = (t, e, r) => e.has(t) ? N("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(t) : e.set(t, r), A = (t, e, r, s) => (j(t, e, "write to private field"), s ? s.call(t, r) : e.set(t, r), r);
import { UMB_DOCUMENT_ENTITY_TYPE as P } from "@umbraco-cms/backoffice/document";
import { tryExecute as z } from "@umbraco-cms/backoffice/resources";
import { UmbEntityActionBase as F } from "@umbraco-cms/backoffice/entity-action";
import { UmbModalToken as D, UMB_MODAL_MANAGER_CONTEXT as H } from "@umbraco-cms/backoffice/modal";
import { UMB_NOTIFICATION_CONTEXT as W } from "@umbraco-cms/backoffice/notification";
import { UMB_AUTH_CONTEXT as k } from "@umbraco-cms/backoffice/auth";
const B = new D("hcs.pagenotfound.modal", {
  modal: {
    type: "sidebar",
    size: "medium"
    // full, large, medium, small
  }
});
var G = async (t, e) => {
  let r = typeof e == "function" ? await e(t) : e;
  if (r) return t.scheme === "bearer" ? `Bearer ${r}` : t.scheme === "basic" ? `Basic ${btoa(r)}` : r;
}, V = { bodySerializer: (t) => JSON.stringify(t, (e, r) => typeof r == "bigint" ? r.toString() : r) }, J = (t) => {
  switch (t) {
    case "label":
      return ".";
    case "matrix":
      return ";";
    case "simple":
      return ",";
    default:
      return "&";
  }
}, L = (t) => {
  switch (t) {
    case "form":
      return ",";
    case "pipeDelimited":
      return "|";
    case "spaceDelimited":
      return "%20";
    default:
      return ",";
  }
}, X = (t) => {
  switch (t) {
    case "label":
      return ".";
    case "matrix":
      return ";";
    case "simple":
      return ",";
    default:
      return "&";
  }
}, O = ({ allowReserved: t, explode: e, name: r, style: s, value: o }) => {
  if (!e) {
    let a = (t ? o : o.map((l) => encodeURIComponent(l))).join(L(s));
    switch (s) {
      case "label":
        return `.${a}`;
      case "matrix":
        return `;${r}=${a}`;
      case "simple":
        return a;
      default:
        return `${r}=${a}`;
    }
  }
  let i = J(s), n = o.map((a) => s === "label" || s === "simple" ? t ? a : encodeURIComponent(a) : g({ allowReserved: t, name: r, value: a })).join(i);
  return s === "label" || s === "matrix" ? i + n : n;
}, g = ({ allowReserved: t, name: e, value: r }) => {
  if (r == null) return "";
  if (typeof r == "object") throw new Error("Deeply-nested arrays/objects aren’t supported. Provide your own `querySerializer()` to handle these.");
  return `${e}=${t ? r : encodeURIComponent(r)}`;
}, $ = ({ allowReserved: t, explode: e, name: r, style: s, value: o }) => {
  if (o instanceof Date) return `${r}=${o.toISOString()}`;
  if (s !== "deepObject" && !e) {
    let a = [];
    Object.entries(o).forEach(([f, d]) => {
      a = [...a, f, t ? d : encodeURIComponent(d)];
    });
    let l = a.join(",");
    switch (s) {
      case "form":
        return `${r}=${l}`;
      case "label":
        return `.${l}`;
      case "matrix":
        return `;${r}=${l}`;
      default:
        return l;
    }
  }
  let i = X(s), n = Object.entries(o).map(([a, l]) => g({ allowReserved: t, name: s === "deepObject" ? `${r}[${a}]` : a, value: l })).join(i);
  return s === "label" || s === "matrix" ? i + n : n;
}, Y = /\{[^{}]+\}/g, K = ({ path: t, url: e }) => {
  let r = e, s = e.match(Y);
  if (s) for (let o of s) {
    let i = !1, n = o.substring(1, o.length - 1), a = "simple";
    n.endsWith("*") && (i = !0, n = n.substring(0, n.length - 1)), n.startsWith(".") ? (n = n.substring(1), a = "label") : n.startsWith(";") && (n = n.substring(1), a = "matrix");
    let l = t[n];
    if (l == null) continue;
    if (Array.isArray(l)) {
      r = r.replace(o, O({ explode: i, name: n, style: a, value: l }));
      continue;
    }
    if (typeof l == "object") {
      r = r.replace(o, $({ explode: i, name: n, style: a, value: l }));
      continue;
    }
    if (a === "matrix") {
      r = r.replace(o, `;${g({ name: n, value: l })}`);
      continue;
    }
    let f = encodeURIComponent(a === "label" ? `.${l}` : l);
    r = r.replace(o, f);
  }
  return r;
}, q = ({ allowReserved: t, array: e, object: r } = {}) => (s) => {
  let o = [];
  if (s && typeof s == "object") for (let i in s) {
    let n = s[i];
    if (n != null) if (Array.isArray(n)) {
      let a = O({ allowReserved: t, explode: !0, name: i, style: "form", value: n, ...e });
      a && o.push(a);
    } else if (typeof n == "object") {
      let a = $({ allowReserved: t, explode: !0, name: i, style: "deepObject", value: n, ...r });
      a && o.push(a);
    } else {
      let a = g({ allowReserved: t, name: i, value: n });
      a && o.push(a);
    }
  }
  return o.join("&");
}, Q = (t) => {
  var r;
  if (!t) return "stream";
  let e = (r = t.split(";")[0]) == null ? void 0 : r.trim();
  if (e) {
    if (e.startsWith("application/json") || e.endsWith("+json")) return "json";
    if (e === "multipart/form-data") return "formData";
    if (["application/", "audio/", "image/", "video/"].some((s) => e.startsWith(s))) return "blob";
    if (e.startsWith("text/")) return "text";
  }
}, Z = async ({ security: t, ...e }) => {
  for (let r of t) {
    let s = await G(r, e.auth);
    if (!s) continue;
    let o = r.name ?? "Authorization";
    switch (r.in) {
      case "query":
        e.query || (e.query = {}), e.query[o] = s;
        break;
      case "cookie":
        e.headers.append("Cookie", `${o}=${s}`);
        break;
      case "header":
      default:
        e.headers.set(o, s);
        break;
    }
    return;
  }
}, C = (t) => ee({ baseUrl: t.baseUrl, path: t.path, query: t.query, querySerializer: typeof t.querySerializer == "function" ? t.querySerializer : q(t.querySerializer), url: t.url }), ee = ({ baseUrl: t, path: e, query: r, querySerializer: s, url: o }) => {
  let i = o.startsWith("/") ? o : `/${o}`, n = (t ?? "") + i;
  e && (n = K({ path: e, url: n }));
  let a = r ? s(r) : "";
  return a.startsWith("?") && (a = a.substring(1)), a && (n += `?${a}`), n;
}, S = (t, e) => {
  var s;
  let r = { ...t, ...e };
  return (s = r.baseUrl) != null && s.endsWith("/") && (r.baseUrl = r.baseUrl.substring(0, r.baseUrl.length - 1)), r.headers = U(t.headers, e.headers), r;
}, U = (...t) => {
  let e = new Headers();
  for (let r of t) {
    if (!r || typeof r != "object") continue;
    let s = r instanceof Headers ? r.entries() : Object.entries(r);
    for (let [o, i] of s) if (i === null) e.delete(o);
    else if (Array.isArray(i)) for (let n of i) e.append(o, n);
    else i !== void 0 && e.set(o, typeof i == "object" ? JSON.stringify(i) : i);
  }
  return e;
}, w = class {
  constructor() {
    T(this, "_fns");
    this._fns = [];
  }
  clear() {
    this._fns = [];
  }
  getInterceptorIndex(t) {
    return typeof t == "number" ? this._fns[t] ? t : -1 : this._fns.indexOf(t);
  }
  exists(t) {
    let e = this.getInterceptorIndex(t);
    return !!this._fns[e];
  }
  eject(t) {
    let e = this.getInterceptorIndex(t);
    this._fns[e] && (this._fns[e] = null);
  }
  update(t, e) {
    let r = this.getInterceptorIndex(t);
    return this._fns[r] ? (this._fns[r] = e, t) : !1;
  }
  use(t) {
    return this._fns = [...this._fns, t], this._fns.length - 1;
  }
}, te = () => ({ error: new w(), request: new w(), response: new w() }), re = q({ allowReserved: !1, array: { explode: !0, style: "form" }, object: { explode: !0, style: "deepObject" } }), ae = { "Content-Type": "application/json" }, E = (t = {}) => ({ ...V, headers: ae, parseAs: "auto", querySerializer: re, ...t }), ne = (t = {}) => {
  let e = S(E(), t), r = () => ({ ...e }), s = (n) => (e = S(e, n), r()), o = te(), i = async (n) => {
    let a = { ...e, ...n, fetch: n.fetch ?? e.fetch ?? globalThis.fetch, headers: U(e.headers, n.headers) };
    a.security && await Z({ ...a, security: a.security }), a.body && a.bodySerializer && (a.body = a.bodySerializer(a.body)), (a.body === void 0 || a.body === "") && a.headers.delete("Content-Type");
    let l = C(a), f = { redirect: "follow", ...a }, d = new Request(l, f);
    for (let c of o.request._fns) c && (d = await c(d, a));
    let I = a.fetch, u = await I(d);
    for (let c of o.response._fns) c && (u = await c(u, d, a));
    let y = { request: d, response: u };
    if (u.ok) {
      if (u.status === 204 || u.headers.get("Content-Length") === "0") return a.responseStyle === "data" ? {} : { data: {}, ...y };
      let c = (a.parseAs === "auto" ? Q(u.headers.get("Content-Type")) : a.parseAs) ?? "json";
      if (c === "stream") return a.responseStyle === "data" ? u.body : { data: u.body, ...y };
      let p = await u[c]();
      return c === "json" && (a.responseValidator && await a.responseValidator(p), a.responseTransformer && (p = await a.responseTransformer(p))), a.responseStyle === "data" ? p : { data: p, ...y };
    }
    let b = await u.text();
    try {
      b = JSON.parse(b);
    } catch {
    }
    let h = b;
    for (let c of o.error._fns) c && (h = await c(b, u, d, a));
    if (h = h || {}, a.throwOnError) throw h;
    return a.responseStyle === "data" ? void 0 : { error: h, ...y };
  };
  return { buildUrl: C, connect: (n) => i({ ...n, method: "CONNECT" }), delete: (n) => i({ ...n, method: "DELETE" }), get: (n) => i({ ...n, method: "GET" }), getConfig: r, head: (n) => i({ ...n, method: "HEAD" }), interceptors: o, options: (n) => i({ ...n, method: "OPTIONS" }), patch: (n) => i({ ...n, method: "PATCH" }), post: (n) => i({ ...n, method: "POST" }), put: (n) => i({ ...n, method: "PUT" }), request: i, setConfig: s, trace: (n) => i({ ...n, method: "TRACE" }) };
};
const v = ne(E({
  baseUrl: "https://localhost:44394"
}));
class se {
  static getApiV1HcsGetNotFound(e) {
    return ((e == null ? void 0 : e.client) ?? v).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/api/v1/hcs/get-not-found",
      ...e
    });
  }
  static postApiV1HcsSetNotFound(e) {
    return ((e == null ? void 0 : e.client) ?? v).post({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/api/v1/hcs/set-not-found",
      ...e,
      headers: {
        "Content-Type": "application/json",
        ...e == null ? void 0 : e.headers
      }
    });
  }
}
var m;
class oe extends F {
  constructor(r, s) {
    super(r, s);
    // Modal Manager Context - to open modals such as our custom one or a icon picker,
    // content picker etc
    _(this, m);
    this._handleNotification = (o, i) => {
      var a;
      const n = {
        data: {
          headline: "404 Page Set",
          message: `The 404 Page '${i.selectedNodeName}' has been set against '${i.currentNodeName}'`
        }
      };
      (a = this._notificationContext) == null || a.peek(o, n);
    }, this.consumeContext(H, (o) => {
      A(this, m, o);
    }), this.consumeContext(W, (o) => {
      this._notificationContext = o;
    });
  }
  async execute() {
    var i;
    if (!this.args.unique)
      throw new Error("The document unique identifier is missing");
    const r = {
      query: {
        pageId: this.args.unique
      },
      url: "/api/v1/hcs/get-not-found"
    };
    var s = await z(this, se.getApiV1HcsGetNotFound(r));
    const o = (i = x(this, m)) == null ? void 0 : i.open(this, B, {
      data: {
        entityKey: this.args.unique,
        target: typeof s.data == "string" ? void 0 : s.data
      }
    });
    await (o == null ? void 0 : o.onSubmit().catch((n) => {
      console.log("rejected", n);
    }).then((n) => {
      this.isReturnModel(n) && this._handleNotification("positive", n);
    }));
  }
  isReturnModel(r) {
    return r.currentNodeName !== void 0;
  }
}
m = new WeakMap();
const ie = {
  type: "entityAction",
  kind: "default",
  alias: "hcs.pagenotfound.entity.action",
  name: "HCS: Page Not Found Manager: Action",
  weight: 400,
  api: oe,
  meta: {
    icon: "icon-sensor",
    label: "404 Manager"
  },
  forEntityTypes: [P]
}, le = [ie], ue = {
  type: "modal",
  alias: "hcs.pagenotfound.modal",
  name: "Page Not Found Modal",
  js: () => import("./pagenotfound.modal.element-DC_wRudh.js")
}, ce = [ue], ge = (t, e) => {
  e.registerMany([
    ...le,
    ...ce
  ]), t.consumeContext(k, async (r) => {
    const s = r == null ? void 0 : r.getOpenApiConfiguration();
    v.setConfig({
      auth: (s == null ? void 0 : s.token) ?? void 0,
      baseUrl: (s == null ? void 0 : s.base) ?? "",
      credentials: (s == null ? void 0 : s.credentials) ?? "same-origin"
    });
  });
};
export {
  se as P,
  ge as o
};
//# sourceMappingURL=index-DjInfImN.js.map
