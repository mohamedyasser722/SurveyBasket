namespace SurveyBasket.Api.Services;

public class RoleService(RoleManager<ApplicationRole> roleManager, ApplicationDbContext context) : IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
    private readonly ApplicationDbContext _context = context;
    public async Task<IEnumerable<RoleResponse>> GetAllAsync(bool? includeDisabled = false, CancellationToken cancellationToken = default)
    {
        return await _roleManager.Roles
            .Where(role => !role.IsDefault && (includeDisabled == true || !role.IsDeleted))
            .ProjectToType<RoleResponse>()
            .ToListAsync(cancellationToken);
    }

    public async Task<Result<RoleDetailResponse>> GetAsync(string id)
    {
        if (await _roleManager.FindByIdAsync(id) is not { } role)
            return Result.Failure<RoleDetailResponse>(RoleErrors.RoleNotFound);

        var permissions = await _roleManager.GetClaimsAsync(role);


        var response = new RoleDetailResponse(
            role.Id,
            role.Name,
            role.IsDeleted,
            permissions.Select(claim => claim.Value)
        );


        return Result.Success(response);
    }

    public async Task<Result<RoleDetailResponse>> AddAsync(RollRequest request, CancellationToken cancellationToken = default)
    {
        bool roleIsExist = await _roleManager.FindByNameAsync(request.Name) is not null;

        if (roleIsExist)
            return Result.Failure<RoleDetailResponse>(RoleErrors.RoleAlreadyExist);

        // i want to make sure that the permissions u send are valid permissions

        var validPermissions = Permissions.GetAllPermessions().ToHashSet();

        if (request.Permissions.Except(validPermissions).Any())
            return Result.Failure<RoleDetailResponse>(RoleErrors.InvalidPermissions);

        var role = new ApplicationRole
        {
            Name = request.Name,
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };

        var result = await _roleManager.CreateAsync(role);

        if (!result.Succeeded)
        {
            var error = result.Errors.First();
            return Result.Failure<RoleDetailResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }


        var permissions = request.Permissions.Select(permission => new IdentityRoleClaim<string>
        {

            ClaimType = Permissions.Type,
            ClaimValue = permission,
            RoleId = role.Id

        });

        await _context.RoleClaims.AddRangeAsync(permissions, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var response = new RoleDetailResponse(
            role.Id,
            role.Name,
            role.IsDeleted,
            request.Permissions
        );


        return Result.Success(response);
    }
    public async Task<Result> UpdateAsync(string id, RollRequest request)
    {
        // Check if the role exists
        var role = await _roleManager.FindByIdAsync(id);
        if (role == null)
            return Result.Failure<RoleDetailResponse>(RoleErrors.RoleNotFound);

        // Check if the new role name already exists
        var existingRole = await _roleManager.FindByNameAsync(request.Name);
        if (existingRole != null && existingRole.Id != id)
            return Result.Failure<RoleDetailResponse>(RoleErrors.RoleAlreadyExist);

        // Validate permissions
        var validPermissions = Permissions.GetAllPermessions().ToHashSet();
        if (request.Permissions.Except(validPermissions).Any())
            return Result.Failure<RoleDetailResponse>(RoleErrors.InvalidPermissions);

        // Update role name
        role.Name = request.Name;
        var updateResult = await _roleManager.UpdateAsync(role);
        if (!updateResult.Succeeded)
        {
            var error = updateResult.Errors.First();
            return Result.Failure<RoleDetailResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }

        // Get existing claims for the role
        var existingClaims = await _context.RoleClaims
            .Where(rc => rc.RoleId == role.Id && rc.ClaimType == Permissions.Type)
            .ToListAsync();


        // Determine the claims to remove, add, and keep
        var existingPermissions = existingClaims.Select(rc => rc.ClaimValue).ToHashSet();
        var newPermissions = request.Permissions.ToHashSet();

        // Permissions to add
        var permissionsToAdd = newPermissions.Except(existingPermissions);
        var newClaims = permissionsToAdd.Select(permission => new IdentityRoleClaim<string>
        {
            ClaimType = Permissions.Type,
            ClaimValue = permission,
            RoleId = role.Id
        });

        // Permissions to remove
        var permissionsToRemove = existingPermissions.Except(newPermissions);

        // Remove outdated claims
        await _context.RoleClaims
            .Where(x => x.RoleId == id && permissionsToRemove.Contains(x.ClaimValue))
            .ExecuteDeleteAsync();



        // Add new claims
        await _context.RoleClaims.AddRangeAsync(newClaims);

        await _context.SaveChangesAsync();


        return Result.Success();
    }

    public async Task<Result> ToggleStatusAsync(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);
        if (role == null)
            return Result.Failure(RoleErrors.RoleNotFound);

        role.IsDeleted = !role.IsDeleted;
        await _roleManager.UpdateAsync(role);

        return Result.Success();
    }


}