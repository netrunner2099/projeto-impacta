// Função para validar CPF
function validarCPF(cpf) {
    // Remove caracteres não numéricos
    cpf = cpf.replace(/[^\d]/g, '');

    // Verifica se tem 11 dígitos
    if (cpf.length !== 11) {
        return false;
    }

    // Verifica se todos os dígitos são iguais (ex: 111.111.111-11)
    if (/^(\d)\1{10}$/.test(cpf)) {
        return false;
    }

    // Valida o primeiro dígito verificador
    let soma = 0;
    for (let i = 0; i < 9; i++) {
        soma += parseInt(cpf.charAt(i)) * (10 - i);
    }
    let resto = (soma * 10) % 11;
    if (resto === 10 || resto === 11) resto = 0;
    if (resto !== parseInt(cpf.charAt(9))) {
        return false;
    }

    // Valida o segundo dígito verificador
    soma = 0;
    for (let i = 0; i < 10; i++) {
        soma += parseInt(cpf.charAt(i)) * (11 - i);
    }
    resto = (soma * 10) % 11;
    if (resto === 10 || resto === 11) resto = 0;
    if (resto !== parseInt(cpf.charAt(10))) {
        return false;
    }

    return true;
}

function validarEmail(email) {
    let emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
}


// Função para consultar CEP
function consultarCEP(cep) {
    // Remove caracteres não numéricos
    cep = cep.replace(/\D/g, '');

    if (cep.length !== 8) {
        alert('CEP inválido!');
        return;
    }

    // Mostra loading no botão
    $('#btnConsultaCep').prop('disabled', true).html('<i class="bi bi-hourglass-split"></i>');

    // Consulta ViaCEP
    $.getJSON(`https://viacep.com.br/ws/${cep}/json/`, function (dados) {
        if (!dados.erro) {
            // Preenche os campos
            $('#Person_Address').val(dados.logradouro);
            $('#Person_Neighborhood').val(dados.bairro);
            $('#Person_City').val(dados.localidade);
            $('#Person_State').val(dados.uf);

            // Foca no campo número
            $('#Person_Number').focus();
        } else {
            alert('CEP não encontrado!');
        }
    })
        .fail(function () {
            alert('Erro ao consultar CEP. Tente novamente.');
        })
        .always(function () {
            // Restaura botão
            $('#btnConsultaCep').prop('disabled', false).html('<i class="bi bi-search"></i>');
        });
}

// Adicione no início do seu script ou em site.js
function base64Encode(str) {
    return btoa(encodeURIComponent(str).replace(/%([0-9A-F]{2})/g, function (match, p1) {
        return String.fromCharCode('0x' + p1);
    }));
}

function base64Decode(str) {
    return decodeURIComponent(Array.prototype.map.call(atob(str), function (c) {
        return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
    }).join(''));
}