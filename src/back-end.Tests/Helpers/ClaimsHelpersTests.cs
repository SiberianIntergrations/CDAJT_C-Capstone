using Xunit;
using FluentAssertions;
using System.Security.Claims;
using back_end.Helpers;

namespace back_end.Tests.Helpers;

public class ClaimsHelpersTests
{
    [Fact]
    public void GetUserOid_WithObjectIdentifierClaim_ReturnsObjectIdentifier()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", "12345-abcde")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        // Act
        var result = ClaimsHelpers.GetUserOid(user);

        // Assert
        result.Should().Be("12345-abcde");
    }

    [Fact]
    public void GetUserOid_WithOidClaim_ReturnsOid()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim("oid", "oid-12345")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        // Act
        var result = ClaimsHelpers.GetUserOid(user);

        // Assert
        result.Should().Be("oid-12345");
    }

    [Fact]
    public void GetUserOid_WithPreferredUsernameClaim_ReturnsPreferredUsername()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim("preferred_username", "user@example.com")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        // Act
        var result = ClaimsHelpers.GetUserOid(user);

        // Assert
        result.Should().Be("user@example.com");
    }

    [Fact]
    public void GetUserOid_WithEmailClaim_ReturnsEmail()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim("email", "test@example.com")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        // Act
        var result = ClaimsHelpers.GetUserOid(user);

        // Assert
        result.Should().Be("test@example.com");
    }

    [Fact]
    public void GetUserOid_WithNameIdentifierClaim_ReturnsNameIdentifier()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "name-id-123")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        // Act
        var result = ClaimsHelpers.GetUserOid(user);

        // Assert
        result.Should().Be("name-id-123");
    }

    [Fact]
    public void GetUserOid_WithNoClaims_ReturnsEmptyString()
    {
        // Arrange
        var identity = new ClaimsIdentity(new List<Claim>(), "TestAuth");
        var user = new ClaimsPrincipal(identity);

        // Act
        var result = ClaimsHelpers.GetUserOid(user);

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void GetUserOid_WithMultipleClaims_ReturnsFirstPriority()
    {
        // Arrange - ObjectIdentifier should take precedence
        var claims = new List<Claim>
        {
            new Claim("email", "test@example.com"),
            new Claim("oid", "oid-12345"),
            new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", "obj-id-first")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        // Act
        var result = ClaimsHelpers.GetUserOid(user);

        // Assert
        result.Should().Be("obj-id-first", "ObjectIdentifier claim should take precedence");
    }

    [Fact]
    public void GetUserDisplayName_WithGivenAndFamilyName_ReturnsCombinedName()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim("given_name", "John"),
            new Claim("family_name", "Doe")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        // Act
        var result = ClaimsHelpers.GetUserDisplayName(user);

        // Assert
        result.Should().Be("John Doe");
    }

    [Fact]
    public void GetUserDisplayName_WithOnlyGivenName_ReturnsGivenName()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim("given_name", "John")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        // Act
        var result = ClaimsHelpers.GetUserDisplayName(user);

        // Assert
        result.Should().Be("John");
    }

    [Fact]
    public void GetUserDisplayName_WithOnlyFamilyName_ReturnsFamilyName()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim("family_name", "Doe")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        // Act
        var result = ClaimsHelpers.GetUserDisplayName(user);

        // Assert
        result.Should().Be("Doe");
    }

    [Fact]
    public void GetUserDisplayName_WithNameClaim_ReturnsName()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim("name", "John Doe")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        // Act
        var result = ClaimsHelpers.GetUserDisplayName(user);

        // Assert
        result.Should().Be("John Doe");
    }

    [Fact]
    public void GetUserDisplayName_WithWhitespaceNames_TrimsAndHandlesCorrectly()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim("given_name", "  John  "),
            new Claim("family_name", "  Doe  ")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        // Act
        var result = ClaimsHelpers.GetUserDisplayName(user);

        // Assert
        result.Should().Be("John Doe");
    }

    [Fact]
    public void GetUserDisplayName_WithNoClaims_ReturnsUnknown()
    {
        // Arrange
        var identity = new ClaimsIdentity(new List<Claim>(), "TestAuth");
        var user = new ClaimsPrincipal(identity);

        // Act
        var result = ClaimsHelpers.GetUserDisplayName(user);

        // Assert
        result.Should().Be("Unknown");
    }

    [Fact]
    public void GetUserDisplayName_WithEmptyNames_ReturnsUnknown()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim("given_name", "   "),
            new Claim("family_name", "   ")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        // Act
        var result = ClaimsHelpers.GetUserDisplayName(user);

        // Assert
        result.Should().Be("Unknown");
    }

    [Fact]
    public void GetUserDisplayName_GivenAndFamilyTakePrecedenceOverName()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim("given_name", "John"),
            new Claim("family_name", "Doe"),
            new Claim("name", "Jane Smith")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        // Act
        var result = ClaimsHelpers.GetUserDisplayName(user);

        // Assert
        result.Should().Be("John Doe", "given_name and family_name should take precedence over name claim");
    }

    [Fact]
    public void GetUserRole_WithClaimTypesRole_ReturnsRole()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Role, "Admin")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        // Act
        var result = ClaimsHelpers.GetUserRole(user);

        // Assert
        result.Should().Be("Admin");
    }

    [Fact]
    public void GetUserRole_WithRolesClaim_ReturnsRole()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim("roles", "user.Staff")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        // Act
        var result = ClaimsHelpers.GetUserRole(user);

        // Assert
        result.Should().Be("user.Staff");
    }

    [Fact]
    public void GetUserRole_WithMicrosoftRoleClaim_ReturnsRole()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "user.Admin")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        // Act
        var result = ClaimsHelpers.GetUserRole(user);

        // Assert
        result.Should().Be("user.Admin");
    }

    [Fact]
    public void GetUserRole_WithNoRoleClaim_ReturnsNull()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim("name", "John Doe")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        // Act
        var result = ClaimsHelpers.GetUserRole(user);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetUserRole_WithNullUser_ReturnsNull()
    {
        // Act
        var result = ClaimsHelpers.GetUserRole(null);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetUserRole_WithMultipleRoleClaims_ReturnsFirstMatch()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Role, "FirstRole"),
            new Claim("roles", "SecondRole")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        // Act
        var result = ClaimsHelpers.GetUserRole(user);

        // Assert
        result.Should().Be("FirstRole", "Should return the first matching role claim");
    }
}
