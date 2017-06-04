#r "./packages/FAKE/tools/FakeLib.dll"

open Fake
open Fake.Testing.XUnit2

let buildDir  = "./build/"

Target "Clean" (fun _ ->
    CleanDir buildDir
)

Target "Build" (fun _ ->
    !! "/**/*.fsproj"
    |> MSBuildDebug buildDir "Rebuild"
    |> Log "AppBuild-Output: "
)

Target "Test" (fun _ ->
    xUnit2 (fun p -> { p with ToolPath = "./packages/xunit.runner.console/tools/xunit.console.exe" }) [(buildDir + "Examples.dll")]
)

"Clean"
  ==> "Build"
  ==> "Test"

RunTargetOrDefault "Test"
