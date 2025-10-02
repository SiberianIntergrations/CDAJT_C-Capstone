import Head from "next/head";
import RegisterForm from "@/components/auth/RegisterForm";

export default function RegisterPage() {
  return (
    <>
      <Head>
        <title>Register | Sushi Toshi</title>
        <meta
          name="description"
          content="Create a new account at Sushi Toshi"
        />
      </Head>
      <RegisterForm />
    </>
  );
}
