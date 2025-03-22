using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Configuration;

namespace Manajemen_Inventaris.Services
{
    /// <summary>
    /// Implementation of the AI service
    /// </summary>
    public class AIService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _apiToken;

        /// <summary>
        /// Constructor
        /// </summary>
        public AIService()
        {
            // Ensure we use TLS 1.2 for secure connections
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            _httpClient = new HttpClient();
            _baseUrl = ConfigurationManager.AppSettings["AIServiceBaseUrl"] ?? "http://localhost:3000/api/v1/inventory";
            _apiToken = ConfigurationManager.AppSettings["AIServiceToken"] ?? "2MzYzVvZoBMDDc/CkII7WE6Cv4+Mi07GR+GDsyEoLOM=";

            // Set a timeout to prevent hanging if service is unresponsive
            _httpClient.Timeout = TimeSpan.FromSeconds(30);

            // Set default headers for all requests
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiToken);
        }

        /// <inheritdoc/>
        public async Task<List<string>> GenerateTagsAsync(string imageBase64, List<string> existingTags = null, string category = null)
        {
            try
            {
                // Ensure existingTags is never null
                var tagsList = existingTags ?? new List<string>();

                // Ensure the image data is properly formatted with data URI prefix if missing
                if (!string.IsNullOrEmpty(imageBase64) && !imageBase64.StartsWith("data:image/"))
                {
                    // Try to detect the image type from the first few bytes
                    string mimeType = "image/jpeg"; // Default to JPEG
                    if (imageBase64.Length > 10)
                    {
                        byte[] bytes = Convert.FromBase64String(imageBase64.Substring(0, Math.Min(20, imageBase64.Length)));
                        if (bytes.Length >= 3 && bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF)
                        {
                            mimeType = "image/jpeg";
                        }
                        else if (bytes.Length >= 8 && bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47)
                        {
                            mimeType = "image/png";
                        }
                    }

                    imageBase64 = $"data:{mimeType};base64,{imageBase64}";
                }

                var requestData = new
                {
                    imageData = imageBase64,
                    existingTags = tagsList,
                    context = category
                };

                System.Diagnostics.Debug.WriteLine($"Sending API request for tag generation: Tags={tagsList.Count}, Category={category}, HasImage={!string.IsNullOrEmpty(imageBase64)}");

                var response = await PostToApiAsync<dynamic, TagGenerationResponse>("/generate-tags", requestData);

                if (response?.tags != null)
                {
                    var result = new List<string>();
                    foreach (var tag in response.tags)
                    {
                        // Handle both formats: tag or name property
                        if (tag != null)
                        {
                            string tagName = null;

                            if (((dynamic)tag).tag != null)
                            {
                                tagName = ((dynamic)tag).tag.ToString();
                            }
                            else if (((dynamic)tag).name != null)
                            {
                                tagName = ((dynamic)tag).name.ToString();
                            }

                            if (!string.IsNullOrEmpty(tagName) && !result.Contains(tagName))
                            {
                                result.Add(tagName);
                            }
                        }
                    }
                    return result;
                }

                return new List<string>();
            }
            catch (Exception ex)
            {
                // Log the exception
                System.Diagnostics.Debug.WriteLine($"Error generating tags: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                return new List<string>();
            }
        }

        /// <inheritdoc/>
        public async Task<Dictionary<string, double>> SuggestCategoryAsync(string imageBase64, List<string> existingCategories)
        {
            try
            {
                // Ensure image data is properly formatted with data URI prefix if missing
                if (!string.IsNullOrEmpty(imageBase64) && !imageBase64.StartsWith("data:image/"))
                {
                    // Try to detect the image type from the first few bytes
                    string mimeType = "image/jpeg"; // Default to JPEG
                    if (imageBase64.Length > 10)
                    {
                        byte[] bytes = Convert.FromBase64String(imageBase64.Substring(0, Math.Min(20, imageBase64.Length)));
                        if (bytes.Length >= 3 && bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF)
                        {
                            mimeType = "image/jpeg";
                        }
                        else if (bytes.Length >= 8 && bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47)
                        {
                            mimeType = "image/png";
                        }
                    }

                    imageBase64 = $"data:{mimeType};base64,{imageBase64}";
                }

                var requestData = new
                {
                    imageData = imageBase64,
                    existingCategories = existingCategories
                };

                System.Diagnostics.Debug.WriteLine($"Sending API request for category suggestion with {existingCategories?.Count ?? 0} categories. HasImage={!string.IsNullOrEmpty(imageBase64)}");

                var response = await PostToApiAsync<dynamic, CategorySuggestionResponse>("/suggest-category", requestData);

                if (response?.categories != null)
                {
                    var result = new Dictionary<string, double>();
                    foreach (var category in response.categories)
                    {
                        // Safely check if category has name property
                        if (category != null)
                        {
                            string categoryName = null;
                            double confidence = 0;

                            // Check for both formats: either "name" or "category" as property name
                            if (((dynamic)category).name != null)
                            {
                                categoryName = ((dynamic)category).name.ToString();
                                confidence = Convert.ToDouble(((dynamic)category).confidence);
                            }
                            else if (((dynamic)category).category != null)
                            {
                                categoryName = ((dynamic)category).category.ToString();
                                confidence = Convert.ToDouble(((dynamic)category).confidence);
                            }

                            if (!string.IsNullOrEmpty(categoryName) && !result.ContainsKey(categoryName))
                            {
                                result.Add(categoryName, confidence);
                            }
                        }
                    }
                    return result;
                }

                return new Dictionary<string, double>();
            }
            catch (Exception ex)
            {
                // Log the exception
                System.Diagnostics.Debug.WriteLine($"Error suggesting category: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                return new Dictionary<string, double>();
            }
        }

        /// <inheritdoc/>
        public async Task<string> EnhanceSearchAsync(string query, string currentCategory = null)
        {
            try
            {
                var requestData = new
                {
                    query = query,
                    currentCategory = currentCategory
                };

                var response = await PostToApiAsync<dynamic, SearchEnhancementResponse>("/enhance-search", requestData);

                if (response?.enhancedTerms != null && response.enhancedTerms.Count > 0)
                {
                    // Join the enhanced terms with OR operator for SQL search
                    return string.Join(" OR ", response.enhancedTerms);
                }

                // If enhancement failed, return the original query
                return query;
            }
            catch (Exception ex)
            {
                // Log the exception
                System.Diagnostics.Debug.WriteLine($"Error enhancing search: {ex.Message}");
                return query; // Return the original query if enhancement fails
            }
        }

        /// <inheritdoc/>
        public async Task<string> GenerateDescriptionAsync(string imageBase64, string itemName, List<string> tags = null, string category = null)
        {
            try
            {
                // Ensure tags is never null
                var tagsList = tags ?? new List<string>();

                // Ensure the image data is properly formatted with data URI prefix if missing
                if (!string.IsNullOrEmpty(imageBase64) && !imageBase64.StartsWith("data:image/"))
                {
                    // Try to detect the image type from the first few bytes
                    string mimeType = "image/jpeg"; // Default to JPEG
                    if (imageBase64.Length > 10)
                    {
                        byte[] bytes = Convert.FromBase64String(imageBase64.Substring(0, Math.Min(20, imageBase64.Length)));
                        if (bytes.Length >= 3 && bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF)
                        {
                            mimeType = "image/jpeg";
                        }
                        else if (bytes.Length >= 8 && bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47)
                        {
                            mimeType = "image/png";
                        }
                    }

                    imageBase64 = $"data:{mimeType};base64,{imageBase64}";
                }

                System.Diagnostics.Debug.WriteLine($"Sending API request for description generation: Item={itemName}, Tags={tagsList.Count}, HasImage={!string.IsNullOrEmpty(imageBase64)}");

                var requestData = new
                {
                    imageData = imageBase64,
                    itemName = itemName,
                    tags = tagsList,
                    category = category
                };

                var response = await PostToApiAsync<dynamic, DescriptionGenerationResponse>("/generate-description", requestData);

                if (response?.description != null)
                {
                    // Ensure description is not too long (practical SQL Server limit for NVARCHAR(MAX))
                    const int maxDescriptionLength = 8000;
                    string description = response.description.ToString();

                    if (description.Length > maxDescriptionLength)
                    {
                        System.Diagnostics.Debug.WriteLine($"Truncating description from {description.Length} to {maxDescriptionLength} characters");
                        description = description.Substring(0, maxDescriptionLength);
                    }

                    return description;
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                // Log the exception
                System.Diagnostics.Debug.WriteLine($"Error generating description: {ex.Message}");
                return string.Empty;
            }
        }

        #region Private Helper Methods

        private async Task<TResponse> PostToApiAsync<TRequest, TResponse>(string endpoint, TRequest requestData)
        {
            var fullUrl = $"{_baseUrl}{endpoint}";
            var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(fullUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    try
                    {
                        return JsonConvert.DeserializeObject<TResponse>(jsonResponse);
                    }
                    catch (JsonException jsonEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"JSON parsing error: {jsonEx.Message}");
                        System.Diagnostics.Debug.WriteLine($"Response content: {jsonResponse}");
                        throw new Exception($"Error parsing AI service response: {jsonEx.Message}", jsonEx);
                    }
                }

                // Read the error response for more details
                var errorContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"API error: Status code: {response.StatusCode}, Content: {errorContent}");
                throw new Exception($"API request failed with status code: {response.StatusCode}. Details: {errorContent}");
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Network error: {ex.Message}");
                throw new Exception($"Network error when calling AI service: {ex.Message}", ex);
            }
            catch (TaskCanceledException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Request timeout: {ex.Message}");
                throw new Exception("AI service request timed out. Please try again later.", ex);
            }
            catch (Exception ex) when (!(ex.Message.Contains("API request failed")))
            {
                System.Diagnostics.Debug.WriteLine($"General error: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw new Exception($"Error communicating with AI service: {ex.Message}", ex);
            }
        }

        #endregion

        #region Response Classes

        private class TagGenerationResponse
        {
            public List<TagItem> tags { get; set; }
            public bool success { get; set; }
        }

        private class TagItem
        {
            public string tag { get; set; }
            public string name { get; set; }
            public double confidence { get; set; }
        }

        private class CategorySuggestionResponse
        {
            public List<CategoryItem> categories { get; set; }
            public bool newCategoryNeeded { get; set; }
            public bool success { get; set; }
        }

        private class CategoryItem
        {
            public string category { get; set; }
            public string name { get; set; }
            public double confidence { get; set; }
        }

        private class SearchEnhancementResponse
        {
            public string originalQuery { get; set; }
            public List<string> enhancedTerms { get; set; }
            public List<string> relatedConcepts { get; set; }
            public bool success { get; set; }
        }

        private class DescriptionGenerationResponse
        {
            public string description { get; set; }
            public bool success { get; set; }
        }

        #endregion
    }
}