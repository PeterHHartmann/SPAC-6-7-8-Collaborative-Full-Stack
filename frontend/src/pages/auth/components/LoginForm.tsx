import * as z from 'zod';
import { useForm, type SubmitHandler } from "react-hook-form";
import type React from 'react';
import { zodResolver } from '@hookform/resolvers/zod';
import { useLogin } from '@/pages/auth/queries/AuthQueries';
import { useNavigate } from 'react-router-dom';

const schema = z.object({
    username: z
        .string({
            required_error: 'Username is required',
        }),
    password: z
        .string({
            required_error: 'Password is required',
        }),
});

export type FormType = z.infer<typeof schema>;

export type FormProps = {
    onSubmit?: SubmitHandler<FormType>;
};

const ErrorMessage = ({ message }: { message: string | undefined; }) => {
    return (
        <div className="p-4 mb-4 text-sm text-red-800 rounded-lg bg-red-50 dark:bg-gray-800 dark:text-red-400" role="alert">
            <span className="font-medium">{message}</span>
        </div>
    );
};

export const LoginForm: React.FC = () => {
    const navigate = useNavigate();

    const inputStyles = {
        className: 'border-[1px] border-gray rounded-md mb-4 w-72 p-2'
    };
    const { handleSubmit, register, formState: { errors } } = useForm<FormType>({
        resolver: zodResolver(schema),
    });

    const { mutate: login } = useLogin();

    const onSubmit: FormProps['onSubmit'] = (data) => {
        login(data, {
            onSuccess: () => {
                navigate("/");
            }
        });
    };

    return (
        <div className="flex-1 flex justify-center items-center w-screen m-auto">
            <form onSubmit={handleSubmit(onSubmit)} className='m-auto'>
                <div className='flex flex-col justify-between'>
                    <input placeholder='Username' {...inputStyles} defaultValue="" {...register("username")} />
                    {errors.username && <ErrorMessage message={errors?.username?.message} />}
                    <input  {...inputStyles} placeholder='Password' {...register("password")} />
                    {errors.password && <ErrorMessage message={errors?.password?.message} />}
                    <button type="submit" className="text-white bg-gray-800 hover:bg-gray-900 focus:outline-none focus:ring-4 focus:ring-gray-300 font-medium rounded-lg text-sm px-5 py-2.5 me-2 mb-2 dark:bg-gray-800 dark:hover:bg-gray-700 dark:focus:ring-gray-700 dark:border-gray-700">Submit</button>
                </div>
            </form>
        </div>

    );
};