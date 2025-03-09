using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

public static class AutoBuilder
{
    static string[] GetScenePaths()
    {
        return EditorBuildSettings.scenes.Select(scene => scene.path).ToArray();
    }

    [MenuItem("AutoBuilder/iOS")]
    static void PerformiOSBuild()
    {
        PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.iOS, ApiCompatibilityLevel.NET_Unity_4_8);
		PlayerSettings.insecureHttpOption = InsecureHttpOption.AlwaysAllowed;
        PlayerSettings.iOS.targetDevice = iOSTargetDevice.iPhoneOnly;
        PlayerSettings.iOS.targetOSVersionString = "12.0";
        UnityEditor.CrashReporting.CrashReportingSettings.enabled = false;
        BuildPipeline.BuildPlayer(GetScenePaths(), "Builds/iosBuild", BuildTarget.iOS, BuildOptions.CompressWithLz4);
    }


    [PostProcessBuild(99999)]
    public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            OnPostprocessBuildIOS(path);
        }
    }

    static void OnPostprocessBuildIOS(string path)
    {
        string projPath = PBXProject.GetPBXProjectPath(path);
        PBXProject project = new PBXProject();

        project.ReadFromString(File.ReadAllText(projPath));
        string mainTargetGuid = project.GetUnityMainTargetGuid();
        string frameworkTargetGuid = project.GetUnityFrameworkTargetGuid();
        // string projectGuid = project.ProjectGuid();

		//For Unity >=2022
		string gameAssemblyTargetGuid = project.TargetGuidByName("GameAssembly");
		if (!string.IsNullOrEmpty(gameAssemblyTargetGuid))
        {
            AddProcessLibsBuildPhase(path, ref project, gameAssemblyTargetGuid);
			AddFixPermissionsBuildPhase2(path, ref project, gameAssemblyTargetGuid);
        }
		//For Unity <=2021
		else
		{
			AddProcessLibsBuildPhase(path, ref project, frameworkTargetGuid);
			AddFixPermissionsBuildPhase2(path, ref project, frameworkTargetGuid);
		}
		
		project.SetBuildProperty(mainTargetGuid, "ENABLE_BITCODE", "NO");
		project.SetBuildProperty(frameworkTargetGuid, "ENABLE_BITCODE", "NO");
		project.SetBuildProperty(frameworkTargetGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");
		
		string extensionTargetGuid = project.TargetGuidByName("OneSignalNotificationServiceExtension");
        if (!string.IsNullOrEmpty(extensionTargetGuid))
        {
            project.SetBuildProperty(extensionTargetGuid, "ENABLE_BITCODE", "NO");
			project.SetBuildProperty(extensionTargetGuid, "IPHONEOS_DEPLOYMENT_TARGET", "12.0");
        }
		
		AddSPM(path, ref project, mainTargetGuid, frameworkTargetGuid, extensionTargetGuid);

        project.WriteToFile(projPath);

        RemoveLibs(path);
        AddGitattributes(path);
		
		ResaveInfoPlist(path);
    }

    static void AddGitattributes(string path)
    {
        CreateFile("* text=auto", Path.Combine(path, ".gitattributes"));
    }

    static void AddFixPermissionsBuildPhase2(string path, ref PBXProject project, string targetGuid)
    {
        string shellScriptName = "Fix Permissions for process_libs in Windows Generated XcodeProject";
        string shellPath = "/bin/sh";
        string shellScript = "chmod a+x ./process_libraries.sh";//"chmod a+x ./process_libraries.sh\nxattr -dr com.apple.quarantine .";
        project.InsertShellScriptBuildPhase(0, targetGuid, shellScriptName, shellPath, shellScript);
    }

    static void AddProcessLibsBuildPhase(string path, ref PBXProject project, string targetGuid)
    {
        CopyProcessLibsScript(path);
        string shellScriptName = "Process libraries";
        string shellPath = "/bin/sh";
        string shellScript = "\"$PROJECT_DIR/process_libraries.sh\"";
        project.InsertShellScriptBuildPhase(0, targetGuid, shellScriptName, shellPath, shellScript);
    }

    static void CopyProcessLibsScript(string path)
    {
		string procLibsShPath = Path.Combine(path, "process_libraries.sh");
        File.Copy("Assets/Editor/process_libraries.sh", procLibsShPath);
		string shText = File.ReadAllText(procLibsShPath);
		shText = shText.Replace("{UNITY_VERSION}", Application.unityVersion);
		File.WriteAllText(procLibsShPath, shText);
    }

    static void RemoveLibs(string path)
    {
        string[] libs = new string[] { "baselib.a", "libiPhone-lib.a", "libil2cpp.a" };
        foreach (string lib in libs)
        {
            string libPath = Path.Combine(path, "Libraries", lib);
            if (File.Exists(libPath))
            {
                File.Delete(libPath);
            }
        }
    }

    static void CreateFile(string content, string path)
    {
        using (FileStream fs = File.Create(path))
        {
            byte[] info = new UTF8Encoding(true).GetBytes(content);
            fs.Write(info, 0, info.Length);
        }
    }
	
	static void ResaveInfoPlist(string path)
	{
		string plistPath = Path.Combine(path, "Info.plist");
		byte[] plistBytes = File.ReadAllBytes(plistPath);

		byte[] bom = Encoding.UTF8.GetPreamble();
		if (SearchBytes(plistBytes, bom) == 0)
			plistBytes = plistBytes.Skip(bom.Length).ToArray();
		File.WriteAllBytes(plistPath, plistBytes);
	}
	
	static int SearchBytes(byte[] haystack, byte[] needle)
	{
		var len = needle.Length;
		var limit = haystack.Length - len;
		for (var i = 0; i <= limit; i++)
		{
			var k = 0;
			for (; k < len; k++)
			{
				if (needle[k] != haystack[i + k]) break;
			}
			if (k == len) return i;
		}
		return -1;
	}
	
	static void AddSPM(string path, ref PBXProject project, string mainTargetGuid, string frameworkTargetGuid, string extensionTargetGuid = null) {
		string spmJsonPath = "Assets/Editor/AutoBuilderSPM.json";
		string spmJsonText = File.ReadAllText(spmJsonPath);
		AutoBuilderSPMLibArray spmLibsArr = JsonUtility.FromJson<AutoBuilderSPMLibArray>(spmJsonText);
		
		List<AutoBuilderSPM> spms = new List<AutoBuilderSPM>();
		if (spmLibsArr.spm != null) {
			spms = spmLibsArr.spm.ToList();
		}
		
		string podfilePath = Path.Combine(path, "Podfile");
		if (File.Exists(podfilePath))
		{
			string podfileText = File.ReadAllText(podfilePath);
			List<Pod> podList = ParsePodfile(podfileText);
			List<AutoBuilderSPM> spmToAdd = PodsToAutoBuilderSPM(podList, spmLibsArr);
			if (spmToAdd != null)
			{
				spms = spms.Where(spm => !spmToAdd.Any(spma => spma.gitUrl == spm.gitUrl)).ToList();
				spms.AddRange(spmToAdd);
			}
		}
		
		foreach (AutoBuilderSPM spm in spms)
        {
			string spmGuid = null;
			switch (spm.packageRequirement.kind) {
				case "revision":
					spmGuid = project.AddRemotePackageReferenceAtRevision(spm.gitUrl, spm.packageRequirement.revision);
					break;
				case "branch":
					spmGuid = project.AddRemotePackageReferenceAtBranch(spm.gitUrl, spm.packageRequirement.branch);
					break;
				case "exactVersion":
					spmGuid = project.AddRemotePackageReferenceAtVersion(spm.gitUrl, spm.packageRequirement.version);
					break;
				case "versionRange":
					spmGuid = project.AddRemotePackageReferenceWithVersionRange(spm.gitUrl, spm.packageRequirement.minimumVersion, spm.packageRequirement.maximumVersion);
					break;
				case "upToNextMinorVersion":
					spmGuid = project.AddRemotePackageReferenceAtVersionUpToNextMinor(spm.gitUrl, spm.packageRequirement.minimumVersion);
					break;
				case "upToNextMajorVersion":
					spmGuid = project.AddRemotePackageReferenceAtVersionUpToNextMajor(spm.gitUrl, spm.packageRequirement.minimumVersion);
					break;
			}
			if (!string.IsNullOrEmpty(spmGuid))
			{
				foreach (TargetFramework targetFramework in spm.targetFrameworks)
				{
					switch (targetFramework.target) {
						case "Unity-iPhone":
							project.AddRemotePackageFrameworkToProject(mainTargetGuid, targetFramework.name, spmGuid, false);
							break;
						case "UnityFramework":
							project.AddRemotePackageFrameworkToProject(frameworkTargetGuid, targetFramework.name, spmGuid, false);
							break;
						case "OneSignalNotificationServiceExtension":
							if (!string.IsNullOrEmpty(extensionTargetGuid)) {
								project.AddRemotePackageFrameworkToProject(extensionTargetGuid, targetFramework.name, spmGuid, false);
							}
							break;
					}
				}
			}
        }
	}
	
	static List<Pod> ParsePodfile(string input)
	{
		string pattern = @"target\s*\'(?<Target>[^']+?)\'\s*do\s*(?:pod\s*\'(?<FrameworkName>[^']+?)\'(?:,*\s*\'(?<FrameworkVersion>[^']+?)\'(?:,*\s*\'[^']+?\')*|,*\s*(?<FrameworkVersion>:git\s*=>\s*\'[^']+?\'(?:,*\s*[a-zA-Z0-9:_-]+?\s*=>\s*\'[^']+?\')*,*\s*:branch\s*=>\s*\'[^']+?\')(?:,*\s*[a-zA-Z0-9:_-]+?\s*=>\s*\'[^']+?\')*|,*\s*:git\s*=>\s*\'(?<FrameworkVersion>[^']+?)\'(?:,*\s*[a-zA-Z0-9:_-]+?\s*=>\s*\'[^']+?\')*|(?<FrameworkVersion>.*?))\s*)+?end";
		RegexOptions options = RegexOptions.Multiline;
		Regex regex = new Regex(pattern, options);

		List<Pod> podList = new List<Pod>();
		Match match = regex.Match(input);

		do
		{
			GroupCollection groups = match.Groups;

			CaptureCollection targetCC = groups["Target"].Captures;
			Pod newPod = new Pod()
			{
				target = targetCC.FirstOrDefault().Value,
				frameworks = new PodFramework[] { }
			};
			List<PodFramework> newPodFrameworks = new List<PodFramework>();

			CaptureCollection frameworkNameCC = groups["FrameworkName"].Captures;
			CaptureCollection frameworkVersionCC = groups["FrameworkVersion"].Captures;
			for (int i = 0; i < frameworkNameCC.Count; i++)
			{
				PodFrameworkVersion version = ParsePodFrameworkVersion(frameworkVersionCC[i].Value);
				if (version != null)
				{
					PodFramework newPodFramework = new PodFramework()
					{
						name = frameworkNameCC[i].Value,
						version = version
					};

					newPodFrameworks.Add(newPodFramework);
				}
			}

			newPod.frameworks = newPodFrameworks.ToArray();
			podList.Add(newPod);
			match = match.NextMatch();
		}
		while (match != null && match.Success);

		return podList;
	}

	static PodFrameworkVersion ParsePodFrameworkVersion(string input)
	{
		PodFrameworkVersion newVersion = null;

		if (input.ToLowerInvariant().Contains("git"))
		{
			if (input.ToLowerInvariant().Contains(":branch"))
			{
				string pattern = @":git\s*=>\s*\'(?<GitUrl>[^']+?)\'(?:,*\s*[a-zA-Z0-9:_-]+?\s*=>\s*\'[^']+?\')*,*\s*:branch\s*=>\s*\'(?<GitBranch>[^']+?)\'";
				RegexOptions options = RegexOptions.Multiline;
				Regex regex = new Regex(pattern, options);

				Match match = regex.Match(input);
				if (match != null)
				{
					GroupCollection groups = match.Groups;

					CaptureCollection urlCC = groups["GitUrl"].Captures;
					CaptureCollection branchCC = groups["GitBranch"].Captures;

					newVersion = new PodFrameworkVersion()
					{
						gitUrl = urlCC.FirstOrDefault().Value,
						type = "branch",
						branch = branchCC.FirstOrDefault().Value
					};
				}
			}
			else
			{
				newVersion = new PodFrameworkVersion()
				{
					gitUrl = input,
					type = "branch",
					branch = "master"
				};
			}
		}
		else
		{
			string pattern = @"(?<Type>[~><=]*)\s*(?<Version>[\S]+)";
			RegexOptions options = RegexOptions.Multiline;
			Regex regex = new Regex(pattern, options);

			Match match = regex.Match(input);
			if (match != null)
			{
				GroupCollection groups = match.Groups;

				CaptureCollection typeCC = groups["Type"].Captures;
				CaptureCollection versionCC = groups["Version"].Captures;

				Version version = GetPodFrameworkVersion(versionCC.FirstOrDefault().Value);
				string type = typeCC.FirstOrDefault().Value;

				if (version != null)
				{
					newVersion = new PodFrameworkVersion()
					{
						version = version,
						type = GetPodFrameworkVersionType(version, type)
					};
				}
			}
		}

		return newVersion;
	}

	static Version GetPodFrameworkVersion(string input)
	{
		Version newVersion = null;

		string pattern = @"(?<Major>[\d]+)\.?(?<Minor>[\d]*)\.?(?<Patch>[\d]*)";
		RegexOptions options = RegexOptions.Multiline;
		Regex regex = new Regex(pattern, options);

		Match match = regex.Match(input);
		if (match != null)
		{
			GroupCollection groups = match.Groups;

			CaptureCollection majorCC = groups["Major"].Captures;
			CaptureCollection minorCC = groups["Minor"].Captures;
			CaptureCollection patchCC = groups["Patch"].Captures;

			newVersion = new Version()
			{
				major = majorCC.FirstOrDefault().Value,
				minor = minorCC.FirstOrDefault().Value,
				patch = patchCC.FirstOrDefault().Value
			};
		}

		return newVersion;
	}

	static string GetPodFrameworkVersionType(Version version, string type)
	{
		if (string.IsNullOrEmpty(type))
		{
			return "exactVersion";
		}

		if (type.Contains("="))
		{
			return "upToNextMajorVersion";
		}

		if (string.IsNullOrEmpty(version.patch))
		{
			return "upToNextMajorVersion";
		}

		return "upToNextMinorVersion";
	}

	//here
	static List<AutoBuilderSPM> PodsToAutoBuilderSPM(List<Pod> podList, AutoBuilderSPMLibArray spmLibArr)
	{
		List<AutoBuilderSPM> autoBuilderSPMs = new List<AutoBuilderSPM>();

		if (podList != null && spmLibArr != null && spmLibArr.spmLibs != null)
		{
			Dictionary<string, AutoBuilderSPM> tempSPM = new Dictionary<string, AutoBuilderSPM>();
			foreach (Pod pod in podList)
			{
				foreach (PodFramework podFramework in pod.frameworks)
				{
					AutoBuilderSPMLib[] spmLibs = spmLibArr.spmLibs.Where(spml => spml.podFrameworks.Any(pf => pf.podName == podFramework.name)).ToArray();
					string[] spmFrameworkNames = null;
					string gitUrl = null;
					XCRemoteSwiftPackageReferenceObjectRequirement packageRequirement = GetSPMPackageRequirement(podFramework.version);

					if (spmLibs != null && spmLibs.Length > 0)
					{
						AutoBuilderSPMLib spmLib = null;
						foreach (AutoBuilderSPMLib spmLibTemp in spmLibs)
						{
							if (podFramework.version.version != null && !podFramework.version.version.IsNull())
							{
								bool podFrameworkVersionMajorNotNull = false;
								int podFrameworkVersionMajor = 0;
								if (!string.IsNullOrEmpty(podFramework.version.version.major))
								{
									podFrameworkVersionMajorNotNull = int.TryParse(podFramework.version.version.major, out podFrameworkVersionMajor);
								}
								bool podFrameworkVersionMinorNotNull = false;
								int podFrameworkVersionMinor = 0;
								if (!string.IsNullOrEmpty(podFramework.version.version.minor))
								{
									podFrameworkVersionMinorNotNull = int.TryParse(podFramework.version.version.minor, out podFrameworkVersionMinor);
								}
								bool podFrameworkVersionPatchNotNull = false;
								int podFrameworkVersionPatch = 0;
								if (!string.IsNullOrEmpty(podFramework.version.version.patch))
								{
									podFrameworkVersionPatchNotNull = int.TryParse(podFramework.version.version.patch, out podFrameworkVersionPatch);
								}

								if (spmLibTemp.minVersion != null && !spmLibTemp.minVersion.IsNull())
								{
									bool spmMinVersionIsLess = false;
									if (podFrameworkVersionMajorNotNull && !string.IsNullOrEmpty(spmLibTemp.minVersion.major))
									{
										int spmMajor = 0;
										if (int.TryParse(spmLibTemp.minVersion.major, out spmMajor))
										{
											spmMinVersionIsLess = spmMajor < podFrameworkVersionMajor;
											if (spmMajor > podFrameworkVersionMajor)
											{
												continue;
											}
										}
									}
									if (!spmMinVersionIsLess && podFrameworkVersionMinorNotNull && !string.IsNullOrEmpty(spmLibTemp.minVersion.minor))
									{
										int spmMinor = 0;
										if (int.TryParse(spmLibTemp.minVersion.minor, out spmMinor))
										{
											spmMinVersionIsLess = spmMinor < podFrameworkVersionMinor;
											if (spmMinor > podFrameworkVersionMinor)
											{
												continue;
											}
										}
									}
									if (!spmMinVersionIsLess && podFrameworkVersionPatchNotNull && !string.IsNullOrEmpty(spmLibTemp.minVersion.patch))
									{
										int spmPatch = 0;
										if (int.TryParse(spmLibTemp.minVersion.patch, out spmPatch))
										{
											if (spmPatch > podFrameworkVersionPatch)
											{
												continue;
											}
										}
									}
								}

								if (spmLibTemp.maxVersion != null && !spmLibTemp.maxVersion.IsNull())
								{
									bool spmMaxVersionIsBigger = false;
									if (podFrameworkVersionMajorNotNull && !string.IsNullOrEmpty(spmLibTemp.maxVersion.major))
									{
										int spmMajor = 0;
										if (int.TryParse(spmLibTemp.maxVersion.major, out spmMajor))
										{
											spmMaxVersionIsBigger = spmMajor > podFrameworkVersionMajor;
											if (spmMajor < podFrameworkVersionMajor)
											{
												continue;
											}
										}
									}
									if (!spmMaxVersionIsBigger && podFrameworkVersionMinorNotNull && !string.IsNullOrEmpty(spmLibTemp.maxVersion.minor))
									{
										int spmMinor = 0;
										if (int.TryParse(spmLibTemp.maxVersion.minor, out spmMinor))
										{
											spmMaxVersionIsBigger = spmMinor > podFrameworkVersionMinor;
											if (spmMinor < podFrameworkVersionMinor)
											{
												continue;
											}
										}
									}
									if (!spmMaxVersionIsBigger && podFrameworkVersionPatchNotNull && !string.IsNullOrEmpty(spmLibTemp.maxVersion.patch))
									{
										int spmPatch = 0;
										if (int.TryParse(spmLibTemp.maxVersion.patch, out spmPatch))
										{
											if (spmPatch < podFrameworkVersionPatch)
											{
												continue;
											}
										}
									}
								}
							}
							spmLib = spmLibTemp;
							break;
						}

						if (spmLib != null)
						{
							SPMLibPodFramework spmLibPodFramework = spmLib.podFrameworks.FirstOrDefault(pf =>
							{
								if (!string.IsNullOrEmpty(pf.target))
								{
									return pf.target == pod.target && pf.podName == podFramework.name;
								}
								return pf.podName == podFramework.name;
							});
							if (spmLibPodFramework != null)
							{
								spmFrameworkNames = spmLibPodFramework.spmNames;
							}
							gitUrl = spmLib.gitUrl;
							if (spmLib.packageRequirement != null && PackageRequirementIsValid(spmLib.packageRequirement))
							{
								packageRequirement = spmLib.packageRequirement;
							}
						}
					}

					if (!string.IsNullOrEmpty(podFramework.version.gitUrl))
					{
						if (spmFrameworkNames == null || spmFrameworkNames.Length == 0)
						{
							spmFrameworkNames = new string[] { podFramework.name };
						}
						gitUrl = podFramework.version.gitUrl;
					}

					if (spmFrameworkNames != null && spmFrameworkNames.Length > 0 && !string.IsNullOrEmpty(gitUrl))
					{
						foreach (string spmFrameworkName in spmFrameworkNames)
						{
							if (!string.IsNullOrEmpty(spmFrameworkName))
							{
								TargetFramework newTargetFramework = new TargetFramework()
								{
									name = spmFrameworkName,
									target = pod.target
								};

								if (tempSPM.ContainsKey(gitUrl))
								{
									if (!tempSPM[gitUrl].targetFrameworks.Any(tf => tf.name == newTargetFramework.name && tf.target == newTargetFramework.target))
									{
										List<TargetFramework> targetFrameworksList = tempSPM[gitUrl].targetFrameworks.ToList();
										targetFrameworksList.Add(newTargetFramework);
										tempSPM[gitUrl].targetFrameworks = targetFrameworksList.ToArray();
									}
								}
								else
								{
									if (packageRequirement != null)
									{
										AutoBuilderSPM newAutoBuilderSPM = new AutoBuilderSPM()
										{
											gitUrl = gitUrl,
											packageRequirement = packageRequirement,
											targetFrameworks = new TargetFramework[] { newTargetFramework }
										};
										tempSPM.Add(gitUrl, newAutoBuilderSPM);
									}
								}

								if (tempSPM.ContainsKey(gitUrl) && pod.target.ToLowerInvariant().Contains("framework"))
								{
									TargetFramework newTargetFrameworkToProject = new TargetFramework()
									{
										name = spmFrameworkName,
										target = "Unity-iPhone"
									};
									if (!tempSPM[gitUrl].targetFrameworks.Any(tf => tf.name == newTargetFrameworkToProject.name && tf.target == newTargetFrameworkToProject.target))
									{
										List<TargetFramework> targetFrameworksList = tempSPM[gitUrl].targetFrameworks.ToList();
										targetFrameworksList.Add(newTargetFrameworkToProject);
										tempSPM[gitUrl].targetFrameworks = targetFrameworksList.ToArray();
									}
								}
							}
						}
					}
				}
			}
			autoBuilderSPMs = tempSPM.Values.ToList();
		}

		return autoBuilderSPMs;
	}

	static XCRemoteSwiftPackageReferenceObjectRequirement GetSPMPackageRequirement(PodFrameworkVersion version)
	{
		XCRemoteSwiftPackageReferenceObjectRequirement packageRequirement = null;

		if (version != null)
		{
			switch (version.type)
			{
				case "exactVersion":
					packageRequirement = new XCRemoteSwiftPackageReferenceObjectRequirement()
					{
						kind = version.type,
						version = version.version.ToString()
					};
					break;
				case "upToNextMajorVersion":
					packageRequirement = new XCRemoteSwiftPackageReferenceObjectRequirement()
					{
						kind = version.type,
						minimumVersion = version.version.ToString()
					};
					break;
				case "upToNextMinorVersion":
					packageRequirement = new XCRemoteSwiftPackageReferenceObjectRequirement()
					{
						kind = version.type,
						minimumVersion = version.version.ToString()
					};
					break;
				case "branch":
					packageRequirement = new XCRemoteSwiftPackageReferenceObjectRequirement()
					{
						kind = version.type,
						branch = version.branch
					};
					break;
				default:
					break;
			}
		}

		return packageRequirement;
	}

	static bool PackageRequirementIsValid(XCRemoteSwiftPackageReferenceObjectRequirement packageRequirement)
	{
		bool isValid = false;
		switch (packageRequirement.kind)
		{
			case "revision":
				isValid = !string.IsNullOrEmpty(packageRequirement.revision);
				break;
			case "branch":
				isValid = !string.IsNullOrEmpty(packageRequirement.branch);
				break;
			case "exactVersion":
				isValid = !string.IsNullOrEmpty(packageRequirement.version);
				break;
			case "versionRange":
				isValid = !string.IsNullOrEmpty(packageRequirement.minimumVersion) && !string.IsNullOrEmpty(packageRequirement.maximumVersion);
				break;
			case "upToNextMinorVersion":
				isValid = !string.IsNullOrEmpty(packageRequirement.minimumVersion);
				break;
			case "upToNextMajorVersion":
				isValid = !string.IsNullOrEmpty(packageRequirement.minimumVersion);
				break;
		}
		return isValid;
	}

	[System.Serializable]
	public class Version
	{
		public string major;
		public string minor;
		public string patch;

		public bool IsNull()
		{
			bool isNull = string.IsNullOrEmpty(major) && string.IsNullOrEmpty(minor) && string.IsNullOrEmpty(patch);
			return isNull;
		}

		public override string ToString()
		{
			string retStr = "";
			if (!string.IsNullOrEmpty(major))
			{
				retStr += major;
			}
			else
			{
				retStr += "0";
			}
			if (!string.IsNullOrEmpty(minor))
			{
				retStr += "." + minor;
			}
			else
			{
				retStr += ".0";
			}
			if (!string.IsNullOrEmpty(patch))
			{
				retStr += "." + patch;
			}
			else
			{
				retStr += ".0";
			}
			return retStr;
		}
	}

	[System.Serializable]
	public class PodFrameworkVersion
	{
		public Version version;
		public string type;
		public string gitUrl;
		public string branch;
	}

	[System.Serializable]
	public class PodFramework
	{
		public string name;
		public PodFrameworkVersion version;
	}

	[System.Serializable]
	public class Pod
	{
		public string target;
		public PodFramework[] frameworks;
	}

	[System.Serializable]
	public class AutoBuilderSPMLibArray
	{
		public AutoBuilderSPMLib[] spmLibs;
		public AutoBuilderSPM[] spm;
	}

	[System.Serializable]
	public class SPMLibPodFramework
	{
		public string podName;
		public string[] spmNames;
		public string target;
	}

	[System.Serializable]
	public class AutoBuilderSPMLib
	{
		public string gitUrl;
		public Version minVersion;
		public Version maxVersion;
		public XCRemoteSwiftPackageReferenceObjectRequirement packageRequirement;
		public SPMLibPodFramework[] podFrameworks;
	}

	[System.Serializable]
	public class AutoBuilderSPM
	{
		public string gitUrl;
		public XCRemoteSwiftPackageReferenceObjectRequirement packageRequirement;
		public TargetFramework[] targetFrameworks;
	}

	[System.Serializable]
	public class XCRemoteSwiftPackageReferenceObjectRequirement
	{
		public string kind;
		public string revision;
		public string branch;
		public string minimumVersion;
		public string maximumVersion;
		public string version;
	}

	[System.Serializable]
	public class TargetFramework
	{
		public string name;
		public string target;
	}
}