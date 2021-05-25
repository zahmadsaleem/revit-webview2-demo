// the keys should match enum keys in c#
export const ACTIONS = {
  SelectionChanged: "selection-changed",
};

// this is called by WebView2 from C#
window.dispatchWebViewEvent = function dispatchWebViewEvent({ action, payload }) {
  // console.log(`dispatch requested ${action}`);
  if (ACTIONS[action] !== undefined) {
    // console.log(`dispatching event ${ACTIONS[action]}`);
    console.log(payload);
    document.dispatchEvent(
      new CustomEvent(ACTIONS[action], { detail: payload })
    );
  }
}

export function postWebView2Message({ action, payload }) {
  if (!action) {
    return;
  }
  window.chrome?.webview?.postMessage({ action, payload });
}
