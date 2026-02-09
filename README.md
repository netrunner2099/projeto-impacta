# projeto-impacta
Repositório de códigos do projeto de Software Product: Analysis, Specification, Project

# Sistema de Credenciamento de Eventos Corporativos

![.NET 8](https://img.shields.io/badge/.NET-8.0-purple)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-Razor%20Pages-blue)
![Entity Framework Core](https://img.shields.io/badge/EF%20Core-8.0-green)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2019+-red)

## 📋 Sobre o Projeto

Sistema completo de venda de ingressos e credenciamento para eventos corporativos desenvolvido como projeto acadêmico do curso de **Software Product: Analysis, Specification, Project** da Impacta.

O sistema possui duas funcionalidades principais:

### 🎫 Venda de Ingressos (Pré-Evento)
- Cadastro de participantes
- Gerenciamento de eventos corporativos
- Venda e emissão de ingressos
- Controle de status de pagamento

### ✅ Credenciamento (Dia do Evento)
- Validação de ingressos pagos
- Controle de entrada no evento
- Entrega de pulseiras de identificação
- Registro de presença dos participantes

Apenas participantes que realizaram o pagamento do ingresso receberão a pulseira de identificação e terão acesso ao evento.

## 🏗️ Arquitetura

O projeto segue uma arquitetura em camadas (Layered Architecture) com separação clara de responsabilidades:

### 📦 Camadas

- **Web**: Interface do usuário com Razor Pages
- **Application**: Casos de uso e lógica de aplicação
- **Domain**: Regras de negócio e entidades do domínio
- **Infrastructure**: Acesso a dados, repositórios e configurações

## 🛠️ Tecnologias Utilizadas

- **.NET 8** - Framework principal
- **ASP.NET Core Razor Pages** - Interface web
- **Entity Framework Core 8** - ORM para acesso a dados
- **SQL Server** - Banco de dados relacional
- **C# 12** - Linguagem de programação

## 📊 Modelo de Dados

### Entidades Principais

- **Person** - Dados cadastrais dos participantes
- **Event** - Informações dos eventos corporativos
- **Ticket** - Ingressos vendidos e status de pagamento
- **User** - Usuários do sistema (administradores e operadores)

### Relacionamentos

## 🚀 Como Executar

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server 2019+](https://www.microsoft.com/sql-server/sql-server-downloads) ou [SQL Server Express](https://www.microsoft.com/sql-server/sql-server-downloads)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [Visual Studio Code](https://code.visualstudio.com/)

### Configuração

1. Clone o repositório:
git clone https://github.com/netrunner2099/projeto-impacta.git cd projeto-impacta

2. Execute os scripts de criação dos objetos de banco de dados:
/database/01-create-database.sql
/database/02-create-objects.sql

3. Configure a string de conexão no `appsettings.json`:
{ "ConnectionStrings": { "DefaultConnection": "Server=localhost;Database=CredenciamentoDB;User=sa;Password=123456;TrustServerCertificate=True;" } }

5. Inicie o servidor web:
dotnet run

### Acessando o Sistema

- URL padrão: `http://localhost:5000`
- Página inicial de venda de ingressos deve ser exibida

## 👨‍💻 Contribuição

Contribuições são bem-vindas! Veja o arquivo [CONTRIBUTING.md](CONTRIBUTING.md) para detalhes sobre como contribuir para este projeto.

## 📝 Licença

Este projeto está licenciado sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

---

> Status do Projeto: Em Andamento 🚧
> Última Atualização: 08/02/2026
> Para mais informações, consulte a documentação técnica do projeto.
