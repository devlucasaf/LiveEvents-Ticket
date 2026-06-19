<img 
    width=100% 
    src="https://capsule-render.vercel.app/api?type=waving&color=A020F0&height=120&section=header"
/>

<p align="center">
    <img 
        src="https://img.shields.io/badge/m%C3%B3dulo-pdv-A020F0?style=for-the-badge" 
    />
    <img 
        src="https://img.shields.io/badge/status-em%20progresso-yellow?style=for-the-badge" 
    />
    <img 
        src="https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" 
    />
    <img 
        src="https://img.shields.io/badge/HTML-5-E34F26?style=for-the-badge&logo=html5&logoColor=white" 
    />
    <img 
        src="https://img.shields.io/badge/CSS-3-1572B6?style=for-the-badge&logo=css3&logoColor=white" 
    />
    <img 
        src="https://img.shields.io/badge/JavaScript-vanilla-F7DF1E?style=for-the-badge&logo=javascript&logoColor=black" 
    />
    <img 
        src="https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white" 
    />
    <img 
        src="https://img.shields.io/badge/license-MIT-c62828?style=for-the-badge" 
    />
</p>

<br/>

> **LiveEvents-Ticket — PDV** é o módulo de bilheteria presencial do sistema, responsável pela venda de ingressos no balcão do evento: login do atendente, mapa de assentos por evento, registro da venda com método de pagamento (Dinheiro, Cartão, PIX) e relatório local com comprovante imprimível.
>
> 👉 Para a venda online B2C, consulte o módulo [`web`](../web).

---

## 📋 Sobre o Módulo

O módulo **PDV (Ponto de Venda)** atende o cenário de bilheteria física: um atendente loga no sistema, escolhe o evento, seleciona um assento disponível no mapa, define o método de pagamento e registra a venda. O foco é em rapidez de operação e simplicidade — por isso o frontend é **HTML + CSS + JavaScript puro**, sem build step nem framework, podendo rodar em qualquer máquina com um servidor estático simples.

---

## 🏗️ Arquitetura

Arquitetura **cliente-servidor (Client-Server)** com separação clara entre frontend e backend:

- **Frontend:** HTML 5 + CSS 3 + JavaScript ES2022 vanilla, sem build, com `fetch` direto na API
- **Backend:** API REST com ASP.NET Core 8, Entity Framework Core, arquitetura modular
- **Banco de Dados:** SQL Server (`PontoVendaDb`) com Integrated Security
- **Segurança:** JWT para autenticação do atendente, BCrypt para hash de senhas, CORS aberto
- **Padrão DTO:** Separação completa entre entidades e objetos de transferência
- **Seed automático:** operador `admin` + evento de exemplo com 130 assentos criados na primeira execução

---

## 🗂️ Estrutura do Módulo

```
📂 pdv
├── 📂 backend
│   ├── 📂 src
│   │   ├── 📂 entity                    # Entidade base (AuditEntity)
│   │   ├── 📂 exception                 # Tratamento global de erros
│   │   ├── 📂 infra
│   │   │   ├── 📂 config                # AppDbContext + DataSeeder
│   │   │   └── 📂 security              # JWT, CurrentUserService
│   │   └── 📂 modules
│   │       └── 📂 ponto-venda
│   │           ├── 📂 model             # Evento, Assento, VendaFisica, Operador, Produto
│   │           ├── 📂 dto               # CriarVendaFisicaDto, EventoDtos, LoginDto, etc.
│   │           ├── 📂 repository        # Interfaces + implementações EF Core
│   │           ├── 📂 service           # Regras de venda física, login, eventos
│   │           └── 📂 rest              # AuthController, EventoController, VendaFisicaController
│   ├── 📄 Program.cs
│   ├── 📄 appsettings.json
│   └── 📄 PontoVenda.Backend.csproj
└── 📂 frontend
    ├── 📄 index.html                    # Login do atendente (raiz)
    ├── 📂 pages
    │   ├── 📄 venda.html                # Registrar venda + mapa de assentos
    │   └── 📄 relatorio.html            # Relatório local com totais
    ├── 📂 styles
    │   └── 📄 styles.css                # Estilos globais
    └── 📂 scripts
        ├── 📄 api.js                    # Cliente fetch + injeção de JWT
        ├── 📄 auth.js                   # Sessão em localStorage + Rotas
        ├── 📄 login.js                  # Lógica da tela de login
        ├── 📄 venda.js                  # Lógica de venda + comprovante
        └── 📄 relatorio.js              # Lógica do relatório
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
        alt="JavaScript" 
        title="JavaScript" 
        width="40px" 
        style="padding: 5px;" 
        src="https://skillicons.dev/icons?i=javascript" 
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
- [Python 3](https://www.python.org/) ou Live Server (para servir o frontend estático)
- [SQL Server](https://www.microsoft.com/sql-server) rodando localmente

### 1. Clone o repositório

```bash
git clone https://github.com/devlucasaf/LiveEvents-Ticket.git
cd LiveEvents-Ticket
```

### 2. Configure o Backend PDV

```bash
cd pdv/backend
```

Verifique o `appsettings.json` e ajuste a connection string se necessário:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=PontoVendaDb;Integrated Security=True;TrustServerCertificate=True;"
  }
}
```

Inicie o servidor:

```bash
dotnet run
```

> ✅ API disponível em `http://localhost:5100`  
> ✅ Swagger disponível em `http://localhost:5100/swagger`

> 💡 Na primeira execução, o banco `PontoVendaDb` é criado automaticamente e populado com o operador **admin** (senha `Admin@123`) e um evento de exemplo com 130 assentos.

### 3. Configure o Frontend PDV

```bash
cd ../frontend
python -m http.server 8080
# ou: npx serve .
# ou: Live Server (extensão do VS Code)
```

> ✅ Aplicação disponível em `http://localhost:8080/index.html`  
> ⚠️ **Não abra os HTMLs com `file://`** — o navegador bloqueia `fetch`. Sempre use um servidor estático.

### 4. Login inicial

| Campo | Valor |
|-------|-------|
| Login | `admin` |
| Senha | `Admin@123` |

---

## ✨ Funcionalidades

| Feature | Descrição |
|---------|-----------|
| 👤 **Login do atendente** | Autenticação JWT armazenada em `localStorage` |
| 🎫 **Mapa de assentos** | Visualização por setor → fileira, com cores por status (DISPONIVEL / VENDIDO / RESERVADO) |
| 💵 **Métodos de pagamento** | Dinheiro, Cartão e PIX |
| 🧾 **Comprovante imprimível** | Modal de confirmação com `@media print` para impressão direta |
| 📊 **Relatório local** | Totais (vendas, faturamento, ticket médio), filtros e tabela detalhada |
| 🔌 **Fallback offline** | Se o backend cair, o relatório mostra o histórico salvo no `localStorage` |
| 🔐 **Sessão protegida** | Redirect automático para login em 401 |
| 🧭 **Rotas adaptáveis** | Scripts detectam se estão na raiz ou em `/pages/` e ajustam paths sozinhos |

---

## 🔌 Endpoints Consumidos

| Método | URL | Uso |
|--------|-----|-----|
| POST | `/api/auth/login` | Login do atendente |
| GET | `/api/eventos` | Lista eventos ativos para o `<select>` |
| GET | `/api/eventos/{id}/assentos` | Mapa de assentos do evento |
| POST | `/api/vendas-fisicas` | Registra a venda |
| GET | `/api/vendas-fisicas` | Lista vendas no relatório |

---

## 🔗 Outros módulos

| Módulo | Descrição |
|--------|-----------|
| [`web`](../web) | Loja online B2C — React 19 + Vite 8 + ASP.NET Core 8 com checkout PIX/cartão, favoritos, tema claro/escuro |

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
