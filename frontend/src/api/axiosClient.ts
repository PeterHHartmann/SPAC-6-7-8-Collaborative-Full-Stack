import axios, { type AxiosError, type AxiosRequestConfig, type AxiosResponse } from 'axios';

const client = axios.create({
    baseURL: 'https://127.0.0.1:7117/api',
    headers: {
        'Content-Type': 'application/json',
        "Accept": "application/json"
    }
});

export const request = async (options: AxiosRequestConfig) => {
    const onSuccess = (response: AxiosResponse) => {
        const { data } = response;
        return data;
    };

    const onError = function (error: AxiosError) {
        return Promise.reject({
            message: error.message,
            code: error.code,
            response: error.response,
        });
    };

    return client(options).then(onSuccess).catch(onError);
};


// client.interceptors.request.use(
//     (config: InternalAxiosRequestConfig) => {
//         // const accessToken = localStorage.getItem(STORAGE_TOKEN.ACCESS_TOKEN);
//         // if (accessToken) {
//         //     config.headers.Authorization = `Bearer ${accessToken}`;
//         // }
//         return config;
//     },
//     (error: AxiosError) => {
//         return Promise.reject(error);
//     },
// );

// client.interceptors.response.use(
//     (res: AxiosResponse) => {
//         return res; // Simply return the response
//     },
//     async (err) => {
//         const status = err.response ? err.response.status : null;
//         // if (status === 401) {
//         //     try {
//         //         const refreshTokenFromStorage = localStorage.getItem(
//         //             STORAGE_TOKEN.REFRESH_TOKEN
//         //         );
//         //         const { accessToken, refreshToken } = await AuthService.refresh(
//         //             refreshTokenFromStorage
//         //         );

//         //         LocalStorageService.setTokens(accessToken, refreshToken);
//         //         client.defaults.headers.common.Authorization = `Bearer ${accessToken}`;

//         //         return await client(originalConfig);
//         //     } catch (error: AxiosError) {
//         //         return Promise.reject(error);
//         //     }
//         // }

//         if (status === 403 && err.response.data) {
//             return Promise.reject(err.response.data);
//         }

//         return Promise.reject(err);
//     }
// );

export default client;

// export async function getProducts() {
//     const response = await client.get<Product[]>('/api/Products');
//     return response.data;
// }