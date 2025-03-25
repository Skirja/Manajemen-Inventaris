using System;

namespace Manajemen_Inventaris.Models
{
    /// <summary>
    /// Model for category suggestions from AI service
    /// </summary>
    public class CategorySuggestionModel
    {
        /// <summary>
        /// Gets or sets the category name
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the category ID
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Gets or sets the confidence score
        /// </summary>
        public double Confidence { get; set; }
    }
}