using System;

namespace HotKeyApi
{
    class KeyAlreadyExistsException: Exception
    {
        public KeyAlreadyExistsException()
        {

        }
        public KeyAlreadyExistsException(string key)
            : base(string.Format("Selected keys are already used to launch: {0}", key))
        {

        }
    }
}
