import { Loader } from 'lucide-react';

export const Spinner = () => {



    return (
        <div className="flex items-center justify-center p-2">
            <Loader className='animate-spin' />
        </div>
    );
};