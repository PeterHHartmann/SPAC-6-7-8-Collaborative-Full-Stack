import { DetailView } from '@/components/ui/detail-view';
import { useProduct } from '@/pages/products/queries/ProductQueries';
import type { Product, ProductCategory } from '@/types';
import type { ColumnDef } from '@tanstack/react-table';
import { Link } from 'react-router-dom';

const columns: ColumnDef<Product>[] = [
    {
        header: "Id",
        accessorKey: "productId"
    },
    {
        header: "Name",
        accessorKey: "productName"
    },
    {
        header: "Price",
        accessorKey: "price",
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
        header: "Category",
        accessorKey: "category",
        cell: ({ row }) => {
            const { categoryId, categoryName }: ProductCategory = row.getValue("category");
            return (
                <Link to={`/products/categories/${categoryId}`}>
                    {categoryId + " | " + categoryName}
                </Link>
            );
        }
    }
];

export const ProductDetail: React.FC = () => {
    return (
        <DetailView dataName="product" query={useProduct} columns={columns} tableTitle='Product Detail' />
    );
};