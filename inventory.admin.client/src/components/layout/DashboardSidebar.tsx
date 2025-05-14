import {
    Sidebar,
    SidebarContent,
    SidebarFooter,
    SidebarGroup,
    SidebarGroupContent,
    SidebarGroupLabel,
    SidebarHeader,
    SidebarMenu,
    SidebarMenuButton,
    SidebarMenuItem,
} from "@/components/ui/sidebar";
import type { SidebarGroupArgs } from '@/types';
import type React from 'react';
import { Link } from 'react-router-dom';

export const DashboardSidebar: React.FC = () => {

    const groups: SidebarGroupArgs[] = [
        {
            label: "Products",
            items: [
                {
                    label: "Products",
                    url: "/products"
                },
                {
                    label: "Product categories",
                    url: "/products/categories"
                }
            ]
        },
        {
            label: "Orders",
        },
        {
            label: "Customers",
        }
    ];

    return (
        <Sidebar>
            <SidebarHeader className='px-4 pt-4'>Contents:</SidebarHeader>
            <SidebarContent className='pl-4'>
                {groups.map(({ label, items }) => (
                    <SidebarGroup key={`sidebarGroup-${label}`}>
                        <SidebarGroupLabel>
                            {label}
                        </SidebarGroupLabel>
                        <SidebarGroupContent>
                            <SidebarMenu>
                                {items ?
                                    items.map((item) => (
                                        <SidebarMenuItem key={label + item.label}>
                                            <SidebarMenuButton asChild>
                                                <Link to={item.url}>
                                                    - {item.label}
                                                </Link>
                                            </SidebarMenuButton>
                                        </SidebarMenuItem>
                                    ))
                                    : null}
                            </SidebarMenu>
                        </SidebarGroupContent>
                    </SidebarGroup>
                ))}
            </SidebarContent>
            <SidebarFooter />
        </Sidebar >
    );
};