
Default Material

During the build process Unity optimizes the included meshes based on the related shaders and their usage/need for vertex colors and uvs. Unity does not recognize a mesh being used by ATG and its shaders so we have to somewhere declare the relationship between mesh and shader. Otherwise vertex colors most likely will be stripped.