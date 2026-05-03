$(document).ready(function () {
    // Máscaras
    $('#Person_Phone').mask('(00) 00000-0000');
    $('#Person_ZipCode').mask('00000-000');
    $('#Person_Document').mask('000.000.000-00');

    // Buscar CEP
    $('#Person_ZipCode').blur(function () {
        var cep = $(this).val().replace(/\D/g, '');
        if (cep.length === 8) {
            $.getJSON(`https://viacep.com.br/ws/${cep}/json/`, function (data) {
                if (!data.erro) {
                    $('#Person_Address').val(data.logradouro);
                    $('#Person_Neighborhood').val(data.bairro);
                    $('#Person_City').val(data.localidade);
                    $('#Person_State').val(data.uf);
                }
            });
        }
    });

    // Modal de QR Code
    $('.btn-show-qrcode').on('click', function () {
        const ticketId = $(this).data('ticket-id');
        const transaction = $(this).data('transaction');
        const eventName = $(this).data('event-name');

        console.log('Abrindo modal QR Code para transação:', transaction);

        // Mostrar modal
        const modal = new bootstrap.Modal(document.getElementById('qrcodeModal'));
        modal.show();

        // Resetar estados
        $('#qrcodeLoading').removeClass('d-none');
        $('#qrcodeContent').addClass('d-none');
        $('#qrcodeError').addClass('d-none');

        // Carregar QR Code
        $.ajax({
            url: `/Ticket/GetQRCode/${transaction}`,
            method: 'GET',
            success: function (response) {
                console.log('QR Code carregado:', response);
                if (response && response.qrCodeDataUrl) {
                    $('#qrcodeImage').attr('src', response.qrCodeDataUrl);
                    $('#qrcodeEventName').text(eventName);
                    $('#qrcodeTransaction').text(transaction);

                    $('#qrcodeLoading').addClass('d-none');
                    $('#qrcodeContent').removeClass('d-none');

                    // Configurar download
                    $('#btnDownloadQRCode').off('click').on('click', function () {
                        const link = document.createElement('a');
                        link.href = response.qrCodeDataUrl;
                        link.download = `qrcode-${transaction}.png`;
                        link.click();
                    });
                } else {
                    showQRCodeError();
                }
            },
            error: function (xhr, status, error) {
                console.error('Erro ao carregar QR Code:', error);
                showQRCodeError();
            }
        });
    });

    function showQRCodeError() {
        $('#qrcodeLoading').addClass('d-none');
        $('#qrcodeContent').addClass('d-none');
        $('#qrcodeError').removeClass('d-none');
    }

    const $ChangePasswordForm = $("#formChangePassword");
    const $btPasswordChange = $ChangePasswordForm.find("#btPasswordChange");

    $ChangePasswordForm.on('submit', (e) => {
        e.preventDefault();
        return false;
    });

    $btPasswordChange.on('click', () => {
        const $password = $ChangePasswordForm.find("#Password");
        const $passwordConfirm = $ChangePasswordForm.find("#PasswordConfirm");
        const $success = $ChangePasswordForm.find(".div-alert-success");
        const $error = $ChangePasswordForm.find(".div-alert-error");

        if ($password.val() != $passwordConfirm.val()) {
            alert("Senhas não coincidem");
            return false;
        }

        $.ajax({
            url: '/profile/changepassword',
            type: 'post',
            data: { Password: $password.val(), PasswordConfirm: $passwordConfirm.val() },
            success: (data) => {
                if (data !== null && data)
                    $success.removeClass("d-none");
                else
                    $error.removeClass("d-none");
            },
            error: (error) => {
                $error.removeClass("d-none");
            }
        });

        $password.val('');
        $passwordConfirm.val('');

        return false;
    });
});