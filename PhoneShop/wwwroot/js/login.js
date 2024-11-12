    <link href="css/style.css" rel="stylesheet">

const form = document.querySelector('form');
const usernameInput = document.getElementById('#Username');
const passwordInput = document.getElementById('#Password');

const usernameError = document.querySelector('.error-message:nth-of-type(1)');

const passwordError = document.querySelector('.error-message:nth-of-type(2)');

form.addEventListener('submit', (event) => {
    event.preventDefault();

    if (usernameInput.value.trim() === '') {
        usernameError.textContent = 'Please enter your username';
    } else {
        usernameError.textContent = '';
    }

    if (passwordInput.value.trim() === '') {
        passwordError.textContent = 'Please enter your password';
    } else {
        passwordError.textContent = '';
    }

    if (usernameInput.value.trim() !== '' && passwordInput.value.trim() !== '') {
        // Submit the form
        form.submit();
    }
});