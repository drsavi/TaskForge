﻿using Microsoft.AspNetCore.Identity;

namespace TaskForge.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}
