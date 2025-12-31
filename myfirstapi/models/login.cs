using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LoginApi.Model
{
    public class Login
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = MongoDB.Bson.ObjectId.GenerateNewId().ToString();

        [BsonElement("username")]
        public string Name { get; set; } = string.Empty;
        
        [BsonElement("password")]
        public string Password { get; set; } = string.Empty;
        
        [BsonElement("email")]
        public string? Email { get; set; }
        
        [BsonElement("firstName")]
        public string? FirstName { get; set; }
        
        [BsonElement("lastName")]
        public string? LastName { get; set; }
        
        [BsonElement("isActive")]
        public bool IsActive { get; set; } = true;
        
        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        [BsonElement("updatedAt")]
        public DateTime? UpdatedAt { get; set; }
        
        [BsonElement("lastLogin")]
        public DateTime? LastLogin { get; set; }
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public List<string>? Errors { get; set; }
    }
}