﻿global using Microsoft.AspNetCore.Mvc;
global using System.Reflection;
global using SurveyBasket.Api.Entities;
global using SurveyBasket.Api.Services;
global using SurveyBasket.Api.Contracts;
global using SurveyBasket.Api.Mapping;
global using Mapster;
global using FluentValidation;
global using FluentValidation.AspNetCore;
global using Microsoft.EntityFrameworkCore;
global using SurveyBasket.Api.Persistence;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;
global using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
global using Microsoft.AspNetCore.Identity;
global using SurveyBasket.Api.Contracts.Authentication;
global using System.Security.Claims;
global using System.IdentityModel.Tokens.Jwt;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using SurveyBasket.Api.Authentication;
global using SurveyBasket.Api.Contracts.Polls;
global using System.Text;
global using Microsoft.IdentityModel.Tokens;
global using Microsoft.Extensions.Options;
global using System.ComponentModel.DataAnnotations;
global using SurveyBasket.Api.Abstraction;
global using System.Security.Cryptography;
global using Microsoft.AspNetCore.Authorization;
global using SurveyBasket.Api.Errors;
global using SurveyBasket.Api.Contracts.Questions;
global using SurveyBasket.Api.Extensions;
global using SurveyBasket.Api.Contracts.Votes;
global using SurveyBasket.Api.Contracts.Results;
global using SurveyBasket.Api.Services.Services.Interfaces;
global using Microsoft.Extensions.Caching.Distributed;
global using System.Text.Json;
global using SurveyBasket.Api.Abstraction.Consts;
global using SurveyBasket.Api.Settings;
global using Microsoft.OpenApi.Models;
global using Microsoft.AspNetCore.Identity.UI.Services;
global using MimeKit;
global using Hangfire.Dashboard;
global using Hangfire;
global using SurveyBasket.Api.Helpers;
global using SurveyBasket.Api.Contracts.Users;
global using SurveyBasket.Api.Authentication.Filters;
global using SurveyBasket.Api.Contracts.Roles;