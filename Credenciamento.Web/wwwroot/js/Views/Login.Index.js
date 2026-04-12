$(document).ready(function () {
    const $loginField = $('#Login');
    const $passwordField = $('#passwordField');

    // Toggle Senha
    $('#togglePassword').on('click', function () {
        const type = $passwordField.attr('type') === 'password' ? 'text' : 'password';
        $passwordField.attr('type', type);
        $('#toggleIcon').toggleClass('bi-eye bi-eye-slash');
    });

    // Configuração de validação
    $.validator.setDefaults({
        onsubmit: true,
        onfocusout: function (element) {
            if ($(element).is('select')) {
                return false;
            }
            return false;
        },
        onkeyup: false,
        onclick: false,
        highlight: function (element) {
            $(element).addClass('is-invalid').removeClass('is-valid');
        },
        unhighlight: function (element) {
            $(element).removeClass('is-invalid').addClass('is-valid');
        },
        errorElement: 'div',
        errorClass: 'invalid-feedback',
        errorPlacement: function (error, element) {
            error.insertAfter(element);
        },
        ignore: []
    });

    // Validar email
    function validateEmail() {
        const loginValue = $loginField.val().trim();

        $loginField.removeClass('is-invalid is-valid');
        $loginField.next('.text-danger').remove();

        if (!loginValue) {
            $loginField.addClass('is-invalid');
            $loginField.after('<span class="text-danger small">Por favor, informe o email.</span>');
            $loginField.focus();
            return false;
        }

        if (!validarEmail(loginValue)) {
            $loginField.addClass('is-invalid');
            $loginField.after('<span class="text-danger small">Por favor, informe um email válido.</span>');
            $loginField.focus();
            return false;
        }

        $loginField.addClass('is-valid');
        return true;
    }

    // Botão Esqueci a Senha
    $(".btn-forgot").on('click', function (e) {
        e.preventDefault();

        if (validateEmail()) {
            const modal = new bootstrap.Modal(document.getElementById('forgotModal'));
            modal.show();
        }
    });

    $('#confirmForgot').on('click', function () {
        const loginValue = $loginField.val().trim();
        globalThis.location.href = '/Login/Forgot/' + base64Encode(loginValue);
    });

    // Botão OTP
    $(".btn-onetime").on('click', function (e) {
        e.preventDefault();

        if (validateEmail()) {
            const modal = new bootstrap.Modal(document.getElementById('otpModal'));
            modal.show();
        }
    });

    $('#confirmOTP').on('click', function () {
        const loginValue = $loginField.val().trim();
        window.location.href = '/Login/OneTime/' + base64Encode(loginValue);
    });

    // Animação de entrada
    $('.login-form-container').css({
        'opacity': '0',
        'transform': 'translateX(30px)'
    });

    setTimeout(() => {
        $('.login-form-container').css({
            'transition': 'all 0.8s ease',
            'opacity': '1',
            'transform': 'translateX(0)'
        });
    }, 100);
});