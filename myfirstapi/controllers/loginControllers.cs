using LoginApi.Model;
using LoginApi.Services;
using Microsoft.AspNetCore.Mvc;


namespace LoginApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly MongoDbService _mongoDbService;
        private readonly ILogger<LoginController> _logger;

        public LoginController(MongoDbService mongoDbService, ILogger<LoginController> logger)
        {
            _mongoDbService = mongoDbService ?? throw new ArgumentNullException(nameof(mongoDbService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: api/login
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<Login>>>> GetLogins()
        {
            try
            {
                var logins = await _mongoDbService.GetAsync();
                
                return Ok(new ApiResponse<List<Login>>
                {
                    Success = true,
                    Message = $"Found {logins.Count} users",
                    Data = logins
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all logins");
                return StatusCode(500, new ApiResponse<List<Login>>
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                });
            }
        }

        // GET: api/login/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Login>>> GetLogin(string id)
        {
            try
            {
                var login = await _mongoDbService.GetAsync(id);
                
                if (login == null)
                {
                    return NotFound(new ApiResponse<Login>
                    {
                        Success = false,
                        Message = $"User with ID {id} not found"
                    });
                }
                
                return Ok(new ApiResponse<Login>
                {
                    Success = true,
                    Message = "User found successfully",
                    Data = login
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting login by ID: {id}");
                return StatusCode(500, new ApiResponse<Login>
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                });
            }
        }

        // GET: api/login/username/johndoe
        [HttpGet("username/{username}")]
        public async Task<ActionResult<ApiResponse<Login>>> GetLoginByUsername(string username)
        {
            try
            {
                var login = await _mongoDbService.GetByUsernameAsync(username);
                
                if (login == null)
                {
                    return NotFound(new ApiResponse<Login>
                    {
                        Success = false,
                        Message = $"User '{username}' not found"
                    });
                }
                
                return Ok(new ApiResponse<Login>
                {
                    Success = true,
                    Message = "User found by username",
                    Data = login
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting login by username: {username}");
                return StatusCode(500, new ApiResponse<Login>
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                });
            }
        }

        // POST: api/login/register
        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<Login>>> Register([FromBody] UserRegistrationDto registrationDto)
        {
            try
            {
                _logger.LogInformation($"Registration attempt for: {registrationDto?.Name}");

                // Check if DTO is null
                if (registrationDto == null)
                {
                    return BadRequest(new ApiResponse<Login>
                    {
                        Success = false,
                        Message = "Registration data is required"
                    });
                }

                // Manual validation
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    
                    return BadRequest(new ApiResponse<Login>
                    {
                        Success = false,
                        Message = "Validation failed",
                        Errors = errors
                    });
                }

                // Check if username exists
                var existingUser = await _mongoDbService.GetByUsernameAsync(registrationDto.Name);
                if (existingUser != null)
                {
                    return Conflict(new ApiResponse<Login>
                    {
                        Success = false,
                        Message = $"Username '{registrationDto.Name}' already exists"
                    });
                }

                // Check if email exists (if provided)
                if (!string.IsNullOrEmpty(registrationDto.Email))
                {
                    var existingEmail = await _mongoDbService.GetByEmailAsync(registrationDto.Email);
                    if (existingEmail != null)
                    {
                        return Conflict(new ApiResponse<Login>
                        {
                            Success = false,
                            Message = $"Email '{registrationDto.Email}' already registered"
                        });
                    }
                }

                // Create user
                var login = new Login
                {
                    Name = registrationDto.Name,
                    Password = registrationDto.Password,
                    Email = registrationDto.Email,
                    FirstName = registrationDto.FirstName,
                    LastName = registrationDto.LastName,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                await _mongoDbService.CreateAsync(login);

                _logger.LogInformation($"User registered successfully: {login.Name}");

                return CreatedAtAction(nameof(GetLogin), new { id = login.Id }, 
                    new ApiResponse<Login>
                    {
                        Success = true,
                        Message = "User registered successfully",
                        Data = login
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in user registration");
                return StatusCode(500, new ApiResponse<Login>
                {
                    Success = false,
                    Message = $"Registration failed: {ex.Message}"
                });
            }
        }

        // POST: api/login
        [HttpPost]
        public async Task<ActionResult<ApiResponse<Login>>> CreateLogin([FromBody] LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    
                    return BadRequest(new ApiResponse<Login>
                    {
                        Success = false,
                        Message = "Validation failed",
                        Errors = errors
                    });
                }

                // Check if username exists
                var existingUser = await _mongoDbService.GetByUsernameAsync(loginDto.Name);
                if (existingUser != null)
                {
                    return Conflict(new ApiResponse<Login>
                    {
                        Success = false,
                        Message = $"Username '{loginDto.Name}' already exists"
                    });
                }

                var login = new Login
                {
                    Name = loginDto.Name,
                    Password = loginDto.Password,
                    CreatedAt = DateTime.UtcNow
                };

                await _mongoDbService.CreateAsync(login);

                return CreatedAtAction(nameof(GetLogin), new { id = login.Id }, 
                    new ApiResponse<Login>
                    {
                        Success = true,
                        Message = "User created successfully",
                        Data = login
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating login");
                return StatusCode(500, new ApiResponse<Login>
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                });
            }
        }

    }
}