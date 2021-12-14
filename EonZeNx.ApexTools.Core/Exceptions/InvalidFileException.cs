using System.Runtime.Serialization;

namespace EonZeNx.ApexTools.Core.Exceptions
{
    [Serializable]
    public class InvalidFileVersion : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public InvalidFileVersion() { }

        public InvalidFileVersion(string message) : base(message) { }

        public InvalidFileVersion(string message, Exception inner) : base(message, inner) { }

        protected InvalidFileVersion(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        { }
    }
}