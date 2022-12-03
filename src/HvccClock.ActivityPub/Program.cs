//
// HvccClock - A Twitter bot that chimes the time every hour.
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

using HvccClock.ActivityPub;
using HvccClock.Common;
using Serilog;

Console.WriteLine( $"Version: {typeof( HvccClockConfig ).Assembly.GetName()?.Version?.ToString( 3 ) ?? string.Empty}." );

Serilog.ILogger? log = null;

void OnTelegramFailure( Exception e )
{
    log?.Warning( $"Telegram message did not send:{Environment.NewLine}{e}" );
}

try
{
    HvccClockConfig config = HvccClockConfigExtensions.FromEnvVar();

    log = HostingExtensions.CreateLog( config, OnTelegramFailure );
    using( var api = new HvccClockApi( log, config ) )
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder( args );

        // Add services to the container.
        builder.Services.AddSingleton<IHvccClockApi>( api );
        builder.Services.AddControllersWithViews();
        builder.Host.UseSerilog( log );
        builder.Services.ConfigureHvccServices<UpdateJob, HvccClockConfig>( config );
        builder.WebHost.UseUrls( config.Urls );

        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline.
        if( !app.Environment.IsDevelopment() )
        {
            app.UseExceptionHandler( "/Profile/Error" );
        }
        app.UseStaticFiles();

        app.UseRouting();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Profile}/{action=Index}/{id?}" );

        app.Run();
    }
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
