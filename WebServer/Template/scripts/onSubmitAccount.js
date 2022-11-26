function onSubmitAccount() {
    let form = document.editForm;
    let stealth = document.getElementById("stealth");
    /*let warning = document.getElementById("warning");
    let msg = document.getElementById("msg");*/

    let xnr = new XMLHttpRequest();
    let body = "StudentID=" +
        encodeURIComponent(stealth.innerHTML) + "&Firstname=" +
        encodeURIComponent(form.elements.Firstname.value) + "&Lastname=" +
        encodeURIComponent(form.elements.Lastname.value) + "&Patronymic=" +
        encodeURIComponent(form.elements.Patronymic.value) + "&Email=" +
        encodeURIComponent(form.elements.Email.value);
    xnr.open("POST", window.location.origin + "/students/update", false);
    xnr.send(body);
    
    window.location.href = window.location.origin + "/personal/account";
}