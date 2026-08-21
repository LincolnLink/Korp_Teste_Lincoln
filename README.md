# Korp_Teste_Lincoln
Esse repositório tem o proposito de criar um projeto teste para a Korp, é um Sistema de emissão de Notas Fiscais.

- .NET/c#
- Angular

criando a migration:

dotnet ef migrations add InitialCreate --project Korp.Estoque.Infrastructure --startup-project Korp.Estoque.Api

criando o banco e as tabelas:

dotnet ef database update --project Korp.Estoque.Infrastructure --startup-project Korp.Estoque.Api

para atualizar

dotnet ef database update --project Korp.Estoque.Infrastructure --startup-project Korp.Estoque.Api

database update


