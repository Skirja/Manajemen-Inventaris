using System;

namespace Manajemen_Inventaris.Models
{
    /// <summary>
    /// Represents a history record for inventory item changes
    /// </summary>
    public class ItemHistory
    {
        /// <summary>
        /// Gets or sets the history ID
        /// </summary>
        public int HistoryID { get; set; }

        /// <summary>
        /// Gets or sets the item ID
        /// </summary>
        public int ItemID { get; set; }

        /// <summary>
        /// Gets or sets the item name (for display purposes)
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// Gets or sets the type of change
        /// </summary>
        public string ChangeType { get; set; }

        /// <summary>
        /// Gets or sets the quantity changed
        /// </summary>
        public int QuantityChanged { get; set; }

        /// <summary>
        /// Gets or sets the previous quantity
        /// </summary>
        public int PreviousQuantity { get; set; }

        /// <summary>
        /// Gets or sets the new quantity
        /// </summary>
        public int NewQuantity { get; set; }

        /// <summary>
        /// Gets or sets the notes
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user who made the change
        /// </summary>
        public int ChangedBy { get; set; }

        /// <summary>
        /// Gets or sets the username of the user who made the change (for display purposes)
        /// </summary>
        public string ChangedByUsername { get; set; }

        /// <summary>
        /// Gets or sets the date when the change was made
        /// </summary>
        public DateTime ChangedDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the item has AI tags
        /// </summary>
        public bool HasAITags { get; set; }

        /// <summary>
        /// Gets or sets the count of AI tags
        /// </summary>
        public int AITagsCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the item has an image
        /// </summary>
        public bool HasImage { get; set; }
    }
}