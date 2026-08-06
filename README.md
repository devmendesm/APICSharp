# API Catálogo

API REST desenvolvida em **ASP.NET Core 8** para gerenciamento de produtos e categorias.

O projeto foi desenvolvido durante os estudos do curso **Web API ASP.NET Core Essencial (.NET 8 / .NET 9)**, do professor José Carlos Macoratti, com o objetivo de aplicar boas práticas no desenvolvimento de APIs utilizando o ecossistema .NET.

---

## Tecnologias utilizadas

- ASP.NET Core 8
- C#
- Entity Framework Core
- MySQL
- Pomelo Entity Framework Core MySQL
- AutoMapper
- Swagger / OpenAPI

---

## Arquitetura

O projeto foi estruturado seguindo boas práticas de separação de responsabilidades.

- Controllers
- Models
- DTOs
- Repository Pattern
- Unit of Work
- Entity Framework Core
- AutoMapper
- Filtros personalizados
- Logging customizado

---

## Funcionalidades

- Cadastro de categorias
- Consulta de categorias
- Atualização de categorias
- Exclusão de categorias

- Cadastro de produtos
- Consulta de produtos
- Atualização de produtos
- Exclusão de produtos

---

## Recursos implementados

- Repository Pattern
- Generic Repository
- Unit of Work
- DTOs
- AutoMapper
- Dependency Injection
- Entity Framework Core
- Migrations
- Swagger
- Tratamento global de exceções
- Filtros de Logging
- Serialização JSON

---

## Como executar

### Clone o repositório

```bash
git clone https://github.com/seuusuario/APICatalogo.git
```

### Configure a conexão com o banco

Edite o arquivo

```
appsettings.json
```

e configure sua Connection String.

### Execute as migrations

```bash
dotnet ef database update
```

### Execute a aplicação

```bash
dotnet run
```

---

## Documentação

Após iniciar a aplicação, acesse o Swagger:

```
https://localhost:xxxx/swagger
```

---

## Objetivos de aprendizado

Durante o desenvolvimento foram praticados conceitos importantes do desenvolvimento de APIs utilizando ASP.NET Core:

- Criação de APIs REST
- Entity Framework Core
- Repository Pattern
- Generic Repository
- Unit of Work
- DTOs
- AutoMapper
- Injeção de Dependência
- Logging
- Tratamento de exceções
- Swagger

---

## Observações

Este projeto possui finalidade educacional e foi desenvolvido acompanhando o curso **Web API ASP.NET Core Essencial (.NET 8 / .NET 9)**.

Algumas dependências foram mantidas em versões compatíveis com o conteúdo apresentado durante o curso.

Algumas implementações estão diferentes justamente devido a jeitos diferentes de obter resultados,
Ex. classes CategoriaController e ProdutoController fazendo mapeamento para DTO de formas distintas.

---

## Autor

Matheus (devmendesm)
