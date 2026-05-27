# SGI - Sistema de Gestão de Ingressos

Sistema de vendas de ingressos para eventos ao vivo (shows internacionais, nacionais, teatro, comédia, etc.), inspirado em plataformas como Eventim, Ticketmaster e Livepass.

## Tecnologias

### Backend
- **C# / .NET 8** (ASP.NET Core Web API)
- **Entity Framework Core** (SQL Server)
- **Autenticação JWT**
- **QRCoder** (geração de QR Codes)
- **Swagger/OpenAPI**

### Frontend
- **React 18** (Vite)
- **React Router**
- **Axios**

## Estrutura do Projeto

```
├── Backend/
│   └── src/
│       ├── dto/                  # DTOs de entrada/saída
│       ├── entity/               # Entidades EF Core
│       ├── exception/            # Tratamento global de erros
│       ├── infra/
│       │   ├── config/           # DbContext + DataSeeder
│       │   ├── pagination/       # Utilitários de paginação
│       │   └── security/         # JWT Service
│       └── modules/
│           ├── autenticacao/     # Login e registro
│           ├── usuario/          # Gestão de usuários
│           ├── evento/           # CRUD de eventos
│           ├── ingresso/         # Ingressos e validação
│           ├── pedido/           # Carrinho e pedidos
│           ├── pagamento/        # Pagamento fictício (Cartão/PIX)
│           └── relatorio/        # Dashboard administrativo
├── frontend/
│   └── src/
│       ├── components/           # Componentes reutilizáveis
│       ├── pages/                # Páginas da aplicação
│       ├── services/             # Chamadas à API
│       └── styles/               # CSS global
└── README.md
```

## Como Executar

### Backend

```bash
cd Backend
dotnet restore
dotnet run
```

A API estará disponível em `https://localhost:5001` com Swagger em `/swagger`.

### Frontend

```bash
cd frontend
npm install
npm run dev
```

O frontend estará disponível em `http://localhost:5173`.

## Funcionalidades

- **Autenticação**: Login/Registro com JWT
- **Eventos**: Listagem, detalhes, CRUD administrativo
- **Ingressos**: Seleção por setor, QR Code gerado após pagamento
- **Carrinho/Pedidos**: Criação de pedido com múltiplos ingressos
- **Pagamento Fictício**:
  - Cartão: aprovado se último dígito for par
  - PIX: gera código aleatório e aprova automaticamente
- **Dashboard Admin**: Relatórios de vendas, total de receita

## Credenciais Padrão (Seed)

- **Admin**: admin@sgi.com / admin123

Sistema de Gestão de Eventos
