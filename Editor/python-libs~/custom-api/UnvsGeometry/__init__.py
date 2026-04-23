import numpy as np
from PIL import Image, ImageDraw
import os
from libs import create_psd_from_png

def CreateGuidelinePng(data: dict):
    """
    Tạo ảnh PNG guideline với tính năng Smart Crop để tránh tạo ảnh quá lớn.
    """
    ppu = data.get('ppu', 4) 
    raw_points = data['points'] #<-- luu y day la danh sach cac diem duoc 
    #chuyen tu unity polycollider2d khi dan o che do edit 1 scene duoi dang 1 prefab, do bin tap vien chay tu unity
    output_path = data['output_path']
    output_path_psd = data.get('output_path_psd')
    padding = data.get('padding', 50)# <-- kho co can phai canh le

    if not raw_points:
        return "Error: No points provided"

    # 1. Chuyển đổi tọa độ sang pixel (Numpy)
    pts_arr = np.array([[p['x'], p['y']] for p in raw_points])
    pts_arr = pts_arr * ppu

    # 2. Smart Crop: Tính toán Bounding Box
    min_x, min_y = np.min(pts_arr, axis=0)
    max_x, max_y = np.max(pts_arr, axis=0)

    width = int(max_x - min_x) + padding * 2
    height = int(max_y - min_y) + padding * 2

    # # Giới hạn an toàn (16k pixels)
    # MAX_ALLOWED = 16384
    # if width > MAX_ALLOWED or height > MAX_ALLOWED:
    #     return f"Error: Image too large ({width}x{height}). Max is {MAX_ALLOWED}px. Check PPU or world bounds."

    # 3. Offset tọa độ để khớp với khung ảnh đã crop
    pts_arr[:, 0] -= (min_x - padding)
    pts_arr[:, 1] -= (min_y - padding)
    pts_arr[:, 1] = height - pts_arr[:, 1] # Lật trục Y
    pts = [tuple(p) for p in pts_arr]

    # 4. Vẽ ảnh
    img = Image.new('RGBA', (width, height), (255, 255, 255, 0))
    draw = ImageDraw.Draw(img)
    if len(pts) > 1:
        draw.line(pts, fill=(0, 255, 0, 255), width=3)

    # 5. Lưu file PNG
    os.makedirs(os.path.dirname(os.path.abspath(output_path)), exist_ok=True)
    img.save(output_path, compress_level=1)
    
    # 6. Xuất thêm file PSD nếu có yêu cầu
    psd_result = ""
    if output_path_psd:
        psd_result = f" | PSD: {create_psd_from_png(output_path, output_path_psd)}"
    
    # Metadata để Unity căn chỉnh lại vị trí
    world_offset_x = (min_x - padding) / ppu
    world_offset_y = (min_y - padding) / ppu
    
    return f"Smart Export OK: {width}x{height} pixels | World Offset: ({world_offset_x:.2f}, {world_offset_y:.2f}){psd_result}"

def smooth_polygon(points, iterations=3):
    """
    Thuật toán Chaikin mượt hóa đường gấp khúc (Vectorized with Numpy).
    """
    if not points or len(points) < 2: 
        return [tuple([p['x'], p['y']]) for p in points] if points else []
        
    pts = np.array([[p['x'], p['y']] for p in points])
    
    for _ in range(iterations):
        L = pts[:-1]
        R = pts[1:]
        new_pts = np.empty((2 * len(L), 2))
        new_pts[0::2] = 0.75 * L + 0.25 * R
        new_pts[1::2] = 0.25 * L + 0.75 * R
        pts = new_pts

    return [tuple(p) for p in pts]

def CreateGuidelinePngCurve(data: dict):
    """
    Tạo ảnh PNG guideline với đường cong mượt và Smart Crop.
    """
    ppu = data.get('ppu', 100)
    points = data['points']
    output_path = data['output_path']
    padding = data.get('padding', 50)
    
    if not points:
        return "Error: No points"

    # 1. Làm mượt điểm (tọa độ world)
    smooth_pts_raw = smooth_polygon(points)
    
    # 2. Chuyển sang tọa độ pixel
    pts_arr = np.array(smooth_pts_raw)
    pts_arr = pts_arr * ppu

    # 3. Smart Crop
    min_x, min_y = np.min(pts_arr, axis=0)
    max_x, max_y = np.max(pts_arr, axis=0)
    width = int(max_x - min_x) + padding * 2
    height = int(max_y - min_y) + padding * 2

    MAX_ALLOWED = 16384
    if width > MAX_ALLOWED or height > MAX_ALLOWED:
        return f"Error: Image too large ({width}x{height}). Max is {MAX_ALLOWED}px."

    pts_arr[:, 0] -= (min_x - padding)
    pts_arr[:, 1] -= (min_y - padding)
    pts_arr[:, 1] = height - pts_arr[:, 1]
    pts = [tuple(p) for p in pts_arr]

    # 4. Vẽ
    img = Image.new('RGBA', (width, height), (255, 255, 255, 0))
    draw = ImageDraw.Draw(img)
    if len(pts) > 1:
        draw.line(pts, fill=(255, 0, 0, 255), width=max(1, int(ppu/15)))
    
    os.makedirs(os.path.dirname(os.path.abspath(output_path)), exist_ok=True)
    img.save(output_path, compress_level=1)
    
    world_offset_x = (min_x - padding) / ppu
    world_offset_y = (min_y - padding) / ppu

    return f"Curve Smart Export OK: {width}x{height} | World Offset: ({world_offset_x:.2f}, {world_offset_y:.2f})"

def CreateGuidelinePsd(data: dict):
    """
    Tạo file PSD chứa layer guideline (Cũng sử dụng Smart Crop).
    """
    ppu = data.get('ppu', 100)
    raw_points = data['points']
    output_path = data['output_path']
    padding = data.get('padding', 50)

    if not raw_points:
        return "Error: No points"

    # 1. Làm mượt và chuyển pixel
    pts_world = smooth_polygon(raw_points)
    pts_arr = np.array(pts_world)
    pts_arr = pts_arr * ppu

    # 2. Smart Crop
    min_x, min_y = np.min(pts_arr, axis=0)
    max_x, max_y = np.max(pts_arr, axis=0)
    width = int(max_x - min_x) + padding * 2
    height = int(max_y - min_y) + padding * 2

    MAX_ALLOWED = 16384
    if width > MAX_ALLOWED or height > MAX_ALLOWED:
        return f"Error: PSD Layer too large ({width}x{height})."

    pts_arr[:, 0] -= (min_x - padding)
    pts_arr[:, 1] -= (min_y - padding)
    pts_arr[:, 1] = height - pts_arr[:, 1]
    smooth_tuples = [tuple(p) for p in pts_arr]

    # 3. Vẽ ra Image
    layer_img = Image.new('RGBA', (width, height), (255, 255, 255, 0))
    draw = ImageDraw.Draw(layer_img)
    if len(smooth_tuples) > 1:
        draw.line(smooth_tuples, fill=(0, 255, 255, 255), width=max(1, int(ppu/20)))

    # 4. Lưu thành PSD
    os.makedirs(os.path.dirname(os.path.abspath(output_path)), exist_ok=True)
    layer_img.save(output_path, format="PSD")
    
    world_offset_x = (min_x - padding) / ppu
    world_offset_y = (min_y - padding) / ppu

    return f"PSD Smart Export OK: {width}x{height} | Offset: ({world_offset_x:.2f}, {world_offset_y:.2f})"