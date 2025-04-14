import {
    useProducts,
} from "../queries/ProductQueries";
import {
    ColumnDef,
    type ColumnFiltersState,
    type PaginationState,
    type SortingState,
} from "@tanstack/react-table";
import type { ProductCategory, Product } from '@/types';
import { DataTable } from '@/components/ui/data-table';
import { Link } from 'react-router-dom';
import { useState } from 'react';
import { Button } from '@/components/ui/button';
import { ArrowUpDown } from 'lucide-react';
import { convertToSortBy } from '@/pages/products/utils/ordering';

const columns: ColumnDef<Product>[] = [
    {
        accessorKey: "productId",
        header: ({ column }) => {
            return (
                <Button
                    variant="ghost"
                    onClick={() => column.toggleSorting(column.getIsSorted() ? column.getIsSorted() === "asc" : true)}>
                    Id
                    <ArrowUpDown className="ml-2 h-4 w-4" />
                </Button>
            );
        },
        cell: ({ row }) => {
            const id: number = row.getValue("productId");
            return (
                <Link to={`/products/${id}`}>
                    {id}
                </Link>
            );
        }
    },
    {
        accessorKey: "productName",
        header: ({ column }) => {
            return (
                <Button
                    variant="ghost"
                    onClick={() => column.toggleSorting(column.getIsSorted() === "asc")}>
                    Name
                    <ArrowUpDown className="ml-2 h-4 w-4" />
                </Button>
            );
        },
    },
    {
        accessorKey: "price",
        header: ({ column }) => {
            return (
                <Button
                    variant="ghost"
                    onClick={() => column.toggleSorting(column.getIsSorted() === "asc")}>
                    Price
                    <ArrowUpDown className="ml-2 h-4 w-4" />
                </Button>
            );
        },
        cell: ({ row }) => {
            const amount = parseFloat(row.getValue("price"));
            const formatted = new Intl.NumberFormat("en-US", {
                style: "currency",
                currency: "USD",
            }).format(amount);

            return <div className="font-medium">{formatted}</div>;
        },
    },
    {
        accessorKey: "category",
        header: ({ column }) => {
            return (
                <Button
                    variant="ghost"
                    onClick={() => column.toggleSorting(column.getIsSorted() === "asc")}>
                    Category
                    <ArrowUpDown className="ml-2 h-4 w-4" />
                </Button>
            );
        },
        cell: ({ row }) => {
            const { categoryId, categoryName }: ProductCategory = row.getValue("category");
            return (
                <Link to={`/products/categories/${categoryId}`}>
                    {categoryId + " | " + categoryName}
                </Link>
            );
        },
    },
    {
        accessorKey: "quantity",
        header: ({ column }) => {
            return (
                <Button
                    variant="ghost"
                    onClick={() => column.toggleSorting(column.getIsSorted() === "asc")}>
                    Quantity
                    <ArrowUpDown className="ml-2 h-4 w-4" />
                </Button>
            );
        },
    }
];

const DEFAULT_PAGE_SIZE = 40;

export const ProductList: React.FC = () => {
    const [pagination, setPagination] = useState<PaginationState>({ pageIndex: 0, pageSize: DEFAULT_PAGE_SIZE });
    const [sorting, setSorting] = useState<SortingState>([]);
    const [filters, setFilters] = useState<ColumnFiltersState>([]);

    const { data, isLoading, error } = useProducts({
        sortBy: sorting.length ? convertToSortBy(sorting[0].id) : "productId",
        isDescending: sorting.length ? sorting[0].desc : false,
        productName: filters.find((filter) => filter.id === "productName")?.value as string,
        categoryName: filters.find((filter) => filter.id === "category")?.value as string,
        minPrice: filters.find((filter) => filter.id === "minPrice")?.value as number,
        maxPrice: filters.find((filter) => filter.id === "maxPrice")?.value as number,
        pageNumber: pagination.pageIndex + 1,
        pageSize: pagination.pageSize,
    });

    // if (isLoading) {
    //     return <div className="p-6">Loading products...</div>;
    // }

    // if (error) {
    //     return <div className="p-6 text-red-500">Error: {error.message}</div>;
    // }

    // if (!data?.products?.length) {
    //     return (
    //         <div className="p-6">
    //             <p className="mb-4">No products found</p>
    //         </div>
    //     );
    // }

    return (
        <DataTable
            columns={columns}
            data={data?.products || []}
            tableTitle='Products'
            pagination={pagination} setPagination={setPagination} pageCount={data?.totalPages}
            sorting={sorting} setSorting={setSorting}
            filters={filters} setFilters={setFilters}
        />
    );
};
