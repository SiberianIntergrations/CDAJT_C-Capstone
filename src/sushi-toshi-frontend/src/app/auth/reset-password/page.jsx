// sushi-toshi-frontend/pages/auth/reset-password.js
import Head from 'next/head';
import ResetPasswordForm from '../../components/auth/ResetPasswordForm';

export default function ResetPasswordPage() {
  return (
    <>
      <Head>
        <title>Reset Password | Sushi Toshi</title>
        <meta name="description" content="Set a new password for your Sushi Toshi account" />
      </Head>
      <ResetPasswordForm />
    </>
  );
}