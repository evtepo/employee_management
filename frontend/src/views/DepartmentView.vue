<template>
  <div id="app">
    <NotificationComponent ref="notification"/>
    <main class="content">
      <h1 class="table-title">Управление отделами</h1>

      <div class="download-departments">
        <button class="btn-download" @click="downloadData" title="Скачать данные отделов">
          Скачать
        </button>
      </div>

      <div class="add-department">
        <button class="btn-add" @click="addDepartment" title="Добавить отдел">
          Добавить отдел
        </button>
      </div>
      <div v-if="showAddWindow" class="modal-overlay">
        <div class="modal-content">
          <form @submit.prevent="saveNewDepartment">
            <label>
              <input type="text" v-model="departmentToAdd.title" placeholder="Название" required>
            </label>
            <div class="modal-actions">
              <button type="submit" class="btn-save">Добавить</button>
              <button type="button" @click="cancelAdd" class="btn-cancel">Отмена</button>
            </div>
          </form>
        </div>
      </div>
      <table class="table">
        <thead>
          <tr>
            <th>Название</th>
            <th>Действие</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="department in departments" :key="department.id">
            <td class="title-column">{{ department.title }}</td>
            <td class="action-column">
              <button class="btn-delete" @click="deleteDepartment(department.id)" title="Удалить запись">
                <i class="fas fa-trash delete"></i>
              </button>
              <button class="btn-edit" @click="changeDepartment(department)" title="Изменить запись">
                <i class="fas fa-edit"></i>
              </button>
            </td>
          </tr>
        </tbody>
      </table>

      <div v-if="showConfirmation" class="modal-overlay">
        <div class="modal">
          <h3>Подвердить удаление?</h3>
          <div class="modal-actions">
            <button @click="deleteConfirmation" class="btn-confirm">Да</button>
            <button @click="cancelDelete" class="btn-cancel">Отмена</button>
          </div>
        </div>
      </div>

      <div v-if="showChangeWindow" class="modal-overlay">
        <div class="modal-content">
          <h3>Изменить данные</h3>
            <form @submit.prevent="saveChanges">
            <label>
              <input type="text" v-model="departmentToChange.title" placeholder="Название" required>
            </label>
            <div class="modal-actions">
              <button type="submit" class="btn-save">Сохранить</button>
              <button type="button" @click="cancelChange" class="btn-cancel">Отмена</button>
            </div>
          </form>
        </div>
      </div>

      <div class="rows-per-page">
        <label for="rows">Количество записей: </label>
        <select id="rows" v-model="rowsPerPage" @change="fetchDepartments">
          <option value="5">5</option>
          <option value="10">10</option>
          <option value="20">20</option>
          <option value="50">50</option>
          <option value="100">100</option>
        </select>
      </div>

      <div class="pagination">
        <div class="page-controls">
          <button v-if="currentPage !== 1" @click="selectPage(1)">1</button>
          <button @click="prevPage" :disabled="currentPage === 1">Назад</button>
          <span>{{ currentPage }}</span>
          <button @click="nextPage" :disabled="currentPage === lastPage || lastPage === 0">Вперед</button>
          <button v-if="currentPage !== lastPage && lastPage !== 0" @click="selectPage(lastPage)">{{ lastPage }}</button>
        </div>
      </div>
    </main>
  </div>
</template>
    
<script>
import * as XLSX from "xlsx";

import NotificationComponent from "@/components/NotificationComponent.vue";

const axios = require('axios').default;
const back_url = process.env.VUE_APP_BACKEND_URL + 'department'
const api = axios.create({baseURL: back_url});

export default {
  components: {
    NotificationComponent
  },
  name: 'DepartmentManagement',
  data() {
    return {
      departments: [],
      rowsPerPage: 10,
      currentPage: 1,
      lastPage: 1,

      showConfirmation: false,
      departmentIdToDelete: null,

      showChangeWindow: false,
      departmentToChange: {},

      showAddWindow: false,
      departmentToAdd: {},
    };
  },
  methods: {
    // Запрос на бек
    async makeRequest(method, url, body = {}, params = {}) {
      try {
        const response = await api({
          method,
          url,
          data: body,
          params,
        });
        return response.data;
      } catch (error) {
        console.error(`Error making ${method} request to ${url}:`, error);
        throw error;
      }
    },
    // Получение отделов
    async fetchDepartments() {
      const params = {
        page: this.currentPage,
        size: this.rowsPerPage,
      }
      try {
        const data = await this.makeRequest('get', '/', {}, params);
        this.departments = data.data;
        this.lastPage = data.links.last;
      } catch (error) {
        console.error('Failed to fetch departments:', error);
      }
    },
    // Удаление отдела
    deleteDepartment(department_id) {
      this.showConfirmation = true;
      this.departmentIdToDelete = department_id;
    },
    async deleteConfirmation() {
      try {
        await this.makeRequest('delete', `/${this.departmentIdToDelete}`, {}, {});
        this.showConfirmation = false;
        this.$refs.notification.getNotification('delete', 'Запись успешно удалена');
        await this.fetchDepartments();
      } catch (error) {
        this.$refs.notification.getNotification('error', 'Ошибка при удалении');
        console.error('Failed to delete department:', error)
      }
    },
    cancelDelete() {
      this.showConfirmation = false;
    },
    // Изменение отдела
    changeDepartment(department) {
      this.showChangeWindow = true;
      this.departmentToChange = JSON.parse(JSON.stringify(department));
    },
    async saveChanges() {
      try {
        await this.makeRequest('put', `/${this.departmentToChange.id}`, this.departmentToChange);
        this.showChangeWindow = false;
        this.departmentToChange = {};
        this.$refs.notification.getNotification('success', 'Запись успешно обновлена');
        await this.fetchDepartments();
      } catch (error) {
        this.$refs.notification.getNotification('error', 'Ошибка при измении');
        console.error('Failed to update department:', error)
      }
    },
    cancelChange() {
      this.showChangeWindow = false;
      this.departmentToChange = {};
    },
    // Добавление отдела
    addDepartment() {
      this.showAddWindow = true;
    },
    async saveNewDepartment() {
      try {
        await this.makeRequest('post', '/', this.departmentToAdd);
        this.showAddWindow = false;
        this.departmentToAdd = {};
        this.$refs.notification.getNotification('success', 'Запись успешно создана');
        await this.fetchDepartments();
      } catch(error) {
        this.$refs.notification.getNotification('error', 'Ошибка при создании');
        console.error('Failed to create new department:', error)
      }
    },
    cancelAdd() {
      this.showAddWindow = false;
      this.departmentToAdd = {};
    },
    // Пагинация
    async prevPage() {
      if (this.currentPage > 1) {
        this.currentPage--;
        await this.fetchDepartments();
      }
    },
    async nextPage() {
      if (this.currentPage < this.lastPage) {
        this.currentPage++;
        console.log(this.currentPage)
        await this.fetchDepartments();
      }
    },
    async selectPage(page) {
      this.currentPage = page;
      await this.fetchDepartments();
    },
    // Скачивание данных
    downloadData() {
      const worksheet = XLSX.utils.json_to_sheet(this.departments);
      const workbook = XLSX.utils.book_new();

      XLSX.utils.book_append_sheet(workbook, worksheet, "Department");
      XLSX.writeFile(workbook, "department.xlsx");
    }
  },
  async created() {
    await this.fetchDepartments();
  },
}
</script>
      
<style>
.content {
  padding: 20px;
  margin-top: 50px;
}

.add-department {
  position: absolute;
  right: 4%;
  width: 250px;
  top: 235px;
  display: flex;
  align-items: center;
}

.download-departments {
  position: absolute;
  right: 8%;
  width: 250px;
  top: 235px;
  display: flex;
  align-items: center;
}
</style>
      