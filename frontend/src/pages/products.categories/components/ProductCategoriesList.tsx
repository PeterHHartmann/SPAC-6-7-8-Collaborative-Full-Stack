import { DataTable } from '@/components/ui/data-table';
import { useProductCategories } from '@/pages/products.categories/queries/ProductCategoryQueries';
import type { ProductCategory } from '@/types';
import type { ColumnDef, PaginationState } from '@tanstack/react-table';
import { useState } from 'react';
import { Link } from 'react-router-dom';

const columns: ColumnDef<ProductCategory>[] = [
    {
        header: "Id",
        accessorKey: "categoryId",
        cell: ({ row }) => {
            const id: ProductCategory["categoryId"] = row.getValue("categoryId");
            return (
                <Link to={`/products/categories/${id}`}>
                    {id}
                </Link>
            );
        }
    },
    {
        header: "Name",
        accessorKey: "categoryName"
    }
];

export const ProductCategoriesList: React.FC = () => {
    const [pagination, setPagination] = useState<PaginationState>({ pageIndex: 0, pageSize: 40 });
    const { data, isLoading, error } = useProductCategories({
        pageNumber: pagination.pageIndex + 1,
        pageSize: pagination.pageSize,
    });
    // if (isLoading) {
    //     return <div className="p-6">Loading products...</div>;
    // }

    // if (error) {
    //     return <div className="p-6 text-red-500">Error: {error.message}</div>;
    // }

    // if (!categories?) {
    //     return (
    //         <div className="p-6">
    //             <p className="mb-4">No product categories found</p>
    //         </div>
    //     );
    // }

    return (
        <DataTable tableTitle='Product Categories' columns={columns} data={data?.categories || []} pagination={pagination} setPagination={setPagination} pageCount={data?.totalPages} />
    );
};