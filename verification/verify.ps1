Param(
    [string]$installPath,
    [string]$arguments
)

$erroractionpreference = "stop"

"Install path is:      $installPath" 
"Arguments are:        $arguments"
""

if (($arguments -ne $null) -and ($arguments -ne ""))
{
    $process = (Start-Process -FilePath $installPath -ArgumentList $arguments -PassThru -Wait)
}
else
{
    $process = (Start-Process -FilePath $installPath -PassThru -Wait)
}

if ($process.ExitCode -ne 0)
{
    throw ("Process exited with an exit code of: " + $process.ExitCode)
}