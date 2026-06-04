# projeto-impacta
Repositório de códigos do projeto de Software Product: Analysis, Specification, Project

# Sistema de Credenciamento de Eventos Corporativos

![.NET 8](https://img.shields.io/badge/.NET-8.0-purple)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-Razor%20Views-blue)
![Entity Framework Core](https://img.shields.io/badge/EF%20Core-8.0-green)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2019+-red)
![C# 12](https://img.shields.io/badge/C%23-12.0-blueviolet)

## 📋 Sobre o Projeto

Sistema completo de venda de ingressos e credenciamento para eventos corporativos desenvolvido como projeto acadêmico do curso de **Software Product: Analysis, Specification, Project** da Impacta.

O sistema é dividido em duas áreas principais: o **portal público** para compra de ingressos e o **backoffice administrativo** para gestão completa do evento.

---

## 🎯 Funcionalidades

### 🎫 Portal Público (Pré-Evento)
- Cadastro de participantes (onboarding)
- Visualização de eventos disponíveis
- Compra e emissão de ingressos
- Pagamento via Pix, Cartão de Crédito ou Boleto Bancário
- Visualização e impressão do ingresso com QR Code

### 🔧 Backoffice Administrativo (`/admin`)
- **Login exclusivo** para administradores
- **Gerenciamento de Usuários**: listagem, criação, edição (com dados de Person para role Usuário), inativação, reativação e exclusão
- **Gerenciamento de Eventos**: listagem (filtro padrão: eventos futuros), criação, edição, ativação, inativação e exclusão
- **Gerenciamento de Tickets**: listagem completa de todos os tickets emitidos com filtros por evento, status e forma de pagamento; impressão de **2ª via** com QR Code

### ✅ Credenciamento (Dia do Evento)
- Validação de ingressos pagos via QR Code
- Controle de acesso ao evento
- Entrega de pulseiras de identificação
- Registro de presença dos participantes

> Apenas participantes com ingresso **pago** receberão a pulseira e terão acesso ao evento.

---

## 🏗️ Arquitetura

O projeto segue uma **arquitetura em camadas** com separação clara de responsabilidades:

| Padrão | Descrição |
|--------|-----------|
| **CQRS** | Command Query Responsibility Segregation via MediatR |
| **Repository** | Abstração do acesso a dados |
| **AutoMapper** | Mapeamento entre camadas (Domain → Application → Web) |

### 📦 Camadas

| Camada | Projeto | Responsabilidade |
|--------|---------|-----------------|
| **Web** | `Credenciamento.Web` | Interface do usuário (Razor Views + Admin Area) |
| **Application** | `Credenciamento.Application` | Casos de uso, Handlers, Commands e Queries |
| **Domain** | `Credenciamento.Domain` | Entidades, enums, interfaces de repositório |
| **Infrastructure** | `Credenciamento.Infrastructure` | EF Core, repositórios, migrations |

---

## 🛠️ Tecnologias Utilizadas

- **.NET 8** — Framework principal
- **ASP.NET Core MVC com Razor Views** — Interface web (portal público + admin)
- **MediatR** — Implementação do padrão CQRS
- **AutoMapper** — Mapeamento entre camadas
- **Entity Framework Core 8** — ORM para acesso a dados
- **FluentValidation** — Validação de commands e queries
- **BCrypt.Net** — Hash seguro de senhas
- **SQL Server** — Banco de dados relacional
- **Bootstrap 5 + Bootstrap Icons** — UI responsiva
- **C# 12** — Linguagem de programação

---

## 📊 Modelo de Dados

### Entidades Principais

| Entidade | Descrição |
|----------|-----------|
| **Person** | Dados cadastrais dos participantes (nome, CPF, e-mail, telefone) |
| **Event** | Informações dos eventos (nome, local, datas, preço, status) |
| **Ticket** | Ingressos emitidos com status de pagamento, forma de pagamento, QR Code e transação |
| **User** | Usuários do sistema (Admin / Operador / Usuário) com hash de senha |

### Status de Ticket (`TicketStatus`)

| Valor | Descrição |
|-------|-----------|
| `Created` | Ingresso criado, aguardando pagamento |
| `Paid` | Pagamento confirmado |
| `Canceled` | Ingresso cancelado |
| `Deleted` | Ingresso excluído |

### Formas de Pagamento (`TicketPayment`)

| Valor | Descrição |
|-------|-----------|
| `Pix` | QRCode Pix |
| `CreditCard` | Cartão de Crédito |
| `Boleto` | Boleto Bancário |

---

## 🚀 Como Executar

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server 2019+](https://www.microsoft.com/sql-server/sql-server-downloads) ou SQL Server Express
- [Visual Studio 2022+](https://visualstudio.microsoft.com/) ou [Visual Studio Code](https://code.visualstudio.com/)

### Configuração

1. **Clone o repositório:**
```bash
git clone https://github.com/netrunner2099/projeto-impacta.git
cd projeto-impacta
```

2. **Execute os scripts de banco de dados** (pasta `database/`):
```
01-create-database.sql  → cria o banco e o usuário (ajuste a senha)
02-create-objects.sql   → cria tabelas e demais objetos
03-first-load.sql       → carga inicial com eventos de 2026
```

3. **Configure a string de conexão** em `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=CredenciamentoDB;User=impacta_user;Password=(sua senha);TrustServerCertificate=True;"
  }
}
```

4. **Execute a aplicação:**
```bash
dotnet run --project Credenciamento.Web
```

### Acessando o Sistema

| Área | URL |
|------|-----|
| Portal público (loja de ingressos) | `http://localhost:5000` |
| Backoffice administrativo | `http://localhost:5000/admin` |

---

## 📁 Estrutura do Banco de Dados

### Scripts SQL

| Arquivo | Descrição |
|---------|-----------|
| `01-create-database.sql` | Criação da database e usuário de acesso |
| `02-create-objects.sql` | Tabelas, índices e constraints |
| `03-first-load.sql` | Carga inicial com eventos de 2026 |

---

## 🔐 Segurança

- Senhas armazenadas com hash **BCrypt**
- Controle de acesso por **roles** (`Admin`, `Operator`, `User`)
- Área administrativa isolada (`/admin`) com autenticação separada
- Validação de dados de entrada via **FluentValidation**
- Proteção contra SQL Injection via EF Core parametrizado

---

## 👥 Equipe

Projeto desenvolvido por **Rodrigo Miranda** — Turma EAD — ADS 5A  
Disciplina: *Software Product: Analysis, Specification, Project* — **Impacta**

---

## 📄 Documentação Técnica

A documentação técnica com diagramas de arquitetura está disponível em:

- 🌐 **Online:** https://iingresso.com/diagrams/class-diagram.html
- 📂 **Local (wwwroot):** `Credenciamento.Web/wwwroot/diagrams/`

| Diagrama | Descrição |
|----------|-----------|
| [Domain](https://iingresso.com/diagrams/domain-diagram.html) | Entidades e enums do domínio |
| [Infrastructure](https://iingresso.com/diagrams/infrastructure-diagram.html) | Repositórios e EF Core |
| [Application](https://iingresso.com/diagrams/application-diagram.html) | CQRS — índice por módulo (Events / Tickets / Users / Persons) |
| [Web](https://iingresso.com/diagrams/web-diagram.html) | Controllers, ViewModels e Admin Area |
| [Database](https://iingresso.com/diagrams/database-diagram.html) | Modelo relacional do banco |
| [Use Cases](https://iingresso.com/diagrams/usecase-diagram.html) | Casos de uso (UC1–UC15) |
| [Visão Completa](https://iingresso.com/diagrams/class-diagram.html) | Arquitetura end-to-end |

---

## 👨‍💻 Gestão do Projeto

O projeto é gerenciado via **GitHub Projects**:  
👉 https://github.com/users/netrunner2099/projects/1

---

## 📝 Licença

Este projeto é um trabalho acadêmico e está disponível para fins educacionais.

---

## 📞 Contato

Para dúvidas ou sugestões, abra uma [issue](https://github.com/netrunner2099/projeto-impacta/issues) no repositório.

---

> **Status do Projeto:** Em Andamento 🚧  
> **Última Atualização:** 21/03/2026  
> Para mais informações, consulte a [documentação técnica](https://iingresso.com/diagrams/class-diagram.html).
