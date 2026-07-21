import { apiRequest } from "./api";

export const checkinService = {
    // --- VALIDA TOKEN DE CHECKIN E REGISTRA LOG DE PORTARIA ---
    validar(token) {
        return apiRequest("/checkin/validar", {
            method: "POST",
            body: JSON.stringify({ token })
        });
    }
};
