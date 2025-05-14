import { Spinner } from '@/components/ui/Spinner';
import { useLogout } from '@/pages/auth/queries/AuthQueries';
import { useEffect } from 'react';

export const LogoutPage: React.FC = () => {

    const { mutate: logout } = useLogout();

    useEffect(() => {
        logout();
    });

    return (
        <div className='flex flex-col justify-center items-center'>
            <p>Signing out</p>
            <Spinner></Spinner>
        </div>
    );
};