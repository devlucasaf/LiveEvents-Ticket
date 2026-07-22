using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiveEventsTicket.Backend.Migrations.App
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Eventos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Categoria = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Local = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataEvento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImagemUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Eventos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ingressos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventoId = table.Column<int>(type: "int", nullable: false),
                    Setor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Preco = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    QuantidadeDisponivel = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ingressos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pagamentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PedidoId = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CodigoPix = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataPagamento = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagamentos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pedidos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    CompradorNome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompradorCpf = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompradorEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompradorTelefone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompradorDataNascimento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EnderecoCep = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EnderecoLogradouro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EnderecoNumero = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EnderecoComplemento = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnderecoBairro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EnderecoCidade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EnderecoEstado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QrCodeBase64 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CheckinToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CheckinUsosRealizados = table.Column<int>(type: "int", nullable: false),
                    CompartilhamentoToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompartilhamentoExpiraEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompartilhamentoRevogadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompartilhamentoMaxAcessos = table.Column<int>(type: "int", nullable: false),
                    CompartilhamentoAcessosRealizados = table.Column<int>(type: "int", nullable: false),
                    ReembolsoSolicitadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReembolsoAprovadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReembolsoMotivoCodigo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReembolsoMotivo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReembolsoRegraAplicada = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pedidos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sobrenome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Cpf = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cep = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Logradouro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Numero = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Complemento = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Bairro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cidade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SenhaHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataNascimento = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ItensPedido",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PedidoId = table.Column<int>(type: "int", nullable: false),
                    IngressoId = table.Column<int>(type: "int", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    PrecoUnitario = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Modalidade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Subtipo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DocumentosJson = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensPedido", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItensPedido_Pedidos_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "Pedidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PedidoCheckinLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PedidoId = table.Column<int>(type: "int", nullable: true),
                    OperadorId = table.Column<int>(type: "int", nullable: false),
                    TokenInformado = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Permitido = table.Column<bool>(type: "bit", nullable: false),
                    Mensagem = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DataCheckin = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidoCheckinLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PedidoCheckinLogs_Pedidos_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "Pedidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItensPedido_PedidoId",
                table: "ItensPedido",
                column: "PedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoCheckinLogs_PedidoId",
                table: "PedidoCheckinLogs",
                column: "PedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Eventos");

            migrationBuilder.DropTable(
                name: "Ingressos");

            migrationBuilder.DropTable(
                name: "ItensPedido");

            migrationBuilder.DropTable(
                name: "Pagamentos");

            migrationBuilder.DropTable(
                name: "PedidoCheckinLogs");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Pedidos");
        }
    }
}
