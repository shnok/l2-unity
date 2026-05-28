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
        default=True,
    )
    
    boolTxtDel: BoolProperty(
        name='Delete *.txt files',
        description='',
        default=False,
    )
    
    boolMatDel: BoolProperty(
        name='Delete.mat files',
        description='',
        default=True,
    )
    
    boolTgaDel: BoolProperty(
        name='Delete .tga files',
        description='',
        default=True,
    )
    
    boolPngDel: BoolProperty(
        name='Delete *.png files', 
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
        
        # Loop through the files and delete based on the boolean flags for file path in files:
        for file_path in files:
           
            if file_path.endswith(".png") and "Texture" in file_path:
                print(file_path)
                new_file_path = file_path.replace("Texture/", "").replace("Texture\\", "")
                print(new_file_path)
                # Rename the file
                os.replace(file_path, new_file_path)
            if file_path.endswith(".props.txt") and "Shader" in file_path:
                print(file_path)
                new_file_path = file_path.replace("Shader", "Materials")
                directory, filename = os.path.split(new_file_path)
                if not os.path.exists(directory):
                    # Create the folder if it doesn't exist
                    os.mkdir(directory)
                    
                print(new_file_path)
                # Rename the file
                os.replace(file_path, new_file_path)
                
                file_path = new_file_path
                
            _, extension = os.path.splitext (file_path)
            
            if (extension.lower() == '.txt' and self.boolTxtDel and "Materials" not in file_path) or \
               (extension.lower() == '.mat' and self.boolMatDel) or \
               (extension.lower() == '.tga' and self.boolTgaDel) or \
               (extension.lower() == '.png' and self.boolPngDel):
               os.remove(file_path)
        
        delete_empty_folders(folder_path)
        
        return {'FINISHED'}

def register():
    bpy.utils.register_class(OT_TestOpenFilebrowser)
    

def unregister():
    bpy.utils.unregister_class(OT_TestOpenFilebrowser)
    
def processDirectory(folder_path, bootModelDel):
    model_files = []
    for root, dirs, files in os.walk(folder_path):
        for file in files:
            if file.endswith(".pskx") or file.endswith(".gltf"):
                model_files.append(os.path.join(root, file))
    
    # loop through files and perform import and export
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
        elif model_file.endswith(".gltf"):
            bpy.ops.import_scene_gltf(filepath=model_file)
        
        output_path = os.path.splitext(model_file)[0] + ".fbx"
        output_path = output_path.replace("StaticMesh\\", "")
        
        if os.path.exists(output_path):
            continue
        
        bpy.ops.export_scene.fbx(
            filepath=output_path,
            use_selection=False,
            bake_space_transform=True,
            #axis_forward='Y',
            #axis_up='-Z'
        )
        
        
        
        # Delete source model file if bootModelDel is True
        if bootModelDel:
            splitPath = os.path.splitext(model_file)
            
            if splitPath[1] == ".gltf":
                os.remove(splitPath[0] + ".bin")
                
            os.remove(model_file)

def delete_empty_folders(top_directory):
    for root, dirs, files in os.walk(top_directory, topdown=False):
        for directory in dirs:
            folder_path = os.path.join(root, directory)
            if not os.listdir(folder_path):  # Check if the folder is empty
                os.rmdir(folder_path)  # Delete the empty folder
                print(f"Deleted empty folder: {folder_path}")
                
if __name__ == "__main__":
    register()
    
    console_visible = bpy.context.area.type == 'CONSOLE'

    if not console_visible:
        # Open the console window
        bpy.ops.wm.console_toggle('INVOKE_DEFAULT')

    bpy.ops.test.open_filebrowser('INVOKE_DEFAULT')