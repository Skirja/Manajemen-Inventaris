using System;
using Manajemen_Inventaris.Models;

namespace Manajemen_Inventaris.Services
{
    /// <summary>
    /// Interface for user management services
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Checks if the user is logged in
        /// </summary>
        /// <returns>True if the user is logged in, otherwise false</returns>
        bool IsLoggedIn();

        /// <summary>
        /// Gets the currently logged in user
        /// </summary>
        /// <returns>The logged in user, or null if no user is logged in</returns>
        User GetLoggedInUser();

        /// <summary>
        /// Gets a user by their ID
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>The user, or null if not found</returns>
        User GetUserById(int userId);

        /// <summary>
        /// Logs the user out
        /// </summary>
        void Logout();
    }
}