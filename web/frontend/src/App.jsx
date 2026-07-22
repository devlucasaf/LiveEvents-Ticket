import { Route, Routes }        from "react-router-dom";
import { CarrinhoProvider }     from "./context/CarrinhoContext";
import Navbar                   from "./components/Navbar";
import Footer                   from "./components/Footer";
import LoginPage                from "./pages/auth/LoginPage";
import CadastroPage             from "./pages/auth/CadastroPage";
import RecuperarSenhaPage       from "./pages/auth/RecuperarSenhaPage";
import EventosPage              from "./pages/evento/EventosPage";
import EventoDetalhePage        from "./pages/evento/EventoDetalhePage";
import BuscaPage                from "./pages/busca/BuscaPage";
import MeusEventosPage          from "./pages/meus-eventos/MeusEventosPage";
import SalvosPage               from "./pages/salvos/SalvosPage";
import SelecaoAssentoPage       from "./pages/ingresso/SelecaoAssentoPage";
import CarrinhoPage             from "./pages/carrinho/CarrinhoPage";
import CheckoutPage             from "./pages/pedido/CheckoutPage";
import PerfilPage               from "./pages/usuario/PerfilPage";
import DashboardAdminPage       from "./pages/admin/DashboardAdminPage";
import CriarEventoPage          from "./pages/admin/CriarEventoPage";
import FuncionariosPage         from "./pages/admin/FuncionariosPage";
import CheckinPortariaPage      from "./pages/admin/CheckinPortariaPage";
import FaqPage                  from "./pages/institucional/FaqPage";
import SobrePage                from "./pages/institucional/SobrePage";
import AcessibilidadePage       from "./pages/institucional/AcessibilidadePage";
import SuportePage              from "./pages/institucional/SuportePage";
import TermosPage               from "./pages/institucional/TermosPage";
import PoliticaCompraPage       from "./pages/institucional/PoliticaCompraPage";
import PoliticaPrivacidadePage  from "./pages/institucional/PoliticaPrivacidadePage";
import LicencaPage              from "./pages/institucional/LicencaPage";

export default function App() {
  return (
    <CarrinhoProvider>
      <div className="layout">
        <Navbar />
        <main className="container">
          <Routes>
            <Route path="/"                     	element={<EventosPage />} />
            <Route path="/auth/login"           	element={<LoginPage />} />
            <Route path="/auth/cadastro"        	element={<CadastroPage />} />
            <Route path="/auth/recuperar-senha" 	element={<RecuperarSenhaPage />} />
            <Route path="/evento/:id"           	element={<EventoDetalhePage />} />
            <Route path="/busca"                	element={<BuscaPage />} />
            <Route path="/meus-eventos"         	element={<MeusEventosPage />} />
            <Route path="/salvos"               	element={<SalvosPage />} />
            <Route path="/ingresso/selecao"     	element={<SelecaoAssentoPage />} />
            <Route path="/carrinho"             	element={<CarrinhoPage />} />
            <Route path="/pedido/checkout"      	element={<CheckoutPage />} />
            <Route path="/usuario/perfil"       	element={<PerfilPage />} />
            <Route path="/admin/dashboard"      	element={<DashboardAdminPage />} />
            <Route path="/admin/criar-evento"   	element={<CriarEventoPage />} />
            <Route path="/admin/funcionarios"   	element={<FuncionariosPage />} />
            <Route path="/admin/checkin"        	element={<CheckinPortariaPage />} />
            <Route path="/faq"                   	element={<FaqPage />} />
            <Route path="/sobre"                 	element={<SobrePage />} />
            <Route path="/acessibilidade"        	element={<AcessibilidadePage />} />
            <Route path="/suporte"               	element={<SuportePage />} />
            <Route path="/termos"                	element={<TermosPage />} />
            <Route path="/politica-compra"       	element={<PoliticaCompraPage />} />
            <Route path="/politica-privacidade"  	element={<PoliticaPrivacidadePage />} />
            <Route path="/licenca"               	element={<LicencaPage />} />
          </Routes>
        </main>
        <Footer />
      </div>
    </CarrinhoProvider>
  );
}