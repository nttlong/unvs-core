$VENV_PATH = "..\venv"

if (Test-Path "$VENV_PATH\Scripts\Activate.ps1") {
    Write-Host "Activating virtual environment..." -ForegroundColor Cyan
    . "$VENV_PATH\Scripts\Activate.ps1"
    
    Write-Host "Starting FastAPI server..." -ForegroundColor Green
    # Sử dụng python main.py vì bạn đã có block if __name__ == "__main__"
    python main.py
}
else {
    Write-Host "Error: Virtual environment not found at $VENV_PATH" -ForegroundColor Red
    Write-Host "Please ensure you have run setup_venv.ps1 first." -ForegroundColor Yellow
    Pause
}
