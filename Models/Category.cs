using System;

namespace Manajemen_Inventaris.Models
{
    /// <summary>
    /// Represents a category for inventory items
    /// </summary>
    public class Category
    {
        /// <summary>
        /// Gets or sets the category ID
        /// </summary>
        public int CategoryID { get; set; }

        /// <summary>
        /// Gets or sets the category name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the category description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user who created the category
        /// </summary>
        public int CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the date when the category was created
        /// </summary>
        public DateTime CreatedDate { get; set; }
    }
}