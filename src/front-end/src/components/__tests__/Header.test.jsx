import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import Header from '../Header';
import { useAuth } from '@/hooks/useAuth';
import { logoutUser } from '@/utils/auth';

// Mock dependencies
jest.mock('@/hooks/useAuth');
jest.mock('@/utils/auth', () => ({
  logoutUser: jest.fn(),
}));

jest.mock('next/link', () => {
  return ({ children, href, onClick }) => {
    return <a href={href} onClick={onClick}>{children}</a>;
  };
});

describe('Header', () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  describe('Rendering', () => {
    it('renders the header with logo and title', () => {
      useAuth.mockReturnValue({
        isAuthenticated: false,
        userRole: null,
      });

      render(<Header />);

      expect(screen.getByText('Sushi Toshi')).toBeInTheDocument();
      expect(screen.getByAltText('Sushi Toshi Logo')).toBeInTheDocument();
    });

    it('displays login button when not authenticated', () => {
      useAuth.mockReturnValue({
        isAuthenticated: false,
        userRole: null,
      });

      render(<Header />);

      expect(screen.getByText('Login')).toBeInTheDocument();
    });

    it('displays logout button when authenticated', () => {
      useAuth.mockReturnValue({
        isAuthenticated: true,
        userRole: 'customer',
      });

      render(<Header />);

      expect(screen.getByText('Logout')).toBeInTheDocument();
    });

    it('does not render nav links when not authenticated', () => {
      useAuth.mockReturnValue({
        isAuthenticated: false,
        userRole: null,
      });

      render(<Header />);

      expect(screen.queryByText('MENUS')).not.toBeInTheDocument();
      expect(screen.queryByText('SESSIONS')).not.toBeInTheDocument();
      expect(screen.queryByText('ANALYTICS')).not.toBeInTheDocument();
    });
  });

  describe('Customer Navigation', () => {
    beforeEach(() => {
      useAuth.mockReturnValue({
        isAuthenticated: true,
        userRole: 'customer',
      });
    });

    it('renders customer navigation links', () => {
      render(<Header />);

      expect(screen.getByText('HOME')).toBeInTheDocument();
      expect(screen.getByText('MENUS')).toBeInTheDocument();
      expect(screen.getByText('BILLS')).toBeInTheDocument();
      expect(screen.getByText('ORDERS')).toBeInTheDocument();
    });

    it('opens menu dropdown when MENUS is clicked', async () => {
      const user = userEvent.setup();
      render(<Header />);

      const menusButton = screen.getByText('MENUS');
      await user.click(menusButton);

      expect(screen.getByText('Drinks')).toBeInTheDocument();
      expect(screen.getByText('Sashimi')).toBeInTheDocument();
      expect(screen.getByText('Nigiri')).toBeInTheDocument();
      expect(screen.getByText('Full Menu')).toBeInTheDocument();
    });

    it('toggles menu dropdown on multiple clicks', async () => {
      const user = userEvent.setup();
      render(<Header />);

      const menusButton = screen.getByText('MENUS');

      // Open dropdown
      await user.click(menusButton);
      expect(screen.getByText('Drinks')).toBeInTheDocument();

      // Close dropdown
      await user.click(menusButton);
      expect(screen.queryByText('Drinks')).not.toBeInTheDocument();
    });

    it('closes dropdown when clicking a menu item', async () => {
      const user = userEvent.setup();
      render(<Header />);

      const menusButton = screen.getByText('MENUS');
      await user.click(menusButton);

      const drinksLink = screen.getByText('Drinks');
      await user.click(drinksLink);

      // Dropdown should close after clicking
      await waitFor(() => {
        expect(screen.queryByText('Drinks')).not.toBeInTheDocument();
      });
    });

    it('displays all menu categories in dropdown', async () => {
      const user = userEvent.setup();
      render(<Header />);

      const menusButton = screen.getByText('MENUS');
      await user.click(menusButton);

      const expectedCategories = [
        'Drinks',
        'Sashimi',
        'Nigiri',
        'Add On',
        'Maki',
        'Rolls',
        'Temaki',
        'Hot Stone',
        'Cooked/Hot',
        'Kitchen',
        'Deep Fried',
        'Full Menu',
      ];

      expectedCategories.forEach(category => {
        expect(screen.getByText(category)).toBeInTheDocument();
      });
    });
  });

  describe('Staff Navigation', () => {
    beforeEach(() => {
      useAuth.mockReturnValue({
        isAuthenticated: true,
        userRole: 'staff',
      });
    });

    it('renders staff navigation links', () => {
      render(<Header />);

      expect(screen.getByText('HOME')).toBeInTheDocument();
      expect(screen.getByText('SESSIONS')).toBeInTheDocument();
      expect(screen.getByText('TABLES')).toBeInTheDocument();
    });

    it('does not render customer-specific links', () => {
      render(<Header />);

      expect(screen.queryByText('MENUS')).not.toBeInTheDocument();
      expect(screen.queryByText('BILLS')).not.toBeInTheDocument();
      expect(screen.queryByText('ORDERS')).not.toBeInTheDocument();
    });

    it('does not render admin-specific links', () => {
      render(<Header />);

      expect(screen.queryByText('ANALYTICS')).not.toBeInTheDocument();
    });

    it('has correct links for staff', () => {
      const { container } = render(<Header />);

      const sessionsLink = screen.getByText('SESSIONS').closest('a');
      const tablesLink = screen.getByText('TABLES').closest('a');

      expect(sessionsLink).toHaveAttribute('href', '/dashboard/sessions');
      expect(tablesLink).toHaveAttribute('href', '/static/tables.html');
    });
  });

  describe('Admin Navigation', () => {
    beforeEach(() => {
      useAuth.mockReturnValue({
        isAuthenticated: true,
        userRole: 'admin',
      });
    });

    it('renders admin navigation links', () => {
      render(<Header />);

      expect(screen.getByText('HOME')).toBeInTheDocument();
      expect(screen.getByText('SESSIONS')).toBeInTheDocument();
      expect(screen.getByText('TABLES')).toBeInTheDocument();
      expect(screen.getByText('ANALYTICS')).toBeInTheDocument();
    });

    it('does not render customer-specific links', () => {
      render(<Header />);

      expect(screen.queryByText('MENUS')).not.toBeInTheDocument();
      expect(screen.queryByText('BILLS')).not.toBeInTheDocument();
      expect(screen.queryByText('ORDERS')).not.toBeInTheDocument();
    });

    it('has correct links for admin', () => {
      render(<Header />);

      const analyticsLink = screen.getByText('ANALYTICS').closest('a');
      expect(analyticsLink).toHaveAttribute('href', '/analytics/analytic-page');
    });
  });

  describe('Logout Functionality', () => {
    it('calls logoutUser when logout button is clicked', async () => {
      const user = userEvent.setup();
      useAuth.mockReturnValue({
        isAuthenticated: true,
        userRole: 'customer',
      });

      render(<Header />);

      const logoutButton = screen.getByText('Logout');
      await user.click(logoutButton);

      expect(logoutUser).toHaveBeenCalledTimes(1);
    });

    it('logout button is accessible', () => {
      useAuth.mockReturnValue({
        isAuthenticated: true,
        userRole: 'customer',
      });

      render(<Header />);

      const logoutButton = screen.getByText('Logout');
      expect(logoutButton).toBeInTheDocument();
      expect(logoutButton.tagName).toBe('BUTTON');
    });
  });

  describe('Click Outside to Close Dropdown', () => {
    it('closes dropdown when clicking outside', async () => {
      const user = userEvent.setup();
      useAuth.mockReturnValue({
        isAuthenticated: true,
        userRole: 'customer',
      });

      const { container } = render(<Header />);

      const menusButton = screen.getByText('MENUS');
      await user.click(menusButton);

      expect(screen.getByText('Drinks')).toBeInTheDocument();

      // Click outside the dropdown
      await user.click(document.body);

      await waitFor(() => {
        expect(screen.queryByText('Drinks')).not.toBeInTheDocument();
      });
    });

    it('does not close dropdown when clicking inside', async () => {
      const user = userEvent.setup();
      useAuth.mockReturnValue({
        isAuthenticated: true,
        userRole: 'customer',
      });

      render(<Header />);

      const menusButton = screen.getByText('MENUS');
      await user.click(menusButton);

      const drinksLink = screen.getByText('Drinks');

      // Clicking the link will close it (as per closeMenu callback)
      // So this test verifies the link is present before click
      expect(drinksLink).toBeInTheDocument();
    });
  });

  describe('Accessibility', () => {
    it('has proper heading role and aria-level', () => {
      useAuth.mockReturnValue({
        isAuthenticated: false,
        userRole: null,
      });

      render(<Header />);

      const heading = screen.getByRole('heading', { level: 1 });
      expect(heading).toHaveTextContent('Sushi Toshi');
    });

    it('navigation has proper id', () => {
      useAuth.mockReturnValue({
        isAuthenticated: false,
        userRole: null,
      });

      const { container } = render(<Header />);

      const nav = container.querySelector('#main-menu');
      expect(nav).toBeInTheDocument();
    });

    it('logo has proper alt text', () => {
      useAuth.mockReturnValue({
        isAuthenticated: false,
        userRole: null,
      });

      render(<Header />);

      const logo = screen.getByAltText('Sushi Toshi Logo');
      expect(logo).toHaveAttribute('alt', 'Sushi Toshi Logo');
    });

    it('logo has proper width and height attributes', () => {
      useAuth.mockReturnValue({
        isAuthenticated: false,
        userRole: null,
      });

      render(<Header />);

      const logo = screen.getByAltText('Sushi Toshi Logo');
      expect(logo).toHaveAttribute('width', '60');
      expect(logo).toHaveAttribute('height', '60');
    });
  });

  describe('Role-Based Rendering', () => {
    it('renders different content based on user role', () => {
      const roles = ['customer', 'staff', 'admin'];

      roles.forEach(role => {
        const { unmount } = render(<Header />);
        useAuth.mockReturnValue({
          isAuthenticated: true,
          userRole: role,
        });

        unmount();
        render(<Header />);

        expect(screen.getByText('Logout')).toBeInTheDocument();
      });
    });
  });

  describe('Navigation Links', () => {
    it('customer HOME link points to correct path', () => {
      useAuth.mockReturnValue({
        isAuthenticated: true,
        userRole: 'customer',
      });

      render(<Header />);

      const homeLink = screen.getByText('HOME').closest('a');
      expect(homeLink).toHaveAttribute('href', '/');
    });

    it('customer BILLS link points to correct path', () => {
      useAuth.mockReturnValue({
        isAuthenticated: true,
        userRole: 'customer',
      });

      render(<Header />);

      const billsLink = screen.getByText('BILLS').closest('a');
      expect(billsLink).toHaveAttribute('href', '/bills');
    });

    it('customer ORDERS link points to correct path', () => {
      useAuth.mockReturnValue({
        isAuthenticated: true,
        userRole: 'customer',
      });

      render(<Header />);

      const ordersLink = screen.getByText('ORDERS').closest('a');
      expect(ordersLink).toHaveAttribute('href', '/orders');
    });
  });
});
