<template>
  <div class="container container-fluid">
    <div class="container container-fluid text-muted text-start" style="font-size: 0.6rem;">
      {{ fixture.type.name }}
    </div>
    <MarketDisplay v-for="market in fixture.markets"
                   :key="fixtureKey + '-' + market.id"
                   :promoted="promoted"
                   :marketKey="fixtureKey + '-' + market.id"
                   :market="market"
                   @checkOutcome="checkOutcome" />
  </div>
</template>

<script lang="ts">
  import { defineComponent } from 'vue'

  import { Fixture } from '../models'

  import MarketDisplay from './MarketDisplay.vue'

  export default defineComponent({
    components: {
      MarketDisplay
    },
    props: {
      promoted: Boolean,
      fixtureKey: String,
      fixture: Fixture
    },
    emits: [ 'checkOutcome' ],
    created() {
      // initialise the data when the view is created and the data is already
      // being observed
      this.initialiseData()
    },
    watch: {
      // call again the method if the route changes
      '$route': 'initialiseData'
    },
    methods: {
      initialiseData(): void { },
      checkOutcome(event: Event): void {
        this.$emit('checkOutcome', event);
      }
    }
  })

</script>
