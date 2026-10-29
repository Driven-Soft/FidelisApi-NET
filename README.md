# Fidelis API

<hr/>

## Equipe Driven Soft

| Nome | RM |
| --- | --- |
| Felipe Bezerra Beatrici | RM 564723 |
| Max Hayashi Batista | RM 563717 |
| Henrique Cunha Torres | RM 565119 |
| Yasmin Nathalin Miranda dos Santos | RM 561365 |
| Lucas da Silva Lima | RM 562118 |

### Repositório no GitHub

* https://github.com/Driven-Soft/FidelisApi-NET

<hr/>

## Sumário

1. [Visão geral](#visão-geral)
2. [Arquitetura](#arquitetura)
3. [Tecnologias](#tecnologias)
4. [Instalação e execução](#instalação-e-execução)
5. [MongoDB: local ou MongoDB Atlas](#mongodb-local-ou-mongodb-atlas)
6. [Autenticação](#autenticação)
7. [Paginação, ordenação e filtros](#paginação-ordenação-e-filtros)
8. [HATEOAS](#hateoas)
9. [Endpoints](#endpoints)
10. [Integração com o serviço de IA](#integração-com-o-serviço-de-ia)
11. [Observabilidade e Health Checks](#observabilidade-e-health-checks)
12. [Testes automatizados](#testes-automatizados)
13. [Roteiro de teste manual no Swagger](#roteiro-de-teste-manual-no-swagger)
14. [Estrutura do projeto](#estrutura-do-projeto)

<hr/>

## Visão geral

O **Fidelis** é uma plataforma de saúde contínua para pets. A API centraliza a jornada clínica do animal e conecta tutores e veterinários em torno de um histórico estruturado: consultas, vacinas, vermifugações, exames, prescrições, peso, comportamento, lembretes e recomendações.

A solução usa **persistência poliglota**:

| Banco | Tipo | O que armazena | Por quê |
| --- | --- | --- | --- |
| **Oracle** (EF Core + Migrations) | Relacional | Tutores, pets, clínicas, veterinários, consultas, vacinas, exames, prescrições, etc. | Dados com relacionamentos fortes e integridade referencial |
| **MongoDB** (MongoDB.Driver) | NoSQL (documentos) | **Diário do pet** (`RegistroDiario`) e **trilha de auditoria** (`LogAuditoria`) | Dados semiestruturados (tags livres, textos), alto volume de escrita e consultas flexíveis |

Principais recursos:

* CRUD completo (GET, POST, PUT, PATCH, DELETE) para 15 recursos
* Paginação, ordenação dinâmica e filtros em todas as listagens
* HATEOAS em todas as respostas (links de navegação e de ações)
* Autenticação JWT (todos os endpoints protegidos, exceto cadastro de tutor e login)
* Auditoria automática de toda operação de escrita, gravada no MongoDB de forma assíncrona
* Tratamento global de exceções com `ProblemDetails` (RFC 7807)
* Logging estruturado (Serilog), tracing e métricas (OpenTelemetry), health checks de Oracle e MongoDB
* 195 casos de teste unitários e 55 de integração (xUnit, padrão AAA)

<hr/>

## Arquitetura

A solução segue **Clean Architecture**: as dependências apontam sempre para dentro, em direção ao domínio. A camada `Application` define os contratos (interfaces de repositório e serviços) e a `Infrastructure` os implementa, então a regra de negócio não conhece Oracle, MongoDB nem HTTP.

### Diagrama de camadas

```mermaid
flowchart TB
    subgraph Cliente
        SW[Swagger UI / Postman / Front-end]
    end

    subgraph API["Fidelis.Api (Apresentação)"]
        MW[Middlewares<br/>Serilog • Auditoria • ExceptionHandler • JWT]
        CT[Controllers REST<br/>+ HateoasController]
        HC[Health Checks<br/>/health/live • /health/ready]
    end

    subgraph APP["Fidelis.Application (Casos de uso)"]
        SV[Services]
        DTO[DTOs + QueryParameters<br/>PagedResult • FilterBuilder]
        IR[Interfaces de Repositório]
    end

    subgraph DOM["Fidelis.Domain (Núcleo)"]
        EN[Entidades + Regras de negócio]
    end

    subgraph INF["Fidelis.Infrastructure (Detalhes técnicos)"]
        EF[EF Core<br/>FidelisContext • Repository&lt;T&gt; • QuerySorter]
        MG[MongoDB<br/>MongoDbContext • Repositórios NoSQL]
    end

    ORA[(Oracle)]
    MDB[(MongoDB / Atlas)]

    SW -->|HTTP + JWT| MW --> CT --> SV
    SV --> IR
    SV --> EN
    DTO --> EN
    EF -. implementa .-> IR
    MG -. implementa .-> IR
    EF --> ORA
    MG --> MDB
    HC --> ORA
    HC --> MDB
```

### Fluxo de uma requisição de escrita

```mermaid
sequenceDiagram
    participant C as Cliente
    participant A as AuditoriaMiddleware
    participant J as JWT
    participant Ctrl as Controller
    participant S as Service
    participant R as Repository
    participant DB as Oracle / MongoDB
    participant Q as Fila de Auditoria
    participant BG as AuditoriaBackgroundService
    participant M as MongoDB

    C->>A: POST /api/Pet
    A->>J: valida token
    J->>Ctrl: usuário autenticado
    Ctrl->>S: Create(request)
    S->>R: Add + SaveChanges
    R->>DB: INSERT
    S-->>Ctrl: PetResponse
    Ctrl-->>A: 201 + dados + links HATEOAS
    A->>Q: Enqueue(LogAuditoria)
    A-->>C: 201 Created
    BG->>Q: lê fila
    BG->>M: insere em logs_auditoria
```

### Responsabilidade de cada camada

| Projeto | Responsabilidade | Depende de |
| --- | --- | --- |
| `Fidelis.Domain` | Entidades e regras de negócio (validações no construtor e nos métodos `Update*`) | nada |
| `Fidelis.Application` | Services, DTOs, filtros por recurso, paginação e interfaces de repositório | Domain |
| `Fidelis.Infrastructure` | EF Core/Oracle (migrations, mapeamentos, repositórios) e MongoDB (mapeamento BSON, repositórios, índices) | Application, Domain |
| `Fidelis.Api` | Controllers, HATEOAS, middlewares, JWT, Swagger, health checks, observabilidade, injeção de dependência | Application, Infrastructure |
| `Fidelis.UnitTests` | Testes de domínio, services, paginação, filtros e ordenação (xUnit + Moq) | Application, Domain, Infrastructure |
| `Fidelis.IntegrationTests` | Testes HTTP ponta a ponta com `WebApplicationFactory`, EF InMemory e repositórios Mongo em memória | Api |

### Princípios aplicados

* **SRP**: controllers só orquestram HTTP, services contêm o caso de uso, entidades contêm as regras, repositórios só acessam dados
* **OCP**: novos filtros entram no `XQueryParameters.ToFilters()` sem alterar o repositório genérico. Novos recursos herdam `Repository<T>` e `HateoasController`
* **LSP/ISP**: interfaces de repositório específicas por agregado (`ITutorRepository` adiciona `GetByEmail` e `ExistsByEmail`)
* **DIP**: services dependem de interfaces da `Application`, e as implementações são injetadas via DI (`ServiceExtensions`, `PersistenceExtensions`, `MongoExtensions`)
* **Clean Code**: nomes expressivos, métodos curtos, sem duplicação de lógica de paginação e links, tratamento de erros centralizado no `GlobalExceptionHandler`

<hr/>

## Tecnologias

| Categoria | Tecnologia |
| --- | --- |
| Runtime | .NET 10 / ASP.NET Core |
| Relacional | Oracle + Entity Framework Core 9 + Migrations |
| NoSQL | MongoDB 7 / MongoDB Atlas + MongoDB.Driver 3 |
| Documentação | Swagger / OpenAPI (Swashbuckle + Annotations) |
| Segurança | JWT Bearer |
| Logs | Serilog (console estruturado + request logging) |
| Tracing e métricas | OpenTelemetry (ActivitySource, Meter, Console Exporter) |
| Health checks | `Microsoft.Extensions.Diagnostics.HealthChecks` + EF Core check + check customizado do Mongo |
| Testes | xUnit, Moq, `Microsoft.AspNetCore.Mvc.Testing`, EF Core InMemory |

<hr/>

## Instalação e execução

### Pré-requisitos

1. .NET SDK 10.0.x (`dotnet --version`)
2. Ferramenta do EF Core: `dotnet tool install --global dotnet-ef`
3. Acesso a um Oracle (ex.: `oracle.fiap.com.br`)
4. Um MongoDB: local via Docker **ou** um cluster gratuito no MongoDB Atlas (veja a [próxima seção](#mongodb-local-ou-mongodb-atlas))

### Passo a passo

```powershell
git clone https://github.com/Driven-Soft/FidelisApi-NET.git
cd FidelisApi-NET
dotnet restore
```

Configure `Fidelis.Api/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "FidelisOracle": "Data Source=oracle.fiap.com.br:1521/orcl;User ID=SEU_RM;Password=SUA_SENHA;"
  },
  "Jwt": {
    "Key": "uma-chave-secreta-bem-longa-com-pelo-menos-32-caracteres",
    "Issuer": "FidelisApi",
    "Audience": "FidelisApiUsers",
    "ExpiresInMinutes": 60
  },
  "MongoDb": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "fidelis",
    "ServerSelectionTimeoutSeconds": 5
  }
}
```

Aplique as migrations no Oracle:

```powershell
dotnet ef database update -p .\Fidelis.Infrastructure\Fidelis.Infrastructure.csproj -s .\Fidelis.Api\Fidelis.Api.csproj
```

Execute:

```powershell
dotnet run --project .\Fidelis.Api\Fidelis.Api.csproj
```

Swagger: **http://localhost:5232/swagger**

> O MongoDB não precisa de migrations: o banco `fidelis`, as coleções `registros_diario` e `logs_auditoria` e seus índices são criados automaticamente na primeira execução.

<hr/>

## MongoDB: local ou MongoDB Atlas

### Opção A: Docker (local)

```powershell
docker compose up -d
```

O `docker-compose.yml` na raiz sobe um `mongo:7.0` em `localhost:27017`. A connection string padrão (`mongodb://localhost:27017`) já funciona.

### Opção B: MongoDB Atlas (cluster gratuito na nuvem)

1. Acesse **https://www.mongodb.com/cloud/atlas/register** e crie uma conta (pode usar o login Google).
2. Em **Create a cluster**, escolha o plano **M0 (Free)**, provedor AWS e a região **São Paulo (sa-east-1)**. Dê o nome `fidelis-cluster` e clique em **Create Deployment**.
3. Na tela **Connect to fidelis-cluster**:
   * **Create a database user**: usuário `fidelis_app` e uma senha (anote, e evite caracteres como `@`, `:` e `/`).
   * Clique em **Create Database User**.
4. No menu lateral, abra **Security → Network Access → Add IP Address**:
   * Para desenvolvimento, clique em **Allow access from anywhere** (`0.0.0.0/0`) ou **Add current IP address**.
   * Confirme e aguarde o status **Active**.
5. Volte em **Database → Clusters → Connect → Drivers**, selecione **C# / .NET** e copie a connection string. Ela tem o formato:

   ```
   mongodb+srv://fidelis_app:<db_password>@fidelis-cluster.xxxxx.mongodb.net/?retryWrites=true&w=majority&appName=fidelis-cluster
   ```

6. Substitua `<db_password>` pela senha do passo 3 e cole no `appsettings.Development.json`:

   ```json
   "MongoDb": {
     "ConnectionString": "mongodb+srv://fidelis_app:SUA_SENHA@fidelis-cluster.xxxxx.mongodb.net/?retryWrites=true&w=majority&appName=fidelis-cluster",
     "DatabaseName": "fidelis",
     "ServerSelectionTimeoutSeconds": 10
   }
   ```

   Se preferir não salvar a senha no arquivo, use variável de ambiente:

   ```powershell
   $env:MongoDb__ConnectionString = "mongodb+srv://fidelis_app:SUA_SENHA@fidelis-cluster.xxxxx.mongodb.net/?retryWrites=true&w=majority"
   ```

7. Rode a API e acesse **http://localhost:5232/health/ready**. O resultado esperado é `"mongodb": { "status": "Healthy" }`.
8. Para ver os dados: no Atlas, abra **Database → Browse Collections → fidelis**. As coleções `registros_diario` e `logs_auditoria` aparecem após o primeiro `POST`.

> Se o `/health/ready` mostrar o Mongo como `Unhealthy`, quase sempre é o IP fora do **Network Access** ou a senha errada na connection string.

<hr/>

## Autenticação

1. `POST /api/Tutor`: cadastro (público)
2. `POST /api/Auth/login` com `{ email, senha, tipo }`, onde `tipo` é `TUTOR` (padrão quando omitido) ou `VETERINARIO`. Retorna `{ token, expiraEm, tipo, tutorId, veterinarioId, nome, email }`: `tutorId` vem preenchido no login de tutor e `veterinarioId` no de veterinário. O token leva o tipo nas claims `role` e `tipo`; `tipo` inválido responde **400**
3. Envie `Authorization: Bearer {token}` nas demais requisições. No Swagger, clique em **Authorize** e cole só o token.

Sem token, qualquer endpoint protegido responde **401 Unauthorized**.

<hr/>

## Paginação, ordenação e filtros

Todas as rotas `GET /api/{recurso}` aceitam:

| Parâmetro | Padrão | Regra |
| --- | --- | --- |
| `page` | `1` | ≥ 1 |
| `pageSize` | `10` | 1 a 100 |
| `sortBy` | `id` (no Mongo: data mais recente) | Qualquer propriedade simples do recurso (`nome`, `dataHora`, `pesoKg`...). Campo inexistente retorna **400** |
| `sortDirection` | `asc` | `asc` ou `desc` |

Filtros específicos (texto é parcial e não diferencia maiúsculas de minúsculas):

| Recurso | Filtros |
| --- | --- |
| `Clinica` | `nome`, `cnpj`, `email` |
| `Comportamento` | `petId`, `descricao`, `dataInicio`, `dataFim` |
| `Consulta` | `petId`, `veterinarioId`, `tipo`, `dataInicio`, `dataFim` |
| `Exame` | `consultaId`, `tipo`, `dataInicio`, `dataFim` |
| `HistoricoPeso` | `petId`, `pesoMinimo`, `pesoMaximo`, `dataInicio`, `dataFim` |
| `Lembrete` | `petId`, `tutorId`, `status` (P/C/X), `tipo`, `dataInicio`, `dataFim` |
| `Medicamento` | `prescricaoId`, `nome` |
| `Pet` | `nome`, `especie`, `raca`, `sexo`, `status`, `tutorId`, `clinicaId` |
| `Prescricao` | `consultaId`, `dosagem`, `duracaoMinimaDias`, `duracaoMaximaDias` |
| `Recomendacao` | `petId`, `tipo`, `dataInicio`, `dataFim` |
| `Tutor` | `nome`, `email`, `cpf` |
| `Vacinacao` | `petId`, `veterinarioId`, `vacina`, `proximaAte` |
| `Vermifugacao` | `petId`, `veterinarioId`, `produto`, `proximaAte` |
| `Veterinario` | `nome`, `crmv`, `especialidade`, `clinicaId` |
| `RegistroDiario` (Mongo) | `petId`, `humor`, `tag`, `texto` (título ou descrição), `dataInicio`, `dataFim` |
| `Auditoria` (Mongo) | `metodo`, `caminho`, `statusCode`, `usuarioId`, `dataInicio`, `dataFim` |

Exemplos:

```
GET /api/Pet?especie=cachorro&sortBy=nome&sortDirection=desc&page=1&pageSize=5
GET /api/Vacinacao?petId=3&proximaAte=2026-12-31
GET /api/RegistroDiario?petId=1&tag=passeio&humor=feliz
GET /api/Auditoria?metodo=DELETE&statusCode=404
```

Como funciona por dentro:

* `QueryParameters` (Application) valida `page`, `pageSize` e `sortDirection` com DataAnnotations. Valores inválidos retornam 400 automaticamente.
* Cada `XQueryParameters.ToFilters()` gera expressões LINQ via `FilterBuilder<T>`, traduzidas pelo EF Core em SQL (`WHERE`, `UPPER(...) LIKE`, `OFFSET/FETCH`).
* `Repository<T>.GetPaged` aplica filtros, conta o total, ordena com `QuerySorter` (expression tree que só aceita propriedades escalares) e pagina no banco.
* No MongoDB, os repositórios montam `FilterDefinition` e `SortDefinition` nativos, com `CountDocuments`, `Skip` e `Limit`.

<hr/>

## HATEOAS

Todo recurso individual é devolvido como `{ data, links }`:

```json
{
  "data": {
    "id": 7,
    "nome": "Rex",
    "especie": "Cachorro",
    "raca": "Labrador",
    "sexo": "M",
    "status": "A",
    "tutorId": 1,
    "clinicaId": null
  },
  "links": [
    { "href": "http://localhost:5232/api/Pet/7", "rel": "self", "method": "GET" },
    { "href": "http://localhost:5232/api/Pet/7", "rel": "update", "method": "PUT" },
    { "href": "http://localhost:5232/api/Pet/7", "rel": "partial-update", "method": "PATCH" },
    { "href": "http://localhost:5232/api/Pet/7", "rel": "delete", "method": "DELETE" },
    { "href": "http://localhost:5232/api/Pet", "rel": "collection", "method": "GET" }
  ]
}
```

Listagens retornam metadados de paginação e links de navegação, preservando os filtros da URL:

```json
{
  "items": [ { "data": { "id": 7, "nome": "Rex" }, "links": [ "..." ] } ],
  "page": 2,
  "pageSize": 5,
  "totalItems": 23,
  "totalPages": 5,
  "links": [
    { "href": "http://localhost:5232/api/Pet?especie=cachorro&page=2&pageSize=5", "rel": "self", "method": "GET" },
    { "href": "http://localhost:5232/api/Pet?especie=cachorro&page=1&pageSize=5", "rel": "first", "method": "GET" },
    { "href": "http://localhost:5232/api/Pet?especie=cachorro&page=1&pageSize=5", "rel": "previous", "method": "GET" },
    { "href": "http://localhost:5232/api/Pet?especie=cachorro&page=3&pageSize=5", "rel": "next", "method": "GET" },
    { "href": "http://localhost:5232/api/Pet?especie=cachorro&page=5&pageSize=5", "rel": "last", "method": "GET" },
    { "href": "http://localhost:5232/api/Pet", "rel": "create", "method": "POST" }
  ]
}
```

A geração dos links está centralizada em `Fidelis.Api/Hateoas/HateoasController.cs`, herdado por todos os controllers. Os links são criados com `Url.Action`, então acompanham automaticamente as rotas reais. Recursos somente leitura, como a `Auditoria`, recebem apenas os links das ações que existem.

<hr/>

## Endpoints

Rota base: `api/[controller]`. Todos exigem JWT, exceto os marcados como **público**.

### Recursos relacionais (Oracle)

Cada recurso abaixo expõe o mesmo conjunto de operações:

| Método | Rota | Descrição | Respostas |
| --- | --- | --- | --- |
| `GET` | `/api/{recurso}` | Lista paginada com filtros e ordenação | 200, 400, 401 |
| `GET` | `/api/{recurso}/{id:int}` | Busca por id | 200, 401, 404 |
| `POST` | `/api/{recurso}` | Cria | 201 (com header `Location`), 400, 401 |
| `PUT` | `/api/{recurso}/{id:int}` | Atualização completa | 200, 400, 401, 404 |
| `PATCH` | `/api/{recurso}/{id:int}` | Atualização parcial (só os campos enviados) | 200, 400, 401, 404 |
| `DELETE` | `/api/{recurso}/{id:int}` | Remove | 204, 401, 404 |

Recursos: `Clinica`, `Comportamento`, `Consulta`, `Exame`, `HistoricoPeso`, `Lembrete`, `Medicamento`, `Pet`, `Prescricao`, `Recomendacao`, `Tutor` (`POST` público), `Vacinacao`, `Vermifugacao`, `Veterinario`.

### Recursos NoSQL (MongoDB)

| Método | Rota | Descrição |
| --- | --- | --- |
| `GET` | `/api/RegistroDiario` | Lista o diário com filtros (`petId`, `humor`, `tag`, `texto`, período) |
| `GET` | `/api/RegistroDiario/{id}` | Busca por ObjectId (24 caracteres) |
| `POST` | `/api/RegistroDiario` | Cria registro. O `petId` precisa existir no Oracle |
| `PUT` | `/api/RegistroDiario/{id}` | Substitui o registro |
| `PATCH` | `/api/RegistroDiario/{id}` | Atualiza campos específicos |
| `DELETE` | `/api/RegistroDiario/{id}` | Remove |
| `GET` | `/api/Auditoria` | Lista logs de auditoria (somente leitura) |
| `GET` | `/api/Auditoria/{id}` | Busca log por ObjectId |

Exemplo de `POST /api/RegistroDiario`:

```json
{
  "petId": 1,
  "titulo": "Passeio no parque",
  "descricao": "Correu bastante e brincou com outros cães.",
  "humor": "Feliz",
  "tags": ["passeio", "energia"],
  "dataRegistro": "2026-10-20T10:00:00Z"
}
```

Humores aceitos: `Feliz`, `Calmo`, `Agitado`, `Ansioso`, `Triste`, `Doente`. Tags são normalizadas (minúsculas, sem repetição, no máximo 10).

Documento salvo no MongoDB (coleção `registros_diario`):

```json
{
  "_id": { "$oid": "6715a1f0c2a4b1e9d0f3a7c2" },
  "petId": 1,
  "titulo": "Passeio no parque",
  "descricao": "Correu bastante e brincou com outros cães.",
  "humor": "Feliz",
  "tags": ["passeio", "energia"],
  "dataRegistro": { "$date": "2026-10-20T10:00:00Z" },
  "criadoEm": { "$date": "2026-10-20T13:05:11Z" },
  "atualizadoEm": null
}
```

### Autenticação e infraestrutura

| Método | Rota | Descrição |
| --- | --- | --- |
| `POST` | `/api/Auth/login` | Login de tutor ou veterinário (**público**) |
| `GET` | `/health/live` | Liveness (**público**) |
| `GET` | `/health/ready` | Readiness: Oracle + MongoDB (**público**) |
| `GET` | `/health` | Relatório completo (**público**) |

### Erros (ProblemDetails)

```json
{
  "type": "https://httpstatuses.io/400",
  "title": "Requisição inválida",
  "status": 400,
  "detail": "Sexo deve ser 'M' ou 'F'",
  "instance": "/api/Pet",
  "traceId": "0HN7..."
}
```

| Exceção | Status |
| --- | --- |
| `ArgumentException` (regras de domínio, ordenação inválida) | 400 |
| `InvalidOperationException` (ex.: email duplicado) | 400 |
| `KeyNotFoundException` | 404 |
| `UnauthorizedAccessException` | 401 |
| `OracleException`, `MongoException`, `TimeoutException` | 503 |
| Qualquer outra | 500 (detalhes só em Development) |

A especificação OpenAPI completa fica disponível em `http://localhost:5232/swagger/v1/swagger.json` com a API rodando.

<hr/>

## Integração com o serviço de IA

A cada pesagem registrada, a API consulta o serviço de IA [FidelisAI](https://github.com/Driven-Soft/FidelisIA) (Python/FastAPI), que estima a faixa de peso esperada para o perfil do pet e classifica o risco em `VERDE`, `AMARELO` ou `VERMELHO`. A API continua sendo a única com acesso ao banco: grava em `RECOMENDACOES` e `LEMBRETES` as ações sugeridas, sem alteração de schema.

```text
POST /api/HistoricoPeso → grava HISTORICO_PESOS → POST /analise (FidelisAI)
                                                  ↓
                     resposta 201 com avaliacaoIa ← grava RECOMENDACOES e LEMBRETES
```

| Regra | Comportamento |
| --- | --- |
| Momento | Depois que a pesagem é gravada (`SaveChanges`) |
| Resiliência | Falha, timeout ou IA desligada **nunca** invalidam a pesagem: a resposta é `201` com `avaliacaoIa: null` e o erro vai para o log |
| Idempotência | Se o pet já tem recomendação do mesmo `Tipo` nos últimos `JanelaRecomendacaoDias` (30), recomendação e lembrete não são repetidos. Uma escalada de risco (atenção → alerta) gera novo registro |
| Pesagem retroativa | Só a pesagem mais recente do pet é analisada; registros de datas passadas não geram alerta |
| Payload | `especie` normalizada de `PETS.Especie` ("Cão", "Cadela", "Gata"… → `Cachorro`/`Gato`; outras espécies não são enviadas), `raca` de `PETS.Raca`, `idade_anos` calculada de `DataNascimento` até `DataMedicao` |
| Lembrete | `DataPrevista` = data da análise + `prazo_dias`; `TutorId` vem de `PETS.TutorId` |

Endpoints:

| Método | Rota | Descrição |
| --- | --- | --- |
| `POST` | `/api/HistoricoPeso` | Grava a pesagem e devolve a avaliação da IA em `avaliacaoIa` |
| `GET` | `/api/HistoricoPeso/pet/{petId}/avaliacao` | Avalia a pesagem mais recente do pet, sem gravar nada (usado pela tela do pet no app). `404` se o pet não existe ou não tem pesagens, `400` para espécie não avaliada, `503` com a IA indisponível |

Configuração (`appsettings.json`, ou variáveis `FidelisAi__*` no App Service):

```json
"FidelisAi": {
  "BaseUrl": "http://localhost:8000",
  "ApiKey": "",
  "TimeoutSeconds": 3,
  "JanelaRecomendacaoDias": 30
}
```

Com `BaseUrl` vazio a análise fica desligada. `ApiKey` vai no header `X-API-Key` e deve ser igual à `FIDELIS_IA_API_KEY` do serviço. Falhas de comunicação com a IA (`HttpRequestException` e timeout) são mapeadas para **503** no `GlobalExceptionHandler`, como as do Oracle e do MongoDB. Para rodar localmente, suba o serviço (`docker run -p 8000:8000 fidelis-ia`, ver README do FidelisAI) antes da API. O código fica em `AnalisePesoService` (Application), `FidelisAiClient` (Infrastructure, via `IHttpClientFactory`) e `FidelisAiExtensions` (Api).

<hr/>

## Observabilidade e Health Checks

### Health Checks

| Endpoint | Verifica | Uso |
| --- | --- | --- |
| `GET /health/live` | Processo em execução | Liveness probe |
| `GET /health/ready` | Oracle (`AddDbContextCheck`) e MongoDB (`ping`) | Readiness probe |
| `GET /health` | Todas as verificações | Diagnóstico e dashboards |

```json
{
  "status": "Healthy",
  "totalDuration": "00:00:00.0451",
  "entries": {
    "oracle-database": { "status": "Healthy" },
    "mongodb": { "status": "Healthy", "description": "MongoDB respondeu ao ping." }
  }
}
```

Se algum banco cair, o endpoint retorna **503** com `"status": "Unhealthy"`.

### Logging estruturado (Serilog)

* Logs no console no formato chave/valor, enriquecidos com `Application`, `TraceId` e `UserId`
* `UseSerilogRequestLogging()` registra método, rota, status e tempo de cada requisição
* Services usam templates estruturados (`_logger.LogInformation("Pet {PetId} atualizado", id)`), nunca interpolação
* O `GlobalExceptionHandler` registra 4xx como `Warning` e 5xx como `Error` com stack trace

### Auditoria (MongoDB)

Toda requisição `POST`, `PUT`, `PATCH` e `DELETE` em `/api` gera um documento em `logs_auditoria` com método, caminho, status, usuário (claim `sub`), email e duração. O middleware só enfileira o log (`Channel<T>`) e um `BackgroundService` grava no Mongo, então a requisição não espera o banco NoSQL e não falha se ele estiver indisponível.

### Tracing e métricas (OpenTelemetry)

* Tracing automático de requisições HTTP de entrada e saída, além de spans manuais nos services (`Pet.Create`, `Tutor.Update`, `RegistroDiario.Create`...)
* Métricas ASP.NET Core e contadores customizados: `pets_created_total`, `tutores_created_total`, `consultas_created_total`, `registros_diario_created_total`
* Exportação para o console. Em produção, basta trocar por OTLP (Grafana, Jaeger, Prometheus)

<hr/>

## Testes automatizados

```powershell
dotnet test
```

Ou separadamente:

```powershell
dotnet test .\Fidelis.UnitTests\Fidelis.UnitTests.csproj
dotnet test .\Fidelis.IntegrationTests\Fidelis.IntegrationTests.csproj
```

Com cobertura:

```powershell
dotnet test --collect:"XPlat Code Coverage"
```

Todos os testes seguem o padrão **AAA (Arrange, Act, Assert)**, com os três blocos separados por linha em branco, e a convenção de nomes `Metodo_Cenario_ResultadoEsperado`.

### Fidelis.UnitTests (xUnit + Moq): 195 casos

| Pasta | O que cobre |
| --- | --- |
| `Domain/` | Regras de **todas as 16 entidades**: Tutor, Pet, Clinica, Veterinario, Consulta, Vacinacao, Vermifugacao, HistoricoPeso, Prescricao, Lembrete, Medicamento, Exame, Recomendacao, Comportamento, RegistroDiario e LogAuditoria |
| `Application/` | `QueryParameters` (validação, skip, direção), `PagedResult` (total de páginas, navegação, map), `FilterBuilder`, `TextFilter` e os filtros de Pet, Consulta, Lembrete, Tutor, Vacinacao e HistoricoPeso aplicados sobre coleções reais |
| `Services/` | Pet, Tutor, Consulta, Auth (geração e validação de JWT), Clinica, Veterinario, Vacinacao, Lembrete, Prescricao, HistoricoPeso, RegistroDiario (inclui validação do pet no Oracle) e Auditoria, com repositórios simulados via Moq. `AnalisePesoService`: gravação das ações por risco, idempotência, falha da IA sem exceção, espécie não suportada, pesagem retroativa e montagem do payload |
| `Infrastructure/` | `QuerySorter`: ordenação ascendente e descendente e rejeição de campos inválidos ou de navegação. `FidelisAiClient`: JSON em snake_case, leitura da resposta e status de erro |

### Fidelis.IntegrationTests (xUnit + WebApplicationFactory): 55 casos

* `CustomWebApplicationFactory` troca o Oracle por **EF Core InMemory** e os repositórios do MongoDB por **implementações em memória**, então os testes rodam sem nenhum banco externo
* `PetControllerTests` e `TutorControllerTests`: CRUD completo, 404, 400 e links HATEOAS
* `PaginacaoTests`: metadados, links `first`/`previous`/`next`/`last`, ordenação desc, filtros e parâmetros inválidos (400)
* `RegistroDiarioControllerTests`: CRUD NoSQL, validação de pet inexistente e humor inválido, filtro por pet e tag
* `AuditoriaControllerTests`: confirma que escritas geram log com usuário e status
* `AuthControllerTests`: login válido e inválido, acesso com e sem token
* `HealthCheckTests`: `/health/live`, `/health/ready` e `/health`
* `HistoricoPesoControllerTests`: pesagem com IA (recomendação e lembrete gravados), IA fora do ar (pesagem mantida), alerta sem duplicação, escalada de risco e o endpoint de avaliação (200, 404, 400, 503), com o serviço de IA substituído por `InMemoryFidelisAiClient`

<hr/>

## Roteiro de teste manual no Swagger

Com a API rodando, abra `http://localhost:5232/swagger`:

1. **`POST /api/Tutor`** (público):
   ```json
   { "cpf": "11122233344", "nome": "Maria Teste", "email": "maria.teste@example.com", "senha": "123456", "telefone": "11988476632", "endereco": "Rua das Flores, 100" }
   ```
2. **`POST /api/Auth/login`** com o mesmo email e senha. Copie o `token`.
3. Clique em **Authorize**, cole o token e confirme.
4. **`POST /api/Clinica`**, depois **`POST /api/Veterinario`** e **`POST /api/Pet`**. Anote os `id` retornados em `data.id` e observe os `links`.
5. **`GET /api/Pet?page=1&pageSize=2&sortBy=nome`**: veja `totalItems`, `totalPages` e os links `next`/`last`.
6. **`POST /api/RegistroDiario`** com o `petId` criado (exemplo na seção [Endpoints](#endpoints)).
7. **`GET /api/RegistroDiario?petId={id}&tag=passeio`**: consulta no MongoDB.
8. **`GET /api/Auditoria?metodo=POST`**: veja os logs das operações anteriores.
9. **`GET /health/ready`**: confirme Oracle e MongoDB como `Healthy`.

Ordem sugerida para criar dados:

`Tutor → Clinica → Veterinario → Pet → Consulta → (Exame | Prescricao → Medicamento) | (Vacinacao | Vermifugacao | HistoricoPeso | Comportamento | Recomendacao | Lembrete | RegistroDiario)`

O arquivo `Fidelis.Api/Fidelis.http` traz as mesmas requisições prontas para Rider, VS Code (REST Client) ou Visual Studio.

<hr/>

## Estrutura do projeto

```text
Fidelis.sln
├── docker-compose.yml                 MongoDB local
├── Fidelis.Api
│   ├── Auditoria/                     Middleware, fila e BackgroundService de auditoria
│   ├── Controllers/                   17 controllers REST
│   ├── Exceptions/                    GlobalExceptionHandler (ProblemDetails)
│   ├── Extensions/                    DI, JWT, Swagger, Mongo, Health Checks, OpenTelemetry
│   ├── Hateoas/                       Link, ResourceResponse, PagedResponse, HateoasController
│   ├── HealthChecks/                  MongoHealthCheck, criação de índices
│   └── Program.cs
├── Fidelis.Application
│   ├── Common/                        QueryParameters, PagedResult, FilterBuilder, TextFilter
│   ├── DTOs/                          Requests, Responses e QueryParameters por recurso
│   ├── Interfaces/Repositories/       Contratos de persistência (relacional e NoSQL)
│   └── Services/                      Casos de uso
├── Fidelis.Domain
│   └── Entities/                      14 entidades relacionais + RegistroDiario + LogAuditoria
├── Fidelis.Infrastructure
│   ├── Migrations/                    Migrations do EF Core (Oracle)
│   ├── Mongo/                         MongoDbContext, mapeamentos BSON, repositórios NoSQL
│   ├── Persistence/                   FidelisContext + configurações Fluent API
│   └── Repositories/                  Repository<T>, QuerySorter e repositórios concretos
├── Fidelis.UnitTests
└── Fidelis.IntegrationTests
```
