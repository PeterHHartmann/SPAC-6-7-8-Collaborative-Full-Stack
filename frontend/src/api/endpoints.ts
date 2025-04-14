export const AuthEndpoints = {
    login: () => '/login'
};

export const ProductEndpoints = {
    getAll: (params: string) => `/products${params ? `?${params}` : ""}`,
    getById: (id: string | number) => `/products/${id}`,
    // getProductsFromOrder: (orderToken: string) => `/product/order-products?token=${orderToken}`,
} as const;

export const ProductCategoryEndpoints = {
    getAll: (params: string) => `/categories${params ? `?${params}` : ""}`,
    getById: (id: string | number) => `/categories/${id}`,
    // getProductsFromOrder: (orderToken: string) => `/product/order-products?token=${orderToken}`,
} as const;