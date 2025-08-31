const apiUrl = 'http://localhost:5164';

function saveToken(token) {
  localStorage.setItem('jwt', token);
}

function getToken() {
  return localStorage.getItem('jwt');
}

async function login() {
  const username = document.getElementById(
    'login-username'
  ).value;
  const password = document.getElementById(
    'login-password'
  ).value;

  // debugger;

  fetch(`${apiUrl}/api/auth/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      UserName: username,
      Password: password,
    }),
  })
    .then((res) => res.json())
    .then((data) => {
      console.log('Login response:', data); // Debug: inspect response
      if (data.token) {
        saveToken(data.token);
        localStorage.setItem('role', data.role);
        localStorage.setItem('username', data.username);
        window.location.href = '../Todo/Todo.html';
      } else {
        alert('Login failed');
      }
    })
    .catch(console.error);
}
