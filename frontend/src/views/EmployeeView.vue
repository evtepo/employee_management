<template>
  <div id="app">
    <main class="content">
      <h1 class="table-title">Управление сотрудниками</h1>

      <div class="download-records">
        <button class="btn-download" @click="downloadData" title="Скачать данные сотрудников">
          Скачать
        </button>
      </div>

      <div class="add-record">
        <button class="btn-add" @click="addEmployee" title="Добавить сотрудника">
          Добавить сотрудника
        </button>
      </div>
      <div v-if="showAddWindow" class="modal-overlay">
        <div class="modal-content">
          <form @submit.prevent="saveNewEmployee">
            <label>
              <input type="text" v-model="employeeToAdd.firstName" placeholder="Имя" required>
            </label>
            <label>
              <input type="text" v-model="employeeToAdd.lastName" placeholder="Фамилия" required>
            </label>
            <label>
              <input type="text" v-model="employeeToAdd.department" placeholder="Отдел" required>
            </label>
            <label>
              <input type="date" v-model="employeeToAdd.dateOfBirth" placeholder="Дата рождения" required>
            </label>
            <div class="modal-actions">
              <button type="submit" class="btn-save">Добавить</button>
              <button type="button" @click="cancelAdd" class="btn-cancel">Отмена</button>
            </div>
          </form>
        </div>
      </div>

      <div class="filters">
        <span><h4>Сортировка</h4></span>
        <div class="sorting-fields">
          <button @click="setSort('firstName')">Имя {{ getSortDirection('firstName') }}</button>
          <button @click="setSort('lastName')">Фамилия {{ getSortDirection('lastName') }}</button>
          <button @click="setSort('departmentTitle')">Отдел {{ getSortDirection('departmentTitle') }}</button>
          <button @click="setSort('dateOfBirth')">Дата {{ getSortDirection('dateOfBirth') }}</button>
        </div>

        <span><h4>Фильтр</h4></span>
        <div class="filter-fields">
          <select v-model="filterField">
            <option value="firstName">Имя</option>
            <option value="lastName">Фамилия</option>
            <option value="departmentTitle">Отдел</option>
            <option value="dateOfBirth">Дата рождения</option>
          </select>
          <input v-if="filterField === 'dateOfBirth'" type="date" v-model="filterValue">
          <input v-else type="text" v-model="filterValue">
          <button class="btn-save" @click="fetchEmployees">Применить</button>
          <button class="btn-cancel" @click="clearFilter">Очистить</button>
        </div>

      </div>
      <table class="table">
        <thead>
          <tr>
            <th>Имя</th>
            <th>Фамилия</th>
            <th>Отдел</th>
            <th>Дата рождения</th>
            <th>Действие</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="employee in employees" :key="employee.id">
            <td class="name-column">{{ employee.firstName }}</td>
            <td class="lastname-column">{{ employee.lastName }}</td>
            <td class="department-column">{{ employee.department.title }}</td>
            <td class="date-column">{{ employee.dateOfBirth }}</td>
            <td class="action-column">
              <button class="btn-delete" @click="deleteEmployee(employee.id)" title="Удалить запись">
                <i class="fas fa-trash delete"></i>
              </button>
              <button class="btn-edit" @click="changeEmployee(employee)" title="Изменить запись">
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
              <input type="text" v-model="employeeToChange.firstName" placeholder="Имя" required>
            </label>
            <label>
              <input type="text" v-model="employeeToChange.lastName" placeholder="Фамилия" required>
            </label>
            <label>
              <input type="text" v-model="employeeToChange.department.title" placeholder="Отдел" required>
            </label>
            <label>
              <input type="date" v-model="employeeToChange.dateOfBirth" placeholder="Дата рождения" required>
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
        <select id="rows" v-model="rowsPerPage" @change="fetchEmployees">
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

const axios = require('axios').default;
const api = axios.create({baseURL: 'http://localhost:5133/api/v1/employee'});

export default {
  name: 'EmployeeManagement',
  data() {
    return {
      employees: [],
      rowsPerPage: 10,
      currentPage: 1,
      lastPage: 1,

      sortBy: "",
      descending: null,

      filterField: "firstName",
      filterValue: "",

      showConfirmation: false,
      employeeIdToDelete: null,

      showChangeWindow: false,
      employeeToChange: {},

      showAddWindow: false,
      employeeToAdd: {},
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
    // Получение сотрудников
    async fetchEmployees() {
      const params = {
        page: this.currentPage,
        size: this.rowsPerPage,
      }
      if (this.sortBy) {
        params.sortBy = this.sortBy;
        params.descending = this.descending;
      }

      if (this.filterField && this.filterValue) {
        params[this.filterField] = this.filterValue;
      }

      try {
        const data = await this.makeRequest('get', '/', {}, params);
        this.employees = data.data;
        this.lastPage = data.links.last;
      } catch (error) {
        console.error('Failed to fetch employees:', error);
      }
    },
    // Удаление сотрудника
    deleteEmployee(employee_id) {
      this.showConfirmation = true;
      this.employeeIdToDelete = employee_id;
    },
    async deleteConfirmation() {
      try {
        await this.makeRequest('delete', `/${this.employeeIdToDelete}`, {}, {});
        this.showConfirmation = false;
        await this.fetchEmployees();
      } catch (error) {
        console.error('Failed to delete employee:', error)
      }
    },
    cancelDelete() {
      this.showConfirmation = false;
    },
    // Изменение сотрудника
    changeEmployee(employee) {
      this.showChangeWindow = true;
      this.employeeToChange = JSON.parse(JSON.stringify(employee));
    },
    async saveChanges() {
      try {
        this.employeeToChange.department = this.employeeToChange.department.title;
        await this.makeRequest('put', `/${this.employeeToChange.id}`, this.employeeToChange);
        this.showChangeWindow = false;
        this.employeeToChange = {};
        await this.fetchEmployees();
      } catch (error) {
        console.error('Failed to update emplyoee:', error)
      }
    },
    cancelChange() {
      this.showChangeWindow = false;
      this.employeeToChange = {};
    },
    // Добавление сотрудника
    addEmployee() {
      this.showAddWindow = true;
    },
    async saveNewEmployee() {
      try {
        await this.makeRequest('post', '/', this.employeeToAdd);
        this.showAddWindow = false;
        this.employeeToAdd = {};
        await this.fetchEmployees();
      } catch(error) {
        console.error('Failed to create new employee:', error)
      }
    },
    cancelAdd() {
      this.showAddWindow = false;
      this.employeeToAdd = {};
    },
    // Сортировка
    async setSort(field) {
      if (this.sortBy === field) {
        if (this.descending === false) {
          this.descending = true;
        } else if (this.descending === true) {
          this.sortBy = "";
          this.descending = null;
        }
      } else {
        this.sortBy = field;
        this.descending = false;
      }
      await this.fetchEmployees();
    },
    getSortDirection(field) {
      if (this.sortBy === field) {
        if (this.descending === true) {
          return '↓';
        }
        return '↑';
      }
    },
    // Фильтрация
    async clearFilter() {
      this.filterField = "firstName";
      this.filterValue = "";
      await this.fetchEmployees();
    },
    // Пагинация
    async prevPage() {
      if (this.currentPage > 1) {
        this.currentPage--;
        await this.fetchEmployees();
      }
    },
    async nextPage() {
      if (this.currentPage < this.lastPage) {
        this.currentPage++;
        console.log(this.currentPage)
        await this.fetchEmployees();
      }
    },
    async selectPage(page) {
      this.currentPage = page;
      await this.fetchEmployees();
    },
    // Скачивание данных
    downloadData() {
      let employeeToDownload = JSON.parse(JSON.stringify(this.employees));
      employeeToDownload.forEach(employee => {
        employee.department = employee.department.title;
      });

      const worksheet = XLSX.utils.json_to_sheet(employeeToDownload);
      const workbook = XLSX.utils.book_new();

      XLSX.utils.book_append_sheet(workbook, worksheet, "Employee");
      XLSX.writeFile(workbook, "employee.xlsx");
    }
  },
  async created() {
    await this.fetchEmployees();
  },
}
</script>
      
<style>
.download-records {
  position: absolute;
  right: 10%;
  width: 250px;
  top: 235px;
  display: flex;
  align-items: center;
}

.add-record {
  position: absolute;
  right: 6%;
  width: 250px;
  top: 235px;
  display: flex;
  align-items: center;
}

.content {
  padding: 20px;
  margin-top: 50px;
}

.filters {
  position: absolute;
  width: 7%;
  height: 290px;
  left: 0;
  margin-left: 20px;
  border: 1px solid #ccc;
  border-radius: 8px;
  padding: 15px;
  background-color: #ffffff;
}

.filters h4 {
  margin-top: 10px;
  margin-bottom: 5px;
  font-size: 16px;
  font-weight: bold;
  color: #333;
  text-align: center;
}

.sorting-fields {
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.sorting-fields button {
  text-align: left;
  cursor: pointer;
  background: none;
  border: none;
  font: inherit;
  border-radius: 4px;
  transition: background-color 0.3s ease;
}

.sorting-fields button:hover {
  background-color: #ff7f5023;
}

.filter-fields {
  display: flex;
  flex-direction: column;
  gap: 5px;
}

.filter-fields select {
  width: 100%;
  padding: 2px 5px;
  border-radius: 6px;
  font-size: 14px;
  border: 1px solid #2f2a2a;
  margin-bottom: 5px;
  cursor: pointer;
}

.filter-fields select:focus {
  outline: none;
}

.filter-fields input {
  width: 91%;
  border: 1px solid #2f2a2a;
  border-radius: 6px;
  font-size: 14px;
  color: #333;
  transition: border-color 0.3s;
  padding: 2px 5px;
  margin-bottom: 10px;
}

.filter-fields input:hover {
  border-color: #5cb85c;
}

.filter-fields input:focus {
  border-color: #5cb85c;
  outline: none;
}

.filter-fields button {
  border-radius: 6px;
  cursor: pointer;
  width: 80px;
  margin: auto;
  padding: 4px 6px;
  transition: all 0.2s ease;
  font-size: 13px;
}

.filter-fields .btn-save {
  border: 1px solid #5cb85c;
  width: 60%;
}

.filter-fields .btn-save:hover {
  transform: scale(1.05);
}

.filter-fields .btn-cancel {
  border: 1px solid #f0f0f0;
  width: 60%;
}

.filter-fields .btn-cancel:hover {
  transform: scale(1.05);
  background-color: #dcdcdc;
}
</style>
      