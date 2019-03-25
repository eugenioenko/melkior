using System;

namespace Melkior
{
    class MelkiorReturn: Exception
    {
        public Any value;

        public MelkiorReturn(Any value)
        {
            this.value = value;
        }
    }
}
