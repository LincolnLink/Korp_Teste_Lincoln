# Korp_Teste_Lincoln

Sistema de emissão de Notas Fiscais desenvolvido como projeto técnico para a Korp.

O projeto foi desenvolvido utilizando uma arquitetura de microsserviços, separando as responsabilidades de **Estoque** e **Faturamento**, com comunicação assíncrona através do RabbitMQ.

## Tecnologias utilizadas

### Backend
- C#
- ASP.NET Core
- Entity Framework Core
- SQL Server
- FluentValidation
- RabbitMQ
- Swagger / OpenAPI

### Frontend
- Angular 22
- TypeScript
- RxJS
- NG-ZORRO
- SCSS

### Infraestrutura
- Docker
- Docker Compose
- SQL Server
- RabbitMQ

---

# Arquitetura

A aplicação foi dividida em dois microsserviços principais:

### Korp.Estoque

Responsável pelo gerenciamento dos produtos e controle dos respectivos saldos.

Principais responsabilidades:

- Cadastro de produtos;
- Alteração de produtos;
- Exclusão de produtos;
- Consulta de produtos;
- Controle de saldo;
- Validação de estoque durante o processamento de uma nota fiscal;
- Atualização do saldo após a impressão de uma nota.

### Korp.Faturamento

Responsável pelo gerenciamento das notas fiscais.

Principais responsabilidades:

- Cadastro de notas fiscais;
- Numeração sequencial;
- Inclusão de múltiplos produtos;
- Consulta das notas fiscais;
- Solicitação de impressão;
- Controle do status da nota;
- Comunicação com o serviço de Estoque através do RabbitMQ.

Os projetos backend foram organizados em camadas:

```text
Api
Application
Domain
Infrastructure
```

### Api

Responsável pelos Controllers, configuração da aplicação, Swagger, tratamento global de exceções e configuração dos serviços.

### Application

Contém os Services, DTOs, validações e regras de aplicação.

### Domain

Contém as entidades, enums, interfaces e contratos utilizados pelo domínio.

### Infrastructure

Responsável pelo acesso ao banco de dados, implementações dos repositórios e comunicação com o RabbitMQ.

---

# Fluxo de emissão da Nota Fiscal

Uma nota fiscal é inicialmente cadastrada com o status:

```text
Aberta
```

Ao clicar no botão **Imprimir**, o frontend exibe um indicador de processamento.

O serviço de Faturamento publica uma mensagem no RabbitMQ solicitando o processamento da nota.

Fluxo simplificado:

```text
Angular
   |
   v
Faturamento API
   |
   v
RabbitMQ
   |
   v
Estoque
   |
   |-- Valida o saldo
   |-- Atualiza o estoque
   |
   v
RabbitMQ
   |
   v
Faturamento
   |
   v
Atualiza o status da Nota Fiscal
```

Quando o processamento é realizado com sucesso, a nota passa para:

```text
Fechada
```

e o saldo dos produtos é atualizado de acordo com as quantidades utilizadas na nota.

Somente notas com status **Aberta** podem ser enviadas para impressão.

---

# Status da Nota Fiscal

Foram utilizados os seguintes estados:

```text
1 - Aberta
2 - Fechada
3 - Falha
```

### Aberta

Nota cadastrada e disponível para processamento.

### Fechada

Nota processada com sucesso e estoque atualizado.

### Falha

O processamento não pôde ser concluído.

Esse estado permite fornecer feedback ao usuário quando ocorre uma falha durante o processamento da nota.

---

# Tratamento de falhas

Foi implementado um cenário de falha entre os microsserviços.

Por exemplo:

1. Um produto possui 10 unidades disponíveis;
2. Uma nota é criada utilizando 10 unidades;
3. Antes da impressão, o saldo do produto é alterado para uma quantidade inferior;
4. A nota é enviada para processamento;
5. O serviço de Estoque identifica que não existe saldo suficiente;
6. O processamento falha;
7. A nota é atualizada para o status **Falha**;
8. O frontend informa ao usuário que não foi possível processar a nota.

O frontend também valida o saldo conhecido no momento da criação da nota, evitando solicitações inválidas quando a quantidade informada já é superior ao estoque disponível.

A validação no backend continua sendo necessária, pois o saldo pode ser alterado entre a criação e o processamento da nota.

---

# RabbitMQ

O RabbitMQ foi utilizado para realizar a comunicação assíncrona entre os microsserviços.

O serviço de Faturamento publica uma solicitação de processamento da nota.

O serviço de Estoque consome essa mensagem, valida os produtos e realiza a atualização do estoque.

Após o processamento, o resultado é enviado novamente através do RabbitMQ para o serviço de Faturamento.

Foram configurados:

- Exchanges;
- Queues;
- Routing Keys;
- Acknowledgement manual;
- Retry;
- Dead Letter Exchange;
- Dead Letter Queue.

Em situações onde uma mensagem não pode ser processada corretamente, ela pode ser encaminhada para uma **Dead Letter Queue (DLQ)**, evitando a perda silenciosa da mensagem e permitindo análise posterior.

---

# Angular

O frontend foi desenvolvido utilizando **Angular 22**.

A aplicação possui telas para:

- Gerenciamento de produtos;
- Cadastro de notas fiscais;
- Listagem de notas fiscais;
- Processamento de notas;
- Visualização do status das notas.

Também foi implementado layout responsivo, incluindo menu lateral para desktop e menu Drawer para dispositivos com telas menores.

---

## Ciclo de vida do Angular

Foi utilizado o ciclo de vida:

```typescript
ngOnInit()
```

A interface `OnInit` é utilizada nos componentes para executar operações necessárias após a inicialização.

Por exemplo, na tela de produtos:

```typescript
ngOnInit(): void {
  this.carregarProdutos();
}
```

E no formulário de nota fiscal:

```typescript
ngOnInit(): void {
  this.carregarProdutos();
  this.adicionarItem();
}
```

Dessa forma, os dados necessários para utilização das telas são carregados durante a inicialização dos componentes.

---

# RxJS

O projeto utiliza **RxJS** na comunicação assíncrona do frontend com as APIs.

Os serviços Angular retornam `Observable`, utilizando o `HttpClient`.

Exemplo:

```typescript
listar(): Observable<Produto[]> {
  return this.http.get<Produto[]>(this.apiUrl);
}
```

Os componentes realizam a inscrição através do:

```typescript
.subscribe()
```

tratando separadamente sucesso e erro:

```typescript
.subscribe({
  next: (produtos) => {
    this.produtos = produtos;
  },
  error: (erro: HttpErrorResponse) => {
    // tratamento do erro
  }
});
```

Também foi utilizado o operador:

```typescript
finalize()
```

para controlar estados de carregamento independentemente do resultado da requisição.

Exemplo:

```typescript
.pipe(
  finalize(() => {
    this.carregando = false;
    this.cdr.markForCheck();
  })
)
```

---

# Acompanhamento do processamento

Como a impressão da nota utiliza comunicação assíncrona através do RabbitMQ, o frontend não recebe imediatamente o resultado final do processamento.

Após solicitar a impressão, o Angular realiza consultas periódicas ao endpoint da nota para acompanhar seu status.

Enquanto o processamento está em andamento, o usuário visualiza:

```text
Processando...
```

O acompanhamento é encerrado quando a nota assume um estado final:

```text
Fechada
```

ou:

```text
Falha
```

Isso permite que a interface forneça feedback ao usuário durante o processamento assíncrono.

---

# Biblioteca de componentes visuais

Foi utilizada a biblioteca:

```text
NG-ZORRO
```

Ela fornece componentes visuais baseados no Ant Design.

No projeto foram utilizados componentes como:

- Tabelas;
- Botões;
- Modais;
- Formulários;
- Selects;
- Input Number;
- Tags;
- Mensagens;
- Popconfirm;
- Drawer;
- Ícones;
- Layout e menu.

A biblioteca foi utilizada para manter uma interface consistente e responsiva.

---

# Frameworks e bibliotecas utilizadas no C#

O backend utiliza **ASP.NET Core** para construção das APIs REST.

Também foram utilizadas:

### Entity Framework Core

Responsável pelo acesso e persistência dos dados no SQL Server.

Foi utilizado através de:

- DbContext;
- Migrations;
- Mapeamentos;
- Repositórios.

### FluentValidation

Utilizado para validação dos DTOs recebidos pela aplicação.

### RabbitMQ.Client

Utilizado para implementação da comunicação assíncrona entre os microsserviços.

### Swagger

Utilizado para documentação e testes dos endpoints das APIs.

---

# Tratamento de erros e exceções no backend

Foi implementado tratamento centralizado de exceções.

As APIs possuem um `GlobalExceptionHandler`, evitando a necessidade de repetir tratamento de exceções em cada Controller.

Exceções específicas da aplicação são utilizadas para representar situações como:

```text
NotFoundException
BusinessException
```

Erros inesperados são tratados pelo manipulador global e retornados de forma padronizada para o cliente.

No frontend também existe um serviço centralizado:

```text
HttpErrorService
```

responsável por obter as mensagens retornadas pelas APIs e apresentar feedback ao usuário através do NG-ZORRO.

---

# Uso de LINQ

LINQ foi utilizado no backend para consulta e transformação de coleções.

Um exemplo ocorre durante a criação da mensagem enviada ao RabbitMQ:

```csharp
Itens = nota.Itens
    .Select(item => new ItemNotaFiscalMessage
    {
        ProdutoId = item.ProdutoId,
        Quantidade = item.Quantidade
    })
    .ToList();
```

Nesse caso, o `Select` transforma os itens da entidade `NotaFiscal` nos objetos utilizados pela mensagem de processamento.

Também é utilizado para mapear entidades para DTOs de resposta:

```csharp
Itens = nota.Itens.Select(item =>
    new ItemNotaFiscalResponseDto
    {
        ProdutoId = item.ProdutoId,
        Quantidade = item.Quantidade
    });
```

---

# Banco de dados

Foi utilizado **SQL Server** para persistência física dos dados.

Cada microsserviço possui seu próprio contexto e banco:

```text
KorpEstoqueDb
KorpFaturamentoDb
```

O Entity Framework Core é responsável pelo gerenciamento das migrations.

Durante a inicialização dos microsserviços, as migrations pendentes são aplicadas automaticamente utilizando:

```csharp
await dbContext.Database.MigrateAsync();
```

Dessa forma, ao iniciar o ambiente através do Docker Compose, os bancos podem ser preparados automaticamente.

---

# Docker

A aplicação pode ser executada através do Docker Compose.

São utilizados os seguintes containers:

```text
korp-frontend
korp-estoque-api
korp-faturamento-api
korp-rabbitmq
korp-sqlserver
```

Para construir as imagens e iniciar todo o ambiente:

```bash
docker compose up -d --build
```

O Docker Compose também configura:

- Rede entre os containers;
- Variáveis de ambiente;
- Persistência através de volumes;
- Healthchecks;
- Dependências entre os serviços.

---

# Endereços da aplicação

Após iniciar o Docker Compose:

### Frontend

```text
http://localhost:4200
```

### Swagger - Estoque

```text
http://localhost:5001/swagger/index.html
```

### Swagger - Faturamento

```text
http://localhost:5002/swagger/index.html
```

### RabbitMQ Management

```text
http://localhost:15672
```

---

# Como executar

Na raiz do projeto execute:

```bash
docker compose up -d --build
```

Depois acesse:

```text
http://localhost:4200
```

As migrations do Entity Framework Core são aplicadas durante a inicialização das APIs.

---

# Comandos úteis / Anotações de desenvolvimento

Os comandos abaixo foram utilizados durante o desenvolvimento e foram mantidos como referência.

## Criando a migration do Estoque

```bash
dotnet ef migrations add InitialCreate --project Korp.Estoque.Infrastructure --startup-project Korp.Estoque.Api
```

## Criando o banco e as tabelas do Estoque

```bash
dotnet ef database update --project Korp.Estoque.Infrastructure --startup-project Korp.Estoque.Api
```

## Atualizando o banco do Estoque

```bash
dotnet ef database update --project Korp.Estoque.Infrastructure --startup-project Korp.Estoque.Api
```

## Criando outra migration do Estoque

```bash
dotnet ef migrations add AdicionarAtivoProduto --project Korp.Estoque.Infrastructure --startup-project Korp.Estoque.Api
```

Depois:

```bash
dotnet ef database update --project Korp.Estoque.Infrastructure --startup-project Korp.Estoque.Api
```

## Criando a migration do Faturamento

```bash
dotnet ef migrations add InitialCreate --project Korp.Faturamento.Infrastructure --startup-project Korp.Faturamento.Api
```

## Criando o banco e as tabelas do Faturamento

```bash
dotnet ef database update --project Korp.Faturamento.Infrastructure --startup-project Korp.Faturamento.Api
```

## Executando todo o Docker Compose

```bash
docker compose up --build
```

Ou em segundo plano:

```bash
docker compose up -d --build
```

## Verificando logs do Estoque

```bash
docker logs korp-estoque-api
```

## Verificando logs do Faturamento

```bash
docker logs korp-faturamento-api
```

---

# Frontend - comandos utilizados durante o desenvolvimento

## Criação do projeto Angular

```bash
ng new korp-frontend --routing --style=scss
```

Versão utilizada:

```text
Angular 22
```

## Instalação do NG-ZORRO

```bash
ng add ng-zorro-antd
```