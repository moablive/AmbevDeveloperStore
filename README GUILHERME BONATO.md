# 📘 Documentação do Projeto: Ambev Developer Evaluation 🚀  

## Arquitetura e Implementação de um Sistema de Vendas com Clean Architecture, DDD, CQRS e Eventos Assíncronos  

<div align="center">
<a href="https://skillicons.dev">
<img src="https://skillicons.dev/icons?i=dotnet,cs,postgres,mongodb,kafka,docker,git,vscode&theme=light" alt="Tecnologias do Projeto" />
</a>
</div>

---

## 📖 Visão Geral do Projeto  

Este projeto implementa um sistema de vendas robusto, baseado em **Clean Architecture** e **Domain-Driven Design (DDD)**.  
Para garantir alta performance e escalabilidade, utiliza o padrão **CQRS (Command Query Responsibility Segregation)**, separando responsabilidades de escrita e leitura.  

A aplicação é construída com **.NET 8** e orquestrada via **Docker**.  

- **Write Model**: PostgreSQL + Entity Framework Core  
- **Read Model**: MongoDB, atualizado de forma assíncrona via **Kafka**  
- **Autenticação**: JWT  
- **Validações**: FluentValidation  
- **Testes**: xUnit + NSubstitute  

---

## 🏗️ Arquitetura do Sistema  

### 🌐 WebApi (Camada de Apresentação)  
- **Responsabilidade**: Recebe requisições HTTP, valida DTOs e retorna respostas.  
- **Componentes**: Controllers, DTOs, AutoMapper, Middlewares.  
- **Regra**: Não contém lógica de negócio, apenas delega para a camada de aplicação via MediatR.  

### 🚀 Application (Camada de Aplicação)  
- **Responsabilidade**: Orquestra os casos de uso (Commands & Queries).  
- **Componentes**: Commands, Queries, Handlers.  
- **Regra**: Define o que o sistema faz, mas não como a lógica é executada.  

### ❤️ Domain (Camada de Domínio)  
- **Responsabilidade**: O coração do sistema, com lógica de negócio, entidades e interfaces.  
- **Exemplo**: `Sale.UpdateItemsAndRecalculateTotal()` encapsula regras de cálculo de descontos.  
- **Regra**: Não depende de frameworks externos.  

### 💾 Infrastructure (Camada de Infraestrutura)  
- **Responsabilidade**: Implementa acesso a dados e integrações externas.  
- **Componentes**: `DbContext`, repositórios concretos, produtores/consumidores Kafka.  
- **Regra**: Implementa interfaces do domínio conectando lógica de negócio a tecnologias reais.  

---

## 📑 CQRS + Event-Driven  

### Write Model (PostgreSQL + EF Core)  
Commands (`CreateSaleCommand`, `UpdateSaleCommand`, `CancelSaleCommand`) garantem consistência transacional.  

### Comunicação Assíncrona via Kafka  
Após salvar no PostgreSQL, eventos são publicados no Kafka e consumidos para atualizar o Read Model.  

### Read Model (MongoDB)  
Consultas (`GetSaleByIdQuery`, `GetAllSalesQuery`) são feitas em documentos desnormalizados otimizados para leitura.  

✅ Essa abordagem garante **consistência eventual** e escalabilidade entre escrita e leitura.  

---

## 🔄 Fluxo de Criação de Venda  

1. Cliente envia **POST /api/sales** com `CreateSaleRequest`.  
2. `SalesController` valida e envia `CreateSaleCommand` via MediatR.  
3. `CreateSaleCommandHandler` aplica regras de negócio e persiste no PostgreSQL.  
4. Evento é publicado no **Kafka**.  
5. `SaleEventConsumerService` atualiza o **MongoDB**.  
6. API responde com **201 Created**.  

<details>
<summary>📊 Diagrama de Sequência</summary>

```mermaid
sequenceDiagram
    participant Client as Cliente
    participant Controller as SalesController
    participant MediatR
    participant Handler as CreateSaleCommandHandler
    participant Sale as Entidade Sale
    participant WriteRepo as ISaleRepository
    participant PostgreSQL
    participant Kafka
    participant Consumer as SaleEventConsumerService
    participant ReadRepo as ISaleReadRepository
    participant MongoDB

    Client->>+Controller: POST /api/sales
    Controller->>MediatR: Envia CreateSaleCommand
    MediatR->>+Handler: Executa Handler
    Handler->>+Sale: new Sale() & Calcula Descontos
    Handler->>+WriteRepo: AddAsync(sale)
    WriteRepo->>+PostgreSQL: INSERT
    PostgreSQL-->>-WriteRepo: Confirma
    Handler->>Kafka: Publica evento de Venda
    Handler-->>-MediatR: Retorna CreateSaleResult
    MediatR-->>-Controller: Retorna Resultado
    Controller-->>-Client: 201 Created

    Consumer->>Kafka: Consome evento
    Consumer->>+ReadRepo: UpsertAsync(sale)
    ReadRepo->>+MongoDB: Salva/Atualiza Documento
    MongoDB-->>-ReadRepo: Confirma
```
</details>

---

## 🔐 Autenticação  

- **Command**: `AuthenticateUserCommand`  
- **Handler**: Verifica senha (`IPasswordHasher`), valida usuário ativo e gera token (`IJwtTokenGenerator`).  
- **Validações**: `FluentValidation` em requisições de login e criação de usuários.  
- **Testes**: Validadores (`UserValidatorTests`) garantem consistência de regras.  

---

## ⚙️ Ambiente Local  

Orquestrado via **Docker**.  

- **PostgreSQL**: `Host=ambev.developerevaluation.database;Port=5432;Database=developer_evaluation;Username=developer;Password=ev@luAt10n`  
- **MongoDB**: `mongodb://developer:ev%40luAt10n@ambev.developerevaluation.nosql:27017`  

Ferramentas:  
- **Kafdrop** → [http://localhost:9000](http://localhost:9000)  
- **Swagger API** → [http://localhost:8080/swagger](http://localhost:8080/swagger)  

---

## 🧪 Testes Unitários  

- **Validadores**: `PhoneValidatorTests`, `PasswordValidatorTests`  
- **Domínio**: `UserTests` validando regras de negócio  
- **Handlers**: `CreateSaleHandlerTests` confirmando cálculos e persistência  

---

## 📋 Tecnologias Utilizadas  

<div align="center">
<a href="https://skillicons.dev">
<img src="https://skillicons.dev/icons?i=dotnet,cs,postgres,mongodb,kafka,docker,git,vscode&theme=light&perline=4" alt="Tecnologias do Projeto" />
</a>
</div>

- Framework: **.NET 8 (C#)**  
- Bancos de Dados: **PostgreSQL (Write)**, **MongoDB (Read)**  
- Mensageria: **Kafka**  
- Orquestração: **Docker**  
- Padrões & Ferramentas: **MediatR, FluentValidation, AutoMapper, EF Core, xUnit, NSubstitute**  

---

## 📝 Nota ao Avaliador  

Este projeto foi desenvolvido com foco em **robustez, escalabilidade e fácil manutenção**, aplicando conceitos modernos de arquitetura de software.  

📂 **Revisar Código-Fonte no GitHub**:  
<div align="center">
  
[![GitHub - MoabLive](https://img.shields.io/badge/GitHub-MoabLive-000?style=for-the-badge&logo=github)](https://github.com/moablive)

</div>
