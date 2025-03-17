using System;
using Manajemen_Inventaris.Models;

namespace Manajemen_Inventaris.Services
{
    /// <summary>
    /// Interface for authentication services
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Authenticates a user with the given credentials
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        /// <returns>The authenticated user, or null if authentication fails</returns>
        User Authenticate(string username, string password);

        /// <summary>
        /// Registers a new user
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        /// <param name="email">The email address</param>
        /// <param name="company">The company name</param>
        /// <returns>The registered user, or null if registration fails</returns>
        User Register(string username, string password, string email, string company);

        /// <summary>
        /// Checks if a username is available
        /// </summary>
        /// <param name="username">The username to check</param>
        /// <returns>True if the username is available, otherwise false</returns>
        bool IsUsernameAvailable(string username);
    }
}