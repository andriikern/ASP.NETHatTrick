<template>
  <input type="checkbox"
         class="btn-check"
         :id="'check-' + outcomeKey"
         :name="'check-' + marketKey"
         :value="outcome.id"
         autocomplete="off"
         :disabled="disabled"
         @change="checkOutcome" />
  <label :class="'btn btn-outline-' + (promoted ? 'primary' : 'secondary') + ' lh-1'"
         :for="'check-' + outcomeKey"
         style="font-size: 0.9rem;">
    <span class="fw-bold">{{ outcome.type.name + (outcome.value === null ? '' : (': ' + outcome.value)) }}</span>
    <br />
    <span style="font-size: 0.75rem;">{{ outcome.odds === null ? '&mdash;' : outcome.odds.toFixed(2) }}</span>
  </label>
</template>

<script lang="ts">
  import { defineComponent } from 'vue'

  import { Outcome } from "../../models"

  interface Data {
    disabled: boolean
  }

  export default defineComponent({
    props: {
      promoted: Boolean,
      marketKey: String,
      outcomeKey: String,
      outcome: Outcome
    },
    emits: [ 'checkOutcome' ],
    data(): Data {
      return {
        disabled: true
      }
    },
    created() {
      this.disabled = true

      // initialise the data when the view is created and the data is already
      // being observed
      this.initialiseData()
    },
    watch: {
      // call again the method if the route changes
      '$route': 'initialiseData'
    },
    methods: {
      initialiseData(): void {
        this.disabled = (this.outcome?.odds === null)
      },
      checkOutcome(event: Event): void {
        this.$emit('checkOutcome', event);
      }
    }
  })

</script>
