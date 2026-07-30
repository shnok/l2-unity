import bpy
import os
#from io_import_scene_unreal_psa_psk_280 import *
import io_import_scene_unreal_psa_psk_280

from bpy.props import StringProperty, BoolProperty
from bpy_extras.io_utils import ImportHelper
from bpy.types import Operator

# Set the file paths
input_folder = "G:/Stock/Projects/L2-Unity/Tools/umodel_win32/export/field_deco_S"
output_folder = "G:/Stock/Projects/L2-Unity/Tools/umodel_win32/export/field_deco_S"

# Ensure the output folder exists
os.makedirs(output_folder, exist_ok=True)

model_files = []
for root, dirs, files in os.walk(input_folder):
    for file in files:
        if file.endswith(".pskx") or file.endswith(".gltf"):
            model_files.append(os.path.join(root, file))
            
for model_file in model_files:
    bpy.ops.wm.read_factory_settings(use_empty=True)
    
    if model_file.endswith(".pskx"):
        io_import_scene_unreal_psa_psk_280.pskimport(
            model_file,
            context = None,
            bImportmesh = True,
            bImportbone = True,
            bSpltiUVdata = False,
            fBonesize = 5.0,
            fBonesizeRatio = 0.6,
            bDontInvertRoot = True,
            bReorientBones = False,
            bReorientDirectly = False,
            bScaleDown = True,
            bToSRGB = True,
            error_callback = None
        )
        
    output_path = os.path.splitext(model_file)[0] + ".fbx"
    
    bpy.ops.export_scene.fbx(
        filepath=output_path,
        use_selection=False,
        bake_space_transform=True,
        #axis_forward='Y',
        #axis_up='-Z'
    )
    
# Set the forward and up axes
#bpy.ops.transform.create_orientation(name="Y-Up", use=true, use_math=true)
#bpy.context.object.rotation_euler = (0, 0, 0)
#bpy.ops.transform.transform(mode='AXIS_ANGLE', orient_matrix=((1, 0, 0), (0, 0, -1), (0, 1, 0)))

# Export as FBX
#for obj in bpy.context.scene.objects:
#    if obj.type == "MESH":
#        fbx_path = os.path.join(output_folder, obj.name + ".fbx")
#        bpy.ops.export_scene.fbx(
#            filepath=fbx_path,
#            use_selection=True,
#            axis_forward='Y',
#            axis_up='Z',
#            add_leaf_bones=False,
#            apply_unit_scale=False,
#            global_scale=1.0,
#        )