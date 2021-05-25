<template>
  <v-app>
    <v-main>
      <v-container>
        <h3>WebView2 + Revit</h3>
        <v-btn
          outlined
          @click="createSheet"
          title="Revit API Transaction"
          class="mr-1"
          >Create Sheet</v-btn
        >
        <v-btn
          outlined
          @click="selectGuid"
          :disabled="elementGuidsToSelect.length === 0"
          >Select From List</v-btn
        >
        <v-card outlined class="mt-2" height="400px">
          <v-card-subtitle>Selected Elements</v-card-subtitle>
          <v-card-text>
            <v-list max-height="320px" style="overflow-y: scroll">
              <v-list-item-group multiple v-model="elementGuidsToSelect">
                <div v-for="el in elementGuids" :key="el.id">
                  <v-list-item dense class="caption" :value="el.id">
                    <v-list-item-content>
                      <v-list-item-title v-text="el.name" />
                      <v-list-item-subtitle v-text="el.id" />
                    </v-list-item-content>
                  </v-list-item>
                  <v-divider />
                </div>
              </v-list-item-group>
            </v-list>
          </v-card-text>
        </v-card>
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
