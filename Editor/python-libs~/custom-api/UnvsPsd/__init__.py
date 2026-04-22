import os
from PIL import Image
from psd_tools import PSDImage
from psd_tools.api.layers import PixelLayer
from libs import create_psd_from_png

def CreatePsdFileOld(data: dict) -> str:
    psfFilePath = data['FilePath']
    pngFilePath = data['PngFile']
    
    if not os.path.exists(pngFilePath):
        return f"Error: Không tìm thấy file PNG tại {pngFilePath}"

    # 1. Mở ảnh PNG
    img = Image.open(pngFilePath).convert("RGBA")
    
    # 2. Tạo PSD mới
    psd = PSDImage.new(mode='RGBA', size=img.size)
    
    # 3. Xóa layer mặc định mà .new() tạo ra để tránh rác
    psd.layers.clear() if hasattr(psd, 'layers') else None

    # 4. Tạo layer mới - Truyền 'psd' vào làm parent
    # Thử 'from_pil' hoặc 'frompil' tùy bản, nhưng phải có tham số thứ 2 là psd
    try:
        new_layer = PixelLayer.from_pil(img, psd)
    except AttributeError:
        new_layer = PixelLayer.frompil(img, psd)

    new_layer.name = "master"
    
    # 5. Đưa layer vào file (nếu dùng from_pil nó thường tự add, nhưng append cho chắc)
    if new_layer not in psd:
        try:
            psd.layers.append(new_layer)
        except AttributeError:
            psd.append(new_layer)

    # 6. Ghi file
    os.makedirs(os.path.dirname(os.path.abspath(psfFilePath)), exist_ok=True)
    psd.save(psfFilePath)

    return f"CreatePsdFile OK: {psfFilePath}"
def CreatePsdFile(data: dict) -> str:
    psfFilePath = data['FilePath']
    pngFilePath = data['PngFile']
    
    if not os.path.exists(pngFilePath):
        return f"Error: Không tìm thấy file PNG tại {pngFilePath}"
    return create_psd_from_png(pngFilePath, psfFilePath)