# Management App
Приложение для работы с данными сотрудников и отделами.

## Подготовка окружения
- Перейти к директорию с frontend
```sh
  cd ./frontend/
```
- Создать файл `.env` и перенести переменные окружения из `.env.example` в `.env`
```sh
  echo > .env
```

## Запуск приложения
### Backend
- Перейти в директорию с сервисом
```sh
  cd ./backend/EmployeeManagementAPI/
```
- Установить зависимости
```sh
  dotnet restore
```
- Запуск сервиса
```sh
  dotnet run
```
### Frontend
- Перейти в директорию с сервисом
```sh
  cd ./frontend/
```
- Установить зависимости
```sh
  npm install
```
- Запуск сервиса
```sh
  vue serve
```
## Документация к API
### Employee
#### 1. Создание сотрудника
- **Метод**: `POST`
- **URL**: `http://localhost:5133/api/v1/employee`
- **Тело запроса** (JSON):
  ```json
  {
    "firstName": "Антон",
    "lastName": "Смирнов",
    "department": "Служба охраны",
    "dateOfBirth": "1985-01-15"
  }
  ```
#### 2. Получение сотрудника
- **Метод**: `GET`
- **URL**: `http://localhost:5133/api/v1/employee/<employee_id>`
#### 3. Получение списка сотрудников
- **Метод**: `GET`
- **URL**: `http://localhost:5133/api/v1/employee/`
- **Параметры запроса**
  - `page`: Номер страницы (Наприме, `1`)
  - `size`: Размер страницы(Например, `10`)
  - `firstName`: Фильтрация по имени (Например, `Андрей`)
  - `lastName`: Фильтрация по фамилии (Например, `Смирнов`)
  - `dateOfBirth`: Фильтрация по дате рождения (Наприме, `1999-02-12`)
  - `departmentTitle`: Фильтрация по отделу (Например, `Служба охраны`)
  - `sortBy`: Сортировка по полю (Например, `firstName`)
  - `descending`: Сортировку по убыванию (`true` || `false`)
#### 4. Обновление сотрудника
- **Метод**: `PUT`
- **URL**: `http://localhost:5133/api/v1/employee/<employee_id>`
- **Тело запроса** (JSON):
  ```json
  {
    "firstName": "Андрей",
    "lastName": "Смирнов",
    "department": "Отдел снабжения",
    "dateOfBirth": "1985-01-15"
  }
  ```
#### 5. Удаление сотрудника
- **Метод**: `DELETE`
- **URL**: `http://localhost:5133/api/v1/employee/<employee_id>`

### Department
#### 1. Создание отдела
- **Метод**: `POST`
- **URL**: `http://localhost:5133/api/v1/department`
- **Тело запроса** (JSON):
  ```json
  {
    "title": "Служба охраны"
  }
  ```
#### 2. Получение отдела
- **Метод**: `GET`
- **URL**: `http://localhost:5133/api/v1/department/<department_id>`
#### 3. Получение списка отделов
- **Метод**: `GET`
- **URL**: `http://localhost:5133/api/v1/department/`
- **Параметры запроса**
  - `page`: Номер страницы (Наприме, `1`)
  - `size`: Размер страницы(Например, `10`)
#### 4. Обновление отдела
- **Метод**: `PUT`
- **URL**: `http://localhost:5133/api/v1/department/<department_id>`
- **Тело запроса** (JSON):
  ```json
  {
    "title": "Отдел снабжения"
  }
  ```
#### 5. Удаление отдела
- **Метод**: `DELETE`
- **URL**: `http://localhost:5133/api/v1/department/<department_id>`

## Используемые технологии
| Компонент                  | Технология                                                                                              |
|----------------------------|---------------------------------------------------------------------------------------------------------|
| **Фреймворк для API**      | [ASP.NET](https://learn.microsoft.com/ru-ru/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-9.0)|
| **Фреймворк для Frontend** | [Vue.js](https://vuejs.org/)                                                                            |
| **База данных**            | [Sqlite](https://www.sqlite.org/)                                                                       |
