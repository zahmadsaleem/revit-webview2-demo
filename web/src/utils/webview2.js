// the keys should match enum keys in c#
export const WV2EVENTS = {
  SelectionChanged: "SelectionChanged",
  getEventName(key) {
    return this[key]?.toLowerCase();
  },
};

const eventCaptureElement = document.createElement("a");

// this is called by WebView2 from C#
window.dispatchWebViewEvent = function dispatchWebViewEvent({ action, payload }) {
  console.log(`dispatch requested ${action}`);
  const e = WV2EVENTS.getEventName(action);
  if (e !== undefined) {
    console.log(`dispatching event ${e}`);
    console.log(`event payload : ${payload}`);
    eventCaptureElement.dispatchEvent(new CustomEvent(e, { detail: payload }));
  }
};

export function subscribeToWebView2Event(eventName, handler) {
  const e = WV2EVENTS.getEventName(eventName);
  // console.log(`subscribing: ${e}`);
  if (e === undefined) {
    return;
  }
  // console.log(`subscribed: ${e}`);
  eventCaptureElement.addEventListener(e, handler);
}

export function unsubscribeToWebView2Event(eventName, handler) {
  const e = WV2EVENTS.getEventName(eventName);
  // console.log(`unsubscribing: ${e}`);
  if (e === undefined) {
    return;
  }
  // console.log(`unsubscribed: ${e}`);
  eventCaptureElement.removeEventListener(e, handler);
}

export function postWebView2Message({ action, payload }) {
  if (!action) {
    return;
  }
  window.chrome?.webview?.postMessage({ action, payload });
}

export function isInWebViewContext() {
  return !!window.chrome?.webview;
}
