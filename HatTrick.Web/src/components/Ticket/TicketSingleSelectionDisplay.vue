<template>
  <li :class="statusClass">
    <div class="container container-fluid">
      <div class="row">
        <div class="col col-4">
          {{ selection.name }} @ {{ new Date(selection.startsAt).toLocaleString('en-GB', { dateStyle: 'short', timeStyle: 'short' }) }}
        </div>
        <div class="col col-2">
          {{ selection.fixtures[0].type.name }}
        </div>
        <div class="col col-2">
          {{ selection.fixtures[0].markets[0].type.name }}{{ selection.fixtures[0].markets[0].value === null ? '' : (': ' + selection.fixtures[0].markets[0].value) }}
        </div>
        <div class="col col-2">
          {{ selection.fixtures[0].markets[0].outcomes[0].type.name }}{{ selection.fixtures[0].markets[0].outcomes[0].value === null ? '' : (': ' + selection.fixtures[0].markets[0].outcomes[0].value) }}
        </div>
        <div class="col">
          {{ selection.fixtures[0].markets[0].outcomes[0].odds.toFixed(2) }}
        </div>
        <div class="col">
          <i v-if="(icon !== null)" :class="iconk"></i>
        </div>
      </div>
    </div>
  </li>
</template>

<script lang="ts">
  import { defineComponent } from 'vue'

  import { Event_ } from "../../models"

  interface Data {
    statusClass: string | null,
    icon: string | null
  }

  export default defineComponent({
    props: {
      selection: Event_
    },
    data(): Data {
      return {
        statusClass: null,
        icon: null
      }
    },
    created() {
      this.statusClass = null
      this.icon = null

      // fetch the data when the view is created and the data is already being
      // observed
      this.initialiseData()
    },
    watch: {
      // call again the method if the route changes
      '$route': 'initialiseData'
    },
    methods: {
      initialiseData(): void {
        this.statusClass = 'list-group-item'
        this.icon = null

        const outcome = this.selection!.fixtures[0].markets[0].outcomes[0]

        this.statusClass = 'list-group-item'

        if (outcome.isResolved) {
          if (outcome.isWinning) {
            this.statusClass += ' list-group-item-success'
            this.icon = 'bi bi-check'
          }
          else {
            this.statusClass += ' list-group-item-danger'
            this.icon = 'bi bi-x'
          }
        }
        else
          this.statusClass += ' list-group-item-primary'
      }
    }
  })

</script>
