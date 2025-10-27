import ResetPasswordForm from "@/components/auth/ResetPasswordForm";

export const metadata = {
  title: "Reset Password | Sushi Toshi",
  description: "Set a new password for your Sushi Toshi account",
};

export default function ResetPasswordPage() {
  return (
    <ResetPasswordForm />
  );
};
