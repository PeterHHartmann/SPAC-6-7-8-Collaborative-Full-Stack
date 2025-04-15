import {
    ColumnDef,
    flexRender,
    getCoreRowModel,
    getSortedRowModel,
    useReactTable,
    type ColumnFiltersState,
    type PaginationState,
    type SortingState,
} from "@tanstack/react-table";

import {
    Table,
    TableBody,
    TableCell,
    TableHead,
    TableHeader,
    TableRow,
} from "@/components/ui/table";
import { type Dispatch, type SetStateAction } from 'react';
import { Pagination, PaginationContent, PaginationItem } from '@/components/ui/pagination';
import { Button } from '@/components/ui/button';
import { Select, SelectContent, SelectItem, SelectTrigger } from '@/components/ui/select';
import { SelectValue } from '@radix-ui/react-select';
import { Input } from '@/components/ui/input';
import { Collapsible, CollapsibleContent, CollapsibleTrigger } from '@/components/ui/collapsible';
import { ChevronsUpDown } from 'lucide-react';
import { Label } from '@/components/ui/label';

type DataTableProps<TData, TValue> = {
    columns: ColumnDef<TData, TValue>[];
    data: TData[];
    tableTitle: string;
    pagination?: PaginationState;
    setPagination?: Dispatch<SetStateAction<PaginationState>>;
    pageCount?: number;
    sorting?: SortingState;
    setSorting?: Dispatch<SetStateAction<SortingState>>;
    filters?: ColumnFiltersState;
    setFilters?: Dispatch<SetStateAction<ColumnFiltersState>>;
};

export function DataTable<TData, TValue>({
    columns,
    data,
    tableTitle,
    pagination,
    setPagination,
    pageCount,
    sorting,
    setSorting,
    filters,
    setFilters
}: DataTableProps<TData, TValue>) {

    const table = useReactTable({
        data,
        columns,
        debugTable: true,
        getCoreRowModel: getCoreRowModel(),
        manualPagination: true,
        onPaginationChange: setPagination,
        getSortedRowModel: getSortedRowModel(),
        onSortingChange: setSorting,
        onColumnFiltersChange: setFilters,
        manualFiltering: true,
        state: {
            pagination,
            sorting,
            columnFilters: filters,
        },
        pageCount: pageCount,
    });

    return (
        <div className="container mx-auto">
            <div className='flex w-full items-end py-2 gap-1'>
                <h2 className='w-max text-4xl whitespace-nowrap'>
                    {tableTitle}
                </h2>
                <div className='flex w-full justify-end items-end gap-2'>
                    {pagination ?
                        <Pagination className='w-full flex items-center justify-end'>
                            <PaginationContent>
                                <PaginationItem>
                                    <Button
                                        variant="outline"
                                        size="sm"
                                        onClick={() => table.firstPage()}
                                        disabled={!table.getCanPreviousPage()}
                                    >
                                        {'<<'}
                                    </Button>
                                </PaginationItem>
                                <PaginationItem>
                                    <Button
                                        variant="outline"
                                        size="sm"
                                        onClick={() => table.previousPage()}
                                        disabled={!table.getCanPreviousPage()}
                                    >
                                        {'<'}
                                    </Button>
                                </PaginationItem>
                                <PaginationItem>
                                    <span className="flex items-center gap-1 mx-2">
                                        <div>Page</div>
                                        <strong>
                                            {table.getState().pagination.pageIndex + 1} of{' '}
                                            {table.getPageCount().toLocaleString()}
                                        </strong>
                                    </span>
                                </PaginationItem>
                                <PaginationItem>
                                    <Button
                                        variant="outline"
                                        size="sm"
                                        onClick={() => table.nextPage()}
                                        disabled={!table.getCanNextPage()}
                                    >
                                        {'>'}
                                    </Button>
                                </PaginationItem>
                                <PaginationItem>
                                    <Button
                                        variant="outline"
                                        size="sm"
                                        onClick={() => table.lastPage()}
                                        disabled={!table.getCanNextPage()}
                                    >
                                        {'>>'}
                                    </Button>
                                </PaginationItem>

                                <PaginationItem>
                                    <Select
                                        value={String(table.getState().pagination.pageSize)}
                                        onValueChange={value => {
                                            table.setPageSize(Number(value));
                                        }}
                                    >
                                        <SelectTrigger>
                                            <SelectValue />
                                        </SelectTrigger>
                                        <SelectContent>
                                            {[10, 20, 30, 40, 50, 60, 70, 80, 90, 100].map(pageSize => (
                                                <SelectItem key={`pageSize-${pageSize}`} value={String(pageSize)}>Show {pageSize}</SelectItem>
                                            ))}
                                        </SelectContent>
                                    </Select>
                                </PaginationItem>
                                <PaginationItem>
                                    <Select
                                        value={String(table.getState().pagination.pageIndex)}
                                        onValueChange={value => {
                                            table.setPageIndex(Number(value));
                                        }}
                                    >
                                        <SelectTrigger>
                                            <SelectValue />
                                        </SelectTrigger>
                                        <SelectContent>
                                            {(Array.from({ length: table.getPageCount() }, (_, i) => i)).map((pageNum) => {
                                                return (
                                                    <SelectItem
                                                        key={`pageSelect-${pageNum}`}
                                                        value={String(pageNum)}
                                                    >
                                                        Go to page: {pageNum + 1}</SelectItem>
                                                );
                                            })}
                                        </SelectContent>
                                    </Select>
                                </PaginationItem>
                            </PaginationContent>
                        </Pagination>
                        : null}
                    {filters && setFilters ?
                        <Collapsible className='w-1/5'>
                            <div className="flex justify-between items-center">
                                <h4 className="text-sm font-semibold">
                                    Filters
                                </h4>
                                <CollapsibleTrigger asChild>
                                    <Button variant="ghost" size="sm">
                                        <ChevronsUpDown className="h-4 w-4" />
                                        <span className="sr-only">Filters</span>
                                    </Button>
                                </CollapsibleTrigger>
                            </div>
                            <CollapsibleContent>
                                <div className="flex flex-col gap-2 py-4">
                                    <Label htmlFor="productName-filter">Product Name</Label>
                                    <Input
                                        id='productName-filter'
                                        type='text'
                                        placeholder="Enter product name..."
                                        value={(table.getColumn("productName")?.getFilterValue() as string) ?? ""}
                                        // value={(table.getColumn("productName")?.getFilterValue() as string) ?? ""}
                                        onChange={(event) =>
                                            table.getColumn("productName")?.setFilterValue(event.target.value)
                                        }
                                        className="max-w-sm"
                                    />
                                </div>
                                <div className="flex flex-col gap-2 py-4">
                                    <Label htmlFor="categoryName-filter">Category Name</Label>
                                    <Input
                                        id='categoryName-filter'
                                        type='text'
                                        placeholder="Enter category name..."
                                        value={(table.getColumn("category")?.getFilterValue() as string) ?? ""}
                                        // value={(table.getColumn("productName")?.getFilterValue() as string) ?? ""}
                                        onChange={(event) =>
                                            table.getColumn("category")?.setFilterValue(event.target.value)
                                        }
                                        className="max-w-sm"
                                    />
                                </div>
                                <div className="flex flex-col gap-2 py-4">
                                    <Label htmlFor="minPrice-filter">Minimum Price</Label>
                                    <Input
                                        id='minPrice-filter'
                                        min={0}
                                        max={100000}
                                        type='number'
                                        defaultValue={''}
                                        placeholder="Enter minimum price..."
                                        value={filters.find((filter) => filter.id === "minPrice")?.value as number}
                                        onChange={(event) => {
                                            const newValue = { id: "minPrice", value: Number(event.target.value) };
                                            if (newValue.value > 0) {

                                                const idx = filters.findIndex((filter) => filter.id === "minPrice");
                                                const newFilters = filters.slice();
                                                if (idx < 0) {
                                                    newFilters.push(newValue);
                                                } else {
                                                    newFilters[idx] = newValue;
                                                }
                                                setFilters(newFilters);
                                            }
                                        }}
                                        className="max-w-sm"
                                    />
                                </div>
                                <div className="flex flex-col gap-2 py-4">
                                    <Label htmlFor="maxPrice-filter">Maximum Price</Label>
                                    <Input
                                        id='maxPrice-filter'
                                        min={0}
                                        max={100000}
                                        type='number'
                                        defaultValue={''}
                                        placeholder="Enter maximum price..."
                                        value={filters.find((filter) => filter.id === "maxPrice")?.value as number}
                                        onChange={(event) => {
                                            const newValue = { id: "maxPrice", value: Number(event.target.value) };
                                            if (newValue.value > 0) {
                                                const idx = filters.findIndex((filter) => filter.id === "maxPrice");
                                                const newFilters = filters.slice();
                                                if (idx < 0) {
                                                    newFilters.push(newValue);
                                                } else {
                                                    newFilters[idx] = newValue;
                                                }
                                                setFilters(newFilters);
                                            }
                                        }}
                                        className="max-w-sm"
                                    />
                                </div>
                            </CollapsibleContent>
                        </Collapsible>
                        : null
                    }
                </div>
            </div>
            <div className="rounded-md border overflow-auto">
                <Table>
                    <TableHeader className='bg-sidebar'>
                        {table.getHeaderGroups().map((headerGroup) => (
                            <TableRow key={headerGroup.id}>
                                {headerGroup.headers.map((header) => {
                                    return (
                                        <TableHead key={header.id}>
                                            {header.isPlaceholder
                                                ? null
                                                : flexRender(
                                                    header.column.columnDef.header,
                                                    header.getContext()
                                                )}
                                        </TableHead>
                                    );
                                })}
                            </TableRow>
                        ))}
                    </TableHeader>
                    <TableBody>
                        {table.getRowModel().rows?.length ? (
                            table.getRowModel().rows.map((row) => (
                                <TableRow
                                    key={row.id}
                                    data-state={row.getIsSelected() && "selected"}
                                >
                                    {row.getVisibleCells().map((cell) => (
                                        <TableCell key={cell.id}>
                                            {flexRender(cell.column.columnDef.cell, cell.getContext())}
                                        </TableCell>
                                    ))}
                                </TableRow>
                            ))
                        ) : (
                            <TableRow>
                                <TableCell colSpan={columns.length} className="h-24 text-center">
                                    No results.
                                </TableCell>
                            </TableRow>
                        )}
                    </TableBody>
                </Table>
            </div>
        </div>
    );
}