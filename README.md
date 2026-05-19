# Fidelis

<hr/>

## Equipe Driven Soft:
- Felipe Bezerra Beatrici - RM564723
- Max Hayashi Batista - RM564723
- Henrique Cunha Torres - RM565119

<hr/>

API REST em .NET para gerenciamento de recursos do domínio Fidelis, organizada em camadas e separada em:

- `Fidelis.Api` — camada HTTP, controllers, Swagger e tratamento global de erros
- `Fidelis.Application` — DTOs, contratos e serviços de aplicação
- `Fidelis.Domain` — entidades e regras de negócio
- `Fidelis.Infrastructure` — EF Core, persistência, repositórios e mapeamentos

<hr/>

## Visão geral

A solução expõe uma API REST com operações de CRUD completas para os principais recursos do sistema, usando Oracle como banco de dados e Entity Framework Core para acesso aos dados.

### Recursos disponíveis

- `Clinica`
- `Comportamento`
- `Consulta`
- `Exame`
- `HistoricoPeso`
- `Lembrete`
- `Medicamento`
- `Pet`
- `Prescricao`
- `Recomendacao`
- `Tutor`
- `Vacinacao`
- `Vermifugacao`
- `Veterinario`

## Requisitos

- .NET SDK compatível com `net10.0`
- Oracle Database acessível
- Connection string configurada em `Fidelis.Api/appsettings.Development.json` ou em outro mecanismo de configuração local

## Configuração do banco

A API lê a connection string da seção `ConnectionStrings`.

Exemplo esperado:

```json
{
  "ConnectionStrings": {
	"FidelisOracle": "Data Source=SEU_HOST:PORTA/SERVICE_NAME;User ID=SEU_USUARIO;Password=SUA_SENHA;"
  }
}
```

## Instalação

1. Clone ou abra a solução localmente.
2. Verifique se o .NET SDK está instalado:

```powershell
dotnet --version
```

3. Restaure os pacotes da solução:

```powershell
cd C:\Chalkboard\NET\Fidelis
dotnet restore
```

4. Configure a string de conexão em `Fidelis.Api/appsettings.Development.json`.

## Migrations e banco

Se for criar o banco do zero, gere e aplique a migration inicial:

```powershell
cd C:\Chalkboard\NET\Fidelis

dotnet ef migrations add InitialCreate -p .\Fidelis.Infrastructure\Fidelis.Infrastructure.csproj -s .\Fidelis.Api\Fidelis.Api.csproj

dotnet ef database update -p .\Fidelis.Infrastructure\Fidelis.Infrastructure.csproj -s .\Fidelis.Api\Fidelis.Api.csproj
```

> Observação: se o banco já estiver criado, você pode aplicar apenas as migrations pendentes.

## Execução da API

Execute a aplicação:

```powershell
dotnet run --project .\Fidelis.Api\Fidelis.Api.csproj
```

Quando estiver em ambiente de desenvolvimento, o Swagger fica disponível em:

- `http://localhost:5232/swagger`

> A porta pode variar conforme o perfil de execução configurado no seu ambiente.

## Rotas da API

Todos os controllers seguem o padrão REST abaixo, usando a rota base `api/[controller]`:

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/api/{recurso}` | Lista todos os registros |
| `GET` | `/api/{recurso}/{id:int}` | Busca um registro por id |
| `POST` | `/api/{recurso}` | Cria um novo registro |
| `PUT` | `/api/{recurso}/{id:int}` | Atualiza um registro existente |
| `PATCH` | `/api/{recurso}/{id:int}` | Atualiza parcialmente um registro |
| `DELETE` | `/api/{recurso}/{id:int}` | Remove um registro |

### Controllers disponíveis

Cada recurso abaixo expõe o mesmo conjunto de operações CRUD:

- `api/Clinica`
- `api/Comportamento`
- `api/Consulta`
- `api/Exame`
- `api/HistoricoPeso`
- `api/Lembrete`
- `api/Medicamento`
- `api/Pet`
- `api/Prescricao`
- `api/Recomendacao`
- `api/Tutor`
- `api/Vacinacao`
- `api/Vermifugacao`
- `api/Veterinario`

### Códigos HTTP utilizados

- `200 OK` — consulta, atualização com retorno
- `201 Created` — criação de recurso
- `204 NoContent` — remoção concluída com sucesso
- `400 BadRequest` — dados inválidos ou violação de regra de negócio esperada
- `404 NotFound` — recurso não encontrado
- `500 InternalServerError` — falhas inesperadas

## Testando a API

Você pode testar os endpoints de três formas:

1. **Swagger** — acesse `/swagger`, selecione a rota e use `Try it out`
2. **Postman** — crie requests para os endpoints desejados
3. **curl / PowerShell** — envie requisições diretamente para a API

Exemplo de chamada via PowerShell:

```powershell
$body = @{
  cpf = "12312312312"
  nome = "Felipe"
  email = "user@example.com"
  senha = "123456"
  telefone = "11988476632"
  endereco = "Rua Jose das Couves"
} | ConvertTo-Json

Invoke-RestMethod -Method Post -Uri "http://localhost:5232/api/Tutor" -ContentType "application/json" -Body $body
```

## Estrutura do projeto

```text
Fidelis.sln
├── Fidelis.Api
├── Fidelis.Application
├── Fidelis.Domain
└── Fidelis.Infrastructure
```

## Observações

- A API usa Swagger/OpenAPI para documentação dos endpoints.
- As validações de domínio podem retornar `400 BadRequest` quando regras esperadas forem violadas.
- O tratamento global de exceções padroniza as respostas de erro.
