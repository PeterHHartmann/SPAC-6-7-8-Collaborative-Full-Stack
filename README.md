# SPAC - Uge 6-7 - Inventory-Management

## Installation
### Database
For at kunne køre programmet lokalt medfølger der et docker-compose schema der er til for at nemt kunne spinde en PostgreSQL server op.     
Både projektets docker-compose schema og .NET applikation er afhængig af environment variabler. 
Derfor er det nødvendigt at lave en ny fil navngivet '.env' som indeholder de nødvendige environment variabler. 
Der er tilføjet en fil kaldet '.env.example' som viser hvilke variabler er nødvendige samt hvor '.env' filen skal være placeret.

Når dette er gjort kan du starte en ny database op med følgende commando:
```sh
docker compose up -d
```
Projektets database burde nu køre og være klar til at modtage connections.

Derudover medfølger der også en pgadmin container der kan bruges som et database administartions værktøj. Du kan åbne de ved at besøge **[localhost:5433](http://localhost:5433)**. ***(email og password til at logge ind er defineret i '.env')***
