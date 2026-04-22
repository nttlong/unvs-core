import sys
import os
import importlib
import json
import traceback
from typing import Any
from fastapi import FastAPI, HTTPException
from fastapi.responses import RedirectResponse
from pydantic import BaseModel

# Xác định đường dẫn tới các thư mục
current_dir = os.path.dirname(os.path.abspath(__file__))
root_path = os.path.abspath(os.path.join(current_dir, ".."))
custom_api_path = os.path.join(root_path, "custom-api")
libs_path = os.path.join(root_path, "libs")

# Thêm vào sys.path nếu chưa có
if custom_api_path not in sys.path:
    sys.path.append(custom_api_path)
if root_path not in sys.path:
    sys.path.append(root_path)

app = FastAPI(
    title="UNVS Core API Server",
    description="API Server with dynamic module calling",
    version="1.0.1"
)

# Debug: In thông tin đường dẫn khi khởi động
print("\n" + "="*50)
print(f"API Server Path Debug:")
print(f" - Executing File: {__file__}")
print(f" - Current Dir: {current_dir}")
print(f" - Custom API Path: {custom_api_path}")
print(f" - Custom API exists: {os.path.exists(custom_api_path)}")
print(f" - Root Path: {root_path}")
print(f" - Root exists: {os.path.exists(root_path)}")
print(f" - Sys Path updated: {custom_api_path in sys.path and root_path in sys.path}")
print("="*50 + "\n")

# Cấu trúc dữ liệu yêu cầu
class CallRequest(BaseModel):
    module: str
    func: str
    json_data: Any = {}

@app.get("/", include_in_schema=False)
async def root():
    """Tự động chuyển hướng về trang Swagger Docs"""
    return RedirectResponse(url="/docs")

@app.get("/healthcheck", tags=["System"])
async def healthcheck():
    """
    Kiểm tra trạng thái hoạt động của server.
    """
    return "OK"

@app.post("/call", tags=["Custom API"])
def call(request: CallRequest):
    """
    Gọi một hàm từ module trong thư mục custom-api.
    - **module**: Tên package (ví dụ: UnvsTest)
    - **func**: Tên hàm (ví dụ: FirstTest)
    - **json_data**: Dữ liệu JSON (Body)
    """
    module = request.module
    func = request.func
    data = request.json_data
    
    try:
        # Dynamic import module
        mod = importlib.import_module(module)
        # Reload module để nhận code mới nhất mà không cần restart server
        importlib.reload(mod)
        
        # Lấy function từ module
        if not hasattr(mod, func):
            raise HTTPException(status_code=404, detail=f"Hàm '{func}' không có trong module '{module}'")
            
        f = getattr(mod, func)
        
        # Thực thi hàm (data đã là dict nhờ Pydantic)
        result = f(data)
        
        return {
            "status": "success",
            "module": module,
            "func": func,
            "result": result
        }
        
    except ImportError as e:
        # Kiểm tra xem có phải chính module đó thiếu hay là thư viện bên trong nó thiếu
        error_trace = traceback.format_exc()
        # e.name chỉ có từ Python 3.6+
        if hasattr(e, 'name') and e.name == module:
            raise HTTPException(status_code=404, detail=f"Module '{module}' không tìm thấy tại {custom_api_path}")
        else:
            # Đây là lỗi do module đó import các thư viện khác (như pytoshop) bị thất bại
            print("\n" + "!" * 30 + " IMPORT ERROR " + "!" * 30)
            print(f"Module '{module}' found but failed to load its dependencies:")
            print(error_trace)
            print("!" * 74 + "\n")
            raise HTTPException(status_code=500, detail=f"Module '{module}' bị lỗi thư viện bên trong:\n{error_trace}")
    except HTTPException as he:
        raise he
    except Exception as e:
        # Lấy toàn bộ stack trace để gửi về Unity
        error_trace = traceback.format_exc()
        
        # In chi tiết lỗi ra console luôn để dễ debug
        print("\n" + "!" * 30 + " ERROR " + "!" * 30)
        print(f"Module: {module} | Function: {func}")
        print(error_trace)
        print("!" * 67 + "\n")
        raise
        #raise HTTPException(status_code=500, detail=f"Lỗi thực thi Python:\n{error_trace}")


if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="127.0.0.1", port=8000)
