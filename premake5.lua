workspace "lua-language-server-com"
architecture "x86_64"
   configurations { "Debug", "Release" }

   project "lua-language-server-com"
      kind "ConsoleApp" -- CLI application
      dotnetframework "net10.0" -- Targeting .NET 9.0
      location "lua-language-server-com"
      language "C#"
      targetdir "bin/%{cfg.buildcfg}"
      files { "%{prj.name}/src/**.cs" } -- Include all C# source files
      nuget { "StreamJsonRpc:2.24.84","K4os.Compression.LZ4.Streams:1.3.8" }
      vsprops {
         PublishSingleFile = "true",
         SelfContained = "true",
         IncludeNativeLibrariesForSelfExtract = "true",
         PublishTrimmed =  "true",
         Nullable = "enable"
      }
      filter "configurations:Debug"
         defines { "DEBUG" }
         optimize "Off"
      
      filter "configurations:Release"
         symbols "Off"
         defines { "NDEBUG" }
         optimize "On"
      
      -- COPY EXECUTABLE TO TEST PROJECT
      postbuildcommands {
         "{COPY} %{cfg.targetdir}/lua-language-server-com.exe %{wks.location}/lua-language-server.tests/bin"
      }

   project "lua-language-server.tests"
      location "lua-language-server-tests"
      kind "ConsoleApp"
      language "C#"
      dotnetframework "net10.0"
      targetdir "bin/%{cfg.buildcfg}"
      objdir "obj/%{cfg.buildcfg}"

      targetdir "bin/%{cfg.buildcfg}"
      files { "%{wks.location}/lua-language-server-tests/src/**.cs" } -- Include all C# source files
      nuget { "xunit.v3:4.0.0-pre.108" }
      vsprops {
         PublishSingleFile = "true",
         SelfContained = "true",
         IncludeNativeLibrariesForSelfExtract = "true",
         PublishTrimmed =  "true",
         Nullable = "enable",
         EnableMSTestRunner = "true"

      }
      filter "configurations:Debug"
         defines { "DEBUG" }
         optimize "Off"
      
      filter "configurations:Release"
         symbols "Off"
         defines { "NDEBUG" }
         optimize "On"
      links { "lua-language-server-com" }