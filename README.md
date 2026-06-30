# LoginUser 🔐

API de autenticação desenvolvida com ASP.NET Core (.NET 10), com foco em autenticação segura utilizando JWT, armazenamento protegido de senhas e fluxo de recuperação de senha por e-mail.

## Funcionalidades

* Cadastro de usuários
* Login com autenticação JWT
* Hash de senha utilizando BCrypt
* Recuperação de senha por código enviado por e-mail
* Reset de senha com expiração do código
* Documentação e testes via Swagger
* Persistência com Entity Framework Core + SQL Server

---

## Tecnologias utilizadas

* ASP.NET Core (.NET 10)
* Entity Framework Core
* SQL Server
* JWT Authentication
* BCrypt
* Swagger / OpenAPI
* SMTP (envio de e-mail)

---

## Estrutura do projeto

```text
LoginUser
│
├── Controllers
│   └── AuthController.cs
│
├── Data
│   └── AppDbContext.cs
│
├── Models
│
├── Services
│   └── EmailService.cs
│
├── Program.cs
├── appsettings.json
└── README.md
```

---

## Fluxo de autenticação

### Registro

```http
POST /api/auth/register
```

Exemplo:

```json
{
  "email": "usuario@email.com",
  "password": "Senha123"
}
```

---

### Login

```http
POST /api/auth/login
```

Exemplo:

```json
{
  "email": "usuario@email.com",
  "password": "Senha123"
}
```

Resposta:

```json
{
  "token": "jwt_token"
}
```

---

### Solicitar recuperação de senha

```http
POST /api/auth/request-password-reset
```

Exemplo:

```json
{
  "email": "usuario@email.com"
}
```

---

### Confirmar recuperação

```http
POST /api/auth/reset-password
```

Exemplo:

```json
{
  "email": "usuario@email.com",
  "code": "123456",
  "newPassword": "NovaSenha123"
}
```

---

## Como executar localmente

### 1. Clonar o projeto

```bash
git clone <URL_DO_REPOSITORIO>
```

### 2. Restaurar dependências

```bash
dotnet restore
```

### 3. Configurar o appsettings.json

Exemplo:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "SUA_CONNECTION_STRING"
  },

  "Jwt": {
    "Key": "SUA_CHAVE_JWT",
    "Issuer": "LoginUser",
    "Audience": "LoginUserClient"
  },

  "Email": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "Username": "EMAIL",
    "Password": "APP_PASSWORD"
  }
}
```

### 4. Executar migrations

```bash
dotnet ef database update
```

### 5. Iniciar aplicação

```bash
dotnet run
```

---

## Documentação

Com a aplicação rodando:

```text
/swagger
```

---

## Segurança

* Senhas armazenadas utilizando hash BCrypt
* Tokens JWT com expiração
* Chaves e credenciais devem permanecer fora do repositório
* Recuperação de senha com código temporário

---

## Objetivo do projeto

Este projeto foi desenvolvido com foco em estudo e prática de desenvolvimento back-end utilizando autenticação moderna, boas práticas de segurança e construção de APIs REST.

Sugestões e melhorias são bem-vindas.
