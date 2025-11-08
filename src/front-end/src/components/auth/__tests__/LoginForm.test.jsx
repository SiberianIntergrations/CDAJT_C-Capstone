import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { useRouter } from 'next/navigation';
import LoginForm from '../LoginForm';
import { loginUser } from '@/utils/auth';

// Mock dependencies
jest.mock('next/navigation', () => ({
  useRouter: jest.fn(),
}));

jest.mock('@/utils/auth', () => ({
  loginUser: jest.fn(),
}));

describe('LoginForm', () => {
  const mockPush = jest.fn();

  beforeEach(() => {
    jest.clearAllMocks();
    useRouter.mockReturnValue({
      push: mockPush,
    });
  });

  describe('Rendering', () => {
    it('renders the login form with all elements', () => {
      render(<LoginForm />);

      expect(screen.getByText(/Login to Sushi Toshi/i)).toBeInTheDocument();
      expect(screen.getByLabelText(/Email Address/i)).toBeInTheDocument();
      expect(screen.getByLabelText(/Password/i)).toBeInTheDocument();
      expect(screen.getByRole('button', { name: /Sign In/i })).toBeInTheDocument();
      expect(screen.getByText(/Forgot Password?/i)).toBeInTheDocument();
      expect(screen.getByText(/Create New Account/i)).toBeInTheDocument();
    });

    it('renders with default test credentials', () => {
      render(<LoginForm />);

      const emailInput = screen.getByLabelText(/Email Address/i);
      const passwordInput = screen.getByLabelText(/Password/i);

      expect(emailInput).toHaveValue('admin.user@sushitoshi.ca');
      expect(passwordInput).toHaveValue('AdminPass123!');
    });

    it('renders the lock icon', () => {
      const { container } = render(<LoginForm />);

      // Lock icon is rendered via lucide-react
      const lockIcon = container.querySelector('svg');
      expect(lockIcon).toBeInTheDocument();
    });
  });

  describe('Form Interaction', () => {
    it('allows users to type in email field', async () => {
      const user = userEvent.setup();
      render(<LoginForm />);

      const emailInput = screen.getByLabelText(/Email Address/i);
      await user.clear(emailInput);
      await user.type(emailInput, 'test@example.com');

      expect(emailInput).toHaveValue('test@example.com');
    });

    it('allows users to type in password field', async () => {
      const user = userEvent.setup();
      render(<LoginForm />);

      const passwordInput = screen.getByLabelText(/Password/i);
      await user.clear(passwordInput);
      await user.type(passwordInput, 'newpassword123');

      expect(passwordInput).toHaveValue('newpassword123');
    });

    it('updates form state when inputs change', async () => {
      const user = userEvent.setup();
      render(<LoginForm />);

      const emailInput = screen.getByLabelText(/Email Address/i);
      const passwordInput = screen.getByLabelText(/Password/i);

      await user.clear(emailInput);
      await user.type(emailInput, 'user@test.com');
      await user.clear(passwordInput);
      await user.type(passwordInput, 'password123');

      expect(emailInput).toHaveValue('user@test.com');
      expect(passwordInput).toHaveValue('password123');
    });
  });

  describe('Form Submission', () => {
    it('submits form with valid credentials', async () => {
      const user = userEvent.setup();
      loginUser.mockResolvedValue({ success: true });

      render(<LoginForm />);

      const emailInput = screen.getByLabelText(/Email Address/i);
      const passwordInput = screen.getByLabelText(/Password/i);
      const submitButton = screen.getByRole('button', { name: /Sign In/i });

      await user.clear(emailInput);
      await user.type(emailInput, 'test@example.com');
      await user.clear(passwordInput);
      await user.type(passwordInput, 'password123');

      await user.click(submitButton);

      await waitFor(() => {
        expect(loginUser).toHaveBeenCalledWith('test@example.com', 'password123');
      });
    });

    it('shows loading state during submission', async () => {
      const user = userEvent.setup();
      loginUser.mockImplementation(() => new Promise(resolve => setTimeout(resolve, 1000)));

      render(<LoginForm />);

      const submitButton = screen.getByRole('button', { name: /Sign In/i });

      await user.click(submitButton);

      // Check for loading spinner
      expect(screen.getByRole('progressbar')).toBeInTheDocument();
      expect(submitButton).toBeDisabled();
    });

    it('redirects to home page on successful login', async () => {
      const user = userEvent.setup();
      loginUser.mockResolvedValue({ success: true });

      render(<LoginForm />);

      const submitButton = screen.getByRole('button', { name: /Sign In/i });
      await user.click(submitButton);

      await waitFor(() => {
        expect(mockPush).toHaveBeenCalledWith('/');
      });
    });

    it('prevents submission while loading', async () => {
      const user = userEvent.setup();
      loginUser.mockImplementation(() => new Promise(resolve => setTimeout(resolve, 1000)));

      render(<LoginForm />);

      const submitButton = screen.getByRole('button', { name: /Sign In/i });

      await user.click(submitButton);

      // Try to click again while loading
      await user.click(submitButton);

      // Should only be called once
      expect(loginUser).toHaveBeenCalledTimes(1);
    });
  });

  describe('Error Handling', () => {
    it('displays error message on failed login', async () => {
      const user = userEvent.setup();
      const errorMessage = 'Invalid credentials';
      loginUser.mockRejectedValue({
        response: {
          data: {
            detail: errorMessage,
          },
        },
      });

      render(<LoginForm />);

      const submitButton = screen.getByRole('button', { name: /Sign In/i });
      await user.click(submitButton);

      await waitFor(() => {
        expect(screen.getByText(errorMessage)).toBeInTheDocument();
      });
    });

    it('displays default error message when no detail provided', async () => {
      const user = userEvent.setup();
      loginUser.mockRejectedValue({
        response: {
          data: {},
        },
      });

      render(<LoginForm />);

      const submitButton = screen.getByRole('button', { name: /Sign In/i });
      await user.click(submitButton);

      await waitFor(() => {
        expect(screen.getByText(/Login failed. Please check your credentials and try again./i)).toBeInTheDocument();
      });
    });

    it('clears error message on new submission', async () => {
      const user = userEvent.setup();
      loginUser.mockRejectedValueOnce({
        response: {
          data: { detail: 'Error' },
        },
      }).mockResolvedValueOnce({ success: true });

      render(<LoginForm />);

      const submitButton = screen.getByRole('button', { name: /Sign In/i });

      // First submission - error
      await user.click(submitButton);
      await waitFor(() => {
        expect(screen.getByText('Error')).toBeInTheDocument();
      });

      // Second submission - should clear error
      await user.click(submitButton);
      await waitFor(() => {
        expect(screen.queryByText('Error')).not.toBeInTheDocument();
      });
    });

    it('hides loading state after error', async () => {
      const user = userEvent.setup();
      loginUser.mockRejectedValue({
        response: {
          data: { detail: 'Error' },
        },
      });

      render(<LoginForm />);

      const submitButton = screen.getByRole('button', { name: /Sign In/i });
      await user.click(submitButton);

      await waitFor(() => {
        expect(screen.getByText('Error')).toBeInTheDocument();
      });

      expect(submitButton).not.toBeDisabled();
      expect(screen.queryByRole('progressbar')).not.toBeInTheDocument();
    });
  });

  describe('Navigation', () => {
    it('navigates to forgot password page when clicked', async () => {
      const user = userEvent.setup();
      render(<LoginForm />);

      const forgotPasswordButton = screen.getByText(/Forgot Password?/i);
      await user.click(forgotPasswordButton);

      expect(mockPush).toHaveBeenCalledWith('/auth/forgot-password');
    });

    it('navigates to register page when clicked', async () => {
      const user = userEvent.setup();
      render(<LoginForm />);

      const registerButton = screen.getByText(/Create New Account/i);
      await user.click(registerButton);

      expect(mockPush).toHaveBeenCalledWith('/auth/register');
    });
  });

  describe('Accessibility', () => {
    it('has proper form labels', () => {
      render(<LoginForm />);

      expect(screen.getByLabelText(/Email Address/i)).toBeInTheDocument();
      expect(screen.getByLabelText(/Password/i)).toBeInTheDocument();
    });

    it('email input has type email', () => {
      render(<LoginForm />);

      const emailInput = screen.getByLabelText(/Email Address/i);
      expect(emailInput).toHaveAttribute('type', 'email');
    });

    it('password input has type password', () => {
      render(<LoginForm />);

      const passwordInput = screen.getByLabelText(/Password/i);
      expect(passwordInput).toHaveAttribute('type', 'password');
    });

    it('submit button is accessible', () => {
      render(<LoginForm />);

      const submitButton = screen.getByRole('button', { name: /Sign In/i });
      expect(submitButton).toHaveAttribute('type', 'submit');
    });
  });

  describe('Form Validation', () => {
    it('email field is required', () => {
      render(<LoginForm />);

      const emailInput = screen.getByLabelText(/Email Address/i);
      expect(emailInput).toHaveAttribute('required');
    });

    it('password field is required', () => {
      render(<LoginForm />);

      const passwordInput = screen.getByLabelText(/Password/i);
      expect(passwordInput).toHaveAttribute('required');
    });
  });
});
