# Testando a Fidelis API pelo Git Bash

## 1. Pré-requisitos

```bash
dotnet --version
dotnet ef --version
curl --version
```

O .NET precisa estar na versão 10.x. Se o `dotnet ef` não existir, instale com:

```bash
dotnet tool install --global dotnet-ef
```

## 2. Entrar na pasta do projeto

```bash
cd /c/Users/SEU_USUARIO/caminho/FidelisApi-NET
```

No Git Bash, `C:\` vira `/c/`. Dica: digite `cd ` e arraste a pasta para a janela.

## 3. Restaurar, compilar e rodar os testes automatizados

```bash
dotnet restore
dotnet build
dotnet test
```

Os testes automatizados não precisam de Oracle nem de MongoDB: eles usam bancos em memória.

## 4. Criar/atualizar as tabelas no Oracle

```bash
dotnet ef database update -p ./Fidelis.Infrastructure/Fidelis.Infrastructure.csproj -s ./Fidelis.Api/Fidelis.Api.csproj
```

## 5. Subir a API (deixe este terminal aberto)

```bash
dotnet run --project ./Fidelis.Api/Fidelis.Api.csproj
```

Espere aparecer `Now listening on: http://localhost:5232`.

## 6. Abrir um segundo Git Bash e testar

### Teste automático completo

```bash
cd /c/Users/SEU_USUARIO/caminho/FidelisApi-NET
./testar-api.sh
```

O script cadastra um tutor, faz login, cria clínica e pets, testa paginação, filtros, HATEOAS, PATCH, validações (400), grava no MongoDB Atlas (diário e auditoria) e apaga os dados de teste. No final ele mostra quantas verificações passaram.

### Testes manuais com curl

Health check (Oracle + MongoDB):

```bash
curl -s http://localhost:5232/health/ready
```

Cadastrar tutor:

```bash
curl -s -X POST http://localhost:5232/api/Tutor \
  -H "Content-Type: application/json" \
  -d '{"cpf":"11122233344","nome":"Maria Teste","email":"maria.gitbash@example.com","senha":"123456","telefone":"11988476632","endereco":"Rua das Flores, 100"}'
```

Login e salvar o token numa variável:

```bash
TOKEN=$(curl -s -X POST http://localhost:5232/api/Auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"maria.gitbash@example.com","senha":"123456"}' | grep -o '"token":"[^"]*"' | cut -d'"' -f4)
echo $TOKEN
```

Listar pets com paginação e HATEOAS:

```bash
curl -s "http://localhost:5232/api/Pet?page=1&pageSize=5&sortBy=nome" -H "Authorization: Bearer $TOKEN"
```

Criar registro no diário (MongoDB), trocando `petId` por um id existente:

```bash
curl -s -X POST http://localhost:5232/api/RegistroDiario \
  -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" \
  -d '{"petId":1,"titulo":"Passeio","descricao":"Brincou no parque","humor":"Feliz","tags":["passeio"],"dataRegistro":"2026-10-20T10:00:00Z"}'
```

Ver a auditoria gravada no MongoDB:

```bash
curl -s "http://localhost:5232/api/Auditoria?pageSize=5" -H "Authorization: Bearer $TOKEN"
```

## 7. Conferir no MongoDB Atlas

Atlas → **Database → Clusters → Browse Collections → fidelis**: as coleções `registros_diario` e `logs_auditoria` devem ter documentos.

## Problemas comuns

| Sintoma | Solução |
| --- | --- |
| `curl: (7) Failed to connect` | A API não está rodando, ou está em outra porta. Veja o terminal do passo 5 |
| `/health/ready` com `mongodb` Unhealthy | Atlas → Network Access: libere seu IP ou `0.0.0.0/0`. Confira usuário e senha na connection string |
| `/health/ready` com `oracle-database` Unhealthy | Credenciais do Oracle erradas em `appsettings.Development.json` ou sem acesso à rede da FIAP |
| `401 Unauthorized` | Token vazio ou expirado: refaça o login |
| `./testar-api.sh: Permission denied` | Rode `bash testar-api.sh` |
| `400` ao cadastrar tutor | Email já usado: troque o email |
