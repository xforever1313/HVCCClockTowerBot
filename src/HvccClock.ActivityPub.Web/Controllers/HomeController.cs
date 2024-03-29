﻿//
// HvccClock - A bot that chimes the time every hour.
// Copyright (C) 2022 Seth Hendrick
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.
//

using HvccClock.ActivityPub.Web.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ActivityPub.Inbox.Common;

namespace HvccClock.ActivityPub.Web.Controllers
{
    public class HomeController : Controller
    {
        // ---------------- Fields ----------------

        private readonly ActivityPubInboxApi inboxApi;

        private readonly Resources resources;

        // ---------------- Constructor ----------------

        public HomeController(
            ActivityPubInboxApi inboxApi,
            Resources resources
        )
        {
            this.inboxApi = inboxApi;
            this.resources = resources;
        }

        // ---------------- Functions ----------------

        public IActionResult Index()
        {
            return View(
                new HomeModel(
                    this.inboxApi,
                    this.resources
                )
            );
        }

        public IActionResult License()
        {
            return View(
                new HomeModel(
                    this.inboxApi,
                    this.resources
                )
            );
        }

        public IActionResult Credits()
        {
            return View(
                new HomeModel(
                    this.inboxApi,
                    this.resources
                )
            );
        }

        [ResponseCache( Duration = 0, Location = ResponseCacheLocation.None, NoStore = true )]
        public IActionResult Error()
        {
            return View( new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier } );
        }
    }
}
