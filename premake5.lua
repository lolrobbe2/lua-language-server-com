workspace "lua-language-server-com"
architecture "x86_64"
   configurations { "Debug", "Release" }
   startproject "lua-language-server-com"

   project "lua-language-server-com"
      kind "ConsoleApp" -- CLI application
      dotnetframework "net10.0" -- Targeting .NET 9.0
      location "lua-language-server-com"
      language "C#"
      targetdir "bin/%{cfg.buildcfg}"
      files { "%{prj.name}/src/**.cs" } -- Include all C# source files
      nuget {  }
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
         "{COPY} %{cfg.buildtarget.abspath} %{wks.location}/lua-language-server.Tests/"
      }
   project "lua-language-server.Tests"
      location "lua-language-server-tests"
      kind "ConsoleApp"
      language "C#"
      dotnetframework "net10.0"
      targetdir "bin/%{cfg.buildcfg}"
      objdir "obj/%{cfg.buildcfg}"

   targetdir "bin/%{cfg.buildcfg}"
      files { "%{prj.name}/src/**.cs" } -- Include all C# source files
      nuget { "MSTest:4.2.2" }
      vsprops {
         PublishSingleFile = "true",
         SelfContained = "true",
         IncludeNativeLibrariesForSelfExtract = "true",
         PublishTrimmed =  "true",
         Nullable = "enable"
      }
      links {"lua-language-server-com"}
      filter "configurations:Debug"
         defines { "DEBUG" }
         optimize "Off"
      
      filter "configurations:Release"
         symbols "Off"
         defines { "NDEBUG" }
         optimize "On"