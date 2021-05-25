<template>
  <v-app>
    <v-main>
      <v-container>
        <h3>WebView2 + Revit</h3>
        <v-btn outlined @click="createSheet" title="Revit API Transaction"
          >Create Sheet</v-btn
        >
        <v-btn
          outlined
          @click="selectGuid"
          :disabled="elementGuidsToSelect.length === 0"
          >Select From List</v-btn
        >
        <v-list>
          <v-subheader>Selected Elements</v-subheader>
          <v-list-item-group multiple v-model="elementGuidsToSelect">
            <v-list-item
              v-for="id in elementGuids"
              :key="id"
              class="caption"
              :value="id"
            >
              <v-list-item-subtitle v-text="id" />
            </v-list-item>
          </v-list-item-group>
        </v-list>
      </v-container>
    </v-main>
  </v-app>
</template>

<script>
import {
  WV2EVENTS,
  postWebView2Message,
  subscribeToWebView2Event,
  unsubscribeToWebView2Event,
} from "@/utils/webview2";

export default {
  name: "RevitDemo",
  mounted() {
    subscribeToWebView2Event(WV2EVENTS.SelectionChanged, this.setElementGuids);
  },
  data() {
    return {
      elementGuids: [],
      elementGuidsToSelect: [],
    };
  },
  methods: {
    setElementGuids(e) {
      this.elementGuids = e.detail;
    },
    createSheet() {
      postWebView2Message({
        action: "create-sheet",
      });
    },
    selectGuid() {
      postWebView2Message({
        action: "select",
        payload: [...this.elementGuidsToSelect],
      });
    },
  },
  beforeDestroy() {
    unsubscribeToWebView2Event(
      WV2EVENTS.SelectionChanged,
      this.setElementGuids
    );
  },
};
</script>

<style scoped></style>
