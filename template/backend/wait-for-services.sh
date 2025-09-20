#!/bin/sh
# wait-for-services.sh

# O "$@" representa o comando principal da aplica��o
CMD="$@"

# Fun��o para esperar por um servi�o com timeout
wait_for() {
  HOST=$1
  PORT=$2
  NAME=$3
  echo "Aguardando $NAME ($HOST:$PORT)..."
  for i in $(seq 1 60); do
    if nc -z "$HOST" "$PORT" >/dev/null 2>&1; then
      echo "$NAME est� dispon�vel!"
      return 0
    fi
    sleep 2
  done
  echo "Timeout: $NAME ($HOST:$PORT) n�o respondeu"
  exit 1
}

# Espera pelos servi�os necess�rios
wait_for "ambev.developerevaluation.database" "5432" "PostgreSQL"
wait_for "ambev.developerevaluation.nosql" "27017" "MongoDB"
wait_for "kafka" "29092" "Kafka"

# Executa o comando principal
echo "Todos os servi�os est�o prontos. Iniciando aplica��o..."
exec $CMD
