// File: sushi-toshi-frontend/pages/login.js
import Head from 'next/head';
import LoginForm from '../../components/auth/LoginForm';

export default function LoginPage() {
  return (
    <>
      <Head>
        <title>Login | Sushi Toshi</title>
        <meta name="description" content="Sign in to your Sushi Toshi account" />
      </Head>
      <LoginForm />
    </>
  );
}
