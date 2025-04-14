import { useMutation } from '@tanstack/react-query';
import AuthService from '@/pages/auth/services/AuthService';
import type { Credentials } from '@/types';
import { useNavigate } from 'react-router-dom';

export const useLogin = () => {
    return useMutation({
        mutationFn: async (credentials: Credentials) =>
            AuthService.login(credentials),
    });
};

export const useLogout = () => {
    const navigate = useNavigate();
    return useMutation({
        mutationFn: async () =>
            AuthService.logout(),
        onSuccess: () => {
            navigate("/login");
        }
    });
};