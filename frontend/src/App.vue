<template>
    <header class="header">
      <router-link to="/employee" class="title">
        <img src="@/assets/logo.png" alt="Logo" class="title-icon">
        <div class="title-content">
          Management
        </div>
      </router-link>
      <nav class="nav">
        <router-link to="/employee" class="nav-link">Сотрудники</router-link>
        <router-link to="/department" class="nav-link">Отделы</router-link>
      </nav>
    </header>
  <router-view/>
</template>

<style>
body, #app {
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
  margin: 0;
  padding: 0;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
}

.header {
  z-index: 1000;
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 50px;
  background-color: #f8f8f8;
  padding: 16px 0px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.nav {
  display: flex;
  gap: 20px;
  width: 100%;
  justify-content: center;
}

.nav-link {
  padding: 12px 24px;
  text-decoration: none;
  color: #333;
  font-weight: 500;
  font-size: 16px;
  border-radius: 50px;
  border: 2px solid transparent;
  transition: all 0.3s ease;
}

.nav-link:hover {
  background-color: #ff7f50;
  color: #fff;
  border-color: #ff7f50;
  transform: scale(1.05);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.nav-link.router-link-active {
  background-color: #ff7f50;
  color: #fff;
  font-weight: 600;
}

.title {
  text-decoration: none;
  display: flex;
  align-items: center;
  color: #333;
  font-weight: 600;
  font-size: 24px;
  gap: 10px;
  transition: color 0.3s ease;
  position: absolute;
  margin-left: 20px;
}

.title:hover {
  color: #ff7f50;
}

.title-icon {
  width: 40px;
  height: 40px;
  transition: transform 0.3s ease;
}

.table {
  width: 80%;
  border-collapse: collapse;
  margin: 0 auto 20px;
}

.table th, .table td {
  border: 1px solid #ddd;
  padding: 8px;
  text-align: left;
  vertical-align: top;
}

.table th {
  background-color: #f4f4f4;
  font-weight: bold;
}

.table th, .table td {
  max-width: 250px;
  word-wrap: break-word;
  white-space: normal;
}

.table tbody tr .action-column {
  width: 80px;
}

.table tbody tr .name-column,
.lastname-column {
  width: 250px;
}

.table tbody tr .date-column {
  width: 140px;
}

.table-title {
  text-align: center;
  margin: 100px 0px 60px;
}

.btn-delete {
  background-color: transparent;
  border: none;
  cursor: pointer;
  margin-left: 10px;
}

.btn-edit {
  background-color: transparent;
  border: none;
  cursor: pointer;
}

.btn-edit i {
  color: black;
  font-size: 14px;
}

.btn-delete i {
  color: #ff0000;
  font-size: 14px;
}

.rows-per-page {
  position: absolute;
  margin-left: 190px;
  width: 12%;
}

.rows-per-page select {
  border-radius: 5px;
  cursor: pointer;
}

.rows-per-page select:focus {
  outline: none;
}

.pagination {
  display: flex;
  justify-content: center;
  height: 100px;
  margin: auto;
}

.page-controls {
  display: flex;
  align-items: center;
  gap: 10px;
  padding-bottom: 80px;
}

.page-controls button {
  padding: 5px 10px;
  border: 1px solid #ccc;
  background-color: #ffffff;
  color: #333;
  font-size: 14px;
  font-weight: bold;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.page-controls button:hover {
  background-color: #007bff;
  color: #ffffff;
  border-color: #007bff;
}

.page-controls button:disabled {
  background-color: #f1f1f1;
  color: #aaa;
  cursor: not-allowed;
  box-shadow: none;
  transform: none;
}

.page-controls button:disabled:hover {
  border-color: #ccc;
}

.modal-overlay {
  z-index: 1000;
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
}

.modal {
  background-color: white;
  padding: 20px;
  border-radius: 8px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  width: 300px;
  text-align: center;
  transition: transform 0.3s ease-in-out;
}

.modal h3 {
  margin-bottom: 20px;
  font-size: 18px;
  font-weight: bold;
}

.modal-actions {
  display: flex;
  justify-content: space-around;
  margin: 0px 60px;
}

.modal-actions button {
  padding: 8px 16px;
  font-size: 14px;
  border: none;
  cursor: pointer;
  border-radius: 4px;
  transition: background-color 0.3s ease, transform 0.2s ease;
}

.modal-actions .btn-confirm {
  background-color: #ff4d4d;
  color: white;
}

.modal-actions .btn-cancel {
  background-color: #f0f0f0;
  color: #333;
}

.modal-actions button:hover {
  transform: scale(1.05);
}

.modal-actions .btn-confirm:hover {
  background-color: #ff1a1a;
}

.modal-actions .btn-cancel:hover {
  background-color: #dcdcdc;
}

.modal-content {
  background: #fff;
  padding: 30px;
  border-radius: 12px;
  width: 450px;
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
}

.modal-content input {
  width: 95%;
  padding: 10px;
  margin-bottom: 20px;
  border: 1px solid #ddd;
  border-radius: 6px;
  font-size: 14px;
  color: #333;
  transition: border-color 0.3s;
}

.modal-content input:focus {
  border-color: #5cb85c;
  outline: none;
}

.modal-content h3 {
  text-align: center;
  font-size: 24px;
  margin-bottom: 20px;
  color: #333;
}

.modal-content label {
  font-size: 14px;
  color: #555;
}

.modal-overlay .modal-content .modal-actions {
  padding: 0px 40px;
}

.modal-actions {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.btn-save {
  background-color: #5cb85c;
  color: white;
}

.btn-add {
  padding: 8px;
  background-color: #5cb85c;
  border: 1px solid;
  border-radius: 8px;
  cursor: pointer;
  color: white;
  font-size: 14px;
  transition: transform 0.3s ease;
}

.btn-add:hover {
  transform: scale(1.05);
}

.btn-download {
  padding: 8px;
  background-color: #007bff;
  border: 1px solid;
  border-radius: 8px;
  cursor: pointer;
  color: white;
  font-size: 14px;
  transition: transform 0.3s ease;
}

.btn-download:hover {
  transform: scale(1.05);
}
</style>
