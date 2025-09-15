using Dealership.Api.Models;

namespace Dealership.Api.Services;

// Responsible for creating JWT tokens for authenticated users
public interface IJwtService
{
    string CreateToken(User user);
}
