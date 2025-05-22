using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace API.Tests.Helpers
{
    public static class MockHelpers
    {
        public static Mock<UserManager<TUser>> TestUserManager<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var userManager = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
            userManager.Setup(x => x.CreateAsync(It.IsAny<TUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            userManager.Setup(x => x.AddToRoleAsync(It.IsAny<TUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            return userManager;
        }
        
        public static Mock<SignInManager<TUser>> TestSignInManager<TUser>() where TUser : class
        {
            var userManager = TestUserManager<TUser>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            var userPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<TUser>>();
            var signInManager = new Mock<SignInManager<TUser>>(
                userManager.Object, 
                contextAccessor.Object,
                userPrincipalFactory.Object,
                null, null, null, null);
            
            return signInManager;
        }
    }
}
