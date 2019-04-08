// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Security.Claims;
using AuthServer.Models;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Identity;

namespace AuthServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        public static IEnumerable<ApiResource> GetApis()
        {
            return new ApiResource[]
            {
                new ApiResource("api1", "My API #1")
            };
        }

        public static IEnumerable<IdentityUser> GetUser()
        {
            return new[]
            {
                new IdentityUser
            {
                UserName = "admin",
                Email = "admin@email.com",
                EmailConfirmed = true,
                PhoneNumber = "555-555-5555",
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = false,
                SecurityStamp = Guid.NewGuid().ToString()

            }
            };

        }

        public static List<TestUser> GetTestUsers()
        {
            return new List<TestUser> {
                new TestUser {
                    SubjectId = "5BE86359-073C-434B-AD2D-A3932222DABE",
                    Username = "admin",
                    Password = "Password123!",
                    Claims = new List<Claim> {
                        new Claim(JwtClaimTypes.Email, "admin@email.com"),
                        new Claim(JwtClaimTypes.Role, "admin")
                    }
                }
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new[]
            {
                // client credentials flow client
                new Client
                {
                    ClientId = "client",
                    ClientName = "Client Credentials Client",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                    AllowedScopes = { "api1" }
                },

                // MVC client using hybrid flow
                new Client
                {
                    ClientId = "mvc",
                    ClientName = "MVC Client",

                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                    ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

                    RedirectUris = { "http://localhost:5001/signin-oidc" },
                    FrontChannelLogoutUri = "http://localhost:5001/signout-oidc",
                    PostLogoutRedirectUris = { "http://localhost:5001/signout-callback-oidc" },

                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "api1" }
                },

                // SPA client using implicit flow
                new Client
                {
                    ClientId = "spa",
                    ClientName = "SPA Client",
                    ClientUri = "http://identityserver.io",

                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris =
                    {
                        "http://localhost:5002/index.html",
                        "http://localhost:5002/callback.html",
                        "http://localhost:5002/silent.html",
                        "http://localhost:5002/popup.html",
                    },

                    PostLogoutRedirectUris = { "http://localhost:5002/index.html" },
                    AllowedCorsOrigins = { "http://localhost:5002" },

                    AllowedScopes = { "openid", "profile", "api1" }
                },
                //legacy resource owner password flow
                new Client
                {
                    ClientId = "spa2",
                    ClientName = "My Custom Client",
                    ClientUri = "http://localhost:4200",
                    AccessTokenLifetime = 60 * 60 * 24,
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    RequireClientSecret = false,
                    AllowedCorsOrigins = { "http://localhost:4200" },
                    PostLogoutRedirectUris = { "http://localhost:4200/" },
                    RedirectUris =
                    {
                        "http://localhost:4200/",
                    },
                    AllowedScopes =
                    {
                        "openid", "profile", "api1"
                    }
                },
                //adfs client
                new Client
                {
                ClientId = "urn:idsrv4:wsfed:client",
                ProtocolType = IdentityServerConstants.ProtocolTypes.WsFederation,
                RedirectUris = { "http://localhost:63307/signin-wsfed" },
                }

            };

        }
    }
}
