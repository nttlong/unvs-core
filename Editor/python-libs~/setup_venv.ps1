# Script to create and setup Python Virtual Environment
# Location: Editor/python-libs/setup_venv.ps1

$VENV_NAME = "venv"
$REQUIREMENTS_FILE = "requirement.txt"

# Get the directory where the script is located
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
if ($ScriptDir -eq "") { $ScriptDir = Get-Location }

# Change directory to the script's location
Push-Location $ScriptDir

Write-Host "--- Setting up Python Virtual Environment in $ScriptDir ---" -ForegroundColor Blue

# Check if Python is installed
try {
    $pythonPath = (Get-Command python -ErrorAction Stop).Source
    $pythonVersion = & python --version 2>&1
    Write-Host "Using Python from: $pythonPath ($pythonVersion)" -ForegroundColor Gray
} catch {
    Write-Error "Python is not installed or not in PATH. Please install Python first."
    Pop-Location
    exit 1
}

# Path to python executable in venv (Windows)
$VenvPython = Join-Path $ScriptDir "$VENV_NAME\Scripts\python.exe"

# If venv folder exists but no python.exe, it might be corrupted/empty
if (Test-Path $VENV_NAME) {
    if (-not (Test-Path $VenvPython)) {
        Write-Host "Found existing but invalid/empty '$VENV_NAME' folder. Re-creating..." -ForegroundColor Yellow
        Remove-Item -Path $VENV_NAME -Recurse -Force -ErrorAction SilentlyContinue
    }
}

# Create virtual environment if it doesn't exist
if (-not (Test-Path $VENV_NAME)) {
    Write-Host "Creating virtual environment: $VENV_NAME..." -ForegroundColor Cyan
    python -m venv $VENV_NAME
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Failed to create virtual environment."
        Pop-Location
        exit 1
    }
} else {
    Write-Host "Virtual environment '$VENV_NAME' already exists." -ForegroundColor Yellow
}

# Verify venv python exists
if (-not (Test-Path $VenvPython)) {
    Write-Error "Could not find python.exe in virtual environment at $VenvPython"
    Pop-Location
    exit 1
}

# Upgrade pip
Write-Host "Upgrading pip..." -ForegroundColor Cyan
& $VenvPython -m pip install --upgrade pip

# Install requirements
$ReqPath = Join-Path $ScriptDir $REQUIREMENTS_FILE
if (Test-Path $ReqPath) {
    Write-Host "Installing requirements from $REQUIREMENTS_FILE..." -ForegroundColor Cyan
    & $VenvPython -m pip install -r $ReqPath
} else {
    Write-Warning "Requirement file '$REQUIREMENTS_FILE' not found at $ReqPath"
}

Write-Host "`nSetup completed successfully!" -ForegroundColor Green
Pop-Location
