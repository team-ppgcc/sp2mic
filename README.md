
# Artigo: SP2Mic: Uma ferramenta para geração de código de microsserviços a partir de stored procedures

        Ingrid Guedes Teles Coutinho (UECE) <ingridteles@gmail.com>, 
        Paulo Henrique Mendes Maia (UECE) <pauloh.maia@uece.br>



# SP2Mic
A tool to migrate stored procedures to microservices.


# Initial Setup (sp2mic-frontend)
```
Install TypeScript
Install NodeJs
yarn install
yarn serve
```


# Initial Setup (sp2mic-backend)
```
Install Microsoft .NET Framework >= 4.6.1
Install PostgreSQL
Run script sp2mic-backend\db\ScriptCreateTablesSp2Mic.sql
Edit "ConnectionStrings" in file sp2mic-backend\sp2mic.WebAPI\appsettings.local.json
```
