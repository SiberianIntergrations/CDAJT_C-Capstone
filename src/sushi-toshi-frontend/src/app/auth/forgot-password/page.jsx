// sushi-toshi-frontend/pages/auth/forgot-password.js
import Head from 'next/head';
import ForgotPasswordForm from '../../components/auth/ForgotPasswordForm';

export default function ForgotPasswordPage() {
  return (
    <>
      <Head>
        <title>Forgot Password | Sushi Toshi</title>
        <meta name="description" content="Reset your Sushi Toshi password" />
      </Head>
      <ForgotPasswordForm />
    </>
  );
}