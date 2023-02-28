//UNITY_SHADER_NO_UPGRADE
#ifndef MYHLSLINCLUDE_INCLUDED
#define MYHLSLINCLUDE_INCLUDED

float3x3 kernel_x1=float3x3(-1, 0, 1,
                            -2, 0, 2,
                            -1, 0, 1);
float3x3 kernel_x2=float3x3(-2, 0, 2,
                            -4, 0, 4,
                            -2, 0, 2);
float3x3 kernel_x3=float3x3(-1, 0, 1,
                            -2, 0, 2,
                            -1, 0, 1);

float3x3 kernel_y1=float3x3(-1,-2,-1,
                             0, 0, 0,
                             1, 2, 1);
float3x3 kernel_y2=float3x3(-2,-4,-2,
                             0, 0, 0,
                             2, 4, 2);
float3x3 kernel_y3=float3x3(-1,-2,-1,
                             0, 0, 0,
                             1, 2, 1);

float3x3 kernel_z1=float3x3(-1,-2,-1,
                            -2,-4,-2,
                            -1,-2,-1);
float3x3 kernel_z2=float3x3( 0, 0, 0,
                             0, 0, 0,
                             0, 0, 0);
float3x3 kernel_z3=float3x3( 1, 2, 1,
                             2, 4, 2,
                             1, 2, 1);

void Sobel_float(const float3 colour, out float3 Out)
{
    float x = mul(colour, kernel_x1) * mul(colour, kernel_x2) * mul(colour, kernel_x3);
    float y = mul(colour, kernel_y1) * mul(colour, kernel_y2) * mul(colour, kernel_y3);
    float z = mul(colour, kernel_z1) * mul(colour, kernel_z2) * mul(colour, kernel_z3);
    Out = float3(x,y,z);
}

#endif //MYHLSLINCLUDE_INCLUDED