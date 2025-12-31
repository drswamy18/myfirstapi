using MongoDB.Driver;
using LoginApi.Model;
using ProductApi.Models;

namespace LoginApi.Services
{
    public class MongoDbService
    {
        private readonly IMongoCollection<Product> _userProductCollection;
        private readonly IMongoCollection<Login> _usersCollection;
        
        private readonly ILogger<MongoDbService> _logger;


        public MongoDbService(IConfiguration configuration, ILogger<MongoDbService> logger)

        {
            _logger = logger;

            try
            {
                var connectionString = configuration["MongoDb:ConnectionString"];
                var databaseName = configuration["MongoDb:DatabaseName"];
                var collectionName = configuration["MongoDb:UsersCollectionName"];
                var productsCollectionName = configuration["MongoDb:ProductsCollectionName"] ?? "Products";

                if (string.IsNullOrEmpty(connectionString))
                    throw new ArgumentNullException(nameof(connectionString), "MongoDB connection string is not configured");
                
                if (string.IsNullOrEmpty(databaseName))
                    throw new ArgumentNullException(nameof(databaseName), "Database name is not configured");
                
                if (string.IsNullOrEmpty(collectionName))
                    throw new ArgumentNullException(nameof(collectionName), "Collection name is not configured");

                _logger.LogInformation($"Connecting to MongoDB: {databaseName}.{collectionName}");

                var mongoClient = new MongoClient(connectionString);
                var mongoDatabase = mongoClient.GetDatabase(databaseName);
                _usersCollection = mongoDatabase.GetCollection<Login>(collectionName);
                _userProductCollection=mongoDatabase.GetCollection<Product>(productsCollectionName);

                // Test connection
                var count = _usersCollection.CountDocuments(_ => true);
                _logger.LogInformation($" MongoDB connected. Existing documents: {count}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize MongoDB service");
                throw;
            }
        }

        public async Task<List<Login>> GetAsync()
        {
            try
            {
                return await _usersCollection.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAsync");
                throw;
            }
        }

        public async Task<Login?> GetAsync(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    throw new ArgumentNullException(nameof(id));

                return await _usersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in GetAsync for id: {id}");
                throw;
            }
        }

        public async Task<Login?> GetByUsernameAsync(string username)
        {
            try
            {
                if (string.IsNullOrEmpty(username))
                    throw new ArgumentNullException(nameof(username));

                return await _usersCollection.Find(x => x.Name == username).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in GetByUsernameAsync for username: {username}");
                throw;
            }
        }

        public async Task<Login?> GetByEmailAsync(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                    return null;

                return await _usersCollection.Find(x => x.Email == email).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in GetByEmailAsync for email: {email}");
                throw;
            }
        }

        public async Task CreateAsync(Login login)
        {
            try
            {
                if (login == null)
                    throw new ArgumentNullException(nameof(login));

                
                if (string.IsNullOrEmpty(login.Id))
                {
                    login.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
                }

              
                if (login.CreatedAt == default)
                {
                    login.CreatedAt = DateTime.UtcNow;
                }

                await _usersCollection.InsertOneAsync(login);
                _logger.LogInformation($"Created user: {login.Name} (ID: {login.Id})");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating user: {login?.Name}");
                throw;
            }
        }

         public async Task createProduct(Product product)
        {

            try
            {
                if (product == null)
                {
                    throw new ArgumentNullException(nameof(product));
                }
                await _userProductCollection.InsertOneAsync(product);
            _logger.LogInformation($"Product Added to the Cart");
            }catch (Exception ex)
            {
                _logger.LogError(ex,"Failed to Add to Cart");
                throw;
            }
        }

        public async Task<Product?> GetAsyncProduct(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    throw new ArgumentNullException(nameof(id));

                return await _userProductCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in GetAsync for id: {id}");
                throw;
            }
        }

        
    }
}