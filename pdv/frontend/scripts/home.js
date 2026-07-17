(function () {
    Auth.exigirLogin();

    // --- SAUDACAO COM O NOME DO OPERADOR LOGADO ---
    const hubNome = document.getElementById("hub-nome");
    if (hubNome) {
        const usuario = Auth.obterUsuario();
        const nomeCompleto = usuario?.nome || usuario?.login || "Atendente";
        hubNome.textContent = nomeCompleto.split(" ")[0];
    }
})();
