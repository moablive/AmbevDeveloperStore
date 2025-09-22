# Documentação do Projeto: Ambev Developer Evaluation 🚀

Arquitetura e Implementação de um Sistema de Vendas com Clean Architecture, DDD, CQRS e Eventos Assíncronos

---

<div align="center">
  <a href="https://skillicons.dev">
    <img src="https://skillicons.dev/icons?i=dotnet,cs,postgres,mongodb,kafka,docker,git,vscode&theme=light" alt="Tecnologias do Projeto" />
  </a>
</div>

## 📖 Visão Geral do Projeto

Este projeto implementa um sistema de vendas baseado em **Clean Architecture** e **Domain-Driven Design (DDD)**, utilizando **CQRS** (Command Query Responsibility Segregation) para separar responsabilidades de escrita e leitura. A aplicação é construída com **.NET 8**, usa **Entity Framework** para persistência em **PostgreSQL** (Write Model), **MongoDB** para consultas otimizadas (Read Model) e **Kafka** para comunicação assíncrona de eventos. A autenticação é gerenciada com **JWT**, e as validações são implementadas com **FluentValidation**, com testes unitários robustos (ex.: `PhoneValidatorTests`, `UserValidatorTests`).

---

## 🏗️ Arquitetura do Sistema

A arquitetura é estruturada em quatro camadas, promovendo separação de responsabilidades e manutenibilidade:

### 🌐 WebApi (Camada de Apresentação)
- **Responsabilidade**: Gerencia requisições HTTP, valida dados de entrada (ex.: `CreateSaleRequest`) e retorna respostas (ex.: `CreateSaleResponse`).
- **Componentes**: Controllers (ex.: `SalesController`), DTOs, **AutoMapper**, Middlewares.
- **Regra**: Não contém lógica de negócio; delega para a camada de Aplicação.

### 🚀 Application (Camada de Aplicação)
- **Responsabilidade**: Orquestra casos de uso com **MediatR** (ex.: `CreateSaleCommand`, `AuthenticateUserHandler`, `CancelSaleCommand`).
- **Componentes**: Commands, Queries, Handlers.
- **Regra**: Define **o que** o sistema faz, sem implementar lógica de negócio.

### ❤️ Domain (Camada de Domínio)
- **Responsabilidade**: Contém a lógica de negócio, entidades (ex.: `Sale`, `User`), Value Objects e interfaces (ex.: `ISaleRepository`, `ISaleReadRepository`).
- **Exemplo**: Método `UpdateItemsAndRecalculateTotal` na entidade `Sale` calcula descontos e total.
- **Regra**: Independente de tecnologias externas.

### 💾 Infrastructure (Camada de Infraestrutura)
- **Responsabilidade**: Implementa acesso a dados e integrações externas.
- **Componentes**: `DbContext` (Entity Framework), repositórios (ex.: `SaleRepository`), **Kafka** (eventos), **MongoDB** (leitura).
- **Exemplo**: Persistência em **PostgreSQL** e atualizações assíncronas em **MongoDB** via **Kafka**.

---

## 📑 Detalhes da Arquitetura CQRS

O projeto aplica **CQRS (Command Query Responsibility Segregation)** para separar operações de **escrita** e **leitura**, aumentando escalabilidade e performance:

- **Write Model (PostgreSQL + Entity Framework)**  
  - Todos os **commands** (`CreateSaleCommand`, `CancelSaleCommand`) escrevem no banco relacional.  
  - A consistência transacional é garantida pela persistência síncrona no PostgreSQL.  

- **Eventual Consistency via Kafka**  
  - Após o commit de escrita, um **evento de domínio** (ex.: `VendaCriada`) é publicado no **Kafka**.  
  - Consumidores processam o evento e atualizam o **Read Model** de forma assíncrona.  

- **Read Model (MongoDB)**  
  - As consultas são otimizadas em documentos **denormalizados** no MongoDB.  
  - Queries (`GetSaleByIdQuery`, `ListSalesQuery`) são direcionadas ao banco de leitura, garantindo performance em cenários de alto volume.  

👉 Essa abordagem permite escalar consultas de forma independente das escritas, mantendo o núcleo de negócio consistente.

---

## 🔄 Fluxo de Criação de Venda

O fluxo de criação de uma venda ilustra a interação entre as camadas:

1. **Requisição**: Cliente envia `POST /api/sales` com JSON (`CreateSaleRequest`).
2. **Controller**: Valida dados e mapeia para `CreateSaleCommand` via **AutoMapper**, enviando ao **MediatR**.
3. **Handler**: O `CreateSaleCommandHandler` instancia a entidade `Sale`, aplica lógica de negócio (ex.: cálculo de descontos) e chama `AddAsync` no `ISaleRepository`.
4. **Persistência**: O repositório usa **Entity Framework** para salvar no **PostgreSQL** e publica um evento ("VendaCriada") no **Kafka**.
5. **Leitura Assíncrona**: Um consumidor **Kafka** atualiza o **MongoDB** para consultas rápidas (ex.: `GET /api/sales/{id}`).
6. **Resposta**: O controller retorna `201 Created` com a URL do recurso.

### Diagrama de Sequência
```mermaid
sequenceDiagram
    participant Client as Cliente
    participant Controller as SalesController
    participant MediatR
    participant Handler as CreateSaleCommandHandler
    participant Sale as Entidade Sale
    participant Repo as ISaleRepository
    participant Db as PostgreSQL
    participant Kafka
    participant Mongo as MongoDB

    Client->>+Controller: POST /api/sales
    Controller->>MediatR: CreateSaleCommand
    MediatR->>+Handler: Executa Handler
    Handler->>+Sale: Cria Sale
    Sale->>Sale: Calcula descontos, total
    Handler->>+Repo: AddAsync(sale)
    Repo->>+Db: INSERT via EF Core
    Db-->>-Repo: Confirma
    Repo->>Kafka: Publica "VendaCriada"
    Kafka->>Mongo: Atualiza Read Model
    Handler-->>-MediatR: CreateSaleResult
    MediatR-->>-Controller: Resultado
    Controller-->>-Client: 201 Created
```

---

## 🔐 Autenticação

A autenticação é implementada com **JWT** e validação rigorosa:
- **Command**: `AuthenticateUserCommand` (email, senha).
- **Handler**: `AuthenticateUserHandler` verifica credenciais com `IPasswordHasher`, valida usuário ativo com `ActiveUserSpecification` e gera token com `IJwtTokenGenerator`.
- **Validação**: `AuthenticateUserValidator` usa **FluentValidation** para garantir email válido e senha com mínimo de 6 caracteres.
- **Testes**: Validações para username (3-50 caracteres), email, senha (8+ caracteres, complexidade), telefone, status (`Active`/`Suspended`) e role (`Customer`/`Admin`) via `UserValidatorTests`.

---

## ⚙️ Configuração do Ambiente Local

### Connection Strings
- **PostgreSQL (Write Model)**:
  ```
  postgresql://developer:ev@luAt10n@localhost:5432/developer_evaluation
  ```

- **MongoDB (Read Model)**:
  ```
  mongodb://developer:ev%40luAt10n@localhost:27017
  ```

### Ferramentas
- **Kafdrop**: `localhost:9000`
- **Docker**: Orquestra serviços (API, PostgreSQL, MongoDB, Kafka).

---

## 🧪 Testes Unitários

- **PhoneValidatorTests**: Regex (`^\+?[1-9]\d{1,14}$`).
- **UserValidatorTests**: Username, email, senha, telefone, status e role.
- **CancelSaleCommand**: Testa cancelamento de vendas.

---

## 📋 Tecnologias Utilizadas
<p align="center">
  <a href="https://skillicons.dev">
    <img src="https://skillicons.dev/icons?i=dotnet,cs,postgres,mongodb,kafka,docker,git,vscode&theme=light&perline=4" alt="Tecnologias do Projeto" />
  </a>
</p>

- **.NET 8 & C#**
- **PostgreSQL** (Write Model)
- **MongoDB** (Read Model)
- **Kafka** (Eventos assíncronos)
- **Docker**
- **MediatR**
- **FluentValidation**
- **AutoMapper**
- **Entity Framework**

---

## 📝 Nota para o Avaliador
Por favor, visite meu perfil no GitHub em **[https://github.com/moablive](https://github.com/moablive)** para revisar o código-fonte.
