from typing import List
from typing import Dict
import svgwrite
import os
def points_to_svg_path(points: List[Dict[str: float,str: float]]):
    if not points:
        return ""
    # Tạo chuỗi: M x1,y1 L x2,y2 L x3,y3 ...
    d = f"M {points[0]['x']},{points[0]['y']}"
    for p in points[1:]:
        d += f" L {p['x']},{p['y']}"
    return d
def create_inkscape_route(svg_path, points, layer_name, stroke_color="#00FF00"):
    # 1. Khởi tạo hoặc Load file SVG
    if os.path.exists(svg_path):
        tree = etree.parse(svg_path)
        root = tree.getroot()
    else:
        # Tạo mới nếu chưa có file
        root = etree.Element("svg", nsmap={
            None: "http://www.w3.org/2000/svg",
            "inkscape": "http://www.inkscape.org/namespaces/inkscape",
            "sodipodi": "http://sodipodi.sourceforge.net/DTD/sodipodi-0.dtd"
        })
        root.set("width", "20000") # Set kích thước lớn cho map của bạn
        root.set("height", "10000")
        tree = etree.ElementTree(root)

    # 2. Tạo Layer mới
    # Namespace Inkscape cần thiết để hiện trong bảng Layer
    ink_ns = "{http://www.inkscape.org/namespaces/inkscape}"
    new_layer = etree.SubElement(root, "{http://www.w3.org/2000/svg}g")
    new_layer.set(f"{ink_ns}groupmode", "layer")
    new_layer.set(f"{ink_ns}label", layer_name)
    new_layer.set("id", layer_name.replace(" ", "_"))

    # 3. Tạo đường Path từ 1200 điểm
    path_data = points_to_svg_path(points)
    
    path_element = etree.SubElement(new_layer, "{http://www.w3.org/2000/svg}path")
    path_element.set("d", path_data)
    path_element.set("style", f"fill:none;stroke:{stroke_color};stroke-width:5px;stroke-linejoin:round;stroke-linecap:round;")
    
    # 4. Lưu file
    tree.write(svg_path, encoding="utf-8", xml_declaration=True)
    print(f"Đã thêm layer '{layer_name}' vào {svg_path}")

# Ví dụ dữ liệu của bạn
my_points = [{"x": i*10, "y": (i%50)*5} for i in range(1200)]
create_inkscape_route("map_debug.svg", my_points, "Path_Layer_01", "#FF5733")