import { Route, Routes }  from "react-router-dom";
import { CarrinhoProvider } from "./context/CarrinhoContext";
import Navbar             from "./components/Navbar";
import LoginPage          from "./pages/auth/LoginPage";
import CadastroPage       from "./pages/auth/CadastroPage";
import RecuperarSenhaPage from "./pages/auth/RecuperarSenhaPage";
import EventosPage        from "./pages/evento/EventosPage";
import EventoDetalhePage  from "./pages/evento/EventoDetalhePage";
import BuscaPage          from "./pages/busca/BuscaPage";
import MeusEventosPage    from "./pages/meus-eventos/MeusEventosPage";
import SalvosPage         from "./pages/salvos/SalvosPage";
import SelecaoAssentoPage from "./pages/ingresso/SelecaoAssentoPage";
import CarrinhoPage       from "./pages/carrinho/CarrinhoPage";
import CheckoutPage       from "./pages/pedido/CheckoutPage";
import PerfilPage         from "./pages/usuario/PerfilPage";
import DashboardAdminPage from "./pages/admin/DashboardAdminPage";
import CriarEventoPage    from "./pages/admin/CriarEventoPage";
import FuncionariosPage   from "./pages/admin/FuncionariosPage";

export default function App() {
  return (
    <CarrinhoProvider>
      <div className="layout">
        <Navbar />
        <main className="container">
          <Routes>
            <Route path="/"                     element={<EventosPage />} />
            <Route path="/auth/login"           element={<LoginPage />} />
            <Route path="/auth/cadastro"        element={<CadastroPage />} />
            <Route path="/auth/recuperar-senha" element={<RecuperarSenhaPage />} />
            <Route path="/evento/:id"           element={<EventoDetalhePage />} />
            <Route path="/busca"                element={<BuscaPage />} />
            <Route path="/meus-eventos"         element={<MeusEventosPage />} />
            <Route path="/salvos"               element={<SalvosPage />} />
            <Route path="/ingresso/selecao"     element={<SelecaoAssentoPage />} />
            <Route path="/carrinho"             element={<CarrinhoPage />} />
            <Route path="/pedido/checkout"      element={<CheckoutPage />} />
            <Route path="/usuario/perfil"       element={<PerfilPage />} />
            <Route path="/admin/dashboard"      element={<DashboardAdminPage />} />
            <Route path="/admin/criar-evento"   element={<CriarEventoPage />} />
            <Route path="/admin/funcionarios"   element={<FuncionariosPage />} />
          </Routes>
        </main>
      </div>
    </CarrinhoProvider>
  );
}