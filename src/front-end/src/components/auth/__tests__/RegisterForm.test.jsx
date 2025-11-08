import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { useRouter } from 'next/navigation';
import RegisterForm from '../RegisterForm';
import { registerUser } from '@/utils/auth';

// Mock dependencies
jest.mock('next/navigation', () => ({
  useRouter: jest.fn(),
}));

jest.mock('@/utils/auth', () => ({
  registerUser: jest.fn(),
}));

describe('RegisterForm', () => {
  const mockPush = jest.fn();

  beforeEach(() => {
    jest.clearAllMocks();
    useRouter.mockReturnValue({
      push: mockPush,
    });
  });

  describe('Rendering', () => {
    it('renders the registration form with all elements', () => {
      render(<RegisterForm />);

      expect(screen.getByText(/Create Account/i)).toBeInTheDocument();
      expect(screen.getByLabelText(/First Name/i)).toBeInTheDocument();
      expect(screen.getByLabelText(/Last Name/i)).toBeInTheDocument();
      expect(screen.getByLabelText(/Email Address/i)).toBeInTheDocument();
      // Check that we have at least 2 password fields
      expect(screen.getAllByLabelText(/Password/i).length).toBeGreaterThanOrEqual(2);
      expect(screen.getByRole('button', { name: /Register/i })).toBeInTheDocument();
    });

    it('renders with default test data', () => {
      render(<RegisterForm />);

      expect(screen.getByLabelText(/First Name/i)).toHaveValue('James');
      expect(screen.getByLabelText(/Last Name/i)).toHaveValue('Smith');
      expect(screen.getByLabelText(/Email Address/i)).toHaveValue('jamie2@example.com');
      expect(screen.getByLabelText(/^Password$/i)).toHaveValue('somethingCool1');
      expect(screen.getByLabelText(/Confirm Password/i)).toHaveValue('somethingCool1');
    });

    it('renders the UserPlus icon', () => {
      const { container } = render(<RegisterForm />);

      const icon = container.querySelector('svg');
      expect(icon).toBeInTheDocument();
    });

    it('shows login link', () => {
      render(<RegisterForm />);

      expect(screen.getByText(/Already have an account\? Sign in/i)).toBeInTheDocument();
    });
  });

  describe('Form Interaction', () => {
    it('allows users to type in all fields', async () => {
      const user = userEvent.setup();
      render(<RegisterForm />);

      const firstNameInput = screen.getByLabelText(/First Name/i);
      const lastNameInput = screen.getByLabelText(/Last Name/i);
      const emailInput = screen.getByLabelText(/Email Address/i);
      const passwordInput = screen.getByLabelText(/^Password$/i);
      const confirmPasswordInput = screen.getByLabelText(/Confirm Password/i);

      await user.clear(firstNameInput);
      await user.type(firstNameInput, 'John');
      await user.clear(lastNameInput);
      await user.type(lastNameInput, 'Doe');
      await user.clear(emailInput);
      await user.type(emailInput, 'john@example.com');
      await user.clear(passwordInput);
      await user.type(passwordInput, 'Password123');
      await user.clear(confirmPasswordInput);
      await user.type(confirmPasswordInput, 'Password123');

      expect(firstNameInput).toHaveValue('John');
      expect(lastNameInput).toHaveValue('Doe');
      expect(emailInput).toHaveValue('john@example.com');
      expect(passwordInput).toHaveValue('Password123');
      expect(confirmPasswordInput).toHaveValue('Password123');
    });
  });

  describe('Form Submission', () => {
    it('submits form with valid data', async () => {
      const user = userEvent.setup();
      registerUser.mockResolvedValue({ success: true });

      render(<RegisterForm />);

      const firstNameInput = screen.getByLabelText(/First Name/i);
      const lastNameInput = screen.getByLabelText(/Last Name/i);
      const emailInput = screen.getByLabelText(/Email Address/i);
      const passwordInput = screen.getByLabelText(/^Password$/i);
      const confirmPasswordInput = screen.getByLabelText(/Confirm Password/i);
      const submitButton = screen.getByRole('button', { name: /Register/i });

      await user.clear(firstNameInput);
      await user.type(firstNameInput, 'John');
      await user.clear(lastNameInput);
      await user.type(lastNameInput, 'Doe');
      await user.clear(emailInput);
      await user.type(emailInput, 'john@example.com');
      await user.clear(passwordInput);
      await user.type(passwordInput, 'Password123');
      await user.clear(confirmPasswordInput);
      await user.type(confirmPasswordInput, 'Password123');

      await user.click(submitButton);

      await waitFor(() => {
        expect(registerUser).toHaveBeenCalledWith({
          email: 'john@example.com',
          password: 'Password123',
          first_name: 'John',
          last_name: 'Doe',
        });
      });
    });

    it('shows loading state during submission', async () => {
      const user = userEvent.setup();
      registerUser.mockImplementation(() => new Promise(resolve => setTimeout(resolve, 1000)));

      render(<RegisterForm />);

      const submitButton = screen.getByRole('button', { name: /Register/i });
      await user.click(submitButton);

      expect(screen.getByRole('progressbar')).toBeInTheDocument();
      expect(submitButton).toBeDisabled();
    });

    it('shows success message on successful registration', async () => {
      const user = userEvent.setup();
      registerUser.mockResolvedValue({ success: true });

      render(<RegisterForm />);

      const submitButton = screen.getByRole('button', { name: /Register/i });
      await user.click(submitButton);

      await waitFor(() => {
        expect(screen.getByText(/Registration successful!/i)).toBeInTheDocument();
      });
    });

    it('clears form fields after successful registration', async () => {
      const user = userEvent.setup();
      registerUser.mockResolvedValue({ success: true });

      render(<RegisterForm />);

      const submitButton = screen.getByRole('button', { name: /Register/i });
      await user.click(submitButton);

      await waitFor(() => {
        expect(screen.getByText(/Registration successful!/i)).toBeInTheDocument();
      });

      // Note: Form is replaced with success message, so fields won't be present
      expect(screen.queryByLabelText(/First Name/i)).not.toBeInTheDocument();
    });

    it('shows "Go to Login" button after successful registration', async () => {
      const user = userEvent.setup();
      registerUser.mockResolvedValue({ success: true });

      render(<RegisterForm />);

      const submitButton = screen.getByRole('button', { name: /Register/i });
      await user.click(submitButton);

      await waitFor(() => {
        expect(screen.getByText(/Go to Login/i)).toBeInTheDocument();
      });
    });

    it('navigates to login page when "Go to Login" is clicked', async () => {
      const user = userEvent.setup();
      registerUser.mockResolvedValue({ success: true });

      render(<RegisterForm />);

      const submitButton = screen.getByRole('button', { name: /Register/i });
      await user.click(submitButton);

      await waitFor(() => {
        expect(screen.getByText(/Go to Login/i)).toBeInTheDocument();
      });

      const goToLoginButton = screen.getByText(/Go to Login/i);
      await user.click(goToLoginButton);

      expect(mockPush).toHaveBeenCalledWith('/auth/login');
    });
  });

  describe('Password Validation', () => {
    it('shows error when passwords do not match', async () => {
      const user = userEvent.setup();
      render(<RegisterForm />);

      const passwordInput = screen.getByLabelText(/^Password$/i);
      const confirmPasswordInput = screen.getByLabelText(/Confirm Password/i);
      const submitButton = screen.getByRole('button', { name: /Register/i });

      await user.clear(passwordInput);
      await user.type(passwordInput, 'Password123');
      await user.clear(confirmPasswordInput);
      await user.type(confirmPasswordInput, 'DifferentPassword');

      await user.click(submitButton);

      await waitFor(() => {
        expect(screen.getByText(/Passwords do not match/i)).toBeInTheDocument();
      });
    });

    it('does not call registerUser when passwords do not match', async () => {
      const user = userEvent.setup();
      render(<RegisterForm />);

      const passwordInput = screen.getByLabelText(/^Password$/i);
      const confirmPasswordInput = screen.getByLabelText(/Confirm Password/i);
      const submitButton = screen.getByRole('button', { name: /Register/i });

      await user.clear(passwordInput);
      await user.type(passwordInput, 'Password123');
      await user.clear(confirmPasswordInput);
      await user.type(confirmPasswordInput, 'DifferentPassword');

      await user.click(submitButton);

      await waitFor(() => {
        expect(screen.getByText(/Passwords do not match/i)).toBeInTheDocument();
      });

      expect(registerUser).not.toHaveBeenCalled();
    });
  });

  describe('Error Handling', () => {
    it('displays error for 400 status code', async () => {
      const user = userEvent.setup();
      const errorMessage = 'Invalid email format';
      registerUser.mockRejectedValue({
        response: {
          status: 400,
          data: { detail: errorMessage },
        },
      });

      render(<RegisterForm />);

      const submitButton = screen.getByRole('button', { name: /Register/i });
      await user.click(submitButton);

      await waitFor(() => {
        expect(screen.getByText(errorMessage)).toBeInTheDocument();
      });
    });

    it('displays error for 409 status code (email exists)', async () => {
      const user = userEvent.setup();
      registerUser.mockRejectedValue({
        response: {
          status: 409,
        },
      });

      render(<RegisterForm />);

      const submitButton = screen.getByRole('button', { name: /Register/i });
      await user.click(submitButton);

      await waitFor(() => {
        expect(screen.getByText(/Email already registered/i)).toBeInTheDocument();
      });
    });

    it('displays error for 422 status code', async () => {
      const user = userEvent.setup();
      registerUser.mockRejectedValue({
        response: {
          status: 422,
          data: { detail: 'Validation error' },
        },
      });

      render(<RegisterForm />);

      const submitButton = screen.getByRole('button', { name: /Register/i });
      await user.click(submitButton);

      await waitFor(() => {
        expect(screen.getByText(/Validation error/i)).toBeInTheDocument();
      });
    });

    it('displays default error message for unknown status codes', async () => {
      const user = userEvent.setup();
      registerUser.mockRejectedValue({
        response: {
          status: 500,
        },
      });

      render(<RegisterForm />);

      const submitButton = screen.getByRole('button', { name: /Register/i });
      await user.click(submitButton);

      await waitFor(() => {
        expect(screen.getByText(/Registration failed. Please try again./i)).toBeInTheDocument();
      });
    });

    it('handles network errors', async () => {
      const user = userEvent.setup();
      registerUser.mockRejectedValue({
        request: {},
      });

      render(<RegisterForm />);

      const submitButton = screen.getByRole('button', { name: /Register/i });
      await user.click(submitButton);

      await waitFor(() => {
        expect(screen.getByText(/No response from server/i)).toBeInTheDocument();
      });
    });

    it('handles request setup errors', async () => {
      const user = userEvent.setup();
      registerUser.mockRejectedValue({
        message: 'Request setup failed',
      });

      render(<RegisterForm />);

      const submitButton = screen.getByRole('button', { name: /Register/i });
      await user.click(submitButton);

      await waitFor(() => {
        expect(screen.getByText(/Failed to send registration request/i)).toBeInTheDocument();
      });
    });

    it('clears error message on new submission', async () => {
      const user = userEvent.setup();
      registerUser
        .mockRejectedValueOnce({
          response: {
            status: 400,
            data: { detail: 'Error' },
          },
        })
        .mockResolvedValueOnce({ success: true });

      render(<RegisterForm />);

      const submitButton = screen.getByRole('button', { name: /Register/i });

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
  });

  describe('Accessibility', () => {
    it('has proper form labels', () => {
      render(<RegisterForm />);

      expect(screen.getByLabelText(/First Name/i)).toBeInTheDocument();
      expect(screen.getByLabelText(/Last Name/i)).toBeInTheDocument();
      expect(screen.getByLabelText(/Email Address/i)).toBeInTheDocument();
      expect(screen.getByLabelText(/^Password$/i)).toBeInTheDocument();
      expect(screen.getByLabelText(/Confirm Password/i)).toBeInTheDocument();
    });

    it('email input has type email', () => {
      render(<RegisterForm />);

      const emailInput = screen.getByLabelText(/Email Address/i);
      expect(emailInput).toHaveAttribute('type', 'email');
    });

    it('password inputs have type password', () => {
      render(<RegisterForm />);

      const passwordInput = screen.getByLabelText(/^Password$/i);
      const confirmPasswordInput = screen.getByLabelText(/Confirm Password/i);

      expect(passwordInput).toHaveAttribute('type', 'password');
      expect(confirmPasswordInput).toHaveAttribute('type', 'password');
    });
  });

  describe('Form Validation', () => {
    it('all fields are required', () => {
      render(<RegisterForm />);

      expect(screen.getByLabelText(/First Name/i)).toHaveAttribute('required');
      expect(screen.getByLabelText(/Last Name/i)).toHaveAttribute('required');
      expect(screen.getByLabelText(/Email Address/i)).toHaveAttribute('required');
      expect(screen.getByLabelText(/^Password$/i)).toHaveAttribute('required');
      expect(screen.getByLabelText(/Confirm Password/i)).toHaveAttribute('required');
    });

    it('has proper autocomplete attributes', () => {
      render(<RegisterForm />);

      expect(screen.getByLabelText(/First Name/i)).toHaveAttribute('autocomplete', 'given-name');
      expect(screen.getByLabelText(/Last Name/i)).toHaveAttribute('autocomplete', 'family-name');
      expect(screen.getByLabelText(/Email Address/i)).toHaveAttribute('autocomplete', 'email');
      expect(screen.getByLabelText(/^Password$/i)).toHaveAttribute('autocomplete', 'new-password');
      expect(screen.getByLabelText(/Confirm Password/i)).toHaveAttribute('autocomplete', 'new-password');
    });
  });
});
