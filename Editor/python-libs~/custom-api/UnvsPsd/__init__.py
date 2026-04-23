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

def CreatePsdBigSise(data: dict) -> str:
    folder_path = data.get('folder_path')
    file_name = data.get('file_name', "geometry_chunks.psd")
    split_width = int(data.get('split_width', 2048))
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


        

    print(f"Success: Created individual PSDs in {folder_path}")
    return f"Success: Created individual PSDs in {folder_path}"