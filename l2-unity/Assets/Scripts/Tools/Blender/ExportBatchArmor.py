import bpy
import os

# export to blend file location
basedir = os.path.dirname(bpy.data.filepath)
if not basedir:
    raise Exception("Blend file is not saved")

view_layer = bpy.context.view_layer
obj_active = view_layer.objects.active
selection = bpy.context.selected_objects

# Group objects by armature relationships
armature_groups = {}
standalone_objects = []

for obj in selection:
    if obj.type == 'ARMATURE':
        # Find meshes that use this armature
        armature_groups[obj] = [obj]
        for other_obj in selection:
            if other_obj.type == 'MESH' and other_obj.find_armature() == obj:
                armature_groups[obj].append(other_obj)
    elif obj.type == 'MESH':
        # Check if this mesh has an armature modifier
        armature_obj = obj.find_armature()
        if not armature_obj or armature_obj not in selection:
            standalone_objects.append(obj)

# Export armature groups
bpy.ops.object.select_all(action='DESELECT')
for armature, group_objects in armature_groups.items():
    # Select all objects in this group
    for obj in group_objects:
        obj.select_set(True)
    
    view_layer.objects.active = armature
    name = bpy.path.clean_name(armature.name)
    fn = os.path.join(basedir, name)
    
    bpy.ops.export_scene.fbx(
        filepath=fn.replace("_mo", "").replace("_ao","") + ".fbx", 
        use_selection=True,
        use_visible=False,
        use_active_collection=False,
        global_scale=1.0, 
        apply_unit_scale=True,
        apply_scale_options='FBX_SCALE_NONE', 
        use_space_transform=True, 
        bake_space_transform=True, 
        object_types={'MESH', 'ARMATURE'}, 
        use_mesh_modifiers=True, 
        use_mesh_modifiers_render=True, 
        mesh_smooth_type='FACE', 
        colors_type='SRGB', 
        prioritize_active_color=False, 
        use_subsurf=False, 
        use_mesh_edges=False, 
        use_tspace=True, 
        use_triangles=False, 
        use_custom_props=True, 
        add_leaf_bones=True, 
        primary_bone_axis='Y',
        secondary_bone_axis='X', 
        use_armature_deform_only=False, 
        armature_nodetype='NULL', 
        bake_anim=True, 
        bake_anim_use_all_bones=True,
        bake_anim_use_nla_strips=True, 
        bake_anim_use_all_actions=False, 
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
    
    # Deselect all objects in this group
    for obj in group_objects:
        obj.select_set(False)
    
    print("written:", fn)

# Export standalone objects
for obj in standalone_objects:
    obj.select_set(True)
    view_layer.objects.active = obj
    name = bpy.path.clean_name(obj.name)
    fn = os.path.join(basedir, name)
    
    bpy.ops.export_scene.fbx(
        filepath=fn.replace("_mo", "").replace("_ao","") + ".fbx", 
        use_selection=True,
        # ... same export settings as above
    )
    
    obj.select_set(False)
    print("written:", fn)

# Restore original selection
view_layer.objects.active = obj_active
for obj in selection:
    obj.select_set(True)