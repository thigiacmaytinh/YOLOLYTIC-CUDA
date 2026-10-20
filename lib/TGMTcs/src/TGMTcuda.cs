using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using TGMTcs;


namespace TGMTcs
{
    class TGMTcuda
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct nvmlUtilization_t
        {
            public uint gpu;
            public uint memory;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct nvmlMemory_t
        {
            public ulong total;
            public ulong free;
            public ulong used;
        }

        [DllImport("nvml.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nvmlInit_v2();

        [DllImport("nvml.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nvmlDeviceGetHandleByIndex_v2(uint index, out IntPtr device);

        [DllImport("nvml.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr nvmlErrorString(int result);
        [DllImport("nvml.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nvmlDeviceGetMemoryInfo(IntPtr device, out nvmlMemory_t memory);


        [DllImport("nvml.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nvmlDeviceGetUtilizationRates(
            IntPtr device,
            out nvmlUtilization_t utilization);

        private string NvmlError(int code)
        {
            IntPtr p = nvmlErrorString(code);
            return p == IntPtr.Zero ? ("NVML error " + code) : Marshal.PtrToStringAnsi(p);
        }

        [DllImport("nvml.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int nvmlShutdown();

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr SendMessageTimeout(
            IntPtr hWnd, uint msg, IntPtr wParam, string lParam,
            uint flags, uint timeout, out IntPtr result);

        private const string UserEnvironmentRegistryKey = @"Environment";
        private const int WM_SETTINGCHANGE = 0x001A;
        private const uint SMTO_ABORTIFHUNG = 0x0002;
        private static readonly IntPtr HWND_BROADCAST = new IntPtr(0xffff);

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        static TGMTcuda m_instance;
        private bool _nvmlReady = false;
        private IntPtr _gpuDevice = IntPtr.Zero;

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        TGMTcuda()
        {

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        ~TGMTcuda()
        {
            ShutdownNvml();
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static TGMTcuda GetInstance()
        {
            if(m_instance == null)
                m_instance = new TGMTcuda();
            return m_instance;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool InitNvml()
        {
            try
            {
                int rc = nvmlInit_v2();
                if(rc != 0)
                {
                    Debug.WriteLine("nvmlInit_v2 failed: " + NvmlError(rc));
                    return false;
                }

                rc = nvmlDeviceGetHandleByIndex_v2(0, out _gpuDevice);
                if(rc != 0)
                {
                    Debug.WriteLine("nvmlDeviceGetHandleByIndex_v2 failed: " + NvmlError(rc));
                    nvmlShutdown();
                    return false;
                }

                _nvmlReady = true;
                return true;
            }
            catch(Exception ex)
            {
                Debug.WriteLine("InitNvml exception: " + ex.Message);
                return false;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ShutdownNvml()
        {
            if(!_nvmlReady)
                return;

            try
            {
                nvmlShutdown();
            }
            catch
            {
            }

            _nvmlReady = false;
            _gpuDevice = IntPtr.Zero;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public Tuple<int, int, int> GetUsage()
        {
            IntPtr GPUpointer;
            nvmlDeviceGetHandleByIndex_v2(0, out GPUpointer);

            nvmlUtilization_t util;
            nvmlDeviceGetUtilizationRates(GPUpointer, out util);

            


            nvmlMemory_t mem;
            nvmlDeviceGetMemoryInfo(GPUpointer, out mem);

            double VRAMusageMB = mem.used / 1024 / 1024;
            double VRAMtotalMB = mem.total / 1024 / 1024;

            return new Tuple<int, int, int>((int)util.gpu, (int)VRAMusageMB, (int)VRAMtotalMB);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static CheckResult Check()
        {
            var result = new CheckResult();
            Version cudaVersion = null;
            string cudaRoot = null;
            string cuDnnRoot = null;

            cudaRoot = Environment.GetEnvironmentVariable("CUDA_PATH");

            if (!string.IsNullOrWhiteSpace(cudaRoot) && Directory.Exists(cudaRoot))
            {
                cudaVersion = ParseCudaVersionFromPath(cudaRoot);
            }

            if (cudaVersion == null)
            {
                var baseDir = @"C:\Program Files\NVIDIA GPU Computing Toolkit\CUDA";
                if (Directory.Exists(baseDir))
                {
                    var best = Directory.GetDirectories(baseDir, "v*")
                        .Select(d => new
                        {
                            Path = d,
                            Version = ParseCudaVersionFromPath(d)
                        })
                        .Where(x => x.Version != null)
                        .OrderByDescending(x => x.Version)
                        .FirstOrDefault();

                    if (best != null)
                    {
                        cudaRoot = best.Path;
                        cudaVersion = best.Version;
                    }
                }
            }

            if (cudaVersion == null)
            {
                Version nvccVersion;
                if (TryGetCudaVersionFromNvcc(out nvccVersion))
                {
                    cudaVersion = nvccVersion;
                }
            }


            bool isEnoughFiles = CheckCudaFiles();

            result.CudaRoot = cudaRoot;
            result.CudaVersion = cudaVersion;
            result.CudaInstalled = cudaVersion != null && isEnoughFiles;
            result.CudaAtLeast12_8 = cudaVersion != null && cudaVersion >= new Version(12, 8);

            Version cuDnnVersion;
            if (TryGetCuDnnVersion(out cuDnnRoot, out cuDnnVersion, cudaRoot))
            {
                isEnoughFiles = CheckCudnnFiles();
                result.CuDnnInstalled = isEnoughFiles;
                result.CuDnnRoot = cuDnnRoot;
                result.CuDnnVersion = cuDnnVersion;
                result.CuDnnAtLeast9_8 = cuDnnVersion >= new Version(9, 8);
            }
            else
            {
                result.CuDnnInstalled = false;
                result.CuDnnRoot = null;
                result.CuDnnAtLeast9_8 = false;
            }

            result.Details =
                "CUDA: " + (result.CudaInstalled ? result.CudaVersion.ToString() : "not found") +
                ", cuDNN: " + (result.CuDnnInstalled ? result.CuDnnVersion.ToString() : "not found") +
                ", CUDA root: " + (result.CudaRoot ?? "n/a") +
                ", cuDNN root: " + (result.CuDnnRoot ?? "n/a");

            return result;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private static Version ParseCudaVersionFromPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;

            var name = new DirectoryInfo(path).Name;
            var m = Regex.Match(name, @"^v(\d+)\.(\d+)$", RegexOptions.IgnoreCase);
            if (!m.Success)
                return null;

            int major, minor;
            if (int.TryParse(m.Groups[1].Value, out major) &&
                int.TryParse(m.Groups[2].Value, out minor))
            {
                return new Version(major, minor);
            }

            return null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private static bool TryGetCudaVersionFromNvcc(out Version version)
        {
            version = null;

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c nvcc -V",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var p = Process.Start(psi))
                {
                    string output = p.StandardOutput.ReadToEnd();
                    string err = p.StandardError.ReadToEnd();
                    p.WaitForExit(3000);

                    var text = output + Environment.NewLine + err;
                    var m = Regex.Match(text, @"release\s+(\d+)\.(\d+)", RegexOptions.IgnoreCase);
                    if (m.Success)
                    {
                        version = new Version(
                            int.Parse(m.Groups[1].Value),
                            int.Parse(m.Groups[2].Value));
                        return true;
                    }
                }
            }
            catch
            {
            }

            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private static bool TryGetCuDnnVersion(out string cuDnnRoot, out Version version, string cudaRoot)
        {
            cuDnnRoot = null;
            version = null;

            Version cudaVersion = ParseCudaVersionFromPath(cudaRoot);
            string cudaFolderName = cudaVersion != null
                ? cudaVersion.Major + "." + cudaVersion.Minor
                : null;

            var baseDir = @"C:\Program Files\NVIDIA\CUDNN";
            if (Directory.Exists(baseDir))
            {
                var best = Directory.GetDirectories(baseDir, "v*")
                    .Select(d => new
                    {
                        Path = d,
                        Version = ParseCuDnnVersionFromPath(d)
                    })
                    .Where(x => x.Version != null)
                    .OrderByDescending(x => x.Version)
                    .FirstOrDefault();

                if (best != null)
                {
                    Version parsed;

                    // 1) First try CUDA-specific subfolder, for example:
                    // C:\Program Files\NVIDIA\CUDNN\v9.8\include\12.8\cudnn_version.h
                    if (!string.IsNullOrWhiteSpace(cudaFolderName))
                    {
                        var cudaSpecificCandidates = new[]
                        {
                            Path.Combine(best.Path, "include", cudaFolderName, "cudnn_version.h"),
                            Path.Combine(best.Path, "include", cudaFolderName, "cudnn.h"),
                            Path.Combine(best.Path, "bin", cudaFolderName, "cudnn_version.h"),
                            Path.Combine(best.Path, "bin", cudaFolderName, "cudnn.h")
                        };

                        foreach (var file in cudaSpecificCandidates)
                        {
                            if (TryReadCuDnnHeader(file, out parsed))
                            {
                                cuDnnRoot = best.Path;
                                version = parsed;
                                return true;
                            }
                        }

                        // fallback if headers are not in subfolder but CUDA-specific folder structure exists
                        if ((Directory.Exists(Path.Combine(best.Path, "include", cudaFolderName)) ||
                             Directory.Exists(Path.Combine(best.Path, "bin", cudaFolderName)) ||
                             Directory.Exists(Path.Combine(best.Path, "lib", cudaFolderName)) ||
                             Directory.Exists(Path.Combine(best.Path, "lib", cudaFolderName, "x64"))))
                        {
                            cuDnnRoot = best.Path;
                            version = best.Version;
                            return true;
                        }
                    }

                    // 2) Standard cuDNN root layout
                    var rootCandidates = new[]
                    {
                        Path.Combine(best.Path, "include", "cudnn_version.h"),
                        Path.Combine(best.Path, "include", "cudnn.h")
                    };

                    foreach (var file in rootCandidates)
                    {
                        if (TryReadCuDnnHeader(file, out parsed))
                        {
                            cuDnnRoot = best.Path;
                            version = parsed;
                            return true;
                        }
                    }

                    // fallback: use folder version if standard structure exists
                    if (Directory.Exists(Path.Combine(best.Path, "bin")) &&
                        Directory.Exists(Path.Combine(best.Path, "include")))
                    {
                        cuDnnRoot = best.Path;
                        version = best.Version;
                        return true;
                    }
                }
            }

            // 3) Legacy fallback: cuDNN copied into CUDA include folder
            if (!string.IsNullOrWhiteSpace(cudaRoot) && Directory.Exists(cudaRoot))
            {
                var candidates = new[]
                {
                    Path.Combine(cudaRoot, "include", "cudnn_version.h"),
                    Path.Combine(cudaRoot, "include", "cudnn.h")
                };

                foreach (var file in candidates)
                {
                    Version parsed;
                    if (TryReadCuDnnHeader(file, out parsed))
                    {
                        cuDnnRoot = cudaRoot;
                        version = parsed;
                        return true;
                    }
                }
            }

            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private static Version ParseCuDnnVersionFromPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;

            var name = new DirectoryInfo(path).Name;
            var m = Regex.Match(name, @"^v(\d+)\.(\d+)$", RegexOptions.IgnoreCase);
            if (!m.Success)
                return null;

            int major, minor;
            if (int.TryParse(m.Groups[1].Value, out major) &&
                int.TryParse(m.Groups[2].Value, out minor))
            {
                return new Version(major, minor);
            }

            return null;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private static bool TryReadCuDnnHeader(string file, out Version version)
        {
            version = null;

            if (!File.Exists(file))
                return false;

            var text = File.ReadAllText(file);

            int major, minor, patch;
            if (TryParseDefine(text, "CUDNN_MAJOR", out major) &&
                TryParseDefine(text, "CUDNN_MINOR", out minor) &&
                TryParseDefine(text, "CUDNN_PATCHLEVEL", out patch))
            {
                version = new Version(major, minor, patch);
                return true;
            }

            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private static bool TryParseDefine(string text, string name, out int value)
        {
            value = 0;
            var m = Regex.Match(
                text,
                @"#define\s+" + Regex.Escape(name) + @"\s+(\d+)",
                RegexOptions.Multiline);

            return m.Success && int.TryParse(m.Groups[1].Value, out value);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void ConfigureEnvironmentPaths()
        {
            VerifyFiles();

            string[] runtimePaths = { CudaBin, FindCuDnnBin() };

            // Preserve pre-existing %VARIABLE% references and registry value type.
            using(RegistryKey key = Registry.CurrentUser.CreateSubKey(
                UserEnvironmentRegistryKey, true))
            {
                if(key == null)
                    throw new UnauthorizedAccessException(
                        "Cannot open current user's Environment registry key.");

                object raw = key.GetValue("Path", null,
                    RegistryValueOptions.DoNotExpandEnvironmentNames);
                string oldPath = raw as string ?? "";
                string newPath = PutFirstWithoutDuplicates(oldPath, runtimePaths);

                if(!string.Equals(oldPath, newPath, StringComparison.Ordinal))
                {
                    RegistryValueKind kind = raw == null
                        ? RegistryValueKind.ExpandString
                        : key.GetValueKind("Path");

                    if(kind != RegistryValueKind.String &&
                        kind != RegistryValueKind.ExpandString)
                        kind = RegistryValueKind.ExpandString;

                    key.SetValue("Path", newPath, kind);
                }
            }

            // A running process keeps its existing environment block.
            // Update our process so child processes inherit the new PATH.
            string processPath = Environment.GetEnvironmentVariable(
                "Path", EnvironmentVariableTarget.Process) ?? "";
            Environment.SetEnvironmentVariable(
                "Path", PutFirstWithoutDuplicates(processPath, runtimePaths),
                EnvironmentVariableTarget.Process);

            // CUDA installer normally creates CUDA_PATH at Machine scope;
            // this only ensures our current process sees the expected path.
            Environment.SetEnvironmentVariable(
                "CUDA_PATH", CudaRoot, EnvironmentVariableTarget.Process);

            // Ask environment-aware GUI launchers (e.g., Explorer) to refresh.
            IntPtr ignored;
            SendMessageTimeout(HWND_BROADCAST, WM_SETTINGCHANGE, IntPtr.Zero,
                "Environment", SMTO_ABORTIFHUNG, 5000, out ignored);
        }

        public static bool VerifyFiles()
        {
            string[] cudaDlls = { "cudart64_12.dll", "cublas64_12.dll" };
            foreach(string dll in cudaDlls)
            {
                string fullPath = Path.Combine(CudaBin, dll);
                if(!File.Exists(fullPath))
                    return false;
                    //throw new FileNotFoundException("CUDA runtime DLL is missing: " + fullPath);
            }

            FindCuDnnBin();

            return true;
        }

        public static string CudaRoot
        {
            get
            {
                return Path.Combine(ProgramFiles64Path,
                    "NVIDIA GPU Computing Toolkit", "CUDA", "v12.8");
            }
        }

        public static string CudaBin
        {
            get { return Path.Combine(CudaRoot, "bin"); }
        }


        public static string CuDnnRoot
        {
            get { return Path.Combine(ProgramFiles64Path, "NVIDIA", "CUDNN", "v9.8"); }
        }

        //NVIDIA EXE often uses bin\12.8; a manually extracted installation
        // may use bin directly.Validate the actual DLL, not just the folder.
        public static string FindCuDnnBin()
        {
            string[] candidates =
            {
                Path.Combine(CuDnnRoot, "bin", "12.8"),
                Path.Combine(CuDnnRoot, "bin")
            };

            foreach(string candidate in candidates)
            {
                if(File.Exists(Path.Combine(candidate, "cudnn64_9.dll")))
                    return candidate;
            }

            return "";
            //throw new FileNotFoundException(
            //    "cuDNN 9 DLL was not found under: " + CuDnnRoot);
        }

        public static string ProgramFiles64Path
        {
            get
            {
                // Handle 32-bit installers on 64-bit Windows.
                string path = Environment.GetEnvironmentVariable("ProgramW6432");
                return !string.IsNullOrWhiteSpace(path)
                    ? path
                    : Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            }
        }

        private static string PutFirstWithoutDuplicates(
            string existingPath, IEnumerable<string> preferredPaths)
        {
            var result = new List<string>();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            IEnumerable<string> all = preferredPaths.Concat(
                (existingPath ?? "").Split(
                    new[] { ';' }, StringSplitOptions.RemoveEmptyEntries));

            foreach(string part in all)
            {
                string path = part.Trim();
                string normalized = path.Trim('"').TrimEnd('\\', '/');
                if(normalized.Length > 0 && seen.Add(normalized))
                    result.Add(path);
            }

            return string.Join(";", result);
        }

        public static bool CheckCudaFiles()
        {
            string cudaBinPath = @"C:\Program Files\NVIDIA GPU Computing Toolkit\CUDA\v12.8\bin";

            string[] requiredFiles =
            {
                "nvcc.exe",
                "cublas64_12.dll",
                "cublasLt64_12.dll",
                "cudart64_12.dll",
                "cufft64_11.dll",
                "cufftw64_11.dll",
                "cuinj64_128.dll",
                "curand64_10.dll",
                "cusolver64_11.dll",
                "cusolverMg64_11.dll",
                "cusparse64_12.dll",
                "nppc64_12.dll",
                "nppial64_12.dll",
                "nppicc64_12.dll",
                "nppidei64_12.dll",
                "nppif64_12.dll",
                "nppig64_12.dll",
                "nppim64_12.dll",
                "nppist64_12.dll",
                "nppisu64_12.dll",
                "nppitc64_12.dll",
                "npps64_12.dll",
                "nvblas64_12.dll",
                "nvfatbin_120_0.dll",
                "nvJitLink_120_0.dll",
                "nvjpeg64_12.dll",
                "nvrtc-builtins64_128.dll",
                "nvrtc64_120_0.alt.dll",
                "nvrtc64_120_0.dll"
            };

            foreach(string file in requiredFiles)
            {
                if(!System.IO.File.Exists(
                    System.IO.Path.Combine(cudaBinPath, file)))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool CheckCudnnFiles()
        {
            string cudaBinPath = @"C:\Program Files\NVIDIA\CUDNN\v9.8\bin\12.8";
            string[] requiredFiles =
            {
                "cudnn64_9.dll",
                "cudnn_adv64_9.dll",
                "cudnn_cnn64_9.dll",
                "cudnn_engines_precompiled64_9.dll",
                "cudnn_engines_runtime_compiled64_9.dll",
                "cudnn_graph64_9.dll",
                "cudnn_heuristic64_9.dll",
                "cudnn_ops64_9.dll"
            };

            foreach(string file in requiredFiles)
            {
                if(!System.IO.File.Exists(
                    System.IO.Path.Combine(cudaBinPath, file)))
                {
                    return false;
                }
            }

            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public sealed class CheckResult
        {
            public bool CudaInstalled;
            public Version CudaVersion;
            public bool CudaAtLeast12_8;

            public bool CuDnnInstalled;
            public Version CuDnnVersion;
            public bool CuDnnAtLeast9_8;

            public string CudaRoot;
            public string CuDnnRoot;
            public string Details;
        }
    }
}