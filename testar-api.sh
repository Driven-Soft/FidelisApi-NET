#!/usr/bin/env bash

BASE_URL="${BASE_URL:-http://localhost:5232}"
SUFIXO="$(date +%s)"
EMAIL="teste${SUFIXO}@example.com"
SENHA="123456"
TOKEN=""
OK=0
FALHA=0

verde() { printf "\033[32m%s\033[0m\n" "$1"; }
vermelho() { printf "\033[31m%s\033[0m\n" "$1"; }
titulo() { printf "\n\033[1;36m=== %s ===\033[0m\n" "$1"; }

req() {
  local metodo="$1" rota="$2" corpo="${3:-}"
  local args=(-s -w "\n%{http_code}" -X "$metodo" "$BASE_URL$rota" -H "Content-Type: application/json")
  [ -n "$TOKEN" ] && args+=(-H "Authorization: Bearer $TOKEN")
  [ -n "$corpo" ] && args+=(-d "$corpo")
  local resposta
  resposta="$(curl "${args[@]}")"
  STATUS="$(printf "%s" "$resposta" | tail -n1)"
  BODY="$(printf "%s" "$resposta" | sed '$d')"
  echo "$metodo $rota -> HTTP $STATUS"
  echo "$BODY" | cut -c1-600
}

esperar() {
  if [ "$STATUS" = "$1" ]; then verde "OK (esperado $1)"; OK=$((OK+1)); else vermelho "FALHOU (esperado $1, recebido $STATUS)"; FALHA=$((FALHA+1)); fi
}

json_num() { echo "$BODY" | grep -o "\"$1\":[0-9]*" | head -1 | cut -d: -f2; }
json_str() { echo "$BODY" | grep -o "\"$1\":\"[^\"]*\"" | head -1 | cut -d'"' -f4; }

titulo "1. Health checks"
req GET /health/live; esperar 200
req GET /health/ready; esperar 200

titulo "2. Cadastro de tutor (publico)"
req POST /api/Tutor "{\"cpf\":\"${SUFIXO:0:11}\",\"nome\":\"Tutor Git Bash\",\"email\":\"$EMAIL\",\"senha\":\"$SENHA\",\"telefone\":\"11988476632\",\"endereco\":\"Rua Teste, 1\"}"
esperar 201

titulo "3. Acesso sem token"
req GET /api/Pet; esperar 401

titulo "4. Login JWT"
req POST /api/Auth/login "{\"email\":\"$EMAIL\",\"senha\":\"$SENHA\"}"
esperar 200
TOKEN="$(json_str token)"
TUTOR_ID="$(json_num tutorId)"
echo "tutorId=$TUTOR_ID"

titulo "5. Clinica"
req POST /api/Clinica "{\"nome\":\"Clinica Git Bash\",\"cnpj\":\"12345678000190\",\"telefone\":\"1133334444\",\"email\":\"clinica@example.com\",\"endereco\":\"Av. Paulista, 1000\"}"
esperar 201
CLINICA_ID="$(json_num id)"

titulo "6. Pets (Oracle)"
for nome in Rex Amora Thor; do
  req POST /api/Pet "{\"nome\":\"$nome\",\"especie\":\"Cachorro\",\"raca\":\"SRD\",\"sexo\":\"M\",\"dataNascimento\":\"2020-01-01T00:00:00\",\"fotoUrl\":\"https://example.com/$nome.jpg\",\"tutorId\":$TUTOR_ID,\"clinicaId\":$CLINICA_ID}"
  esperar 201
done
PET_ID="$(json_num id)"

titulo "7. Paginacao, ordenacao, filtros e HATEOAS"
req GET "/api/Pet?tutorId=$TUTOR_ID&page=1&pageSize=2&sortBy=nome&sortDirection=asc"
esperar 200
echo "$BODY" | grep -q '"rel":"next"' && verde "Link HATEOAS 'next' presente" || vermelho "Link 'next' ausente"

titulo "8. PATCH parcial"
req PATCH "/api/Pet/$PET_ID" '{"nome":"Thor Atualizado"}'
esperar 200

titulo "9. Validacoes (400)"
req GET "/api/Pet?pageSize=500"; esperar 400
req GET "/api/Pet?sortBy=campoInexistente"; esperar 400
req POST /api/Pet "{\"nome\":\"X\",\"especie\":\"Gato\",\"raca\":\"SRD\",\"sexo\":\"Z\",\"dataNascimento\":\"2020-01-01T00:00:00\",\"fotoUrl\":\"x.jpg\",\"tutorId\":$TUTOR_ID}"; esperar 400

titulo "10. Diario do pet (MongoDB Atlas)"
req POST /api/RegistroDiario "{\"petId\":$PET_ID,\"titulo\":\"Passeio no parque\",\"descricao\":\"Correu bastante.\",\"humor\":\"Feliz\",\"tags\":[\"passeio\",\"energia\"],\"dataRegistro\":\"2026-10-20T10:00:00Z\"}"
esperar 201
REGISTRO_ID="$(json_str id)"
echo "registroId=$REGISTRO_ID"
req GET "/api/RegistroDiario?petId=$PET_ID&tag=passeio"; esperar 200
req PATCH "/api/RegistroDiario/$REGISTRO_ID" '{"humor":"Calmo"}'; esperar 200
req POST /api/RegistroDiario "{\"petId\":$PET_ID,\"titulo\":\"Teste\",\"descricao\":\"Teste\",\"humor\":\"Radiante\",\"dataRegistro\":\"2026-10-20T10:00:00Z\"}"; esperar 400

titulo "11. Auditoria (MongoDB Atlas)"
sleep 2
req GET "/api/Auditoria?metodo=POST&pageSize=5"; esperar 200

titulo "12. DELETE"
req DELETE "/api/RegistroDiario/$REGISTRO_ID"; esperar 204
req GET "/api/RegistroDiario/$REGISTRO_ID"; esperar 404
req DELETE "/api/Pet/$PET_ID"; esperar 204

titulo "Resultado"
verde "Passaram: $OK"
[ "$FALHA" -gt 0 ] && vermelho "Falharam: $FALHA" || verde "Falharam: 0"
