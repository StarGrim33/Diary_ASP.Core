﻿using FonTech.Domain.Dto;
using FonTech.Domain.Result;
using System.Security.Claims;

namespace FonTech.Domain.Interfaces.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(IEnumerable<Claim> claims);

        string GenerateRefreshToken();

        ClaimsPrincipal GetClaimsPrincipalFromExpiredToken(string accessToken);

        Task<BaseResult<TokenDto>> RefreshToken(TokenDto dto);
    }
}
