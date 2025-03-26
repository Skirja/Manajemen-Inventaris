using System;
using System.Collections.Generic;
using System.Data;
using Manajemen_Inventaris.Models;

namespace Manajemen_Inventaris.DataAccess
{
    /// <summary>
    /// Repository for item operations
    /// </summary>
    public class ItemRepository : IItemRepository
    {
        private readonly IDataAccess _dataAccess;

        /// <summary>
        /// Initializes a new instance of the ItemRepository class
        /// </summary>
        /// <param name="dataAccess">The data access instance</param>
        public ItemRepository(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        /// <summary>
        /// Gets all items
        /// </summary>
        /// <returns>A list of items</returns>
        public List<Item> GetAllItems()
        {
            string sql = @"
                SELECT i.*, c.Name AS CategoryName
                FROM Items i
                INNER JOIN Categories c ON i.CategoryID = c.CategoryID
                ORDER BY i.Name";

            DataTable dataTable = _dataAccess.ExecuteQuery(sql);

            List<Item> items = new List<Item>();
            foreach (DataRow row in dataTable.Rows)
            {
                items.Add(MapRowToItem(row));
            }

            return items;
        }

        /// <summary>
        /// Gets items by category ID
        /// </summary>
        /// <param name="categoryId">The category ID</param>
        /// <returns>A list of items in the specified category</returns>
        public List<Item> GetItemsByCategoryId(int categoryId)
        {
            string sql = @"
                SELECT i.*, c.Name AS CategoryName
                FROM Items i
                INNER JOIN Categories c ON i.CategoryID = c.CategoryID
                WHERE i.CategoryID = @CategoryID
                ORDER BY i.Name";

            var parameters = new Dictionary<string, object>
            {
                { "@CategoryID", categoryId }
            };

            DataTable dataTable = _dataAccess.ExecuteQuery(sql, parameters);

            List<Item> items = new List<Item>();
            foreach (DataRow row in dataTable.Rows)
            {
                items.Add(MapRowToItem(row));
            }

            return items;
        }

        /// <summary>
        /// Gets an item by ID
        /// </summary>
        /// <param name="itemId">The item ID</param>
        /// <returns>The item, or null if not found</returns>
        public Item GetItemById(int itemId)
        {
            string sql = @"
                SELECT i.*, c.Name AS CategoryName
                FROM Items i
                INNER JOIN Categories c ON i.CategoryID = c.CategoryID
                WHERE i.ItemID = @ItemID";

            var parameters = new Dictionary<string, object>
            {
                { "@ItemID", itemId }
            };

            DataTable dataTable = _dataAccess.ExecuteQuery(sql, parameters);
            if (dataTable.Rows.Count == 0)
            {
                return null;
            }

            return MapRowToItem(dataTable.Rows[0]);
        }

        /// <summary>
        /// Searches for items by name or description
        /// </summary>
        /// <param name="searchTerm">The search term</param>
        /// <returns>A list of matching items</returns>
        public List<Item> SearchItems(string searchTerm)
        {
            string sql = @"
                SELECT i.*, c.Name AS CategoryName
                FROM Items i
                INNER JOIN Categories c ON i.CategoryID = c.CategoryID
                WHERE i.Name LIKE @SearchTerm OR i.Description LIKE @SearchTerm OR i.AITags LIKE @SearchTerm
                ORDER BY i.Name";

            var parameters = new Dictionary<string, object>
            {
                { "@SearchTerm", "%" + searchTerm + "%" }
            };

            DataTable dataTable = _dataAccess.ExecuteQuery(sql, parameters);

            List<Item> items = new List<Item>();
            foreach (DataRow row in dataTable.Rows)
            {
                items.Add(MapRowToItem(row));
            }

            return items;
        }

        /// <summary>
        /// Creates a new item
        /// </summary>
        /// <param name="item">The item to create</param>
        /// <returns>The ID of the created item</returns>
        public int CreateItem(Item item)
        {
            string sql = @"
                INSERT INTO Items (Name, Description, CategoryID, Quantity, ImagePath, AITags, CreatedBy, CreatedDate, LastModifiedBy, LastModifiedDate)
                VALUES (@Name, @Description, @CategoryID, @Quantity, @ImagePath, @AITags, @CreatedBy, @CreatedDate, @LastModifiedBy, @LastModifiedDate);
                SELECT SCOPE_IDENTITY();";

            var parameters = new Dictionary<string, object>
            {
                { "@Name", item.Name },
                { "@Description", item.Description },
                { "@CategoryID", item.CategoryID },
                { "@Quantity", item.Quantity },
                { "@ImagePath", item.ImagePath },
                { "@AITags", item.AITags },
                { "@CreatedBy", item.CreatedBy },
                { "@CreatedDate", DateTime.Now },
                { "@LastModifiedBy", item.LastModifiedBy },
                { "@LastModifiedDate", DateTime.Now }
            };

            object result = _dataAccess.ExecuteScalar(sql, parameters);
            int itemId = Convert.ToInt32(result);

            // Add history record for item creation
            if (item.Quantity > 0)
            {
                AddItemHistory(itemId, "Created", item.Quantity, 0, item.Quantity, "Initial stock", item.CreatedBy);
            }

            return itemId;
        }

        /// <summary>
        /// Updates an item
        /// </summary>
        /// <param name="item">The item to update</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool UpdateItem(Item item)
        {
            // Get the current item to check for quantity changes
            Item currentItem = GetItemById(item.ItemID);
            if (currentItem == null)
            {
                return false;
            }

            string sql = @"
                UPDATE Items
                SET Name = @Name,
                    Description = @Description,
                    CategoryID = @CategoryID,
                    Quantity = @Quantity,
                    ImagePath = @ImagePath,
                    AITags = @AITags,
                    LastModifiedBy = @LastModifiedBy,
                    LastModifiedDate = @LastModifiedDate
                WHERE ItemID = @ItemID";

            var parameters = new Dictionary<string, object>
            {
                { "@ItemID", item.ItemID },
                { "@Name", item.Name },
                { "@Description", item.Description },
                { "@CategoryID", item.CategoryID },
                { "@Quantity", item.Quantity },
                { "@ImagePath", item.ImagePath },
                { "@AITags", item.AITags },
                { "@LastModifiedBy", item.LastModifiedBy },
                { "@LastModifiedDate", DateTime.Now }
            };

            int rowsAffected = _dataAccess.ExecuteNonQuery(sql, parameters);

            // Add history record if quantity changed
            if (rowsAffected > 0 && currentItem.Quantity != item.Quantity)
            {
                int quantityChange = item.Quantity - currentItem.Quantity;
                string changeType = quantityChange > 0 ? "StockIn" : "StockOut";
                AddItemHistory(item.ItemID, changeType, Math.Abs(quantityChange), currentItem.Quantity, item.Quantity, "Updated via edit", item.LastModifiedBy);
            }

            return rowsAffected > 0;
        }

        /// <summary>
        /// Deletes an item
        /// </summary>
        /// <param name="itemId">The ID of the item to delete</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool DeleteItem(int itemId)
        {
            // First delete history records
            string historyDeleteSql = "DELETE FROM ItemHistory WHERE ItemID = @ItemID";
            var historyDeleteParameters = new Dictionary<string, object>
            {
                { "@ItemID", itemId }
            };
            _dataAccess.ExecuteNonQuery(historyDeleteSql, historyDeleteParameters);

            // Then delete the item
            string sql = "DELETE FROM Items WHERE ItemID = @ItemID";
            var parameters = new Dictionary<string, object>
            {
                { "@ItemID", itemId }
            };

            int rowsAffected = _dataAccess.ExecuteNonQuery(sql, parameters);
            return rowsAffected > 0;
        }

        /// <summary>
        /// Adjusts the quantity of an item
        /// </summary>
        /// <param name="itemId">The item ID</param>
        /// <param name="quantityChange">The quantity change (positive for stock in, negative for stock out)</param>
        /// <param name="notes">Notes about the adjustment</param>
        /// <param name="userId">The ID of the user making the adjustment</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool AdjustItemQuantity(int itemId, int quantityChange, string notes, int userId)
        {
            // Get the current item
            Item item = GetItemById(itemId);
            if (item == null)
            {
                return false;
            }

            // Calculate new quantity
            int newQuantity = item.Quantity + quantityChange;
            if (newQuantity < 0)
            {
                // Cannot have negative quantity
                return false;
            }

            // Update the item quantity
            string sql = @"
                UPDATE Items
                SET Quantity = @Quantity,
                    LastModifiedBy = @LastModifiedBy,
                    LastModifiedDate = @LastModifiedDate
                WHERE ItemID = @ItemID";

            var parameters = new Dictionary<string, object>
            {
                { "@ItemID", itemId },
                { "@Quantity", newQuantity },
                { "@LastModifiedBy", userId },
                { "@LastModifiedDate", DateTime.Now }
            };

            int rowsAffected = _dataAccess.ExecuteNonQuery(sql, parameters);
            if (rowsAffected > 0)
            {
                // Add history record
                string changeType = quantityChange > 0 ? "StockIn" : "StockOut";
                AddItemHistory(itemId, changeType, Math.Abs(quantityChange), item.Quantity, newQuantity, notes, userId);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the history for an item
        /// </summary>
        /// <param name="itemId">The item ID</param>
        /// <returns>A list of history records for the item</returns>
        public List<ItemHistory> GetItemHistory(int itemId)
        {
            string sql = @"
                SELECT h.*, i.Name AS ItemName, u.Username AS ChangedByUsername
                FROM ItemHistory h
                INNER JOIN Items i ON h.ItemID = i.ItemID
                INNER JOIN Users u ON h.ChangedBy = u.UserID
                WHERE h.ItemID = @ItemID
                ORDER BY h.ChangedDate DESC";

            var parameters = new Dictionary<string, object>
            {
                { "@ItemID", itemId }
            };

            DataTable dataTable = _dataAccess.ExecuteQuery(sql, parameters);

            List<ItemHistory> history = new List<ItemHistory>();
            foreach (DataRow row in dataTable.Rows)
            {
                history.Add(MapRowToItemHistory(row));
            }

            return history;
        }

        /// <summary>
        /// Gets all item history records
        /// </summary>
        /// <returns>A list of all item history records</returns>
        public List<ItemHistory> GetItemHistory()
        {
            string sql = @"
                SELECT ih.*, i.Name AS ItemName, u.Username AS ChangedByUsername
                FROM ItemHistory ih
                INNER JOIN Items i ON ih.ItemID = i.ItemID
                INNER JOIN Users u ON ih.ChangedBy = u.UserID
                ORDER BY ih.ChangedDate DESC";

            DataTable dataTable = _dataAccess.ExecuteQuery(sql);

            List<ItemHistory> history = new List<ItemHistory>();
            foreach (DataRow row in dataTable.Rows)
            {
                history.Add(MapRowToItemHistory(row));
            }

            return history;
        }

        /// <summary>
        /// Gets low stock items
        /// </summary>
        /// <param name="threshold">The low stock threshold</param>
        /// <returns>A list of items with stock below the threshold</returns>
        public List<Item> GetLowStockItems(int threshold)
        {
            string sql = @"
                SELECT i.*, c.Name AS CategoryName
                FROM Items i
                INNER JOIN Categories c ON i.CategoryID = c.CategoryID
                WHERE i.Quantity <= @Threshold
                ORDER BY i.Quantity, i.Name";

            var parameters = new Dictionary<string, object>
            {
                { "@Threshold", threshold }
            };

            DataTable dataTable = _dataAccess.ExecuteQuery(sql, parameters);

            List<Item> items = new List<Item>();
            foreach (DataRow row in dataTable.Rows)
            {
                items.Add(MapRowToItem(row));
            }

            return items;
        }

        /// <summary>
        /// Adds a history record for an item
        /// </summary>
        /// <param name="history">The history record to add</param>
        /// <returns>The ID of the created history record</returns>
        public int AddItemHistory(ItemHistory history)
        {
            string sql = @"
                INSERT INTO ItemHistory (ItemID, ChangeType, QuantityChanged, PreviousQuantity, NewQuantity, Notes, ChangedBy, ChangedDate)
                VALUES (@ItemID, @ChangeType, @QuantityChanged, @PreviousQuantity, @NewQuantity, @Notes, @ChangedBy, @ChangedDate);
                SELECT SCOPE_IDENTITY();";

            var parameters = new Dictionary<string, object>
            {
                { "@ItemID", history.ItemID },
                { "@ChangeType", history.ChangeType },
                { "@QuantityChanged", history.QuantityChanged },
                { "@PreviousQuantity", history.PreviousQuantity },
                { "@NewQuantity", history.NewQuantity },
                { "@Notes", history.Notes },
                { "@ChangedBy", history.ChangedBy },
                { "@ChangedDate", history.ChangedDate }
            };

            return Convert.ToInt32(_dataAccess.ExecuteScalar(sql, parameters));
        }

        private bool AddItemHistory(int itemId, string changeType, int quantityChanged, int previousQuantity, int newQuantity, string notes, int userId)
        {
            var history = new ItemHistory
            {
                ItemID = itemId,
                ChangeType = changeType,
                QuantityChanged = quantityChanged,
                PreviousQuantity = previousQuantity,
                NewQuantity = newQuantity,
                Notes = notes,
                ChangedBy = userId,
                ChangedDate = DateTime.Now
            };

            return AddItemHistory(history) > 0;
        }

        private Item MapRowToItem(DataRow row)
        {
            return new Item
            {
                ItemID = Convert.ToInt32(row["ItemID"]),
                Name = row["Name"].ToString(),
                Description = row["Description"] != DBNull.Value ? row["Description"].ToString() : null,
                CategoryID = Convert.ToInt32(row["CategoryID"]),
                CategoryName = row["CategoryName"].ToString(),
                Quantity = Convert.ToInt32(row["Quantity"]),
                ImagePath = row["ImagePath"] != DBNull.Value ? row["ImagePath"].ToString() : null,
                AITags = row["AITags"] != DBNull.Value ? row["AITags"].ToString() : null,
                CreatedBy = Convert.ToInt32(row["CreatedBy"]),
                CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                LastModifiedBy = Convert.ToInt32(row["LastModifiedBy"]),
                LastModifiedDate = Convert.ToDateTime(row["LastModifiedDate"])
            };
        }

        private ItemHistory MapRowToItemHistory(DataRow row)
        {
            return new ItemHistory
            {
                HistoryID = Convert.ToInt32(row["HistoryID"]),
                ItemID = Convert.ToInt32(row["ItemID"]),
                ItemName = row["ItemName"].ToString(),
                ChangeType = row["ChangeType"].ToString(),
                QuantityChanged = Convert.ToInt32(row["QuantityChanged"]),
                PreviousQuantity = Convert.ToInt32(row["PreviousQuantity"]),
                NewQuantity = Convert.ToInt32(row["NewQuantity"]),
                Notes = row["Notes"] != DBNull.Value ? row["Notes"].ToString() : null,
                ChangedBy = Convert.ToInt32(row["ChangedBy"]),
                ChangedByUsername = row["ChangedByUsername"].ToString(),
                ChangedDate = Convert.ToDateTime(row["ChangedDate"])
            };
        }
    }
}