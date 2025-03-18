using System;

namespace Manajemen_Inventaris.Models
{
    /// <summary>
    /// Represents an inventory item
    /// </summary>
    public class Item
    {
        /// <summary>
        /// Gets or sets the item ID
        /// </summary>
        public int ItemID { get; set; }

        /// <summary>
        /// Gets or sets the item name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the item description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the category ID
        /// </summary>
        public int CategoryID { get; set; }

        /// <summary>
        /// Gets or sets the category name (for display purposes)
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// Gets or sets the quantity in stock
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the image path
        /// </summary>
        public string ImagePath { get; set; }

        /// <summary>
        /// Gets or sets the AI-generated tags
        /// </summary>
        public string AITags { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user who created the item
        /// </summary>
        public int CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the date when the item was created
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user who last modified the item
        /// </summary>
        public int LastModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the date when the item was last modified
        /// </summary>
        public DateTime LastModifiedDate { get; set; }
    }
}