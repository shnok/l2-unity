import bpy
import os
#from io_import_scene_unreal_psa_psk_280 import *
import io_import_scene_unreal_psa_psk_280

from bpy.props import StringProperty, BoolProperty
from bpy_extras.io_utils import ImportHelper
from bpy.types import Operator

class OT_TestOpenFilebrowser (Operator, ImportHelper):

    bl_idname = "test.open_filebrowser" 
    bl_label = "Select Directory"
    
    filter_glob: StringProperty( 
        default='',
        options={'HIDDEN'}
    )
    
    boolModelDel: BoolProperty(
        name='Delete processed model files', 
        description='',
        default=False,
    )

    def execute(self, context):
        """Do something with the selected file(s)."""
        
        folder_path = self.filepath
        
        # Get a list of all files in the folder hierarchy 
        files = []
        for root, dirs, filenames in os.walk(folder_path): 
            for filename in filenames:
                files.append(os.path.join(root, filename))
                
        # Process the directory
        processDirectory(folder_path, self.boolModelDel)
        
        return {'FINISHED'}

def register():
    bpy.utils.register_class(OT_TestOpenFilebrowser)
    

def unregister():
    bpy.utils.unregister_class(OT_TestOpenFilebrowser)
    
def processDirectory(folder_path, bootModelDel):
    model_files = []
    for root, dirs, files in os.walk(folder_path):
        for file in files:
            if file.endswith(".psk") or file.endswith(".gltf"):
                model_files.append(os.path.join(root, file))
    
    # loop through files and perform import and export
    for model_file in model_files:
        bpy.ops.wm.read_factory_settings(use_empty=True)
        
        if model_file.endswith(".psk"):
            io_import_scene_unreal_psa_psk_280.pskimport(
                model_file,
                context = None,
                bImportmesh = True,
                bImportbone = True,
                bSpltiUVdata = False,
                fBonesize = 5.0,
                fBonesizeRatio = 0.4,
                bDontInvertRoot = True,
                bReorientBones = False,
                bReorientDirectly = False,
                bScaleDown = True,
                bToSRGB = True,
                error_callback = None
            )
          #  io_import_scene_unreal_psa_psk_280.psaimport(
          #      model_file.replace("_m00.psk", "_anim.psa"),
          #      oArmature = io_import_scene_unreal_psa_psk_280.blen_get_armature_from_selection()
          #  )
        
        #output_path = os.path.splitext(model_file)[0] + ".fbx"
        
        #bpy.ops.export_scene.fbx(
        #    filepath=output_path,
        #    use_selection=False,
        #    bake_space_transform=True,
            #axis_forward='Y',
            #axis_up='-Z'
       # )
        
                
if __name__ == "__main__":
    register()
    
    console_visible = bpy.context.area.type == 'CONSOLE'

    if not console_visible:
        # Open the console window
        bpy.ops.wm.console_toggle('INVOKE_DEFAULT')

    bpy.ops.test.open_filebrowser('INVOKE_DEFAULT')