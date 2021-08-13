
/// <summary>
///  Given a coordinate x,y, 
///  returns a random 32 bit integer.
/// </summary>
/// <remarks>
/// Squirrel noise as described in:
/// https://www.youtube.com/watch?v=LWFzPP8ZbdU&t=2021s
/// </remarks>
 public readonly struct Squirrel3
    {
        private readonly uint seed;

        /// <summary>
        /// Initializes the noise generator with a seed.
        /// </summary>
        public Squirrel3(uint seed)
        {
            this.seed = seed;
        }

        const uint NOISE1 = 0xb5297a4d; //0b0110'1000'1110'0011'0001'1101'1010'0100
        const uint NOISE2 = 0x68e31da4; //0b1011'0101'0010'1001'0111'1010'0100'1101
        const uint NOISE3 = 0x1b56c4e9; //0b0001'1011'0101'0110'1100'0100'1110'1001

        public uint this[int position]
        {
            get 
            {
                unchecked
                {
                    uint mangledBits = (uint)position;
                    mangledBits *= NOISE1;
                    mangledBits += seed;
                    mangledBits ^= mangledBits >> 8;
                    mangledBits += NOISE2;
                    mangledBits ^= mangledBits << 8;
                    mangledBits *= NOISE3;
                    mangledBits ^= mangledBits >> 8;
                    return mangledBits;
                }
            }
        }

        private const int PRIME1=198491317;
        private const int PRIME2=6542089;

        public uint this[int x, int y] => this[x + y * PRIME1];

        public uint this[int x, int y, int z] => this[x + y * PRIME1 + z * PRIME2];
    }