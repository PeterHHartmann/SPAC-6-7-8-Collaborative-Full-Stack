import axiosClient from "@/api/axiosClient";
import { AuthEndpoints } from '@/api/endpoints';
import type { Credentials } from '@/types';
import Cookies from 'js-cookie';

export const SESSION_COOKIE_NAME = 'session_token' as const;
const cookieOptions: Cookies.CookieAttributes = { path: "/", sameSite: "lax" };

/**
 * Service class for handling auth-related API calls
 */
export default class AuthService {
    /**
    Submit login POST request
    **/
    static async login(credentials: Credentials): Promise<any> {
        const response = await axiosClient.post(AuthEndpoints.login(), credentials);

        const cookieValue: string = response.data.token;

        Cookies.set(SESSION_COOKIE_NAME, cookieValue, cookieOptions);
        axiosClient.defaults.headers.common.Authorization = `Bearer ${cookieValue}`;
        return response.data;
    }

    /**
    Logout of application
    **/
    static async logout(): Promise<any> {
        Cookies.remove(SESSION_COOKIE_NAME);
        axiosClient.defaults.headers.common.Authorization = null;
        return;
    };


    /**
    Check if user is logged in
    **/
    static userIsLoggedIn() {
        return Boolean(Cookies.get(SESSION_COOKIE_NAME));
    };

}