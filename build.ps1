Param (
	[Parameter(Mandatory=$false)]
	[ValidateSet("Debug", "Release")]
	[string]$Configuration = 'Release'
)

$os = [System.Runtime.InteropServices.RuntimeInformation]::IsOSPlatform([System.Runtime.InteropServices.OSPlatform]::Windows) ? "win" :
      [System.Runtime.InteropServices.RuntimeInformation]::IsOSPlatform([System.Runtime.InteropServices.OSPlatform]::Linux) ? "linux" :
      [System.Runtime.InteropServices.RuntimeInformation]::IsOSPlatform([System.Runtime.InteropServices.OSPlatform]::OSX) ? "osx" : "unknown"

$arch = [System.Runtime.InteropServices.RuntimeInformation]::OSArchitecture.ToString().ToLower()

Push-Location -Path './libbulletc'

mkdir -Force 'build'

Push-Location -Path './build'

$bullet3_path = Resolve-Path -Path "../../bullet3/src/"

cmake -DCMAKE_BUILD_TYPE="$Configuration" -DBULLET_INCLUDE_DIR="$bullet3_path" ..

cmake --build . --config $Configuration

Pop-Location
Pop-Location

Push-Location -Path './BulletSharp'

dotnet build -c $Configuration BulletSharp.csproj

Pop-Location

$fileExt = ""

if ($os -eq "win") {
	$fileExt = ".dll"
} elseif ($os -eq "linux") {
	$fileExt = ".so"
} elseif ($os -eq "osx") {
	$fileExt = ".dylib"
}

$srcFileName = ""
if ($Configuration -eq 'Debug')
{
	$srcFileName = "libbulletc_Debug$fileExt"
}
else
{
	$srcFileName = "libbulletc$fileExt"
}

New-Item -Path "./BulletSharp/runtimes/$os-$arch" -ItemType Directory -Force

Copy-Item "./libbulletc/build/lib/$Configuration/$srcFileName" "./BulletSharp/runtimes/$os-$arch/libbulletc.dll"