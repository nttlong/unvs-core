import os
import numpy as np
from PIL import Image, ImageDraw
from psd_tools import PSDImage
from psd_tools.api.layers import PixelLayer
from libs import create_psd_from_png

def CreatePsdFileOld(data: dict) -> str:
    psfFilePath = data['FilePath']
    pngFilePath = data['PngFile']
    
    if not os.path.exists(pngFilePath):
        return f"Error: Không tìm thấy file PNG tại {pngFilePath}"

    img = Image.open(pngFilePath).convert("RGBA")
    psd = PSDImage.new(mode='RGBA', size=img.size)
    
    # Xóa các layer mặc định
    while len(psd) > 0:
        try: psd.pop()
        except: break

    try:
        new_layer = PixelLayer.from_pil(img, psd)
    except AttributeError:
        new_layer = PixelLayer.frompil(img, psd)

    new_layer.name = "master"
    psd.append(new_layer)

    os.makedirs(os.path.dirname(os.path.abspath(psfFilePath)), exist_ok=True)
    psd.save(psfFilePath)

    return f"CreatePsdFile OK: {psfFilePath}"

def CreatePsdFile(data: dict) -> str:
    psfFilePath = data['FilePath']
    pngFilePath = data['PngFile']
    
    if not os.path.exists(pngFilePath):
        return f"Error: Không tìm thấy file PNG tại {pngFilePath}"
    return create_psd_from_png(pngFilePath, psfFilePath)

import numpy as np
import os
from PIL import Image, ImageDraw
from psd_tools import PSDImage
from psd_tools.api.layers import PixelLayer

def CreatePsdBigSize(data: dict) -> str:
    folder_path = data.get('folder_path')
    file_name = data.get('file_name', "geometry_chunks.psd")
    split_width = int(128) #int(data.get('split_width', 2048))
    screen_width = int(data.get('screen_width', 2048))
    screen_height = int(data.get('screen_height', 2048))
    points = data.get('points', [])

    if not points:
        return "Error: No points provided"

    # 1. Coordinate Normalization (Shift to 0,0)
    min_x = min(p['x'] for p in points)
    min_y = min(p['y'] for p in points)

    # 2. Create full image and draw lines
    full_img = Image.new('RGBA', (screen_width, screen_height), (0, 0, 0, 0))
    draw = ImageDraw.Draw(full_img)
    
    # Convert points and apply Y-flip
    pts = []
    for p in points:
        local_x = p['x'] - min_x
        local_y = p['y'] - min_y
        # Flip Y: screen_height - local_y
        pts.append((float(local_x), float(screen_height - local_y)))
    
    if len(pts) > 1:
        # Using Red color and 15px width for better visibility on large canvas
        draw.line(pts, fill=(255, 0, 0, 255), width=1, joint="round")

    # 3. Create chunks folder
    chunks_dir = os.path.join(folder_path, 'chunks')
    os.makedirs(chunks_dir, exist_ok=True)
    
    # Save the full image for reference
    full_img_path = os.path.join(folder_path, "full_image.png")
    full_img.save(full_img_path)

    # 4. Split into chunks and save as PNGs and individual PSDs
    chunks_to_assemble = []
    if screen_width > split_width:
        index = 0
        for x in range(0, screen_width, split_width):
            right = min(x + split_width, screen_width)
            chunk = full_img.crop((x, 0, right, screen_height))
            
            bbox = chunk.getbbox()
            if not bbox:
                continue
                
            # Crop only width, keep full height
            cropped_chunk = chunk.crop((bbox[0], 0, bbox[2], screen_height))
            
            # Pad to split_width to ensure consistent file width without distortion
            padded_chunk = Image.new('RGBA', (split_width, screen_height), (0, 0, 0, 0))
            # Paste at (0,0) to maintain the shared pivot alignment
            padded_chunk.paste(cropped_chunk, (0, 0))
            
            chunk_filename = f"chunk_{index}.png"
            chunk_path = os.path.join(chunks_dir, chunk_filename)
            padded_chunk.save(chunk_path)
            
            # Create individual PSD for this chunk
            create_psd_from_png(
                png_file_path=chunk_path,
                psd_file_path=os.path.join(folder_path, f"chunk_{index}.psd"),
                layer_name=f"chunk_{index}"
            )
            # Store with global offsets for master PSD reconstruction
            chunks_to_assemble.append({
                'img': padded_chunk, 
                'name': f"chunk_{index}",
                'left': 0,
                'top': 0 # We kept full height, so vertical offset relative to canvas is 0 if we consider the ink is at the top
            })
            index += 1
    else:
        bbox = full_img.getbbox()
        if bbox:
            # Crop only width, keep full height
            cropped_full = full_img.crop((bbox[0], 0, bbox[2], screen_height))
            
            # Pad to split_width
            padded_full = Image.new('RGBA', (split_width, screen_height), (0, 0, 0, 0))
            padded_full.paste(cropped_full, (0, 0))
            
            chunk_path = os.path.join(chunks_dir, "chunk_0.png")
            padded_full.save(chunk_path)
            
            # Create individual PSD for this chunk
            create_psd_from_png(
                png_file_path=chunk_path,
                psd_file_path=os.path.join(folder_path, "chunk_0.psd"),
                layer_name="chunk_0"
            )
            chunks_to_assemble.append({
                'img': padded_full, 
                'name': "chunk_0",
                'left': bbox[0],
                'top': 0
            })

    # 5. Create final master PSD with all chunks
    if chunks_to_assemble:
        # Use full screen dimensions for the master PSD to reconstruct the original image
        psd_master = PSDImage.new(mode='RGBA', size=(screen_width, screen_height))
        
        if hasattr(psd_master, 'layers'):
            psd_master.layers.clear()
        elif hasattr(psd_master, 'pop'):
            while len(psd_master) > 0:
                psd_master.pop()

        group = psd_master.create_group(name="out-line")
        
        for chunk in chunks_to_assemble:
            try:
                new_layer = PixelLayer.from_pil(chunk['img'], psd_master)
            except AttributeError:
                new_layer = PixelLayer.frompil(chunk['img'], psd_master)
            
            new_layer.name = chunk['name']
            new_layer.left = chunk['left']
            new_layer.top = chunk['top']
            group.append(new_layer)

        master_psd_path = os.path.join(folder_path, file_name)
        psd_master.save(master_psd_path)

    return f"Success: Created individual PSDs and master PSD in {folder_path}"
import os
from PIL import Image, ImageDraw
from psd_tools import PSDImage
from psd_tools.api.layers import PixelLayer

def CreatePsdBigSizeOneFile(data: dict) -> str:
    folder_path = data.get('folder_path')
    file_name = data.get('file_name', "geometry_chunks.psd")
    # Ép split_width cố định 128 hoặc theo data
    split_width = int(64) 
    screen_width = int(data.get('screen_width', 2048))
    screen_height = int(data.get('screen_height', 2048))
    points = data.get('points', [])

    if not points:
        return "Error: No points provided"

    # 1. Coordinate Normalization
    min_x = min(p['x'] for p in points)
    min_y = min(p['y'] for p in points)

    # 2. Create full image in memory
    full_img = Image.new('RGBA', (screen_width, screen_height), (0, 0, 0, 0))
    draw = ImageDraw.Draw(full_img)
    
    pts = []
    for p in points:
        local_x = p['x'] - min_x
        local_y = p['y'] - min_y
        pts.append((float(local_x), float(screen_height - local_y)))
    
    if len(pts) > 1:
        draw.line(pts, fill=(255, 0, 0, 255), width=1, joint="round")

    # 3. Initialize Master PSD với kích thước layer tiêu chuẩn
    # Lưu ý: Kích thước PSD master sẽ theo split_width để các layer không bị "lọt thỏm"
    psd_master = PSDImage.new(mode='RGBA', size=(split_width, screen_height))
    
    if hasattr(psd_master, 'layers'):
        psd_master.layers.clear()
    elif hasattr(psd_master, 'pop'):
        while len(psd_master) > 0: psd_master.pop()

    group = psd_master.create_group(name="out-line")

    # 4. Chia nhỏ và ép offset = 0
    index = 0
    for x in range(0, screen_width, split_width):
        right = min(x + split_width, screen_width)
        
        # Cắt một vùng từ ảnh tổng
        chunk_region = full_img.crop((x, 0, right, screen_height))
        
        # Kiểm tra nếu chunk rỗng (không có pixel) thì bỏ qua để nhẹ file
        if not chunk_region.getbbox():
            continue

        # Tạo một layer mới luôn có kích thước cố định split_width
        padded_layer_img = Image.new('RGBA', (split_width, screen_height), (0, 0, 0, 0))
        
        # Dán phần vừa cắt vào (luôn dán tại 0,0 của layer đó)
        padded_layer_img.paste(chunk_region, (0, 0))
        
        # Chuyển sang PixelLayer
        try:
            new_layer = PixelLayer.from_pil(padded_layer_img, psd_master)
        except AttributeError:
            new_layer = PixelLayer.frompil(padded_layer_img, psd_master)
        
        new_layer.name = f"chunk_{index}"
        # ÉP OFFSET LUÔN LÀ 0
        new_layer.left = 0 
        new_layer.top = 0
        
        group.append(new_layer)
        index += 1
        print(f"Added layer {index} at position ({new_layer.left}, {new_layer.top})")

    # 5. Save final PSD
    os.makedirs(folder_path, exist_ok=True)
    master_psd_path = os.path.join(folder_path, file_name)
    psd_master.save(master_psd_path)
    print(f"Saved PSD at {master_psd_path}")

    return f"Success: Created PSD with fixed-size layers (offset 0) at {master_psd_path}"

import os
import struct
from PIL import Image

def create_single_psd(data: dict) -> str:
    psd_file_path = data.get('psd_file')
    points = data.get('points', [])
    
    if not points or len(points) < 2:
        return "Error: Need at least 2 points to draw lines"

    # 1. Tìm giới hạn (Bounding Box) để tính size
    min_x = min(p['x'] for p in points)
    max_x = max(p['x'] for p in points)
    min_y = min(p['y'] for p in points)
    max_y = max(p['y'] for p in points)

    padding = data.get('padding', 50)
    canvas_width = int(max_x - min_x) + (padding * 2)
    canvas_height = int(max_y - min_y) + (padding * 2)

    # 2. Tạo ảnh bằng PIL và vẽ line
    img = Image.new('RGBA', (canvas_width, canvas_height), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)

    # Convert tọa độ (giữ nguyên tỷ lệ, chỉ dời gốc tọa độ và thêm lề)
    pts_to_draw = [
        (float(p['x'] - min_x + padding), float(p['y'] - min_y + padding)) 
        for p in points
    ]
    if len(pts_to_draw) > 1:
        pts_to_draw.append(pts_to_draw[0])
    # Vẽ đường line màu đỏ
    draw.line(pts_to_draw, fill=(255, 0, 0, 255), width=2, joint="round")

    # 3. Khởi tạo PSD và dọn dẹp layer (Dựa theo logic hàm CreatePsdBigSize của bạn)
    psd = PSDImage.new(mode='RGBA', size=(canvas_width, canvas_height))
    
    # Cách xóa layer an toàn nhất cho phiên bản của bạn
    if hasattr(psd, 'layers'):
        psd.layers.clear()
    elif hasattr(psd, 'pop'):
        while len(psd) > 0:
            psd.pop()

    # 4. Tạo layer từ PIL
    try:
        new_layer = PixelLayer.from_pil(img, psd)
    except AttributeError:
        new_layer = PixelLayer.frompil(img, psd)

    new_layer.name = "Lines_Layer"
    new_layer.left = 0
    new_layer.top = 0

    # 5. Thêm layer vào PSD
    # Kiểm tra xem psd có method append hay không (thường là psd.append hoặc psd.layers.append)
    if hasattr(psd, 'append'):
        psd.append(new_layer)
    else:
        psd.layers.append(new_layer)

    # 6. Lưu file
    output_dir = os.path.dirname(psd_file_path)
    if output_dir and not os.path.exists(output_dir):
        os.makedirs(output_dir, exist_ok=True)
        
    psd.save(psd_file_path)

    return f"Success: Created PSD at {psd_file_path}"

def create_dumny_actor_psd(data: dict) -> str:
    """
    Tạo PSD file cho UnvsDummyActor.
    
    Data nhận từ C# (UnvsDummyActor.EditorCreatePsdFile):
      - file_path  : str  – đường dẫn lưu file .psd
      - shapes     : list – danh sách sprite renderer, mỗi phần tử gồm:
            name   : str  – tên layer trong PSD
            index  : int  – thứ tự sắp xếp layer (nhỏ = dưới cùng)
            pivot  : {x, y} – vị trí tâm trong world-space
            points : [{x,y}, {x,y}, {x,y}, {x,y}] – 4 góc bbox (local-space)
                     [BL, BR, TR, TL] tương ứng (0,0),(w,0),(w,h),(0,h)

    Cấu trúc PSD tạo ra:
        PSD
        └── Group: "dummy"
                ├── Layer: <name_0>   (shape có index nhỏ nhất → dưới cùng)
                ├── Layer: <name_1>
                └── ...
    """
    print(data)

    file_path = data.get('file_path')
    shapes    = data.get('shapes', [])

    if not file_path:
        return "Error: 'file_path' is required"
    if not shapes:
        return "Error: 'shapes' is empty – nothing to draw"

    # ------------------------------------------------------------------ #
    # 1. Tính canvas chung để tất cả layer dùng chung hệ tọa độ
    #    Canvas bao phủ toàn bộ pivot + bbox của mọi shape.
    # ------------------------------------------------------------------ #
    padding = 50

    # Mỗi điểm thực = pivot + local_point
    all_world_xs = []
    all_world_ys = []
    for shape in shapes:
        px = shape['pivot']['x']
        py = shape['pivot']['y']
        for pt in shape['points']:
            all_world_xs.append(px + pt['x'])
            all_world_ys.append(py + pt['y'])

    min_wx = min(all_world_xs)
    max_wx = max(all_world_xs)
    min_wy = min(all_world_ys)
    max_wy = max(all_world_ys)

    canvas_w = int(max_wx - min_wx) + padding * 2
    canvas_h = int(max_wy - min_wy) + padding * 2

    # ------------------------------------------------------------------ #
    # 2. Tạo PSDImage với kích thước canvas chung
    # ------------------------------------------------------------------ #
    psd = PSDImage.new(mode='RGBA', size=(canvas_w, canvas_h))

    # Xoá mọi layer mặc định (tương tự create_single_psd)
    if hasattr(psd, 'layers'):
        psd.layers.clear()
    elif hasattr(psd, 'pop'):
        while len(psd) > 0:
            psd.pop()

    # ------------------------------------------------------------------ #
    # 3. Tạo Group tên "dummy" – toàn bộ layer nằm trong group này
    # ------------------------------------------------------------------ #
    group = psd.create_group(name="dummy")

    # ------------------------------------------------------------------ #
    # 4. Sắp xếp shapes theo index (tăng dần = layer dưới cùng trước)
    # ------------------------------------------------------------------ #
    sorted_shapes = sorted(shapes, key=lambda s: s['index'])

    for shape in sorted_shapes:
        layer_name = shape['name']
        px = shape['pivot']['x']
        py = shape['pivot']['y']

        # Tọa độ 4 góc trong world-space
        world_pts = [(px + pt['x'], py + pt['y']) for pt in shape['points']]

        # Bounding box của riêng shape này
        xs = [p[0] for p in world_pts]
        ys = [p[1] for p in world_pts]
        s_min_x, s_max_x = min(xs), max(xs)
        s_min_y, s_max_y = min(ys), max(ys)

        layer_w = max(int(s_max_x - s_min_x), 1)
        layer_h = max(int(s_max_y - s_min_y), 1)

        # Tạo ảnh RGBA riêng cho layer (kích thước bbox của shape)
        layer_img = Image.new('RGBA', (layer_w, layer_h), (0, 0, 0, 0))
        draw = ImageDraw.Draw(layer_img)

        # Chuyển world_pts → tọa độ cục bộ trong layer_img
        local_pts = [
            (float(p[0] - s_min_x), float(p[1] - s_min_y))
            for p in world_pts
        ]
        # Đóng polygon
        local_pts_closed = local_pts + [local_pts[0]]
        draw.line(local_pts_closed, fill=(255, 0,0, 200), width=2, joint="curve")

        # Đánh dấu pivot (chấm xanh lá) tương đối trong layer
        pivot_local_x = float(px - s_min_x)
        pivot_local_y = float(py - s_min_y)
        r = 4
        draw.ellipse(
            [pivot_local_x - r, pivot_local_y - r,
             pivot_local_x + r, pivot_local_y + r],
            fill=(0, 255, 0, 255)
        )

        # Tạo PixelLayer từ ảnh PIL (tương tự create_single_psd)
        try:
            new_layer = PixelLayer.from_pil(layer_img, psd)
        except AttributeError:
            new_layer = PixelLayer.frompil(layer_img, psd)

        new_layer.name  = layer_name
        # Đặt offset layer trong canvas (top-left corner của bbox shape)
        new_layer.left  = int(s_min_x - min_wx) + padding
        new_layer.top   = int(s_min_y - min_wy) + padding

        group.append(new_layer)

    # ------------------------------------------------------------------ #
    # 5. Lưu file PSD
    # ------------------------------------------------------------------ #
    output_dir = os.path.dirname(os.path.abspath(file_path))
    os.makedirs(output_dir, exist_ok=True)
    psd.save(file_path)

    return f"Success: create_dumny_actor_psd → {file_path} ({len(sorted_shapes)} layers in group 'dummy')"