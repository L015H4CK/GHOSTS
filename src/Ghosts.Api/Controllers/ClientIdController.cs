﻿// Copyright 2017 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

using System.Threading;
using System.Threading.Tasks;
using Ghosts.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace Ghosts.Api.Controllers
{
    /// <summary>
    /// GHOSTS CLIENT CONTROLLER
    /// These endpoints are typically only used by GHOSTS Clients installed and configured to use the GHOSTS C2
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ClientIdController : Controller
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        private readonly IMachineService _service;

        public ClientIdController(IMachineService service)
        {
            _service = service;
        }

        /// <summary>
        /// Clients use this endpoint to get their unique GHOSTS system ID
        /// </summary>
        /// <param name="ct">Cancellation Token</param>
        /// <returns>A client's particular unique GHOSTS system ID (GUID)</returns>
        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var id = Request.Headers["ghosts-id"];
            log.Trace($"Request by {id}");

            var findMachineResponse = await this._service.FindOrCreate(HttpContext, ct);
            if (!findMachineResponse.IsValid())
            {
                return StatusCode(StatusCodes.Status401Unauthorized, findMachineResponse.Error);
            }

            var m = findMachineResponse.Machine;

            //client saves this for future calls
            return Json(m.Id);
        }
    }
}