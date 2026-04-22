import os
from datetime import datetime
from PIL import Image
from psd_tools import PSDImage
from psd_tools.api.layers import PixelLayer

def create_psd_from_png(png_file_path: str, psd_file_path: str, layer_name: str = "master") -> str:
    """
    Tạo hoặc cập nhật file PSD từ một file PNG cho trước.
    Nếu file PSD đã tồn tại, thêm layer mới với nội dung từ PNG.
    Tên layer được đặt theo quy tắc: {tên_file_png}-{yyyy-MM-dd}.
    
    Args:
        png_file_path (str): Đường dẫn đến file PNG nguồn.
        psd_file_path (str): Đường dẫn đến file PSD đích.
        layer_name (str): Tên layer dự phòng (không sử dụng nếu quy tắc đặt tên tự động được áp dụng).
        
    Returns:
        str: Thông báo kết quả hoặc lỗi.
    """
    if not os.path.exists(png_file_path):
        return f"Error: Không tìm thấy file PNG tại {png_file_path}"

    try:
        # 1. Mở ảnh PNG
        img = Image.open(png_file_path).convert("RGBA")
        
        # 2. Xác định tên layer: {png_filename}-{yyyy-MM-dd}
        png_basename = os.path.splitext(os.path.basename(png_file_path))[0]
        date_str = datetime.now().strftime("%Y-%m-%d")
        final_layer_name = f"{png_basename}-{date_str}"
        
        # 3. Mở PSD cũ hoặc tạo mới
        is_new_psd = not os.path.exists(psd_file_path)
        if is_new_psd:
            psd = PSDImage.new(mode='RGBA', size=img.size)
            # Xóa layer mặc định mà .new() tạo ra để tránh rác
            if hasattr(psd, 'layers'):
                psd.layers.clear()
        else:
            psd = PSDImage.open(psd_file_path)

        # 4. Tạo layer mới
        try:
            new_layer = PixelLayer.from_pil(img, psd)
        except AttributeError:
            new_layer = PixelLayer.frompil(img, psd)

        new_layer.name = final_layer_name
        
        # 5. Đưa layer vào file
        if hasattr(psd, 'layers'):
            psd.layers.append(new_layer)
        elif hasattr(psd, 'append'):
            psd.append(new_layer)
        else:
            # Fallback if both fail (unlikely with psd-tools)
            psd.layers.append(new_layer)

        # 6. Ghi file
        if is_new_psd:
            os.makedirs(os.path.dirname(os.path.abspath(psd_file_path)), exist_ok=True)
        
        psd.save(psd_file_path)

        return f"OK: {psd_file_path}"
        
    except Exception as e:
        return f"Error: {str(e)}"
