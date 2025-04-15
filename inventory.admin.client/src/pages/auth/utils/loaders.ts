import { redirect } from 'react-router-dom';
import AuthService from '@/pages/auth/services/AuthService';

export const authLoader = () => {
    if (!AuthService.userIsLoggedIn()) {
        return redirect("/login");
    }
    return null;
};

export const loginLoader = () => {
    if (AuthService.userIsLoggedIn()) {
        return redirect("/products");
    }
    return null;
};