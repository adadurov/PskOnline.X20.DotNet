$VersionsProps = [xml](Get-Content "Versions.props")
$VersionMajor = $VersionsProps.Project.PropertyGroup.VersionMajor
$VersionMinor = $VersionsProps.Project.PropertyGroup.VersionMinor
$VersionPatch = $VersionsProps.Project.PropertyGroup.VersionPatch

Get-ChildItem -recurse | 
    where {$_.extension -eq ".nuspec"} |
    Foreach-Object {
        $PathToNuSpec = $_.DirectoryName + "\" + $_.BaseName + ".nuspec"
        $nuspec = [xml](Get-Content $PathToNuSpec)

        $nuspec.package.metadata.version = $VersionMajor + '.' + $VersionMinor + '.' + $VersionPatch 

        $nuspec.Save($PathToNuSpec)
    }
