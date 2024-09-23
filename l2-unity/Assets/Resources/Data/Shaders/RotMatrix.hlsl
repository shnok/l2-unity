#ifndef ROT_MATRIX_INCLUDED
#define ROT_MAXTRIX_INCLUDED

void NormalRotationMatrix_float(float3 v1, float3 v2, out float3x3 Out) //-- v1 = up vector and v2 the normal that i want for my vertices
{
                float3 axis = cross(v1, v2);

                float cosA = dot(v1, v2);
                float k = 1.0f / (1.0f + cosA);

                float3x3 result =
                    float3x3(
                        (axis.x * axis.x * k) + cosA,
                        (axis.y * axis.x * k) - axis.z,
                        (axis.z * axis.x * k) + axis.y,
                        (axis.x * axis.y * k) + axis.z,
                        (axis.y * axis.y * k) + cosA,
                        (axis.z * axis.y * k) - axis.x,
                        (axis.x * axis.z * k) - axis.y,
                        (axis.y * axis.z * k) + axis.x,
                        (axis.z * axis.z * k) + cosA
                        );

                Out = result;
}

void DirectionToRotation_float(float3 Direction, out float3 RotationEuler)
{
    // Ensure the direction is normalized
    Direction = normalize(Direction);
    
    // Calculate rotation angles
    float3 rotationRadians;
    
    // Yaw (around Y-axis)
    rotationRadians.y = atan2(Direction.x, Direction.z);
    
    // Pitch (around X-axis)
    rotationRadians.x = -asin(Direction.y);
    
    // Roll (around Z-axis) - in this case, we don't have roll, so set to 0
    rotationRadians.z = 0;
    
    // Convert radians to degrees
    RotationEuler = rotationRadians * (180 / PI);
}


#endif