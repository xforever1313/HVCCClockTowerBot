@Library( "X13JenkinsLib" )_

def CallCake( String arguments )
{
    X13Cmd( "./Cake/dotnet-cake ./checkout/build.cake ${arguments}" );
}

def CallDevops( String arguments )
{
    X13Cmd( "dotnet ./checkout/src/DevOps/bin/Debug/net6.0/DevOps.dll ${arguments}" );
}

def Prepare()
{
    X13Cmd( 'dotnet tool update Cake.Tool --tool-path ./Cake' )
    CallCake( "--showdescription" )
}

def Build()
{
    CallCake( "--target=build" );
}

def RunTests()
{
    CallDevops( "--target=run_tests" );
}

def PublishTwitter()
{
    CallDevops( "--target=publish_twitter" );
}

def PublishActPub()
{
    CallDevops( "--target=publish_actpub" );
}

def Zip()
{
    CallDevops( "--target=publish_twitter_zip" );
}

pipeline
{
    agent
    {
        label "ubuntu && docker && x64";
    }
    environment
    {
        DOTNET_CLI_TELEMETRY_OPTOUT = 'true'
        DOTNET_NOLOGO = 'true'
    }
    options
    {
        skipDefaultCheckout( true );
    }
    stages
    {
        stage( "Clean" )
        {
            steps
            {
                cleanWs();
            }
        }
        stage( "checkout " )
        {
            steps
            {
                checkout scm;
            }
        }
        stage( "In Dotnet Docker" )
        {
            agent
            {
                docker
                {
                    image 'mcr.microsoft.com/dotnet/sdk:6.0'
                    args "-e HOME='${env.WORKSPACE}'"
                    reuseNode true
                }
            }
            stages
            {
                stage( "Prepare" )
                {
                    steps
                    {
                        Prepare();
                    }
                }
                stage( "Build" )
                {
                    steps
                    {
                        Build();
                    }
                }
                stage( "Run Tests" )
                {
                    steps
                    {
                        RunTests();
                    }
                    post
                    {
                        always
                        {
                            X13ParseMsTestResults(
                                filePattern: "checkout/TestResults/HvccClock.Tests/*.xml",
                                abortOnFail: true
                            );
                        }
                    }
                }
                stage( "Publish Twitter" )
                {
                    steps
                    {
                        PublishTwitter();
                    }
                }
                stage( "Publish Zip" )
                {
                    steps
                    {
                        Zip();
                        archiveArtifacts "checkout/dist/twitter/zip/*.*";
                    }
                }
                stage( "Publish Activity Pub" )
                {
                    steps
                    {
                        PublishActPub();
                        archiveArtifacts "checkout/dist/actpub/HvccClock.ActivityPub.Web";
                        archiveArtifacts "checkout/dist/actpub/*.so";
                    }
                }
            }
        }
    }
}
