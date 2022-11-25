function validatePassword() {
    var form = document.forms.signUpForm;
    var password = form.elements.Password.value;
    var repeatPassword = form.elements.RepeatPassword.value;
    var div = document.getElementById("msg");

    if (password != repeatPassword)
        div.style.display = "block";
    else
        div.style.display = "none";
}