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
- **CQRS**: Command Query Responsibility Segregation 
- **MVVM**: Model-View-ViewModel

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
- /database/01-create-database.sql (altere a senha do usuário impacta_user no script)
- /database/02-create-objects.sql
- /database/03-first-load.sql (é uma carga inicial com os eventos de 2026)

Os scripts de criação das tabelas estão disponíveis na pasta `database/`

3. Configure a string de conexão no `appsettings.json`:
{ "ConnectionStrings": { "DefaultConnection": "Server=localhost;Database=CredenciamentoDB;User=impacta_user;Password=(a sua senha aqui);TrustServerCertificate=True;" } }

5. Inicie o servidor web:
dotnet run

### Acessando o Sistema

- URL padrão: `http://localhost:5000`
- Página inicial de venda de ingressos deve ser exibida

## 📁 Estrutura do Banco de Dados

### Scripts SQL
- `01-create-database.sql` - Criação da database
- `02-create-objects.sql` - Criação das tabelas e outros objetos
- `03-first-load.sql` - Carga inicial com os eventos de 2026

## 🔐 Funcionalidades de Segurança

- Senhas armazenadas com hash (BCrypt)
- Controle de acesso por roles (Administrador/Operador)
- Validação de dados de entrada
- Proteção contra SQL Injection via EF Core

## 👥 Equipe

Projeto desenvolvido por Rodrigo Miranda - Turma EAD - ADS 5A para a disciplina Software Product: Analysis, Specification, Project da Impacta.

## 📝 Licença

Este projeto é um trabalho acadêmico e está disponível para fins educacionais.

## 📞 Contato

Para dúvidas ou sugestões, abra uma [issue](https://github.com/netrunner2099/projeto-impacta/issues) no repositório.

## 👨‍💻 Projeto 

O projeto está sendo gerenciado utilizando o Github Projects, e o link deste projeto está abaixo:
https://github.com/users/netrunner2099/projects/1

## 📄 Documentação Técnica

A documentação técnica está disponível no link:
https://iingresso.com/diagrams/class-diagram.html

---

> Status do Projeto: Em Andamento 🚧
> Última Atualização: 01/03/2026
> Para mais informações, consulte a documentação técnica do projeto.
  