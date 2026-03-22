$(document).ready(function () {

    $.validator.addMethod("cpf", function (value, element) {
        if (this.optional(element)) {
            return true;
        }
        return validarCPF(value);
    }, "CPF inválido");

    $('#Person_BirthDay').mask('00/00/0000');
    $('#Person_Document').mask('000.000.000-00');
    $('#Person_Phone').mask('(00) 00000-0000');
    $('#Person_ZipCode').mask('00000-000');

    // Adiciona a validação de CPF ao campo Document
    $('#Person_Document').rules('add', {
        cpf: true,
        messages: {
            cpf: "CPF inválido"
        }
    });

    // Evento de click no botão de consulta CEP
    $('#btnConsultaCep').on('click', function () {
        var cep = $('#Person_ZipCode').val();
        consultarCEP(cep);
    });

    // Permite consultar CEP ao pressionar Enter no campo
    $('#Person_ZipCode').on('keypress', function (e) {
        if (e.which === 13) {
            e.preventDefault();
            consultarCEP($(this).val());
        }
    });

    // Configuração de validação
    $.validator.setDefaults({
        onsubmit: true,
        onfocusout: function (element) {
            // Não valida inputs, mas permite validar selects
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
        // Ignora erros em tempo real
        ignore: []
    });

    // Animação de entrada dos cards
    $('.card').each(function (index) {
        $(this).css({
            'opacity': '0',
            'transform': 'translateY(30px)'
        });

        setTimeout(() => {
            $(this).css({
                'transition': 'all 0.6s ease',
                'opacity': '1',
                'transform': 'translateY(0)'
            });
        }, index * 150);
    });
});