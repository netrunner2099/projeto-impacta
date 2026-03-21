// Funções para manipular cookies
function setCookie(name, value, days, secure, sameSite) {
    let expires = "";
    if (days) {
        let date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toUTCString();
    }

    let secureFlag = secure ? "; Secure" : "";
    let sameSiteFlag = sameSite ? "; SameSite=" + sameSite : "";

    document.cookie = name + "=" + (value || "") + expires + "; path=/" + secureFlag + sameSiteFlag;
}

function getCookie(name)
{
    let nameEQ = name + "=";
    let ca = document.cookie.split(';');
    for (let i = 0; i < ca.length; i++)
    {
        let c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}

function deleteCookie(name)
{
    document.cookie = name + '=; Path=/; Expires=Thu, 01 Jan 1970 00:00:01 GMT;';
}