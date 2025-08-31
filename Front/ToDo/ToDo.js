function getToken() {
  return localStorage.getItem('jwt');
  
}

function getUserRole() {
  return localStorage.getItem('role');
}

function isAdmin() {
  const role = getUserRole();
  return role === 'admin' || role === 'Admin';
}

function loadTodos() {
  fetch('http://localhost:5164/api/todo', {
    method: 'GET',
    headers: {
      Authorization: 'Bearer ' + getToken(),
    },
  })
    .then((res) => res.json())
    .then((todos) => {
      const ul = document.getElementById('todolist');
      ul.innerHTML = '';
      todos.forEach((todo) => {
        const li = document.createElement('li');
        li.textContent = todo.task;

        const editBtn = document.createElement('button');
        editBtn.textContent = 'Edit';
        editBtn.onclick = function () {
          const newTask = prompt('Edit task:', todo.task);
          if (newTask && newTask.trim() !== '') {
            editTask(todo.id, newTask.trim());
          }
        };

        const removeBtn = document.createElement('button');
        removeBtn.textContent = 'X';
        removeBtn.onclick = function () {
          removeTodo(todo.id);
        };

        li.appendChild(editBtn);
        li.appendChild(removeBtn);
        ul.appendChild(li);
      });
    });
}
window.onload = loadTodos;

function editTask(id, newTask) {
  fetch(`http://localhost:5164/api/todo/${id}`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      Authorization: 'Bearer ' + getToken(),
    },
    body: JSON.stringify({ task: newTask }),
  })
    .then((res) => {
      if (!res.ok) throw new Error('Failed to update task');
      return res.json();
    })
    .then(() => {
      loadTodos();
    })
    .catch((err) => alert(err.message));
}

function addTask() {
  const input = document.getElementById('new-task');
  const taskText = input.value.trim();
  if (!taskText) return;

  fetch('http://localhost:5164/api/todo', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      Authorization: 'Bearer ' + getToken(),
    },
    body: JSON.stringify({ task: taskText }),
  })
    .then((res) => res.json())
    .then(() => {
      loadTodos();
      input.value = '';
    });
}

function logout() {
  localStorage.removeItem('jwt');
  localStorage.removeItem('username');
  localStorage.removeItem('role');
  window.location.href = '/Front/index/index.html';
}
function removeTodo(id) {
  fetch(`http://localhost:5164/api/todo/${id}`, {
    method: 'DELETE',
    headers: {
      Authorization: 'Bearer ' + getToken(),
    },
  }).then(() => loadTodos());
}

function addUser(event) {
  event.preventDefault();
  if (!isAdmin()) {
    alert('Only admin users can add new users.');
    return;
  }
  const username = document
    .getElementById('new-username')
    .value.trim();
  const password = document
    .getElementById('new-password')
    .value.trim();
  const role = document
    .getElementById('new-role')
    .value.trim();
  if (!username || !password || !role) {
    alert('Username, password and role is required.');
    return;
  }
  fetch('http://localhost:5164/api/auth/register', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      Authorization: 'Bearer ' + getToken(),
    },
    body: JSON.stringify({ username, password, role }),
  })
    .then((res) => {
      if (!res.ok) throw new Error('Failed to add user');
      return res.json();
    })
    .then(() => {
      alert('User added successfully!');
      document.getElementById('new-username').value = '';
      document.getElementById('new-password').value = '';
      document.getElementById('new-role').value = '';
    })
    .catch((err) => alert(err.message));
}
