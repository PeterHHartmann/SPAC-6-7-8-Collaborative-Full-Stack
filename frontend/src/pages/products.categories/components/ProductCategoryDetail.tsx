import { DetailView } from '@/components/ui/detail-view';
import { useProductCategory } from '@/pages/products.categories/queries/ProductCategoryQueries';
import type { ProductCategory } from '@/types';
import type { ColumnDef } from '@tanstack/react-table';

export const ProductCategoryDetail: React.FC = () => {

    const columns: ColumnDef<ProductCategory>[] = [
        {
            header: "Id",
            accessorKey: "categoryId"
        },
        {
            header: "Name",
            accessorKey: "categoryName"
        }
    ];

    return (
        <DetailView dataName="product category" query={useProductCategory} columns={columns} />
    );
};