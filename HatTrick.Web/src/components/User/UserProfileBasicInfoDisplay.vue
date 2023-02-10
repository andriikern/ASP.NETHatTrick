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
          <button type="button"
                  class="btn btn-light align-middle my-auto float-end"
                  @click="console.log">
              Manage
          </button>
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
        <td class="font-monospace"><a :href="'mailto:' + user.email" class="link link-dark">&lt;{{ user.email }}&gt;</a></td>
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
        <td>{{ new Date(user.birthdate).toLocaleString('en-GB') }}</td>
      </tr>
      <tr>
        <th scope="row">Registration date</th>
        <td>{{ new Date(user.registeredOn).toLocaleString('en-GB') }}</td>
      </tr>
    </tbody>
  </table>
</template>

<script lang="ts">
  import { defineComponent } from 'vue'

  import { User } from "../../models"

  export default defineComponent({
    props: {
      user: User
    },
    created() { }
  })

</script>
