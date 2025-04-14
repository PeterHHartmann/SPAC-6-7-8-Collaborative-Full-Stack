import { useQuery } from '@tanstack/react-query';
import type { AllProductsResponse, ApiError, PaginationParams, Product, ProductQueryParams } from '@/types';
import { queryKeys } from '@/api/queryClient';
import ProductService from '@/pages/products/services/ProductService';

export const useProducts = (params?: ProductQueryParams & PaginationParams) => {
    return useQuery<AllProductsResponse, ApiError>({
        queryKey: [...queryKeys.products.all, params],
        queryFn: () => ProductService.getAll(params),
    });
};

export const useProduct = (id: string) => {
    return useQuery<Product, ApiError>({
        queryKey: queryKeys.products.byId(id),
        queryFn: () => ProductService.getById(id),
        placeholderData: (prev) => prev
    });
};