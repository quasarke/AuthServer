// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using AuthServer.Controllers;
using AuthServer.Interfaces;
using AuthServer.Models;
using AuthServer.Providers;
using AuthServer.Services;
using AuthServer.Settings;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using  IdentityServer4.AspNetIdentity;
using Microsoft.AspNetCore.Identity;
using  Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Swagger;

namespace AuthServer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public static IHostingEnvironment Environment { get; private set; }

        public Startup(IConfiguration config, IHostingEnvironment env)
        {
            Environment = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            Configuration = config;
        }

       

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.Configure<IISOptions>(options =>
            {
                options.AutomaticAuthentication = false;
                options.AuthenticationDisplayName = "Windows";
            });

            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            //addcertto root and uncomment this and .AddSigningCredentials in env check below
            //var cert = new X509Certificate2(Path.Combine(Environment.ContentRootPath, "company.code.pfx"), "pASSwOrd");

            services.AddDbContext<ApplicationDbContext>(builder =>
                builder.UseSqlServer(connectionString, sqlOptions => sqlOptions.MigrationsAssembly(migrationsAssembly)));

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            var identityServer = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
                //testuser has to be replaced with either custom profile and validator or asp.net identity(or something else)
                .AddAspNetIdentity<IdentityUser>().AddWsFederation()
                //.AddTestUsers(TestUsers.Users)
                //.AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
                //.AddProfileService<ProfileService>()
                
                // this adds the config data from DB (clients, resources, CORS)
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseSqlServer(connectionString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                // this adds the operational data from DB (codes, tokens, consents)
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseSqlServer(connectionString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));

                    // this enables automatic token cleanup. this is optional.
                    options.EnableTokenCleanup = true;
                    // options.TokenCleanupInterval = 15; // interval in seconds. 15 seconds useful for debugging
                });

            //services.AddScoped<IUserRepository, UserDataProvider>();

            //services.AddAuthentication()
            //    .AddGoogle(options =>
            //    {
            //        options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

            //        options.ClientId = "708996912208-9m4dkjb5hscn7cjrn5u0r4tbgkbj1fko.apps.googleusercontent.com";
            //        options.ClientSecret = "wdfPY6t8H8cecgjlxud__4Gh";
            //    });

            services.AddDbContext<IdentityDbContext>(builder =>
                builder.UseSqlServer(connectionString, sqlOptions => sqlOptions.MigrationsAssembly(migrationsAssembly)));


            services.Configure<EmailConfig>(Configuration.GetSection("EmailConfig"));
            services.AddSingleton(Configuration);

            if (Environment.IsDevelopment())
            {
                identityServer.AddDeveloperSigningCredential();
            }
            else
            {
                //when cert is added comment this out and uncomment following line
                throw new Exception("need to configure key material");
                //identityServer.AddSigningCredential(cert);
            }
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "/swagger/v1/swagger.json",
                    Version = "v1"
                });
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            InitializeDatabase(app);
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
           
            app.UseIdentityServer();
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SSO");
            });
        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
                var userContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                userContext.Database.Migrate();
                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();

                
                if (context.Clients.Count() < Config.GetClients().Count())
                {
                    foreach (var client in Config.GetClients())
                    {
                        if (!context.Clients.Any(c => c.ClientName == client.ClientName))
                        {
                            context.Clients.Add(client.ToEntity());
                        }
                       
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in Config.GetIdentityResources())
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in Config.GetApis())
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                if (!userManager.Users.Any())
                {
                    foreach (var testUser in Config.GetTestUsers())
                    {
                        var identityUser = new IdentityUser(testUser.Username)
                        {
                            Id = testUser.SubjectId
                        };

                        userManager.CreateAsync(identityUser, "Password123!").Wait();
                        userManager.AddClaimsAsync(identityUser, testUser.Claims.ToList()).Wait();
                    }
                }

            }
        }
    }

    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    }
}
