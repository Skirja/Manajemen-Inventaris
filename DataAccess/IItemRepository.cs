using System.Collections.Generic;
using Manajemen_Inventaris.Models;

namespace Manajemen_Inventaris.DataAccess
{
    /// <summary>
    /// Interface for item repository operations
    /// </summary>
    public interface IItemRepository
    {
        /// <summary>
        /// Gets all items
        /// </summary>
        /// <returns>A list of items</returns>
        List<Item> GetAllItems();

        /// <summary>
        /// Gets items by category ID
        /// </summary>
        /// <param name="categoryId">The category ID</param>
        /// <returns>A list of items in the specified category</returns>
        List<Item> GetItemsByCategoryId(int categoryId);

        /// <summary>
        /// Gets an item by ID
        /// </summary>
        /// <param name="itemId">The item ID</param>
        /// <returns>The item, or null if not found</returns>
        Item GetItemById(int itemId);

        /// <summary>
        /// Searches for items by name or description
        /// </summary>
        /// <param name="searchTerm">The search term</param>
        /// <returns>A list of matching items</returns>
        List<Item> SearchItems(string searchTerm);

        /// <summary>
        /// Creates a new item
        /// </summary>
        /// <param name="item">The item to create</param>
        /// <returns>The ID of the created item</returns>
        int CreateItem(Item item);

        /// <summary>
        /// Updates an item
        /// </summary>
        /// <param name="item">The item to update</param>
        /// <returns>True if successful, false otherwise</returns>
        bool UpdateItem(Item item);

        /// <summary>
        /// Deletes an item
        /// </summary>
        /// <param name="itemId">The ID of the item to delete</param>
        /// <returns>True if successful, false otherwise</returns>
        bool DeleteItem(int itemId);

        /// <summary>
        /// Adjusts the quantity of an item
        /// </summary>
        /// <param name="itemId">The item ID</param>
        /// <param name="quantityChange">The quantity change (positive for stock in, negative for stock out)</param>
        /// <param name="notes">Notes about the adjustment</param>
        /// <param name="userId">The ID of the user making the adjustment</param>
        /// <returns>True if successful, false otherwise</returns>
        bool AdjustItemQuantity(int itemId, int quantityChange, string notes, int userId);

        /// <summary>
        /// Gets the history for an item
        /// </summary>
        /// <param name="itemId">The item ID</param>
        /// <returns>A list of history records for the item</returns>
        List<ItemHistory> GetItemHistory(int itemId);

        /// <summary>
        /// Gets low stock items
        /// </summary>
        /// <param name="threshold">The low stock threshold</param>
        /// <returns>A list of items with stock below the threshold</returns>
        List<Item> GetLowStockItems(int threshold);
    }
}