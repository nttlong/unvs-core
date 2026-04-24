#ifndef MY_WAVE_INCLUDE
#define MY_WAVE_INCLUDE

void CalculateWave_float(float Seed, float Speed, float Freq, float Amp, out float Out)
{
    float time = _Time.y * Speed;
    float w1 = sin(Seed * Freq + time);
    float w2 = sin(Seed * Freq * 1.6 + time * 1.3 + 1.2);
    float w3 = sin(Seed * Freq * 0.7 - time * 0.8 + 2.5);

    Out = (w1 + w2 * 0.4 + w3 * 0.2) * Amp;
}

#endif