using System;
using System.Collections.Generic;
using Manajemen_Inventaris.Models;

namespace Manajemen_Inventaris.DataAccess
{
    /// <summary>
    /// Interface for user repository operations
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Gets a user by ID
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>The user, or null if not found</returns>
        User GetById(int userId);

        /// <summary>
        /// Gets a user by username
        /// </summary>
        /// <param name="username">The username</param>
        /// <returns>The user, or null if not found</returns>
        User GetByUsername(string username);

        /// <summary>
        /// Validates user credentials
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        /// <returns>The user if credentials are valid, otherwise null</returns>
        User ValidateUser(string username, string password);

        /// <summary>
        /// Checks if a username already exists
        /// </summary>
        /// <param name="username">The username to check</param>
        /// <returns>True if the username exists, otherwise false</returns>
        bool UsernameExists(string username);

        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="user">The user to create</param>
        /// <returns>The ID of the created user</returns>
        int Create(User user);

        /// <summary>
        /// Updates an existing user
        /// </summary>
        /// <param name="user">The user to update</param>
        /// <returns>True if the update was successful, otherwise false</returns>
        bool Update(User user);

        /// <summary>
        /// Deletes a user
        /// </summary>
        /// <param name="userId">The ID of the user to delete</param>
        /// <returns>True if the deletion was successful, otherwise false</returns>
        bool Delete(int userId);
    }
}