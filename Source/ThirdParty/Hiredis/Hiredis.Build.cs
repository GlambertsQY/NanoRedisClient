// Copyright 2022 Danyang Chen https://github.com/DAN-AND-DNA

using System.IO;
using UnrealBuildTool;

public class Hiredis : ModuleRules
{
	private bool _forceStaticLibInEditor = false;
    
	public Hiredis(ReadOnlyTargetRules Target) : base(Target)
	{
		Type = ModuleType.External;
		OptimizeCode = CodeOptimization.Never;

		bEnableExceptions = true;
		bEnableUndefinedIdentifierWarnings = false; 
		ThirdParty(Target);
	}

	void ThirdParty(ReadOnlyTargetRules Target)
	{
		// add header
		PublicIncludePaths.AddRange(new string[] { Path.Combine(ModuleDirectory, "include") });

		// add lib
		string LibraryPath = Path.GetFullPath(Path.Combine(ModuleDirectory, "Lib"));
		if (Target.Platform == UnrealTargetPlatform.Win64 || Target.Platform == UnrealTargetPlatform.HoloLens)
		{
			if (_forceStaticLibInEditor)
			{
				if (Target.Platform == UnrealTargetPlatform.Win64)
				{
					string Win64LibraryPath = Path.Combine(LibraryPath, "Win64");
					PublicAdditionalLibraries.Add(Path.Combine(Win64LibraryPath, "hiredis_static.lib"));
				}else if (Target.Platform == UnrealTargetPlatform.HoloLens)
				{
					string Arm64LibraryPath = Path.Combine(LibraryPath, "Arm64");
					PublicAdditionalLibraries.Add(Path.Combine(Arm64LibraryPath, "hiredis_static.lib"));
				}
			} else {
				WinDll(LibraryPath);
			}
		} else if (Target.Platform == UnrealTargetPlatform.Linux)  {
			string LinuxLibraryPath = Path.Combine(LibraryPath, "Linux");
			PublicAdditionalLibraries.Add(Path.Combine(LinuxLibraryPath, "libhiredis.a"));
		}
	}
	
	void AddRuntimeDependencies(string[] DllNames, string LibraryPath, bool Delay)
	{
		foreach (var DllName in DllNames)
		{
			if(Delay) PublicDelayLoadDLLs.Add(DllName);
			var DllPath = Path.Combine(LibraryPath, DllName);
			var DestDllPath = Path.Combine("$(BinaryOutputDir)", DllName);
			RuntimeDependencies.Add(DestDllPath, DllPath, StagedFileType.NonUFS);
		}
	}
	
	void WinDll(string LibraryPath)
	{
		if (Target.Platform == UnrealTargetPlatform.Win64)
		{
			string Win64LibraryPath = Path.Combine(LibraryPath, "Win64");
			PublicAdditionalLibraries.Add(Path.Combine(Win64LibraryPath, "hiredisd.lib"));

			AddRuntimeDependencies(new string[]
			{
				"hiredisd.dll",
			}, Win64LibraryPath, false);
		}
		else if (Target.Platform == UnrealTargetPlatform.HoloLens)
		{
			string Arm64LibraryPath = Path.Combine(LibraryPath, "Arm64");
			PublicAdditionalLibraries.Add(Path.Combine(Arm64LibraryPath, "hiredis.lib"));

			AddRuntimeDependencies(new string[]
			{
				"hiredis.dll",
			}, Arm64LibraryPath, false);
		}
	}
}