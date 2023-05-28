$(document).ready(function () {
    window.history.pushState(null, document.title, window.location.href);

    window.addEventListener('popstate', function (event) {

        window.history.pushState(null, document.title, window.location.href);

        window.location.href = '/LogIn/Index';
    });
});
