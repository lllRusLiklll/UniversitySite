function onSubmitSignup() {
    let form = document.signUpForm;
    let warning = document.getElementById("warning");
    let msg = document.getElementById("msg");

    let xnr = new XMLHttpRequest();
    let body = "Firstname=" +
        encodeURIComponent(form.elements.Firstname.value) + "&Lastname=" +
        encodeURIComponent(form.elements.Lastname.value) + "&Patronymic=" +
        encodeURIComponent(form.elements.Patronymic.value) + "&Email=" +
        encodeURIComponent(form.elements.Email.value) + "&Password=" +
        encodeURIComponent(form.elements.Password.value) + "&RepeatPassword=" +
        encodeURIComponent(form.elements.RepeatPassword.value) + "&BirthDate=" +
        encodeURIComponent(form.elements.BirthDate.value) + "&Gender=" +
        encodeURIComponent(form.elements.Gender.value);
    xnr.open("POST", window.location.origin + "/students/signup", false);
    xnr.send(body);
    
    if (msg.style.display != "none") {
        return false;
    }
    else if (eval(xnr.responseText)) {
        warning.style.display = "none";
        window.location.href = window.location.origin + "/login.html";
    }
    else {
        warning.style.display = "block";
        return false;
    }
}