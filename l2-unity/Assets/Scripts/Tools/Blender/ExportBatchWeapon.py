# exports each selected object into its own file

import bpy
import os

# export to blend file location
basedir = os.path.dirname(bpy.data.filepath)

if not basedir:
    raise Exception("Blend file is not saved")

view_layer = bpy.context.view_layer

obj_active = view_layer.objects.active
selection = bpy.context.selected_objects

bpy.ops.object.select_all(action='DESELECT')

for obj in selection:

    obj.select_set(True)

    # some exporters only use the active object
    view_layer.objects.active = obj

    name = bpy.path.clean_name(obj.name)
    fn = os.path.join(basedir, name)

    bpy.ops.export_scene.fbx(
        filepath=fn.replace("_mo", "") + ".fbx", 
        use_selection=True,
        use_visible=False,
        use_active_collection=False,
        global_scale=1.0, 
        apply_unit_scale=True,
        apply_scale_options='FBX_SCALE_UNITS', 
        use_space_transform=True, 
        bake_space_transform=True, 
        object_types={'MESH'}, 
        use_mesh_modifiers=True, 
        use_mesh_modifiers_render=True, 
        mesh_smooth_type='OFF', 
        colors_type='SRGB', 
        prioritize_active_color=False, 
        use_subsurf=False, 
        use_mesh_edges=False, 
        use_tspace=False, 
        use_triangles=False, 
        use_custom_props=False, 
        add_leaf_bones=True, 
        primary_bone_axis='Y',
        secondary_bone_axis='X', 
        use_armature_deform_only=False, 
        armature_nodetype='NULL', 
        bake_anim=True, 
        bake_anim_use_all_bones=True,
        bake_anim_use_nla_strips=True, 
        bake_anim_use_all_actions=True, 
        bake_anim_force_startend_keying=True, 
        bake_anim_step=1.0, 
        bake_anim_simplify_factor=1.0, 
        path_mode='AUTO', 
        embed_textures=False, 
        batch_mode='OFF', 
        use_batch_own_dir=True, 
        use_metadata=True, 
        axis_forward='Y', 
        axis_up='Z')

    # Can be used for multiple formats
    # bpy.ops.export_scene.x3d(filepath=fn + ".x3d", use_selection=True)

    obj.select_set(False)

    print("written:", fn)


view_layer.objects.active = obj_active

for obj in selection:
    obj.select_set(True)
