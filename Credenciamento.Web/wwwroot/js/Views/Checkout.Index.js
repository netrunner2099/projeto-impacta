"use strict";

class CheckoutIndex {
    constructor(options = {}) {
        this.EventId = options.EventId;
        this.PersonId = options.PersonId;

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
        this.$btnCreditCard.off("click.checkout").on("click.checkout", () => {
            this.processCreditCard();
        });

        // Bind dos botões dentro do modal
        $("#btnContinuar").off("click").on("click", () => this.fecharModal());
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

    iniciarPagamento() {
        // Exibe o modal com o spinner
        this.mostrarEstado('estadoProcessando');

        // Simula o processamento (substitua pelo seu fetch/axios aqui)
        setTimeout(() => {
            const aprovado = Math.random() > 0.3; // 70% de chance de aprovação

            if (aprovado) {
                document.getElementById('transacaoId').textContent = this.gerarIdTransacao();
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
        this.fecharModal();
        setTimeout(() => this.iniciarPagamento(), 400);
    }

    gerarIdTransacao() {
        return '#TXN-' + Math.random().toString(36).substr(2, 9).toUpperCase();
    }

    processCreditCard() {
        this.openModal('Processando pagamento com Cartão de Crédito');
        this.iniciarPagamento();
    }
}

// export global (pra usar fácil no MVC/Views)
globalThis.CheckoutIndex = CheckoutIndex;   