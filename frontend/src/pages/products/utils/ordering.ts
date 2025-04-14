export function convertToSortBy(accessorKey: string): string {
    if (accessorKey === "category") {
        return "categoryId";
    }
    return accessorKey;
}