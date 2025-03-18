using System;

namespace Manajemen_Inventaris.Models
{
    /// <summary>
    /// Category with item count for display purposes
    /// </summary>
    public class CategoryWithItemCount : Category
    {
        /// <summary>
        /// Number of items in this category
        /// </summary>
        public int ItemCount { get; set; }
    }
}