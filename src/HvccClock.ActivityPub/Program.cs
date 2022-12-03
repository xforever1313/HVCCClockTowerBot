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

using dotenv.net;
using HvccClock.ActivityPub;
using HvccClock.Common;
using Mono.Options;
using Serilog;

bool showHelp = false;
bool showVersion = false;
bool showLicense = false;
bool showCredits = false;
string envFile = string.Empty;

var options = new OptionSet
{
    {
        "h|help",
        "Shows thie mesage and exits.",
        v => showHelp = ( v is not null )
    },
    {
        "version",
        "Shows the version and exits.",
        v => showVersion = ( v is not null )
    },
    {
        "print_license",
        "Prints the software license and exits.",
        v => showLicense = ( v is not null )
    },
    {
        "print_credits",
        "Prints the third-party notices and credits.",
        v => showCredits = ( v is not null )
    },
    {
        "env=",
        "The .env file that contains the environment variable settings.",
        v => envFile = v
    }
};

Serilog.ILogger? log = null;

try
{
    options.Parse( args );

    if( showHelp )
    {
        PrintHelp();
        return 0;
    }
    else if( showVersion )
    {
        PrintVersion();
        return 0;
    }
    else if( showLicense )
    {
        PrintLicense();
        return 0;
    }
    else if( showCredits )
    {
        PrintCredits();
        return 0;
    }

    if( string.IsNullOrWhiteSpace( envFile ) == false )
    {
        Console.WriteLine( $"Using .env file located at '{envFile}'" );
        DotEnv.Load( new DotEnvOptions( envFilePaths: new string[] { envFile } ) );
    }

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

void OnTelegramFailure( Exception e )
{
    log?.Warning( $"Telegram message did not send:{Environment.NewLine}{e}" );
}

void PrintHelp()
{
    options.WriteOptionDescriptions( Console.Out );
}

void PrintVersion()
{
    Console.WriteLine( typeof( Program ).Assembly.GetName().Version?.ToString( 3 ) ?? "Unknown Version" );
}

void PrintLicense()
{
    Console.WriteLine( "NOT IMPLEMENTED YET!" );
}

void PrintCredits()
{
    Console.WriteLine( "NOT IMPLEMENTED YET!" );
}
