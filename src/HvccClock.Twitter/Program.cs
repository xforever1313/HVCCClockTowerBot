//
// HvccClock - A bot that chimes the time every hour.
// Copyright (C) 2022-2024 Seth Hendrick
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

using HvccClock.Common;
using HvccClock.Twitter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Prometheus;
using Serilog;

Console.WriteLine( $"Version: {typeof( HvccClockConfig ).Assembly.GetName()?.Version?.ToString( 3 ) ?? string.Empty}." );

var config = new HvccClockConfig();
if( config.TryValidate( out string error ) == false )
{
    Console.WriteLine( "Bot is misconfigured" );
    Console.WriteLine( error );
    return 1;
}

Serilog.ILogger? log = null;

void OnTelegramFailure( Exception e )
{
    log?.Warning( $"Telegram message did not send:{Environment.NewLine}{e}" );
}

try
{
    log = HostingExtensions.CreateLog( config, OnTelegramFailure );

    WebApplicationBuilder builder = WebApplication.CreateBuilder( args );

    builder.Logging.ClearProviders();
    builder.Services.AddControllersWithViews();
    builder.Host.UseSerilog( log );
    builder.Services.ConfigureHvccServices<TweetJob, HvccClockConfig>( config );
    builder.WebHost.UseUrls( $"http://0.0.0.0:{config.Port}" );

    WebApplication app = builder.Build();
    app.UseRouting();
    app.UseEndpoints(
        endpoints =>
        {
            endpoints.MapMetrics( "/Metrics" );
        }
    );

    log.Information( "Application Running..." );

    app.Run();
}
catch( Exception e )
{
    if( log is null )
    {
        Console.Error.WriteLine( "FATAL ERROR:" );
        Console.Error.WriteLine( e.ToString() );
    }
    else
    {
        log.Fatal( "FATAL ERROR:" + Environment.NewLine + e );
    }
    return 2;
}
finally
{
    log?.Information( "Application Exiting" );
}

return 0;
