"use strict";

class CheckoutIndex {
    constructor(options = {}) {
        this.Payment = options.Payment;
        this.BaseUrl = options.BaseUrl;
        this.EventId = options.EventId;
        this.PersonId = options.PersonId;
        this.TicketId = options.TicketId;
        this.Transaction = options.Transaction;

        this.$form = $(options.formSelector ?? "#formCheckout");
        this.$btnCreditCard = $(options.btnCreditCardSelector ?? "#btnCreditCard");
        this.$btnBarcodeBoleto = $(options.btnBarcodeBoletoSelector ?? "#btnBarcodeBoleto");
        this.$btnQrcodePix = $(options.btnQrcodePixSelector ?? "#btnQrcodePix");

        this.$modal = $(options.modalSelector ?? "#modal");
        this.$cardNumber = this.$form.find("#CardNumber");
        this.$cardExpiration = this.$form.find("#CardExpiration");
        this.$cardCVV = this.$form.find("#CardCVV");

        this.currentField = null;
        this.modalInstance = null;
    }

    init() {
        this.bindEvents();
        this.initModal();
        return this;
    }

    initModal() {
        // Inicializa o modal do Bootstrap
        this.modalInstance = new bootstrap.Modal(this.$modal[0]);
    }

    bindEvents() {
        this.$btnCreditCard.off("click.checkout").on("click.checkout", () => this.processCreditCard());
        this.$btnBarcodeBoleto.off('click.checkout').on('click.checkout', () => this.processBarCode());
        this.$btnQrcodePix.off('click.checkout').on('click.checkout', () => this.processQrCodePix());

        // Bind dos botões dentro do modal
        $("#btnContinuar").off("click").on("click", () => globalThis.location.href = this.BaseUrl + this.Transaction);
        $("#btnCancelarRecusado").off("click").on("click", () => this.fecharModal());
        $("#btnTentarNovamente").off("click").on("click", () => this.tentarNovamente());
    }

    openModal(label) {
        this.$modal.find(".modal-title").text(label);
        this.modalInstance.show();
    }

    mostrarEstado(estado) {
        document.getElementById('estadoProcessando').classList.add('d-none');
        document.getElementById('estadoAprovado').classList.add('d-none');
        document.getElementById('estadoRecusado').classList.add('d-none');
        document.getElementById(estado).classList.remove('d-none');
    }

    criarTicket() {
        let returns = null;
        const data = {
            EventId: Number.parseInt(this.EventId, 10),
            PersonId: Number.parseInt(this.PersonId, 10),
            Payment: Number.parseInt(this.Payment, 10)
        };

        $.ajax({
            'url': '/checkout/createticket',
            'method': 'post', 
            'contentType': 'application/json',
            'async': false,
            'data': JSON.stringify(data),
            'success': (response) => {
                console.info('response: ', response);
                this.TicketId = response.ticketId; 
                returns = response.ticketId; 
            },
            'error': (error) => {
                console.error('Erro ao criar ticket:', error);
            }
        });

        return returns;
    }

    pagarTicket() {
        let returns = null;

        const data = {
            TicketId: this.TicketId
        };

        $.ajax({
            'url': '/checkout/payticket',
            'method': 'post',
            'contentType': 'application/json',
            'async': false,
            'data': JSON.stringify(data),
            'success': (response) => {
                console.info('response: ', response);
                this.Transaction = response.transaction;
                returns = response.transaction;
            },
            'error': (error) => {
                console.error('Erro ao pagar ticket:', error);
            }
        });

        return returns;
    }

    iniciarPagamento() {
        // Exibe o modal com o spinner
        this.mostrarEstado('estadoProcessando');

        // Simula o processamento (substitua pelo seu fetch/axios aqui)
        setTimeout(() => {
            const aprovado = Math.random() > 0.3; // 70% de chance de aprovação

            if (aprovado) {
                let transaction = this.pagarTicket();
                document.getElementById('transacaoId').textContent = transaction;
                this.mostrarEstado('estadoAprovado');
            } else {
                this.mostrarEstado('estadoRecusado');
            }
        }, 5000); // Simula 3 segundos de processamento
    }

    fecharModal() {
        this.modalInstance.hide();
    }

    tentarNovamente() {
        this.mostrarEstado('estadoProcessando');
        setTimeout(() => this.iniciarPagamento(), 400);
    }

    processCreditCard() {
        this.Payment = 2; // Cartão de Crédito
        this.openModal('Processando pagamento com Cartão de Crédito');
        this.criarTicket();
        this.iniciarPagamento();
    }

    processBarCode() {
        this.Payment = 3; // Boleto Bancário
        this.openModal('Processando pagamento com Boleto Bancário');
        this.criarTicket();
        this.iniciarPagamento();
    }

    processQrCodePix() {
        this.Payment = 1; // Pix
        this.openModal('Processando pagamento com Pix');
        this.criarTicket();
        this.iniciarPagamento();
    }
}

// export global (pra usar fácil no MVC/Views)
globalThis.CheckoutIndex = CheckoutIndex;   