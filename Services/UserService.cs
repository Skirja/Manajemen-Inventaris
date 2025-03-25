using System;
using System.Web;
using Manajemen_Inventaris.DataAccess;
using Manajemen_Inventaris.Models;

namespace Manajemen_Inventaris.Services
{
    /// <summary>
    /// Implementation of the user service
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Creates a new instance of the UserService class
        /// </summary>
        public UserService()
        {
            _userRepository = DependencyContainer.UserRepository;
        }

        /// <summary>
        /// Checks if the user is logged in
        /// </summary>
        /// <returns>True if the user is logged in, otherwise false</returns>
        public bool IsLoggedIn()
        {
            return HttpContext.Current.Session["UserID"] != null;
        }

        /// <summary>
        /// Gets the currently logged in user
        /// </summary>
        /// <returns>The logged in user, or null if no user is logged in</returns>
        public User GetLoggedInUser()
        {
            if (!IsLoggedIn())
            {
                return null;
            }

            int userId = Convert.ToInt32(HttpContext.Current.Session["UserID"]);
            return GetUserById(userId);
        }

        /// <summary>
        /// Gets a user by their ID
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>The user, or null if not found</returns>
        public User GetUserById(int userId)
        {
            return _userRepository.GetById(userId);
        }

        /// <summary>
        /// Logs the user out
        /// </summary>
        public void Logout()
        {
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.Abandon();
        }
    }
}