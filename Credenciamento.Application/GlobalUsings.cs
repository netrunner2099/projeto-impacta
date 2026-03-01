global using AutoMapper;
global using Credenciamento.Application.Commands.Person;
global using Credenciamento.Application.Contracts.Event;
global using Credenciamento.Application.Contracts.Person;
global using Credenciamento.Application.Interfaces.Person;
global using Credenciamento.Application.Mappings;
global using Credenciamento.Application.Models;
global using Credenciamento.Application.Queries.Event;
global using Credenciamento.Application.Services.Person;
global using Credenciamento.Application.Validators.Person;
global using Credenciamento.Domain.Entities;
global using Credenciamento.Domain.Interfaces;
global using Credenciamento.Infrastructure;
global using Credenciamento.Shared.Helpers;
global using FluentValidation;
global using MediatR;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Reflection;
global using System.Text;
global using System.Threading;
global using System.Threading.Tasks;
global using Credenciamento.Application.Contracts.User;
global using Credenciamento.Application.Queries.User;
global using Credenciamento.Application.Commands.Ticket;
global using Credenciamento.Application.Contracts.Ticket;
global using Credenciamento.Application.Validators.Ticket;
global using System.Text.Json.Serialization;



