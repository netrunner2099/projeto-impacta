"use strict";

class TicketIndex {
    constructor(options = {}) {
        this.$btnPrint = $(options.btnPrintSelector ?? ".btn-print");
        this.$cardToPrint = $(options.cardSelector ?? ".card");
    }

    init() {
        this.bindEvents();
        return this;
    }

    bindEvents() {
        this.$btnPrint.off("click.ticket").on("click.ticket", () => {
            this.printCard();
        });
    }

    printCard() {
        // Salva o conteúdo original da página
        const originalContent = document.body.innerHTML;
        
        // Obtém apenas o conteúdo do card
        const cardContent = this.$cardToPrint[0].outerHTML;
        
        // Cria um estilo específico para impressão em landscape
        const printStyle = `
            <style>
                @page {
                    size: landscape;
                    margin: 20mm;
                }
                
                @media print {
                    body {
                        margin: 0;
                        padding: 20px;
                    }
                    .card {
                        border: 1px solid #dee2e6;
                        box-shadow: none;
                        page-break-inside: avoid;
                    }
                    .card-header {
                        background-color: #6f42c1 !important;
                        color: white !important;
                        -webkit-print-color-adjust: exact;
                        print-color-adjust: exact;
                    }
                    .btn-print {
                        display: none !important;
                    }
                    .img-qr-code {
                        max-width: 200px;
                    }
                }
            </style>
        `;
        
        // Substitui o conteúdo da página pelo card
        document.body.innerHTML = printStyle + cardContent;
        
        // Aguarda o carregamento das imagens antes de imprimir
        setTimeout(() => {
            window.print();
            
            // Restaura o conteúdo original após a impressão
            document.body.innerHTML = originalContent;
            
            // Reinicializa os eventos (necessário após restaurar o HTML)
            this.init();
        }, 250);
    }
}

// Export global
globalThis.TicketIndex = TicketIndex;