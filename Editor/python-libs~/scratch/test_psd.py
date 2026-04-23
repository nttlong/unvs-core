import sys
import os

# Add relevant paths to sys.path
base_path = r"e:\unitty-data\unvs-packages\com.unvs.core\Editor\python-libs~"
sys.path.append(os.path.join(base_path, "custom-api"))
sys.path.append(base_path)

from UnvsPsd import CreatePsdBigSise

# Setup test data
scratch_dir = os.path.join(base_path, "scratch")
os.makedirs(scratch_dir, exist_ok=True)

data = {
    'folder_path': scratch_dir,
    'file_name': "test_geometry.psd",
    'split_width': 100,
    'screen_width': 300,
    'screen_hight': 200,
    'points': [
        {'x': 10, 'y': 10},
        {'x': 50, 'y': 50},
        {'x': 150, 'y': 150},
        {'x': 250, 'y': 10}
    ]
}

try:
    result = CreatePsdBigSise(data)
    print(result)
    
    # Check if files exist
    psd_path = os.path.join(scratch_dir, "chunks", "test_geometry.psd")
    if os.path.exists(psd_path):
        print(f"PSD file created at: {psd_path}")
        
        # List chunks
        chunks_dir = os.path.join(scratch_dir, "chunks")
        chunks = [f for f in os.listdir(chunks_dir) if f.endswith(".png")]
        print(f"Found {len(chunks)} chunks: {chunks}")
    else:
        print("PSD file NOT found!")

except Exception as e:
    print(f"Error during verification: {e}")
    import traceback
    traceback.print_exc()
