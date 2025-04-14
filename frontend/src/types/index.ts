export type PaginationParams = {
    pageNumber: number,
    pageSize: number;
};

export type PaginatedResponse = {
    totalCount: number,
    pageNumber: number,
    pageSize: number,
    totalPages: number,
};

export type Product = {
    productId: number;
    productName?: string;
    price: number;
    category: ProductCategory;
    quantity: number;
};

export type ProductQueryParams = {
    sortBy?: string;
    isDescending?: boolean;
    productName?: string;
    categoryName?: string;
    minPrice?: number,
    maxPrice?: number,
    pageNumber: number,
    pageSize: number;
};

export type AllProductsResponse = {
    products: Product[];
} & PaginatedResponse;

export type ProductCategory = {
    categoryId: number;
    categoryName: string;
};

export type AllProductCategoriesResponse = {
    categories: ProductCategory[];
} & PaginatedResponse;

export type OrderItemDto = {
    productId: number;
    quantity: number;
};

export type Order = {
    orderId: number;
    customerId: number;
    orderDate: string;
    paymentMethod?: string;
    orderItems?: OrderItem[];
};

export type OrderItem = {
    orderItemId: number;
    orderId: number;
    productId: number;
    quantity: number;
};

export type ApiError = {
    message: string;
    code: string;
    status: number;
};

export type SidebarGroupArgs = {
    label: string;
    items?: {
        label: string;
        url: string;
    }[];
};

export type AuthStatus = {
    success: boolean;
    err?: {
        message: string;
    };
    token: string;
};

export type Credentials = {
    username: string,
    password: string;
};