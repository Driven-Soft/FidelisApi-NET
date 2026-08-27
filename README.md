# Fidelis
<hr/>

## Equipe Driven Soft

### Integrantes

| Nome | RM |
| --- | --- |
| Felipe Bezerra Beatrici | RM 564723 |
| Max Hayashi Batista | RM 563717 |
| Henrique Cunha Torres | RM 565119 |
| Yasmin Nathalin Miranda dos Santos | RM 561365 |
| Lucas da Silva Lima | RM 562118 |
<hr/>

### Repositório no Github:
* https://github.com/Driven-Soft/FidelisApi-NET

<hr/>

API REST em .NET para gerenciamento de recursos do domínio Fidelis, organizada em camadas e separada em:

1. `Fidelis.Api` — camada HTTP, controllers, Swagger, observabilidade (logs, tracing, métricas, health checks) e tratamento global de erros
2. `Fidelis.Application` — DTOs, contratos e serviços de aplicação
3. `Fidelis.Domain` — entidades e regras de negócio
4. `Fidelis.Infrastructure` — EF Core, persistência, repositórios e mapeamentos
5. `Fidelis.UnitTests` — testes unitários (xUnit + Moq) de domínio e serviços
6. `Fidelis.IntegrationTests` — testes de integração (xUnit + WebApplicationFactory) dos endpoints HTTP

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

## 🚀 Início rápido — o que fazer ao abrir o Swagger

Com a API rodando, abra `http://localhost:5232/swagger` e siga esta ordem. **A API é protegida por JWT**, então quase tudo exige estar autenticado antes de testar.

### Passo 1 — Cadastre um tutor (endpoint público, não precisa de token)

Abra **`POST /api/Tutor`** → `Try it out` → cole:

```json
{
  "cpf": "11122233344",
  "nome": "Maria Teste",
  "email": "maria.teste01@example.com",
  "senha": "123456",
  "telefone": "11988476632",
  "endereco": "Rua das Flores, 100"
}
```
`Execute`. Deve retornar **201 Created**.

> ⚠️ Se aparecer `400` com "Já existe um tutor cadastrado com este email", é porque você já testou com esse e-mail antes — troque o e-mail ou pule direto para o Passo 2 usando o e-mail que já existe.

### Passo 2 — Faça login para obter o token

Abra **`POST /api/Auth/login`** → `Try it out` → cole:

```json
{
  "email": "maria.teste01@example.com",
  "senha": "123456"
}
```
`Execute`. Deve retornar **200 OK** com um corpo assim:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "expiraEm": "...",
  "tutorId": 1,
  "nome": "Maria Teste"
}
```
**Copie o valor de `token`** (sem as aspas).

### Passo 3 — Autorize no Swagger

Clique no botão **🔒 Authorize** no canto superior direito da página. Cole o token copiado no campo (não escreva `Bearer`, o Swagger já adiciona isso sozinho). Clique em **Authorize** e depois em **Close**.

### Passo 4 — Agora todos os endpoints estão liberados

Com o cadeado fechado, teste qualquer endpoint normalmente, por exemplo **`GET /api/Pet`** → `Try it out` → `Execute`. Deve retornar **200 OK**.

Se você tentar chamar um endpoint protegido **sem** ter feito o Passo 3, vai receber **401 Unauthorized** — isso é esperado, não é bug.

### Ordem sugerida para criar dados de teste

Como as entidades têm dependências entre si (ex: um Pet precisa de um Tutor, uma Consulta precisa de um Pet e um Veterinário), siga esta ordem ao criar registros:

`Tutor → Clinica → Veterinario → Pet → Consulta → (Exame | Prescricao → Medicamento) | (Vacinacao | Vermifugacao | HistoricoPeso | Comportamento | Recomendacao | Lembrete)`

Sempre anote o `id` retornado por cada `POST` — ele será usado como chave estrangeira (`tutorId`, `petId`, `veterinarioId`, etc.) nos próximos cadastros.

## Autenticação

A API usa **JWT Bearer** para proteger seus endpoints. O fluxo é:

1. Cadastre um tutor (endpoint público): `POST /api/Tutor`
2. Faça login com o email/senha cadastrados: `POST /api/Auth/login`
3. Use o `token` retornado no header `Authorization: Bearer {token}` nas demais requisições

Endpoints públicos (não exigem token): `POST /api/Tutor` (cadastro) e `POST /api/Auth/login`.
Todos os outros endpoints (`Pet`, `Consulta`, `Vacinacao`, etc.) exigem o header `Authorization`.

Configure a chave de assinatura do token na seção `Jwt` do `appsettings.Development.json`:

```json
{
  "Jwt": {
    "Key": "uma-chave-secreta-bem-longa-e-aleatoria-32-chars-ou-mais",
    "Issuer": "FidelisApi",
    "Audience": "FidelisApiUsers",
    "ExpiresInMinutes": 60
  }
}
```

Exemplo de login via PowerShell:

```powershell
$body = @{ email = "user@example.com"; senha = "123456" } | ConvertTo-Json
$login = Invoke-RestMethod -Method Post -Uri "http://localhost:5232/api/Auth/login" -ContentType "application/json" -Body $body

Invoke-RestMethod -Method Get -Uri "http://localhost:5232/api/Pet" -Headers @{ Authorization = "Bearer $($login.token)" }
```

No Swagger (`/swagger`), clique em **Authorize** e cole apenas o token (sem o prefixo `Bearer`).

> ⚠️ A senha do `Tutor` hoje é validada em texto puro (não há hash). Para produção, recomenda-se migrar para BCrypt/PBKDF2.

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

## Observabilidade

A API expõe os três pilares de observabilidade — logs, tracing e métricas — usando Serilog e OpenTelemetry.

### Health Checks

Usados para verificar se a aplicação está viva e se está pronta para receber tráfego (padrão de probes do Kubernetes).

| Endpoint | Verifica | Uso |
|---|---|---|
| `GET /health/live` | Só se o processo está rodando (não depende de nada externo) | Liveness probe |
| `GET /health/ready` | Conectividade com o banco Oracle (`AddDbContextCheck<FidelisContext>`) | Readiness probe |
| `GET /health` | Relatório completo com todas as verificações | Diagnóstico manual / dashboards |

Exemplo de resposta de `/health/ready`:

```json
{
  "status": "Healthy",
  "results": {
    "oracle-database": { "status": "Healthy" }
  }
}
```

Se o banco estiver indisponível, o endpoint retorna `503 Service Unavailable` com `"status": "Unhealthy"`.

### Logging estruturado

Configurado com Serilog em `Program.cs`, escrevendo no console em formato estruturado (chave/valor), com:

- `UseSerilogRequestLogging()` — loga automaticamente cada requisição HTTP (rota, método, status code, tempo de resposta)
- Enrichment via `Enrich.FromLogContext()`
- Uso de templates estruturados no `ILogger<T>` (ex: `_logger.LogInformation("Pet {PetId} atualizado", id)`), nunca interpolação de string — assim os campos ficam pesquisáveis em ferramentas como Seq/Elasticsearch/Grafana Loki

### Tracing e Métricas (OpenTelemetry)

Configurado em `Fidelis.Api/Extensions/ObservabilityExtensions.cs`:

- **Tracing**: instrumentação automática de requisições HTTP de entrada/saída (`AddAspNetCoreInstrumentation`, `AddHttpClientInstrumentation`) + spans customizados criados manualmente nos serviços via `ActivitySource` (ex: `Pet.Create`, `Tutor.Update`)
- **Métricas**: métricas padrão do ASP.NET Core + contadores customizados via `Meter`/`Counter<T>` (ex: `pets_created_total`, `tutores_created_total`, `consultas_created_total`)
- Por padrão, ambos os sinais são exportados para o console (`AddConsoleExporter()`), o que é suficiente para desenvolvimento/estudo. Em produção, basta trocar o exporter por um OTLP apontando para um coletor (Grafana Tempo, Jaeger, Prometheus, etc.)

O padrão de instrumentação foi aplicado em `PetService`, `TutorService` e `ConsultaService` como referência; o mesmo padrão (injeção de `ILogger<T>`, `ActivitySource` estático e `Counter<T>` estático) pode ser replicado para os demais serviços.

## Testes automatizados

O projeto possui dois níveis de teste, cada um em sua própria solução de testes.

### Fidelis.UnitTests (xUnit + Moq)

Testa unidades isoladas, sem tocar em banco de dados ou HTTP, seguindo o padrão **AAA (Arrange, Act, Assert)**:

- `Domain/` — valida as regras de negócio das entidades (ex: `Pet` não aceita sexo diferente de `M`/`F`, `Tutor` exige senha com 6+ caracteres)
- `Services/` — valida os serviços de aplicação (`PetService`, `TutorService`, `ConsultaService`), usando **Moq** para simular os repositórios (`IPetRepository`, `ITutorRepository`, `IConsultaRepository`) e assim testar a lógica de aplicação isoladamente

Convenção de nomes: `MetodoTestado_Cenario_ResultadoEsperado`.

### Fidelis.IntegrationTests (xUnit + WebApplicationFactory)

Sobe a aplicação inteira em memória e testa o fluxo HTTP completo (roteamento, model binding, validação, serialização, tratamento global de erros):

- `CustomWebApplicationFactory` — sobrescreve o `FidelisContext` (que normalmente aponta para o Oracle) por um banco **EF Core InMemory**, isolado por execução, então os testes rodam sem depender de um Oracle real
- `IntegrationTestCollection` — **Collection Fixture** do xUnit que compartilha uma única instância da fábrica (e do host da aplicação) entre as classes de teste, evitando subir a aplicação inteira a cada classe
- `Controllers/PetControllerTests.cs` e `Controllers/TutorControllerTests.cs` — cobrem criação (201), busca por id (200/404), atualização (200), remoção (204/404) e erros de validação (400)
- `Controllers/HealthCheckTests.cs` — valida que `/health/live` e `/health/ready` respondem `200 OK`

- `Controllers/AuthControllerTests.cs` — cobre o fluxo de autenticação: login com credenciais válidas (200 + token), login com senha/email inválidos (401), e acesso a endpoint protegido sem token (401) e com token válido (200)

### Rodando os testes

```powershell
dotnet test
```

Para rodar só um dos projetos de teste:

```powershell
dotnet test .\Fidelis.UnitTests\Fidelis.UnitTests.csproj
dotnet test .\Fidelis.IntegrationTests\Fidelis.IntegrationTests.csproj
```

## Estrutura do projeto

```text
Fidelis.sln
├── Fidelis.Api
├── Fidelis.Application
├── Fidelis.Domain
├── Fidelis.Infrastructure
├── Fidelis.UnitTests
└── Fidelis.IntegrationTests
```

## Observações

1. A API usa Swagger/OpenAPI para documentação dos endpoints.
2. As validações de domínio podem retornar `400 BadRequest` quando regras esperadas forem violadas.
3. O tratamento global de exceções padroniza as respostas de erro.
