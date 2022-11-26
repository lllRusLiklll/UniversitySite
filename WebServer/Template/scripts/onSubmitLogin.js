function onSubmitLogin() {
    let form = document.Login;
    let warning = document.getElementById("warning");
    let remember = document.getElementById("remember");

    let xnr = new XMLHttpRequest();
    xnr.open("GET", window.location.origin + "/students/login/" +
        form.elements.Email.value + "/" +
        form.elements.Password.value + "/" +
        remember.checked, false);
    xnr.send();

    if (eval(xnr.responseText)) {
        warning.style.display = "none";
        window.location.href = window.location.origin + "/";
    }
    else {
        warning.style.display = "block";
        return false;
    }
}