//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.IO.Abstractions;

namespace Test.Mocks
{
    internal sealed class MockPath : PathBase
    {
        public override string ChangeExtension(string path, string extension)
        {
            throw new NotImplementedException();
        }

        public override string Combine(string path1, string path2)
        {
            return Path.Combine(path1, path2);
        }

        public override string GetDirectoryName(string path)
        {
            throw new NotImplementedException();
        }

        public override string GetExtension(string path)
        {
            throw new NotImplementedException();
        }

        public override string GetFileName(string path)
        {
            throw new NotImplementedException();
        }

        public override string GetFileNameWithoutExtension(string path)
        {
            throw new NotImplementedException();
        }

        public override string GetFullPath(string path)
        {
            throw new NotImplementedException();
        }

        public override char[] GetInvalidFileNameChars()
        {
            throw new NotImplementedException();
        }

        public override char[] GetInvalidPathChars()
        {
            throw new NotImplementedException();
        }

        public override string GetPathRoot(string path)
        {
            throw new NotImplementedException();
        }

        public override string GetRandomFileName()
        {
            throw new NotImplementedException();
        }

        public override string GetTempFileName()
        {
            throw new NotImplementedException();
        }

        public override string GetTempPath()
        {
            throw new NotImplementedException();
        }

        public override bool HasExtension(string path)
        {
            throw new NotImplementedException();
        }

        public override bool IsPathRooted(string path)
        {
            throw new NotImplementedException();
        }

        public override char AltDirectorySeparatorChar
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override char DirectorySeparatorChar
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete]
        public override char[] InvalidPathChars
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override char PathSeparator
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override char VolumeSeparatorChar
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
