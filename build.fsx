#r "nuget: Fun.Build"

open Fun.Build


pipeline "publish-to-pi" {
    stage "publish" {
        run "dotnet publish -c Debug"
        run "scp -r ./bin/Debug/net8.0/publish/* pi@192.168.3.48:~/Playground/Walle"
    }
    runIfOnlySpecified false
}
