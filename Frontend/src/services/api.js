const API_BASE_URL = 'http://localhost:5000/api';

export async function apiRequest(path, options = {}) {
    const token = localStorage.getItem('token');

    const response = await fetch(`${API_BASE_URL}${path}`, {
        headers: {
        'Content-Type': 'application/json',
        ...(token ? { Authorization: `Bearer ${token}` } : {}),
        ...(options.headers || {})
        },
        ...options
    });

    if (!response.ok) {
        const payload = await response.json().catch(() => ({}));
        throw new Error(payload.message || 'Erro na requisição');
    }

    return response.status === 204 ? null : response.json();
}
