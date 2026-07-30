// 处理Gles 2.0平台下，一些常规计算方法不支持的情况
#ifndef GLES2_HELPER_INCLUDE  
	#define GLES2_HELPER_INCLUDE

	#define MUL_3x3_WITH_VECTOR(matrix, vector) float3(dot(matrix[0], vector), dot(matrix[1], vector), dot(matrix[2], vector))
	
	#define MUL_VECTOR_WITH_3x3(vector, matrix) vector[0] * matrix[0] + vector[1] * matrix[1] + vector[2] * matrix[2]
#endif