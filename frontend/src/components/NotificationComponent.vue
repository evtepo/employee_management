<template>
    <div v-if="showNotification" class="notification-overlay">
        <div v-if="notificationType === 'success'" class="success">
            <span>{{ notificationMessage }}</span>
        </div>
        <div v-else-if="notificationType === 'delete'" class="delete">
            <span>{{ notificationMessage }}</span>
        </div>
        <div v-else-if="notificationType === 'error'" class="error">
            <span>{{ notificationMessage }}</span>
        </div>
    </div>
</template>

<script>
export default {
    name: "NotificationComponent",
    data() {
        return {
            showNotification: false,
            notificationType: "",
            notificationMessage: ""
        }
    },
    methods: {
        getNotification(notification_type, message) {
            this.showNotification = true;
            this.notificationType = notification_type;
            this.notificationMessage = message;
            setTimeout(
                () => {
                    this.showNotification = false,
                    this.notificationType = "",
                    this.notificationMessage = ""
                },
                3000
            )
        }
    }
}
</script>

<style>
.notification-overlay {
  position: fixed;
  margin: auto;
  left: 0;
  right: 0;
  top: 80px;
  width: 11%;
  padding: 10px;
  border-radius: 5px;
  color: #3a3737;
  font-weight: bold;
  z-index: 1000;
  text-align: center;
  animation: fadeInOut 3s ease;
}

.notification-overlay .success, .error, .delete {
  border-radius: 18px;
  background-color: white;
}

.notification-overlay .success {
  border: 1.5px solid #0fb40799;
  padding: 10px 0;
}

.notification-overlay .error {
  border: 1.5px solid #b4070799;
  padding: 10px 0;
}

.notification-overlay .delete {
  border: 1.5px solid #fb900499;
  padding: 10px 0;
}

@keyframes fadeInOut {
  0%, 100% {
    opacity: 0;
    transform: translateY(20px);
  }
  20% {
    opacity: 1;
    transform: translateY(0);
  }
  80% {
    opacity: 1;
    transform: translateY(0);
  }
}
</style>