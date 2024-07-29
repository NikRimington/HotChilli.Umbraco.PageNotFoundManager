var E = (t) => {
  throw TypeError(t);
};
var b = (t, e, r) => e.has(t) || E("Cannot " + r);
var _ = (t, e, r) => (b(t, e, "read from private field"), r ? r.call(t) : e.get(t)), R = (t, e, r) => e.has(t) ? E("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(t) : e.set(t, r), N = (t, e, r, n) => (b(t, e, "write to private field"), n ? n.call(t, r) : e.set(t, r), r);
import { UMB_DOCUMENT_ENTITY_TYPE as q } from "@umbraco-cms/backoffice/document";
import { tryExecuteAndNotify as I } from "@umbraco-cms/backoffice/resources";
import { UmbEntityActionBase as P } from "@umbraco-cms/backoffice/entity-action";
import { UmbModalToken as x, UMB_MODAL_MANAGER_CONTEXT as D } from "@umbraco-cms/backoffice/modal";
import { UMB_NOTIFICATION_CONTEXT as H } from "@umbraco-cms/backoffice/notification";
import { UMB_AUTH_CONTEXT as M } from "@umbraco-cms/backoffice/auth";
const F = new x("hcs.pagenotfound.modal", {
  modal: {
    type: "sidebar",
    size: "medium"
    // full, large, medium, small
  }
});
class A {
  constructor() {
    this._fns = [];
  }
  eject(e) {
    const r = this._fns.indexOf(e);
    r !== -1 && (this._fns = [...this._fns.slice(0, r), ...this._fns.slice(r + 1)]);
  }
  use(e) {
    this._fns = [...this._fns, e];
  }
}
const l = {
  BASE: "",
  CREDENTIALS: "include",
  ENCODE_PATH: void 0,
  HEADERS: void 0,
  PASSWORD: void 0,
  TOKEN: void 0,
  USERNAME: void 0,
  VERSION: "1.0",
  WITH_CREDENTIALS: !1,
  interceptors: {
    request: new A(),
    response: new A()
  }
};
class C extends Error {
  constructor(e, r, n) {
    super(n), this.name = "ApiError", this.url = r.url, this.status = r.status, this.statusText = r.statusText, this.body = r.body, this.request = e;
  }
}
class v extends Error {
  constructor(e) {
    super(e), this.name = "CancelError";
  }
  get isCancelled() {
    return !0;
  }
}
class B {
  constructor(e) {
    this._isResolved = !1, this._isRejected = !1, this._isCancelled = !1, this.cancelHandlers = [], this.promise = new Promise((r, n) => {
      this._resolve = r, this._reject = n;
      const s = (a) => {
        this._isResolved || this._isRejected || this._isCancelled || (this._isResolved = !0, this._resolve && this._resolve(a));
      }, o = (a) => {
        this._isResolved || this._isRejected || this._isCancelled || (this._isRejected = !0, this._reject && this._reject(a));
      }, i = (a) => {
        this._isResolved || this._isRejected || this._isCancelled || this.cancelHandlers.push(a);
      };
      return Object.defineProperty(i, "isResolved", {
        get: () => this._isResolved
      }), Object.defineProperty(i, "isRejected", {
        get: () => this._isRejected
      }), Object.defineProperty(i, "isCancelled", {
        get: () => this._isCancelled
      }), e(s, o, i);
    });
  }
  get [Symbol.toStringTag]() {
    return "Cancellable Promise";
  }
  then(e, r) {
    return this.promise.then(e, r);
  }
  catch(e) {
    return this.promise.catch(e);
  }
  finally(e) {
    return this.promise.finally(e);
  }
  cancel() {
    if (!(this._isResolved || this._isRejected || this._isCancelled)) {
      if (this._isCancelled = !0, this.cancelHandlers.length)
        try {
          for (const e of this.cancelHandlers)
            e();
        } catch (e) {
          console.warn("Cancellation threw an error", e);
          return;
        }
      this.cancelHandlers.length = 0, this._reject && this._reject(new v("Request aborted"));
    }
  }
  get isCancelled() {
    return this._isCancelled;
  }
}
const h = (t) => typeof t == "string", y = (t) => h(t) && t !== "", p = (t) => t instanceof Blob, w = (t) => t instanceof FormData, U = (t) => {
  try {
    return btoa(t);
  } catch {
    return Buffer.from(t).toString("base64");
  }
}, $ = (t) => {
  const e = [], r = (s, o) => {
    e.push(`${encodeURIComponent(s)}=${encodeURIComponent(String(o))}`);
  }, n = (s, o) => {
    o != null && (o instanceof Date ? r(s, o.toISOString()) : Array.isArray(o) ? o.forEach((i) => n(s, i)) : typeof o == "object" ? Object.entries(o).forEach(([i, a]) => n(`${s}[${i}]`, a)) : r(s, o));
  };
  return Object.entries(t).forEach(([s, o]) => n(s, o)), e.length ? `?${e.join("&")}` : "";
}, L = (t, e) => {
  const r = encodeURI, n = e.url.replace("{api-version}", t.VERSION).replace(/{(.*?)}/g, (o, i) => {
    var a;
    return (a = e.path) != null && a.hasOwnProperty(i) ? r(String(e.path[i])) : o;
  }), s = t.BASE + n;
  return e.query ? s + $(e.query) : s;
}, k = (t) => {
  if (t.formData) {
    const e = new FormData(), r = (n, s) => {
      h(s) || p(s) ? e.append(n, s) : e.append(n, JSON.stringify(s));
    };
    return Object.entries(t.formData).filter(([, n]) => n != null).forEach(([n, s]) => {
      Array.isArray(s) ? s.forEach((o) => r(n, o)) : r(n, s);
    }), e;
  }
}, f = async (t, e) => typeof e == "function" ? e(t) : e, G = async (t, e) => {
  const [r, n, s, o] = await Promise.all([
    // @ts-ignore
    f(e, t.TOKEN),
    // @ts-ignore
    f(e, t.USERNAME),
    // @ts-ignore
    f(e, t.PASSWORD),
    // @ts-ignore
    f(e, t.HEADERS)
  ]), i = Object.entries({
    Accept: "application/json",
    ...o,
    ...e.headers
  }).filter(([, a]) => a != null).reduce((a, [d, c]) => ({
    ...a,
    [d]: String(c)
  }), {});
  if (y(r) && (i.Authorization = `Bearer ${r}`), y(n) && y(s)) {
    const a = U(`${n}:${s}`);
    i.Authorization = `Basic ${a}`;
  }
  return e.body !== void 0 && (e.mediaType ? i["Content-Type"] = e.mediaType : p(e.body) ? i["Content-Type"] = e.body.type || "application/octet-stream" : h(e.body) ? i["Content-Type"] = "text/plain" : w(e.body) || (i["Content-Type"] = "application/json")), new Headers(i);
}, V = (t) => {
  var e, r;
  if (t.body !== void 0)
    return (e = t.mediaType) != null && e.includes("application/json") || (r = t.mediaType) != null && r.includes("+json") ? JSON.stringify(t.body) : h(t.body) || p(t.body) || w(t.body) ? t.body : JSON.stringify(t.body);
}, W = async (t, e, r, n, s, o, i) => {
  const a = new AbortController();
  let d = {
    headers: o,
    body: n ?? s,
    method: e.method,
    signal: a.signal
  };
  t.WITH_CREDENTIALS && (d.credentials = t.CREDENTIALS);
  for (const c of t.interceptors.request._fns)
    d = await c(d);
  return i(() => a.abort()), await fetch(r, d);
}, z = (t, e) => {
  if (e) {
    const r = t.headers.get(e);
    if (h(r))
      return r;
  }
}, J = async (t) => {
  if (t.status !== 204)
    try {
      const e = t.headers.get("Content-Type");
      if (e) {
        const r = ["application/octet-stream", "application/pdf", "application/zip", "audio/", "image/", "video/"];
        if (e.includes("application/json") || e.includes("+json"))
          return await t.json();
        if (r.some((n) => e.includes(n)))
          return await t.blob();
        if (e.includes("multipart/form-data"))
          return await t.formData();
        if (e.includes("text/"))
          return await t.text();
      }
    } catch (e) {
      console.error(e);
    }
}, K = (t, e) => {
  const n = {
    400: "Bad Request",
    401: "Unauthorized",
    402: "Payment Required",
    403: "Forbidden",
    404: "Not Found",
    405: "Method Not Allowed",
    406: "Not Acceptable",
    407: "Proxy Authentication Required",
    408: "Request Timeout",
    409: "Conflict",
    410: "Gone",
    411: "Length Required",
    412: "Precondition Failed",
    413: "Payload Too Large",
    414: "URI Too Long",
    415: "Unsupported Media Type",
    416: "Range Not Satisfiable",
    417: "Expectation Failed",
    418: "Im a teapot",
    421: "Misdirected Request",
    422: "Unprocessable Content",
    423: "Locked",
    424: "Failed Dependency",
    425: "Too Early",
    426: "Upgrade Required",
    428: "Precondition Required",
    429: "Too Many Requests",
    431: "Request Header Fields Too Large",
    451: "Unavailable For Legal Reasons",
    500: "Internal Server Error",
    501: "Not Implemented",
    502: "Bad Gateway",
    503: "Service Unavailable",
    504: "Gateway Timeout",
    505: "HTTP Version Not Supported",
    506: "Variant Also Negotiates",
    507: "Insufficient Storage",
    508: "Loop Detected",
    510: "Not Extended",
    511: "Network Authentication Required",
    ...t.errors
  }[e.status];
  if (n)
    throw new C(t, e, n);
  if (!e.ok) {
    const s = e.status ?? "unknown", o = e.statusText ?? "unknown", i = (() => {
      try {
        return JSON.stringify(e.body, null, 2);
      } catch {
        return;
      }
    })();
    throw new C(
      t,
      e,
      `Generic Error: status: ${s}; status text: ${o}; body: ${i}`
    );
  }
}, S = (t, e) => new B(async (r, n, s) => {
  try {
    const o = L(t, e), i = k(e), a = V(e), d = await G(t, e);
    if (!s.isCancelled) {
      let c = await W(t, e, o, a, i, d, s);
      for (const O of t.interceptors.response._fns)
        c = await O(c);
      const m = await J(c), j = z(c, e.responseHeader);
      let g = m;
      e.responseTransformer && c.ok && (g = await e.responseTransformer(m));
      const T = {
        url: o,
        ok: c.ok,
        status: c.status,
        statusText: c.statusText,
        body: j ?? g
      };
      K(e, T), r(T.body);
    }
  } catch (o) {
    n(o);
  }
});
class X {
  /**
   * @param data The data for the request.
   * @param data.pageId
   * @returns unknown OK
   * @throws ApiError
   */
  static getApiV1HcsGetNotFound(e = {}) {
    return S(l, {
      method: "GET",
      url: "/api/v1/hcs/get-not-found",
      query: {
        pageId: e.pageId
      },
      errors: {
        401: "The resource is protected and requires an authentication token"
      }
    });
  }
  /**
   * @param data The data for the request.
   * @param data.requestBody
   * @returns unknown OK
   * @throws ApiError
   */
  static postApiV1HcsSetNotFound(e = {}) {
    return S(l, {
      method: "POST",
      url: "/api/v1/hcs/set-not-found",
      body: e.requestBody,
      mediaType: "application/json",
      errors: {
        401: "The resource is protected and requires an authentication token"
      }
    });
  }
}
var u;
class Y extends P {
  constructor(r, n) {
    super(r, n);
    // Modal Manager Context - to open modals such as our custom one or a icon picker,
    // content picker etc
    R(this, u);
    this._handleNotification = (s, o) => {
      var a;
      const i = {
        data: {
          headline: "404 Page Set",
          message: `The 404 Page '${o.selectedNodeName}' has been set against '${o.currentNodeName}'`
        }
      };
      (a = this._notificationContext) == null || a.peek(s, i);
    }, this.consumeContext(D, (s) => {
      N(this, u, s);
    }), this.consumeContext(H, (s) => {
      this._notificationContext = s;
    });
  }
  async execute() {
    var s;
    if (!this.args.unique)
      throw new Error("The document unique identifier is missing");
    var r = await I(this, X.getApiV1HcsGetNotFound({ pageId: this.args.unique }));
    const n = (s = _(this, u)) == null ? void 0 : s.open(this, F, {
      data: {
        entityKey: this.args.unique,
        target: r.data
      }
    });
    await (n == null ? void 0 : n.onSubmit().catch((o) => {
      console.log("rejected", o);
    }).then((o) => {
      this.isReturnModel(o) && this._handleNotification("positive", o);
    }));
  }
  isReturnModel(r) {
    return r.currentNodeName !== void 0;
  }
}
u = new WeakMap();
const Q = {
  type: "entityAction",
  kind: "default",
  alias: "hcs.pagenotfound.entity.action",
  name: "HCS: Page Not Found Manager: Action",
  weight: 400,
  api: Y,
  meta: {
    icon: "icon-sensor",
    label: "404 Manager"
  },
  forEntityTypes: [q]
}, Z = [Q], ee = {
  type: "modal",
  alias: "hcs.pagenotfound.modal",
  name: "Page Not Found Modal",
  js: () => import("./pagenotfound.modal.element-DAvG6IRv.js")
}, te = [ee], de = (t, e) => {
  e.registerMany([
    ...Z,
    ...te
  ]), t.consumeContext(M, (r) => {
    const n = r.getOpenApiConfiguration();
    l.TOKEN = n.token, l.BASE = n.base, l.WITH_CREDENTIALS = n.withCredentials;
  });
};
export {
  X as P,
  de as o
};
//# sourceMappingURL=index-D9oc8jC4.js.map
