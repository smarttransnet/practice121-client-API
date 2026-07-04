using Domain.Doctors;
using Domain.Users;

namespace Application.Abstractions.Authentication;

public interface ITokenProvider
{
    string Create(User user);
    string Create(DoctorAccount account);
    string CreateRefreshToken();
}

