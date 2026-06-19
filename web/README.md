<img 
    width=100% 
    src="https://capsule-render.vercel.app/api?type=waving&color=A020F0&height=120&section=header"
/>

<p align="center">
    <img 
        src="https://img.shields.io/badge/m%C3%B3dulo-web-A020F0?style=for-the-badge" 
    />
    <img 
        src="https://img.shields.io/badge/status-em%20progresso-yellow?style=for-the-badge" 
    />
    <img 
        src="https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" 
    />
    <img 
        src="https://img.shields.io/badge/React-19.x-61DAFB?style=for-the-badge&logo=react&logoColor=black" 
    />
    <img 
        src="https://img.shields.io/badge/Vite-8.x-646CFF?style=for-the-badge&logo=vite&logoColor=white" 
    />
    <img 
        src="https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white" 
    />
    <img 
        src="https://img.shields.io/badge/license-MIT-c62828?style=for-the-badge" 
    />
</p>

<br/>

> **LiveEvents-Ticket — Web** é o módulo online do sistema, responsável por toda a venda B2C de ingressos pela internet: criação de eventos, gestão de ingressos por setor, checkout com pagamento integrado e painel administrativo com relatórios de vendas.
>
> 👉 Para a bilheteria presencial, consulte o módulo [`pdv`](../pdv).

---

## 📋 Sobre o Módulo

O módulo **Web** foi desenvolvido para oferecer uma experiência moderna de compra de ingressos online, inspirada em plataformas como Eventim e Ticketmaster. Atende tanto **usuários finais** (busca, compra, favoritos, meus ingressos) quanto **administradores** (criação de eventos, gestão de ingressos, relatórios).

---

## 🏗️ Arquitetura

Arquitetura **cliente-servidor (Client-Server)** com separação clara entre frontend e backend:

- **Frontend:** React 19 + Vite 8, SPA com roteamento client-side e tema claro/escuro
- **Backend:** API REST com ASP.NET Core 8, Entity Framework Core, arquitetura modular
- **Banco de Dados:** SQL Server (`LiveEventsTicketDb`) com Integrated Security
- **Segurança:** JWT para autenticação, BCrypt para hash de senhas, CORS configurado
- **Padrão DTO:** Separação completa entre entidades e objetos de transferência

---

## 🗂️ Estrutura do Módulo

```
📂 web
├── 📂 backend
│   ├── 📂 src
│   │   ├── 📂 entity                    # Entidade base (AuditEntity)
│   │   ├── 📂 exception                 # Tratamento global de erros
│   │   ├── 📂 infra
│   │   │   ├── 📂 config                # AppDbContext + DataSeeder
│   │   │   ├── 📂 pagination            # PagedResult genérico
│   │   │   └── 📂 security              # JWT, CurrentUserService
│   │   └── 📂 modules
│   │       ├── 📂 evento                # CRUD de eventos
│   │       ├── 📂 ingresso              # Gestão de ingressos por setor
│   │       ├── 📂 pagamento             # Processamento de pagamentos
│   │       ├── 📂 pedido                # Pedidos de compra
│   │       ├── 📂 relatorio             # Relatórios de vendas
│   │       └── 📂 usuario               # Autenticação e perfil
│   ├── 📄 Program.cs
│   ├── 📄 appsettings.json
│   └── 📄 LiveEvents-Ticket.Backend.csproj
└── 📂 frontend
    ├── 📂 src
    │   ├── 📂 components                # Navbar, CardEvento, DatePicker
    │   ├── 📂 pages
    │   │   ├── 📂 admin                 # Dashboard administrativo
    │   │   ├── 📂 auth                  # Login, Cadastro, Recuperar Senha
    │   │   ├── 📂 busca                 # Busca de eventos com filtros
    │   │   ├── 📂 evento                # Listagem + Detalhe do evento
    │   │   ├── 📂 ingresso              # Seleção de assentos
    │   │   ├── 📂 meus-eventos          # Meus ingressos (Em breve / Passados)
    │   │   ├── 📂 pedido                # Checkout e pagamento
    │   │   ├── 📂 salvos                # Eventos salvos (favoritos)
    │   │   └── 📂 usuario               # Perfil do usuário
    │   ├── 📂 services                  # api.js, authService, eventoService, pedidoService
    │   └── 📂 styles                    # CSS modular por página
    └── 📄 package.json
```

---

## 🛠️ Tecnologias

<div align="center">
    <img 
        alt="C#" 
        title="C#" 
        width="40px" 
        style="padding: 5px;" 
        src="https://skillicons.dev/icons?i=cs" 
    />
    <img 
        alt=".NET" 
        title=".NET" 
        width="40px" 
        style="padding: 5px;" 
        src="https://skillicons.dev/icons?i=dotnet" 
    />
    <img 
        alt="JavaScript" 
        title="JavaScript" 
        width="40px" 
        style="padding: 5px;" 
        src="https://skillicons.dev/icons?i=javascript" 
    />
    <img 
        alt="React" 
        title="React" 
        width="40px" 
        style="padding: 5px;" 
        src="https://cdn.jsdelivr.net/gh/devicons/devicon@latest/icons/react/react-original.svg" 
    />
    <img 
        alt="Vite" 
        title="Vite" 
        width="40px" 
        style="padding: 5px;" 
        src="https://skillicons.dev/icons?i=vite" 
    />
    <img 
        alt="HTML" 
        title="HTML" 
        width="40px" 
        style="padding: 5px;" 
        src="https://skillicons.dev/icons?i=html" 
    />
    <img 
        alt="CSS" 
        title="CSS" 
        width="40px" 
        style="padding: 5px;" 
        src="https://skillicons.dev/icons?i=css" 
    />
    <img 
        alt="SQL Server" 
        title="SQL Server" 
        width="40px" 
        style="padding: 5px;" 
        src="https://cdn.jsdelivr.net/gh/devicons/devicon@latest/icons/microsoftsqlserver/microsoftsqlserver-original.svg" 
    />
    <img 
        alt="Git" 
        title="Git" 
        width="40px" 
        style="padding: 5px;" 
        src="https://skillicons.dev/icons?i=git" 
    />
    <img 
        alt="GitHub" 
        title="GitHub" 
        width="40px" 
        style="padding: 5px;" 
        src="https://skillicons.dev/icons?i=github" 
    />
    <img 
        alt="VS Code" 
        title="VS Code" 
        width="40px" 
        style="padding: 5px;" 
        src="https://skillicons.dev/icons?i=vscode" 
    />
</div>

---

## 🚀 Como Rodar Localmente

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/)
- [SQL Server](https://www.microsoft.com/sql-server) rodando localmente

### 1. Clone o repositório

```bash
git clone https://github.com/devlucasaf/LiveEvents-Ticket.git
cd LiveEvents-Ticket
```

### 2. Configure o Backend Web

```bash
cd web/backend
```

Verifique o `appsettings.json` e ajuste a connection string se necessário:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=LiveEventsTicketDb;Integrated Security=True;TrustServerCertificate=True;"
  }
}
```

Inicie o servidor:

```bash
dotnet run
```

> ✅ API disponível em `http://localhost:5000`

### 3. Configure o Frontend Web

```bash
cd ../frontend
npm install
npm run dev
```

> ✅ Aplicação disponível em `http://localhost:5173`

---

## ✨ Funcionalidades

| Módulo | Descrição |
|--------|-----------|
| 🎫 **Eventos** | Criação, listagem e busca de eventos com imagens |
| 🎟️ **Ingressos** | Gestão de ingressos por setor com controle de estoque |
| 🛒 **Pedidos** | Fluxo completo de compra com checkout |
| 💳 **Pagamentos** | Processamento de pagamentos integrado (PIX, cartão) |
| 📊 **Relatórios** | Dashboard com relatórios de vendas para admin |
| 🔐 **Autenticação** | Login/Cadastro com JWT e controle de roles (USER/ADMIN) |
| 🔍 **Busca** | Filtros por título, categoria e localização |
| 💾 **Favoritos** | Salvar eventos para ver depois (localStorage) |
| 🌙 **Tema** | Alternância entre tema claro e escuro |

---

## 🔗 Outros módulos

| Módulo | Descrição |
|--------|-----------|
| [`pdv`](../pdv) | Bilheteria física (PDV) — login do atendente, venda de assentos no balcão e relatório local |

---

## 👤 Desenvolvedor

<table>
    <tr>
        <td align="center">
            <a href="https://github.com/devlucasaf">
                <img 
                    src="https://github.com/devlucasaf.png" 
                    width="80px;" 
                    style="border-radius: 50%;" 
                    alt="Lucas Freitas"
                />
                <br/>
                <sub><b>Lucas Freitas</b></sub>
            </a><br/>
            <sub>Fullstack Developer</sub>
        </td>
    </tr>
</table>

---

## 🏆 Licença

Este projeto está sob a licença **MIT**. Consulte o arquivo [LICENSE](../LICENSE) para mais detalhes.

<img 
    width=100% 
    src="https://capsule-render.vercel.app/api?type=waving&color=A020F0&height=120&section=footer"
/>
