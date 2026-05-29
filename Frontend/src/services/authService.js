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
    }
};
