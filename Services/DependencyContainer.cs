using System;
using Manajemen_Inventaris.DataAccess;
using Manajemen_Inventaris.Services;

namespace Manajemen_Inventaris.Services
{
    /// <summary>
    /// Container for managing dependencies
    /// </summary>
    public static class DependencyContainer
    {
        private static IDataAccess _dataAccess;
        private static IUserRepository _userRepository;
        private static IAuthService _authService;
        private static ICategoryRepository _categoryRepository;
        private static IItemRepository _itemRepository;
        private static IInventoryService _inventoryService;

        /// <summary>
        /// Gets the data access instance
        /// </summary>
        public static IDataAccess DataAccess
        {
            get
            {
                if (_dataAccess == null)
                {
                    _dataAccess = new SqlDataAccess();
                }
                return _dataAccess;
            }
        }

        /// <summary>
        /// Gets the user repository instance
        /// </summary>
        public static IUserRepository UserRepository
        {
            get
            {
                if (_userRepository == null)
                {
                    _userRepository = new UserRepository(DataAccess);
                }
                return _userRepository;
            }
        }

        /// <summary>
        /// Gets the authentication service instance
        /// </summary>
        public static IAuthService AuthService
        {
            get
            {
                if (_authService == null)
                {
                    _authService = new AuthService(UserRepository);
                }
                return _authService;
            }
        }

        /// <summary>
        /// Gets the category repository instance
        /// </summary>
        public static ICategoryRepository CategoryRepository
        {
            get
            {
                if (_categoryRepository == null)
                {
                    _categoryRepository = new CategoryRepository(DataAccess);
                }
                return _categoryRepository;
            }
        }

        /// <summary>
        /// Gets the item repository instance
        /// </summary>
        public static IItemRepository ItemRepository
        {
            get
            {
                if (_itemRepository == null)
                {
                    _itemRepository = new ItemRepository(DataAccess);
                }
                return _itemRepository;
            }
        }

        /// <summary>
        /// Gets the inventory service instance
        /// </summary>
        public static IInventoryService InventoryService
        {
            get
            {
                if (_inventoryService == null)
                {
                    _inventoryService = new InventoryService(CategoryRepository, ItemRepository);
                }
                return _inventoryService;
            }
        }

        /// <summary>
        /// Resets all dependencies (useful for testing)
        /// </summary>
        public static void Reset()
        {
            _dataAccess = null;
            _userRepository = null;
            _authService = null;
            _categoryRepository = null;
            _itemRepository = null;
            _inventoryService = null;
        }
    }
}