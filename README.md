# Fidelis

<hr/>

## Equipe Driven Soft:
1. Felipe Bezerra Beatrici - RM564723
2. Max Hayashi Batista - RM564723
3. Henrique Cunha Torres - RM565119

<hr/>

API REST em .NET para gerenciamento de recursos do domínio Fidelis, organizada em camadas e separada em:

1. `Fidelis.Api` — camada HTTP, controllers, Swagger e tratamento global de erros
2. `Fidelis.Application` — DTOs, contratos e serviços de aplicação
3. `Fidelis.Domain` — entidades e regras de negócio
4. `Fidelis.Infrastructure` — EF Core, persistência, repositórios e mapeamentos

<hr/>

## Visão geral

A solução expõe uma API REST com operações de CRUD completas para os principais recursos do sistema, usando Oracle como banco de dados e Entity Framework Core para acesso aos dados.

### Recursos disponíveis

1. `Clinica`
2. `Comportamento`
3. `Consulta`
4. `Exame`
5. `HistoricoPeso`
6. `Lembrete`
7. `Medicamento`
8. `Pet`
9. `Prescricao`
10. `Recomendacao`
11. `Tutor`
12. `Vacinacao`
13. `Vermifugacao`
14. `Veterinario`

## Requisitos

1. .NET SDK compatível com `net10.0`
2. Oracle Database acessível
3. Connection string configurada em `Fidelis.Api/appsettings.Development.json`

## Configuração do banco

A API lê a connection string da seção `ConnectionStrings`.

Coloque suas credenciais de banco e detalhes de conexão no arquivo `appsettings.Development.json` seguindo o formato abaixo:

```json
{
  "ConnectionStrings": {
	"FidelisOracle": "Data Source=SEU_HOST:PORTA/SERVICE_NAME;User ID=SEU_USUARIO;Password=SUA_SENHA;"
  }
}
```

## Instalação

1. Clone ou abra a solução localmente.
2. Verifique se o .NET SDK está instalado na versão 10.0.X ou superior:

```powershell
dotnet --version
```

* Caso o .NET SDK esteja numa versão diferente da esperada, instale a versão correta do SDK a partir do site oficial: https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-10.0.300-windows-x64-installer

* Execute o comando:
```powershell
dotnet --list-sdks
```

* Instale o dotnet ef global tool, caso ainda não tenha:

```powershell
dotnet tool install --global dotnet-ef
```

* Teste o funcionamento do dotnet ef:

```powershell
dotnet ef --version
```

3. Restaure os pacotes da solução:

```powershell
dotnet restore
```

4. Configure a string de conexão em `Fidelis.Api/appsettings.Development.json`.

## Migrations e banco

**Se for criar o banco do zero**, gere e aplique a migration inicial.
Como no projeto as migrations já estão criadas, basta aplicar o segundo comando:

```powershell
dotnet ef migrations add Migration_004 -p .\Fidelis.Infrastructure\Fidelis.Infrastructure.csproj -s .\Fidelis.Api\Fidelis.Api.csproj
```

```powershell
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

1. `api/Clinica`
2. `api/Comportamento`
3. `api/Consulta`
4. `api/Exame`
5. `api/HistoricoPeso`
6. `api/Lembrete`
7. `api/Medicamento`
8. `api/Pet`
9. `api/Prescricao`
10. `api/Recomendacao`
11. `api/Tutor`
12. `api/Vacinacao`
13. `api/Vermifugacao`
14. `api/Veterinario`

### Códigos HTTP utilizados

1. `200 OK` — consulta, atualização com retorno
2. `201 Created` — criação de recurso
3. `204 NoContent` — remoção concluída com sucesso
4. `400 BadRequest` — dados inválidos ou violação de regra de negócio esperada
5. `404 NotFound` — recurso não encontrado
6. `500 InternalServerError` — falhas inesperadas

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

1. A API usa Swagger/OpenAPI para documentação dos endpoints.
2. As validações de domínio podem retornar `400 BadRequest` quando regras esperadas forem violadas.
3. O tratamento global de exceções padroniza as respostas de erro.