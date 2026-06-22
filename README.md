<img 
    width=100% 
    src="https://capsule-render.vercel.app/api?type=waving&color=A020F0&height=120&section=header"
/>

<p align="center">
    <img 
        src="https://img.shields.io/badge/status-em%20progresso-yellow?style=for-the-badge" 
    />
    <img 
        src="https://img.shields.io/badge/arquitetura-modular-A020F0?style=for-the-badge" 
    />
    <img 
        src="https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" 
    />
    <img 
        src="https://img.shields.io/badge/React-19.x-61DAFB?style=for-the-badge&logo=react&logoColor=black" 
    />
    <img 
        src="https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white" 
    />
    <img 
        src="https://img.shields.io/badge/license-MIT-c62828?style=for-the-badge" 
    />
</p>

<br/>

> **LiveEvents-Ticket** é uma plataforma completa de venda de ingressos para eventos ao vivo — shows, festivais, teatro e esportes — com dois canais de venda independentes:
> uma loja online (**web**) para o público final e um sistema de bilheteria (**pdv**) para atendimento presencial.

---

## 🧭 Visão Geral

O projeto está dividido em dois módulos autocontidos, cada um com seu próprio backend, banco de dados e frontend. Isso permite que os times trabalhem de forma independente, que cada lado seja deployado/escalado isoladamente, e mantém o domínio de cada canal de venda muito claro.

| Módulo | Stack | Banco | Porta API | Porta UI | Quando usar |
|--------|-------|-------|-----------|----------|-------------|
| 🌐 [`web`](./web) | ASP.NET Core 8 + React 19 + Vite 8 | `LiveEventsTicketDb` | `5000` | `5173` | Vendas online B2C — usuário compra do site/app, faz checkout com PIX/cartão |
| 🎟️ [`pdv`](./pdv) | ASP.NET Core 8 + HTML/CSS/JS puro | `PontoVendaDb` | `5100` | `8080` | Bilheteria presencial — atendente seleciona evento, assento e registra a venda no balcão |

---

## 🗂️ Estrutura do Repositório

```
📂 LiveEvents-Ticket
├── 📂 web                          # MÓDULO WEB — venda online B2C
│   ├── 📂 backend                  # ASP.NET Core 8 + EF Core + JWT
│   ├── 📂 frontend                 # React 19 + Vite 8 + CSS modular
│   └── 📄 README.md
│
├── 📂 pdv                          # MÓDULO PDV — bilheteria presencial
│   ├── 📂 backend                  # ASP.NET Core 8 + EF Core + JWT
│   ├── 📂 frontend                 # HTML + CSS + JS puro (sem build)
│   └── 📄 README.md
│
├── 📄 LICENSE
└── 📄 README.md                    # Este arquivo
```

---

## 🏛️ Arquitetura

Ambos os módulos seguem o mesmo padrão arquitetural no backend, o que facilita manutenção e onboarding:

```
backend/
└── src/
    ├── entity/         # AuditEntity (CreatedAt, UpdatedAt)
    ├── exception/      # GlobalExceptionMiddleware + ErrorResponse
    ├── infra/
    │   ├── config/     # AppDbContext + DataSeeder
    │   └── security/   # JwtTokenService + CurrentUserService
    └── modules/
        └── <dominio>/
            ├── model/        # Entidades EF Core
            ├── dto/          # Objetos de transferência
            ├── repository/   # Acesso a dados (interface + impl)
            ├── service/      # Regras de negócio
            └── rest/         # Controllers ASP.NET
```

### Princípios
- **Modular por domínio**: cada feature (`evento`, `pedido`, `ponto-venda`...) tem seu próprio módulo com todas as camadas
- **DTOs sempre**: entidades nunca são expostas pela API
- **Cancelamento cooperativo**: todos os métodos async recebem `CancellationToken`
- **Tratamento centralizado de erros**: `GlobalExceptionMiddleware` traduz exceções (`KeyNotFoundException` → 404, `InvalidOperationException` → 400, etc.)
- **Auditoria automática**: `CreatedAt`/`UpdatedAt` herdados de `AuditEntity`

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

### Bibliotecas principais

| Categoria | Tecnologia |
|-----------|------------|
| API | ASP.NET Core 8 Web API |
| ORM | Entity Framework Core 8 |
| Banco | SQL Server (LocalDB ou instância completa) |
| Autenticação | JWT Bearer + BCrypt.Net-Next |
| Documentação | Swashbuckle (Swagger UI) |
| Frontend Web | React 19, Vite 8, React Router |
| Frontend PDV | HTML 5, CSS 3, JavaScript ES2022 (vanilla) |

---

## 🚀 Como Rodar Localmente

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/) (apenas para o módulo `web`)
- [Python 3](https://www.python.org/) ou Live Server (apenas para o módulo `pdv`)
- [SQL Server](https://www.microsoft.com/sql-server) (LocalDB serve)

### Clone

```bash
git clone https://github.com/devlucasaf/LiveEvents-Ticket.git
cd LiveEvents-Ticket
```

### Subindo o módulo Web

```bash
# Terminal 1 — Backend Web (porta 5000)
cd web/backend
dotnet run

# Terminal 2 — Frontend Web (porta 5173)
cd web/frontend
npm install
npm run dev
```

📖 Detalhes completos em [`web/README.md`](./web/README.md).

### Subindo o módulo PDV

```bash
# Terminal 1 — Backend PDV (porta 5100)
cd pdv/backend
dotnet run

# Terminal 2 — Frontend PDV (porta 8080)
cd pdv/frontend
python -m http.server 8080
# Acesse: http://localhost:8080/index.html
# Login: admin / Admin@123
```

📖 Detalhes completos em [`pdv/frontend/README.md`](./pdv/frontend/README.md).

> ⚠️ Os dois bancos (`LiveEventsTicketDb` e `PontoVendaDb`) são criados automaticamente pelo `EnsureCreatedAsync` no primeiro `dotnet run` de cada backend.

---

## ✨ Funcionalidades por Módulo

### 🌐 Web — Venda Online

| Feature | Descrição |
|---------|-----------|
| 🎫 Eventos | Criação, listagem e busca com imagens |
| 🎟️ Ingressos | Setores com controle de estoque |
| 🛒 Pedidos | Checkout com snapshot da compra e confirmação |
| 💳 Pagamentos | PIX (com QR Code) e cartão de crédito |
| 📊 Relatórios | Dashboard administrativo |
| 🔐 Auth | JWT + roles USER/ADMIN, redirect automático em 401 |
| 🌙 Tema | Alternância claro/escuro |
| 💾 Favoritos | Eventos salvos via localStorage |

### 🎟️ PDV — Bilheteria Presencial

| Feature | Descrição |
|---------|-----------|
| 👤 Login do atendente | JWT em localStorage |
| 🪑 Mapa de assentos | Visualização por setor/fileira com status colorido |
| 💵 Métodos de pagamento | Dinheiro, Cartão e PIX |
| 🧾 Comprovante | Modal imprimível (CSS `@media print`) |
| 📈 Relatório local | Totais, ticket médio, filtros e fallback offline |

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

Este projeto está sob a licença **MIT**. Consulte o arquivo [LICENSE](./LICENSE) para mais detalhes.

<img 
    width=100% 
    src="https://capsule-render.vercel.app/api?type=waving&color=A020F0&height=120&section=footer"
/>
