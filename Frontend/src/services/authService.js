import { apiRequest } from './api';

export const authService = {
    async login(payload) {
        const data = await apiRequest('/usuario/login', {
            method: 'POST',
            body: JSON.stringify(payload)
        });

        localStorage.setItem('token', data.token);
        localStorage.setItem('usuario', JSON.stringify(data.usuario));
        return data;
    },

    async cadastro(payload) {
        return apiRequest('/usuario/registrar', {
            method: 'POST',
            body: JSON.stringify(payload)
        });
    },

    logout() {
        localStorage.removeItem('token');
        localStorage.removeItem('usuario');
    },

    async atualizarPerfil(payload) {
        const data = await apiRequest('/usuario/me', {
            method: 'PUT',
            body: JSON.stringify(payload)
        });
        localStorage.setItem('usuario', JSON.stringify(data));
        return data;
    },

    async buscarPerfil() {
        const data = await apiRequest('/usuario/me');
        localStorage.setItem('usuario', JSON.stringify(data));
        return data;
    }
};
