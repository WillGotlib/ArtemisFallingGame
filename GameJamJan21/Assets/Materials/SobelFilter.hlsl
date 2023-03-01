//UNITY_SHADER_NO_UPGRADE
#ifndef MYHLSLINCLUDE_INCLUDED
#define MYHLSLINCLUDE_INCLUDED

float2x2 kernel_x1=float2x2(-1, 1,
                            -1, 1);
float2x2 kernel_x2=float2x2(-1, 1,
                            -1, 1);

float2x2 kernel_y1=float2x2(-1,-1,
                             1, 1);
float2x2 kernel_y2=float2x2(-1,-1,
                             1, 1);

float2x2 kernel_z1=float2x2(-1,-1,
                            -1,-1);
float2x2 kernel_z2=float2x2( 1, 1,
                             1, 1);

void Sobel_float(const float colour, out float3 Out)
{
    float4 input;
    input.x = colour;

    const float dx = ddx(input.x);
    const float dy = ddy(input.x);
    input.y = input.x - dx;
    input.z = input.x - dy;

    const float didx = ddx(input.y); 
    const float didy = ddy(input.z);
    const float tempx = input.y - didx;
    const float tempy = input.z - didy;

    if(abs(input.x)-tempx<0.001)
    {
        input.w = tempx;
    }
    else
    {
        input.w = tempy;
    }
    
    float x = mul(input, kernel_x1) * mul(input, kernel_x2);
    float y = mul(input, kernel_y1) * mul(input, kernel_y2);
    float z = mul(input, kernel_z1) * mul(input, kernel_z2);
    Out = float3(x,y,z);
}

#endif //MYHLSLINCLUDE_INCLUDED