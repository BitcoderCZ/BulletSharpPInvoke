Param (
	[Parameter(Mandatory=$false)]
	[ValidateSet("Debug", "Release")]
	[string]$Configuration = 'Release'
)

Push-Location -Path './libbulletc'

md -Force 'build'

Push-Location -Path './build'

cmake -DCMAKE_BUILD_TYPE=$Configuration ..

cmake --build . --config $Configuration

Pop-Location
Pop-Location

Push-Location -Path './BulletSharp'

dotnet build -c $Configuration BulletSharp.csproj

Pop-Location

$targets = 'netstandard2.1', 'net8.0', 'net9.0'
foreach ($target in $targets) {
	if ($Configuration -eq 'Debug')
	{
		Copy-Item "./libbulletc/build/lib/$Configuration/libbulletc_Debug.dll" "./BulletSharp/bin/$Configuration/$target/libbulletc.dll"
	}
	else
	{
		Copy-Item "./libbulletc/build/lib/$Configuration/libbulletc.dll" -Destination "./BulletSharp/bin/$Configuration/$target"
	}
}