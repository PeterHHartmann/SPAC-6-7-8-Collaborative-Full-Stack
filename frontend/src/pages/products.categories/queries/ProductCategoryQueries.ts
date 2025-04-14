import { useQuery, type UseQueryResult } from '@tanstack/react-query';
import { queryKeys } from '@/api/queryClient';
import ProductCategoryService from '@/pages/products.categories/services/ProductCategoryService';
import type { ApiError, PaginationParams, ProductCategory } from '@/types';

export const useProductCategories = (params?: PaginationParams) => {
    return useQuery({
        queryKey: [...queryKeys.products.categories.all, params],
        queryFn: () => ProductCategoryService.getAll(),
    });
};

export const useProductCategory = (id: string): UseQueryResult<ProductCategory, ApiError> => {
    return useQuery({
        queryKey: queryKeys.products.categories.byId(id),
        queryFn: () => ProductCategoryService.getById(id)
    });
};