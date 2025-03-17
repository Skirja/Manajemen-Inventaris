using System;
using Manajemen_Inventaris.DataAccess;
using Manajemen_Inventaris.Models;

namespace Manajemen_Inventaris.Services
{
    /// <summary>
    /// Implementation of the authentication service
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Creates a new instance of the AuthService class
        /// </summary>
        /// <param name="userRepository">The user repository to use</param>
        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Authenticates a user with the given credentials
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        /// <returns>The authenticated user, or null if authentication fails</returns>
        public User Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return null;
            }

            return _userRepository.ValidateUser(username, password);
        }

        /// <summary>
        /// Registers a new user
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        /// <param name="email">The email address</param>
        /// <param name="company">The company name</param>
        /// <returns>The registered user, or null if registration fails</returns>
        public User Register(string username, string password, string email, string company)
        {
            // Validate input
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return null;
            }

            // Check if username is available
            if (!IsUsernameAvailable(username))
            {
                return null;
            }

            // Create user object
            var user = new User
            {
                Username = username,
                Password = password, // In a real app, this would be hashed
                Email = email,
                Company = company
            };

            // Save user to database
            int userId = _userRepository.Create(user);
            if (userId <= 0)
            {
                return null;
            }

            // Set the ID and return the user
            user.UserID = userId;
            return user;
        }

        /// <summary>
        /// Checks if a username is available
        /// </summary>
        /// <param name="username">The username to check</param>
        /// <returns>True if the username is available, otherwise false</returns>
        public bool IsUsernameAvailable(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return false;
            }

            return !_userRepository.UsernameExists(username);
        }
    }
}