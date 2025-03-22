using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Manajemen_Inventaris.Services
{
    /// <summary>
    /// Interface for AI service operations
    /// </summary>
    public interface IAIService
    {
        /// <summary>
        /// Generates tags for an item based on its image
        /// </summary>
        /// <param name="imageBase64">Base64-encoded image data</param>
        /// <param name="existingTags">Optional existing tags to enhance</param>
        /// <param name="category">Optional category context</param>
        /// <returns>A collection of generated tags</returns>
        Task<List<string>> GenerateTagsAsync(string imageBase64, List<string> existingTags = null, string category = null);

        /// <summary>
        /// Suggests a category for an item based on its image
        /// </summary>
        /// <param name="imageBase64">Base64-encoded image data</param>
        /// <param name="existingCategories">List of existing categories in the system</param>
        /// <returns>A ranked list of suggested categories with confidence scores</returns>
        Task<Dictionary<string, double>> SuggestCategoryAsync(string imageBase64, List<string> existingCategories);

        /// <summary>
        /// Enhances search queries to include semantically related terms
        /// </summary>
        /// <param name="query">Original search query</param>
        /// <param name="currentCategory">Optional current category context</param>
        /// <returns>An enhanced search query with related terms</returns>
        Task<string> EnhanceSearchAsync(string query, string currentCategory = null);

        /// <summary>
        /// Generates a description for an item based on its image and metadata
        /// </summary>
        /// <param name="imageBase64">Base64-encoded image data</param>
        /// <param name="itemName">Name of the item</param>
        /// <param name="tags">Optional array of existing tags</param>
        /// <param name="category">Optional category information</param>
        /// <returns>A generated description for the item</returns>
        Task<string> GenerateDescriptionAsync(string imageBase64, string itemName, List<string> tags = null, string category = null);
    }
}