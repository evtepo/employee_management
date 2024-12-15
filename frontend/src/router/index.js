import { createRouter, createWebHistory } from 'vue-router'

import EmployeeView from '@/views/EmployeeView.vue'
import DepartmentView from '@/views/DepartmentView.vue'

const routes = [
  {
    path: '/employee',
    name: 'employee',
    component: EmployeeView,
  },
  {
    path: '/',
    redirect: {name: 'employee'}
  },
  {
    path: '/department',
    name: 'department',
    component: DepartmentView,
  }
]

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes
})

export default router
