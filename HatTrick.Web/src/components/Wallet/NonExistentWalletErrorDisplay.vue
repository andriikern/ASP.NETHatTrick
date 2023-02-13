<template>
  <div class="card bg-danger text-bg-danger">
    <div class="card-header fw-bold">Error</div>
    <div class="card-body">
      The requested user does not seem to exist.
    </div>
  </div>
</template>

<script lang="ts">
  import { defineComponent } from 'vue'

  export default defineComponent({
    created() { }
  })

</script>
