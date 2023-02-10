<template>
  <div class="container container-fluid" style="min-height: 100%;">
    <nav class="navbar navbar-expand-lg navbar-dark bg-dark">
      <div class="container container-fluid">
        <RouterLink class="navbar-brand" to="/">Hat-Trick</RouterLink>
        <div class="d-flex">
          <ul class="navbar-nav me-auto mb-2 mb-lg-0">
            <li class="nav-item align-middle me-2 my-auto">
              <RouterLink class="nav-link" to="/">
                Today's Offer
              </RouterLink>
            </li>
            <UserNavItem />
          </ul>
        </div>
      </div>
    </nav>

    <div class="container container-fluid p-3">
      <RouterView />
    </div>
  </div>
</template>

<script lang="ts">
  import { defineComponent } from 'vue'
  import { RouterView } from 'vue-router'

  import UserNavItem from "./components/UserNavItem.vue"

  export default defineComponent({
    components: {
      RouterView,
      UserNavItem
    },
    created() { }
  })

</script>
