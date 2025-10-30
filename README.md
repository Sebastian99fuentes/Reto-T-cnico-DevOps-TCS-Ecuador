COMANDOS
1 docker compose up --build -d  
2 docker compose up --scale api=2  para el nodo extra o los que se quiera


1. Obtener un Token JWT

Para poder hacer requests autenticadas al API Gateway, primero necesitas generar un token JWT.

URL:

GET http://localhost:8010/DevOpsToken


Descripción:
Este endpoint no requiere autenticación. Devuelve un token que debe ser usado en el header X-JWT-KWY para las solicitudes protegidas.

Ejemplo de respuesta:

{
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJkZXZvcHNAZ21haWwuY29tIiwianRpIjoiZTZmM2I5OTUtY2Q1...",
    "message": "Use this token in X-JWT-KWY header"
}


Guarda el valor de "token", lo usarás en el siguiente paso.

2. Enviar un mensaje (POST)

Una vez que tengas tu token JWT, puedes enviar un mensaje a través del Gateway.

URL:

POST http://localhost:8010/DevOps


Headers requeridos:

X-Parse-REST-API-Key: 2f5ae96c-b558-4c7b-a590-a501ae1c3f6c
X-JWT-KWY: <TOKEN_OBTENIDO_EN_EL_PASO_1>
Content-Type: application/json


Body (raw JSON):

{
  "message": "This is a test",
  "to": "Sebastian Fuentes", 
  "from": "Rita Asturia",
  "timeToLifeSec": 45
}


Ejemplo de respuesta exitosa:

{
    "message": "Hello Sebastian Fuentes your message will be send"
} 

Se probo con POSTMAN y funciona correctamente.

🔧 Tecnologías Utilizadas
.NET 8.0 - Runtime principal

Docker & Docker Compose - Containerización

Ocelot - API Gateway & Load Balancer

JWT - Autenticación

xUnit - Testing framework

Postman - Pruebas de integración

