using Ara.Domain.Entities;

namespace Ara.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
