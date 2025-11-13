# TODO: Remove Last Active from Admin User Management

## Steps to Complete
- [ ] Remove LastActive property from AdminUserDto.cs
- [ ] Update AdminUserService.cs to not set LastActive in mapping
- [ ] Update AdminUserManagement.razor to remove "Last: @user.LastActive" from Activity column
