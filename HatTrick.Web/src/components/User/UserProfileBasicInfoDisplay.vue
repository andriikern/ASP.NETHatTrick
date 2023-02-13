<template>
  <table class="table table-striped table-hover">
    <thead class="table-dark">
      <tr>
        <th scope="row">Username</th>
        <td class="font-monospace">{{ user.username }}</td>
      </tr>
      <tr>
        <th scope="row"
            class="font-monospace align-middle my-auto">
          Balance
        </th>
        <td class="clearfix">
          <span class="font-monospace align-middle my-auto">{{ user.balance.toFixed(2) }} &euro;</span>
          <RouterLink class="btn btn-light align-middle my-auto float-end"
                      to="wallet"
                      role="button">
              Manage
          </RouterLink>
        </td>
      </tr>
    </thead>
    <tbody>
      <tr>
        <th scope="row">Name</th>
        <td>{{ user.name }}</td>
      </tr>
      <tr>
        <th scope="row">Surname</th>
        <td>{{ user.surname }}</td>
      </tr>
      <tr>
        <th scope="row">Sex</th>
        <td>{{ user.sex }}</td>
      </tr>
      <tr>
        <th scope="row">Email</th>
        <td class="font-monospace"><a class="link link-dark" target="_blank" :href="'mailto:' + user.email">&lt;{{ user.email }}&gt;</a></td>
      </tr>
      <tr>
        <th scope="row">Address</th>
        <td>{{ user.address }}, {{ user.city }}, {{ user.country }}</td>
      </tr>
      <tr>
        <th scope="row">Phone</th>
        <td class="font-monospace">{{ user.phone }}</td>
      </tr>
      <tr>
        <th scope="row">Birthdate</th>
        <td>{{ new Date(user.birthdate).toLocaleString('en-GB', { dateStyle: 'short' }) }}</td>
      </tr>
      <tr>
        <th scope="row">Registration date</th>
        <td>{{ new Date(user.registeredOn).toLocaleString('en-GB', { dateStyle: 'short', timeStyle: 'short' }) }}</td>
      </tr>
    </tbody>
  </table>
</template>

<script lang="ts">
  import { defineComponent } from 'vue'
  import { RouterLink } from 'vue-router'

  import { User } from "../../models"

  export default defineComponent({
    components: {
      RouterLink
    },
    props: {
      user: User
    },
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
      initialiseData(): void { }
    }
  })

</script>
