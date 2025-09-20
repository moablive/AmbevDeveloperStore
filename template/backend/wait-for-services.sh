#!/bin/sh
# wait-for-services.sh

# O "$@" representa o comando principal da aplicação
CMD="$@"

# Função para esperar por um serviço com timeout
wait_for() {
  HOST=$1
  PORT=$2
  NAME=$3
  echo "Aguardando $NAME ($HOST:$PORT)..."
  for i in $(seq 1 60); do
    if nc -z "$HOST" "$PORT" >/dev/null 2>&1; then
      echo "$NAME está disponível!"
      return 0
    fi
    sleep 2
  done
  echo "Timeout: $NAME ($HOST:$PORT) não respondeu"
  exit 1
}

# Espera pelos serviços necessários
wait_for "ambev.developerevaluation.database" "5432" "PostgreSQL"
wait_for "ambev.developerevaluation.nosql" "27017" "MongoDB"
wait_for "kafka" "29092" "Kafka"

# Executa o comando principal
echo "Todos os serviços estão prontos. Iniciando aplicação..."
exec $CMD
