
namespace SurveyBasket.Api.Services;

public class UserService(UserManager<ApplicationUser> userManager,
    IRoleService roleService,
    ApplicationDbContext context) : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IRoleService _roleService = roleService;
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        #region userResponse01
        //var usersResponse01 =  await (from user in _context.Users
        //                     join userRole in _context.UserRoles
        //                     on user.Id equals userRole.UserId
        //                     join role in _context.Roles
        //                     on userRole.RoleId equals role.Id into roles
        //                     where !roles.Any(r => r.Name == DefaultRoles.Member)
        //                     select new
        //                        {
        //                           user.Id,
        //                           user.FirstName,  
        //                           user.LastName,
        //                           user.Email,
        //                           user.IsDisabled,
        //                           Roles = roles.Select(r => r.Name).  ToList()
        //                        }
        //                     )
        //                    .GroupBy(u => new {u.Id, u.FirstName, u.LastName, u.Email, u.IsDisabled })
        //                    .Select(u => new UserResponse(

        //                        u.Key.Id,
        //                        u.Key.FirstName,
        //                        u.Key.LastName,
        //                        u.Key.Email,
        //                        u.Key.IsDisabled,
        //                        u.SelectMany(r => r.Roles).Distinct()

        //                        ))
        //                    .ToListAsync(cancellationToken);

        #endregion

        List<UserResponse> usersResponse02 = 
                            await (from user in _context.Users
                            join userRole in _context.UserRoles
                            on user.Id equals userRole.UserId
                            join role in _context.Roles
                            on userRole.RoleId equals role.Id
                            where role.Name != DefaultRoles.Member.Name // Filtering out Member role
                            group role.Name by new
                            {
                                user.Id,
                                user.FirstName,
                                user.LastName,
                                user.Email,
                                user.IsDisabled
                            } into userGroup
                            select new UserResponse(
                                userGroup.Key.Id,
                                userGroup.Key.FirstName,
                                userGroup.Key.LastName,
                                userGroup.Key.Email,
                                userGroup.Key.IsDisabled,
                                userGroup.Select(roleName => roleName).Distinct().ToList()
                            )
                            
                            ).ToListAsync(cancellationToken);

        return usersResponse02;

    }

    public async Task<Result<UserResponse>> GetAsync(string userId, CancellationToken cancellationToken = default)
    {
        #region userResponse01

        //if (userId == null)
        //    return Result.Failure<UserResponse>(UserErrors.UserNotFound);

        //// Retrieve the user and their roles based on the provided userId
        //var userResponse = await (from user in _context.Users
        //                          where user.Id == userId
        //                          join userRole in _context.UserRoles
        //                          on user.Id equals userRole.UserId
        //                          join role in _context.Roles
        //                          on userRole.RoleId equals role.Id
        //                          select new
        //                          {
        //                              user.Id,
        //                              user.FirstName,
        //                              user.LastName,
        //                              user.Email,
        //                              user.IsDisabled,
        //                              RoleName = role.Name
        //                          })
        //                          .GroupBy(u => new { u.Id, u.FirstName, u.LastName, u.Email, u.IsDisabled })
        //                          .Select(g => new UserResponse(
        //                              g.Key.Id,
        //                              g.Key.FirstName,
        //                              g.Key.LastName,
        //                              g.Key.Email,
        //                              g.Key.IsDisabled,
        //                              g.Select(x => x.RoleName).Distinct().ToList()
        //                          ))
        //                          .FirstOrDefaultAsync(cancellationToken);
        //if (userResponse == null)
        //    return Result.Failure<UserResponse>(UserErrors.UserNotFound);
        #endregion


        #region userResponse02

        if(await _userManager.FindByIdAsync(userId) is not { } User)
            return Result.Failure<UserResponse>(UserErrors.UserNotFound);

        var userRoles = await _userManager.GetRolesAsync(User);

        //var userResponse02 = new UserResponse(
        //    User.Id,
        //    User.FirstName,
        //    User.LastName,
        //    User.Email,
        //    User.IsDisabled,
        //    userRoles
        //);

        var userResponse02 = (User, userRoles).Adapt<UserResponse>();   // mapster maps the User and userRoles to UserResponse


        #endregion

        return Result.Success(userResponse02);
    }

    public async Task<Result<UserResponse>> AddAsync(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var emailIsExists = await _userManager.Users.AnyAsync(u => u.Email == request.Email, cancellationToken);
        if (emailIsExists)
            return Result.Failure<UserResponse>(UserErrors.DuplicatedEmail);

        var allowedRoles = await _roleService.GetAllAsync(cancellationToken: cancellationToken);

        var allowedRolesNames = allowedRoles.Select(r => r.Name).ToHashSet();   // HashSet is used to make the search faster by O(1) time complexity

        if (request.Roles.Except(allowedRolesNames).Any())
            return Result.Failure<UserResponse>(UserErrors.InvalidRoles);

        var user = request.Adapt<ApplicationUser>();  // mapster maps the request to ApplicationUser

        var result = await _userManager.CreateAsync(user, request.Password);

        if(!result.Succeeded)
        {
            var error = result.Errors.First();
            return Result.Failure<UserResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }

        await _userManager.AddToRolesAsync(user, request.Roles);
        UserResponse userResponse = (user, request.Roles).Adapt<UserResponse>();

        return Result.Success(userResponse);
    }

    public async Task<Result> UpdateAsync(string id, UpdateUserRequest request, CancellationToken cancellationToken)
    {
        // Find the user by ID
        if (await _userManager.FindByIdAsync(id) is not { } user)
            return Result.Failure<UserResponse>(UserErrors.UserNotFound);

        // Check if the email is already in use by another user
        var emailIsExists = await _userManager.Users.AnyAsync(u => u.Email == request.Email && u.Id != id, cancellationToken);
        if (emailIsExists)
            return Result.Failure<UserResponse>(UserErrors.DuplicatedEmail);

        // Validate roles in the request
        var allowedRoles = await _roleService.GetAllAsync(cancellationToken: cancellationToken);
        var allowedRolesNames = allowedRoles.Select(r => r.Name).ToHashSet(); // HashSet for O(1) lookup
        if (request.Roles.Except(allowedRolesNames).Any())
            return Result.Failure<UserResponse>(UserErrors.InvalidRoles);

        // Retrieve the user's current roles
        var currentRoles = await _userManager.GetRolesAsync(user);

        // Calculate roles to add and remove
        var rolesToAdd = request.Roles.Except(currentRoles).ToList();
        var rolesToRemove = currentRoles.Except(request.Roles).ToList();

        // Map the updated properties from the request to the user
        user = request.Adapt(user);

        // Update the user in the database
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var error = result.Errors.First();
            return Result.Failure<UserResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }

        // Remove the old roles
        if (rolesToRemove.Any())
            await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

        // Add the new roles
        if (rolesToAdd.Any())
            await _userManager.AddToRolesAsync(user, rolesToAdd);

        return Result.Success();
    }

    public async Task<Result> ToggleStatusAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return Result.Failure(UserErrors.UserNotFound);

        user.IsDisabled = !user.IsDisabled;
        await _userManager.UpdateAsync(user);

        return Result.Success();
    }

    public async Task<Result<UserProfileResponse>> GetProfileAsync(string userId)
    {
        var user = await _userManager.Users
            .Where(u => u.Id == userId)
            .ProjectToType<UserProfileResponse>()
            .SingleOrDefaultAsync();

        return Result.Success(user!);

    }

    public async Task<Result> UnlockUserAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return Result.Failure(UserErrors.UserNotFound);

        user.LockoutEnd = null;
        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            return Result.Failure(UserErrors.UnableToUnlockUser);

        return Result.Success();

    }

    public async Task<Result> UpdateProfileAsync(string userId, UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        //var user = await _userManager.FindByIdAsync(userId);      // too slow 
        //user = request.Adapt(user);
        //await _userManager.UpdateAsync(user!);    

        await _userManager.Users        // new method called ExecuteUpdateAsync so fast
            .Where(u => u.Id == userId)
            .ExecuteUpdateAsync(setters =>
                
                setters
                .SetProperty(u => u.FirstName, request.FirstName)
                .SetProperty(u => u.LastName, request.LastName)

                ,cancellationToken
            );

        return Result.Success();
    }
    public async Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId);

        var result = await _userManager.ChangePasswordAsync(user!, request.CurrentPassword, request.NewPassword);

        if(result.Succeeded)
            return Result.Success();

        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }
}
